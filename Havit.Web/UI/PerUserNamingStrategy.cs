using Havit.Services.FileStorage;
using System.Web;
using System.Web.UI.WebControls;

namespace Havit.Web.UI;

public partial class FilePageStatePersister
{
	/// <summary>
	/// Strategie pro pojmenování souborů s viewstate/controlstate.
	/// </summary>
	public class PerUserNamingStrategy : FilePageStatePersister.IFileNamingStrategy
	{
		private const string FolderNamePattern = "viewstate.{0}";
		private const string AnonymousUserFolderName = "_anonymous";

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public PerUserNamingStrategy()
		{
		}

		/// <summary>
		/// Smaže soubory konkrétního uživatele.
		/// Pokud není uživatel zadán (String.IsNullOrEmpty), maže soubory anonymního uživatele.
		/// Maximální stáří souborů se určuje parametrem fileAge. Pokud má hodnotu null, mažou se alespoň den staré soubory.
		/// V případě, že se některý soubor nepodaří smazat, je mazání ukončeno, ale není vyhozena výjimka.
		/// </summary>
		public static void DeleteUserFiles(IFileStorageService fileStorageService, string username, TimeSpan? fileAge = null)
		{
			FilePageStatePersister.ILogService logService = new FilePageStatePersisterLogService();

			DateTime utcNow = DateTime.Now.ToUniversalTime();

			if (fileAge == null)
			{
				fileAge = new TimeSpan(1, 0, 0, 0);
			}

			foreach (var file in fileStorageService.EnumerateFiles(GetFolderForUserName(username) + "\\*"))
			{
				if ((utcNow - file.LastModifiedUtc) > fileAge.Value)
				{
					try
					{
						fileStorageService.Delete(file.Name);
						logService.Log(String.Format("{0}\tDeleted", file));
					}
					catch (Exception) // pokud nějaký soubor nejde smazat, končíme s mazáním
					{
						logService.Log(String.Format("{0}\tDelete failed", file), System.Diagnostics.TraceEventType.Warning);

						return;
					}
				}
			}
		}

		/// <summary>
		/// Smaže soubory konkrétního uživatele.
		/// Pokud není uživatel zadán (String.IsNullOrEmpty), maže soubory anonymního uživatele.
		/// Maximální stáří souborů se určuje parametrem fileAge. Pokud má hodnotu null, mažou se alespoň den staré soubory.
		/// V případě, že se některý soubor nepodaří smazat, je mazání ukončeno, ale není vyhozena výjimka.
		/// </summary>
		/// <remarks>
		/// Předpokládá se uložení viewstate do file systému.
		/// Metoda existuje z důvodu zpětné kompatibility.
		/// </remarks>
		public static void DeleteUserFiles(string root, string username, TimeSpan? fileAge = null)
		{
			DeleteUserFiles(new FileSystemStorageService(root), username, fileAge);
		}

		/// <summary>
		/// Smaže soubory starší dnou dnů anonymního uživatele uživatele.
		/// V případě, že se některý soubor nepodaří smazat, je mazání ukončeno, ale není vyhozena výjimka.
		/// </summary>
		public static void DeleteOldAnonymousFiles(IFileStorageService fileStorageService, TimeSpan? fileAge = null)
		{
			DeleteUserFiles(fileStorageService, null, fileAge ?? new TimeSpan(2, 0, 0, 0)); // 2 dny
		}

		/// <summary>
		/// Smaže soubory starší dnou dnů anonymního uživatele uživatele.
		/// V případě, že se některý soubor nepodaří smazat, je mazání ukončeno, ale není vyhozena výjimka.
		/// </summary>
		/// <remarks>
		/// Předpokládá se uložení viewstate do file systému.
		/// Metoda existuje z důvodu zpětné kompatibility.
		/// </remarks>
		public static void DeleteOldAnonymousFiles(string root, TimeSpan? fileAge = null)
		{
			DeleteOldAnonymousFiles(new FileSystemStorageService(root), fileAge);
		}

		/// <summary>
		/// Viz IFileNamingStrategy.GetStorageSymbol.
		/// </summary>
		string FilePageStatePersister.IFileNamingStrategy.GetStorageSymbol()
		{
			return Path.Combine(GetFolderForUserName(HttpContext.Current.User.Identity.Name), Guid.NewGuid().ToString());
		}

		/// <summary>
		/// Viz IFileNamingStrategy.GetFilename.
		/// </summary>
		string FilePageStatePersister.IFileNamingStrategy.GetFilename(string symbol)
		{
			foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
			{
				if ((invalidChar != '\\') && symbol.Contains(invalidChar))
				{
					throw new ArgumentException("Symbol obsahuje nepovolený znak."); // ale zpětné lomítko smí obsahovat
				}
			}

			if (symbol.StartsWith("."))
			{
				throw new ArgumentException("Symbol nesmí začínat tečkou.");
			}

			if (symbol.ToArray().Count(c => c.Equals('\\')) > 1)
			{
				throw new ArgumentException("Symbol nesmí obsahovat více než jedno zpětné lomítko.");
			}

			return symbol;
		}

		/// <summary>
		/// Vrátí název SLOŽKY pro uložení viewstate pro daného uživatele (ale nikoliv celou cestu).
		/// </summary>
		private static string GetFolderForUserName(string username)
		{
			if (String.IsNullOrEmpty(username))
			{
				username = AnonymousUserFolderName;
			}

			string userFolder = username.ToLower();
			foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
			{
				userFolder = userFolder.Replace(invalidChar, '_');
			}

			return String.Format(FolderNamePattern, userFolder);
		}
	}
}
