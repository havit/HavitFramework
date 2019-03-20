using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Havit.Web.UI
{
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
			/// Root, do kterého se ukládá viewstate/controlstate.
			/// </summary>
			protected string Root { get; private set; }

			/// <summary>
			/// Konstruktor.
			/// </summary>
			public PerUserNamingStrategy(string root)
			{
				Root = root;
			}

			/// <summary>
			/// Smaže soubory konkrétního uživatele.
			/// Pokud není uživatel zadán (String.IsNullOrEmpty), maže soubory anonymního uživatele.
			/// Maximální stáří souborů se určuje parametrem fileAge. Pokud má hodnotu null, mažou se alespoň den staré soubory.
			/// V případě, že se některý soubor nepodaří smazat, je mazání ukončeno, ale není vyhozena výjimka.
			/// </summary>
			public static void DeleteUserFiles(string root, string username, TimeSpan? fileAge = null)
			{
				FilePageStatePersister.ILogService logService = new FilePageStatePersisterLogService();

				DateTime now = DateTime.Now;

				if (fileAge == null)
				{
					fileAge = new TimeSpan(1, 0, 0, 0);
				}
				string folder = Path.Combine(root, GetFolderForUserName(username));
				if (System.IO.Directory.Exists(folder))
				{
					string[] files = System.IO.Directory.GetFiles(folder, "*", System.IO.SearchOption.AllDirectories);
					foreach (string file in files)
					{
						if ((now - System.IO.File.GetLastWriteTime(file)) > fileAge.Value)
						{
							try
							{
								System.IO.File.Delete(file);
								logService.Log(String.Format("{0}\tDeleted", file));
							}
							catch (Exception) // pokud nějaký soubor nejde smazat, končíme s mazáním
							{
								logService.Log(String.Format("{0}\tDelete failed", file));
								return;
							}
						}
					}
				}
			}

			/// <summary>
			/// Smaže soubory starší dnou dnů anonymního uživatele uživatele.
			/// V případě, že se některý soubor nepodaří smazat, je mazání ukončeno, ale není vyhozena výjimka.
			/// </summary>
			public static void DeleteOldAnonymousFiles(string root)
			{
				DeleteUserFiles(root, null, new TimeSpan(2, 0, 0, 0)); // 2 dny
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

				return System.IO.Path.Combine(Root, symbol);
			}
		}
	}
}
