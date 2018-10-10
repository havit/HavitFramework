using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Havit.Business.Configuration.NetCore
{
	/// <summary>
	/// Implementation of <see cref="IGitRepositoryProvider"/> that reads current branch from .git/HEAD.
	/// </summary>
	public class HeadFileGitRepositoryProvider : IGitRepositoryProvider
	{
		private static readonly Regex HeadRefRegex = new Regex("^ref: refs/heads/(?<branch_name>[\\w\\d]+)$");

		/// <inheritdoc />
		public string GetBranch(string path)
		{
			var parentGitRepository = FindGitRepository(path);
			if (parentGitRepository == null)
			{
				return null;
			}

			return ReadHeadRef(parentGitRepository);
		}

		private static DirectoryInfo FindGitRepository(string directory)
		{
			var currentDirectory = new DirectoryInfo(directory);

			while (currentDirectory != null)
			{
				if (currentDirectory.GetDirectories(".git", SearchOption.TopDirectoryOnly).Any())
				{
					return currentDirectory;
				}

				currentDirectory = currentDirectory.Parent;
			}

			return null;
		}

		private string ReadHeadRef(DirectoryInfo gitRepository)
		{
			FileInfo headFile = gitRepository.GetDirectories(".git").Single().GetFiles("HEAD").Single();

			string line = File.ReadAllLines(headFile.FullName)[0];
			Match match = HeadRefRegex.Match(line);
			return match.Success ? match.Groups["branch_name"].Value : null;
		}
	}
}