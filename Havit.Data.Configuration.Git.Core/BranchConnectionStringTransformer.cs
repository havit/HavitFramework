using System.Text.RegularExpressions;

namespace Havit.Data.Configuration.Git.Core
{
	/// <summary>
	/// Zajištuje dopnění názvu branche do názvu databáze v connection stringu.
	/// </summary>
    public class BranchConnectionStringTransformer
	{
		/// <summary>
		/// Značka pro nahrazení za název git branche.
		/// </summary>
        public const string BranchNamePlaceholder = "#BRANCH_NAME#";

        private readonly IGitRepositoryProvider gitRepositoryProvider;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public BranchConnectionStringTransformer(IGitRepositoryProvider gitRepositoryProvider)
		{
			this.gitRepositoryProvider = gitRepositoryProvider;
		}

		/// <summary>
		/// Vymění v connection string hodnotu pro InitialCatalog - nahrazuje BranchNamePlaceholder za název branche.
		/// </summary>
		public string ChangeDatabaseName(string connectionString, string projectPath)
		{
            if (connectionString == null)
            {
                return null;
            }

			// TODO: consider using System.Data.SqlClient.SqlConnectionStringBuilder
			var match = Regex.Match(connectionString, "Initial Catalog=([^;]*)");
			if (!match.Success)
			{
				return connectionString;
			}
			return connectionString.Replace(match.Value, $"Initial Catalog={DetermineDatabaseName(match.Groups[1].Value, projectPath)}");
		}

		private string DetermineDatabaseName(string originalDbName, string configPath)
		{
			string repositoryBranch = gitRepositoryProvider.GetBranch(configPath);
            if (string.IsNullOrEmpty(repositoryBranch))
            {
                // If there's no branch, there's nothing to replace
                // However currently HeadFileGitRepositoryProvider does not support matching detached HEAD to branch name
                // (scenario commonly occuring on build agent)
                return originalDbName;
            }

			return originalDbName.Replace(BranchNamePlaceholder, repositoryBranch);
		}
	}
}