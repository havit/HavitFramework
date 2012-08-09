using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Havit.Web.UI
{
	public partial class FilePageStatePersister
		/// <summary>
		/// Strategie pro pojmenování souborů s viewstate/controlstate.
		/// </summary>
	{
		public class PerUserNamingStrategy : FilePageStatePersister.IFileNamingStrategy
		{
			#region Private consts
			private const string FolderNamePattern = "viewstate.{0}";
			private const string AnonymousUserFolderName = "_anonymous";
			#endregion

			#region Root
			/// <summary>
			/// Root, do kterého se ukládá viewstate/controlstate.
			/// </summary>
			protected string Root { get; private set; }
			#endregion

			#region PerUserNamingStrategy
			/// <summary>
			/// Konstruktor.
			/// </summary>
			/// <param name="root"></param>
			public PerUserNamingStrategy(string root)
			{
				Root = root;
			}
			#endregion

			#region GetStorageSymbol
			/// <summary>
			/// Viz IFileNamingStrategy.GetStorageSymbol.
			/// </summary>
			/// <returns></returns>
			string FilePageStatePersister.IFileNamingStrategy.GetStorageSymbol()
			{
				return Guid.NewGuid().ToString();
			}
			#endregion

			#region GetFilename
			/// <summary>
			/// Viz IFileNamingStrategy.GetFilename.
			/// </summary>
			/// <param name="symbol"></param>
			/// <returns></returns>
			string FilePageStatePersister.IFileNamingStrategy.GetFilename(string symbol)
			{
				foreach (char invalidChar in System.IO.Path.GetInvalidFileNameChars())
				{
					if (symbol.Contains(invalidChar))
					{
						throw new ArgumentException("Symbol obsahuje nepoovlený znak.");
					}
				}
				return System.IO.Path.Combine(GetFolderForUserName(Root, HttpContext.Current.User.Identity.Name), symbol);
			}
			#endregion

			#region DeleteAllAnonymousFiles (static)
			/// <summary>
			/// Smaže soubory anonymního uživatele.
			/// </summary>
			public static void DeleteAllAnonymousFiles(string root)
			{
				DeleteAllUserFiles(root, null);
			}
			#endregion

			#region DeleteAllUserFiles (static)
			/// <summary>
			/// Smaže soubory konkrétního uživatele.
			/// </summary>
			public static void DeleteAllUserFiles(string root, string username)
			{
				string folder = GetFolderForUserName(root, username);
				string[] files = System.IO.Directory.GetFiles(folder, "*", System.IO.SearchOption.AllDirectories);
				foreach (string file in files)
				{
					System.IO.File.Delete(file);
				}
			}
			#endregion

			#region GetFolderForUserName (static)
			/// <summary>
			/// Vrátí název SLOŽKY pro uložení viewstate pro daného uživatele.
			/// </summary>
			private static string GetFolderForUserName(string root, string username)
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

				return System.IO.Path.Combine(root, String.Format(FolderNamePattern, userFolder));
			}
			#endregion
		}
	}
}
