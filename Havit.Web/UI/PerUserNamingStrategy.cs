using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
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
			#region Private consts
			private const string FolderNamePattern = "viewstate.{0}";
			private const string AnonymousUserFolderName = "_anonymous";
			private const string LoginPageFolderName = "_loginpage";
			#endregion

			#region Root
			/// <summary>
			/// Root, do kterého se ukládá viewstate/controlstate.
			/// </summary>
			protected string Root { get; private set; }
			#endregion

			#region UseSpecialFolderForLoginPage
			/// <summary>
			/// Indikuje, zda se používá zvláštní složka pro login page (ani uživatelská, ani anonymous).
			/// </summary>
			protected bool UseSpecialFolderForLoginPage { get; private set; }
			#endregion

			#region PerUserNamingStrategy
			/// <summary>
			/// Konstruktor.
			/// </summary>
			public PerUserNamingStrategy(string root, bool useSpecialFolderForLoginPage = false)
			{
				UseSpecialFolderForLoginPage = useSpecialFolderForLoginPage;
				Root = root;
			}

			#endregion

			#region DeleteUserFiles (static)
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
				string folder = GetFolderForUserName(root, username, false);
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
			#endregion

			#region DeleteOldAnonymousFiles
			/// <summary>
			/// Smaže soubory starší dnou dnů anonymního uživatele uživatele a souborů login page.
			/// V případě, že se některý soubor nepodaří smazat, je mazání ukončeno, ale není vyhozena výjimka.
			/// </summary>
			public static void DeleteOldAnonymousFiles(string root)
			{
				DeleteUserFiles(root, null, new TimeSpan(2, 0, 0, 0)); // 2 dny
				DeleteUserFiles(root, LoginPageFolderName, new TimeSpan(2, 0, 0, 0)); // 2 dny
			}
			#endregion

			#region GetFolderForUserName (static)
			/// <summary>
			/// Vrátí název SLOŽKY pro uložení viewstate pro daného uživatele.
			/// </summary>
			private static string GetFolderForUserName(string root, string username, bool useSpecialFolderForLoginPage)
			{
				if (useSpecialFolderForLoginPage && (HttpContext.Current != null) && (HttpContext.Current.Request.Url.AbsolutePath == new Uri(HttpContext.Current.Request.Url, FormsAuthentication.LoginUrl).AbsolutePath))
				{
					username = LoginPageFolderName;
				}
				else if (String.IsNullOrEmpty(username))
				{
					username = AnonymousUserFolderName;
				}

				string userFolder = username.ToLower();
				foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
				{
					userFolder = userFolder.Replace(invalidChar, '_');
				}

				return System.IO.Path.Combine(root, String.Format(FolderNamePattern, userFolder));
			}
			#endregion

			#region FilePageStatePersister.IFileNamingStrategy implementation
			/// <summary>
			/// Viz IFileNamingStrategy.GetStorageSymbol.
			/// </summary>
			string FilePageStatePersister.IFileNamingStrategy.GetStorageSymbol()
			{
				return Guid.NewGuid().ToString();
			}
			
			/// <summary>
			/// Viz IFileNamingStrategy.GetFilename.
			/// </summary>
			string FilePageStatePersister.IFileNamingStrategy.GetFilename(string symbol)
			{
				foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
				{
					if (symbol.Contains(invalidChar))
					{
						throw new ArgumentException("Symbol obsahuje nepoovlený znak.");
					}
				}
				return System.IO.Path.Combine(GetFolderForUserName(Root, HttpContext.Current.User.Identity.Name, this.UseSpecialFolderForLoginPage), symbol);
			}
			#endregion

		}
	}
}
