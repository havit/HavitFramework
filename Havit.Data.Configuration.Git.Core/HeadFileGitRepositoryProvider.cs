using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Havit.Data.Configuration.Git.Core
{
	/// <summary>
	/// Simple implementation of <see cref="IGitRepositoryProvider"/> that reads current branch from .git/HEAD.
	/// <remarks>Useful for contexts, where using full-blown Git library like libgit2sharp is not appropriate.</remarks>
	/// </summary>
	public class HeadFileGitRepositoryProvider : IGitRepositoryProvider
	{
		private static readonly Regex HeadRefRegex = new Regex("^ref: refs/heads/(?<branch_name>[^\\s]+)$");

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

		private static string ReadHeadRef(DirectoryInfo gitRepository)
		{
			FileInfo headFile = gitRepository.GetDirectories(".git").Single().GetFiles("HEAD").Single();

			string line = File.ReadAllLines(headFile.FullName)[0];
			return ParseBranchName(line);
        }

        public static string ParseBranchName(string refFromHeadFile)
        {
            Match match = HeadRefRegex.Match(refFromHeadFile);
            return match.Success ? match.Groups["branch_name"].Value : null;
        }
    }
}