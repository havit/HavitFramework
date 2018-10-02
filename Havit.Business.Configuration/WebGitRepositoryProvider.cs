using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using LibGit2Sharp;

namespace Havit.Business.Configuration
{
	/// <summary>
	/// Implementation of <see cref="IGitRepositoryProvider"/> that uses LibGit2Sharp and supports ASP.NET Web projects and regular console/desktop projects (or Tests).
	/// </summary>
	public class WebGitRepositoryProvider : IGitRepositoryProvider
	{
		/// <inheritdoc />
		public string GetBranch(string path)
		{
			var parentGitRepository = FindGitRepository(path);
			if (parentGitRepository == null)
			{
				return null;
			}

			using (var repository = new Repository(parentGitRepository))
			{
				return repository.Head.FriendlyName;
			}
		}

		private static string FindGitRepository(string directory)
		{
			var currentDirectory = new DirectoryInfo(directory);

			while (currentDirectory != null)
			{
				if (currentDirectory.GetDirectories(".git", SearchOption.TopDirectoryOnly).Any())
				{
					return currentDirectory.FullName;
				}

				currentDirectory = currentDirectory.Parent;
			}

			return null;
		}
	}
}