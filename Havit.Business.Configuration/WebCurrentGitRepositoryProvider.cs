using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using LibGit2Sharp;

namespace Havit.Business.Configuration
{
	/// <summary>
	/// Implementation of <see cref="ICurrentGitRepositoryProvider"/> that uses LibGit2Sharp and supports ASP.NET Web projects and regular console/desktop projects (or Tests).
	/// </summary>
	public class WebCurrentGitRepositoryProvider : ICurrentGitRepositoryProvider
	{
		/// <inheritdoc />
		public string GetCurrentBranch()
		{
			var parentGitRepository = FindGitRepository(GetRepositorySearchDirectory());
			using (var repository = new Repository(parentGitRepository))
			{
				return repository.Head.FriendlyName;
			}
		}

		private static string GetRepositorySearchDirectory()
		{
			return !string.IsNullOrEmpty(HttpRuntime.AppDomainAppId) ? HttpRuntime.AppDomainAppPath : Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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