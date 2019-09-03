﻿using System.Text.RegularExpressions;

namespace Havit.Data.Configuration.Git.Core
{
    public class BranchConnectionStringTransformer
	{
        public const string BranchNamePlaceholder = "#BRANCH_NAME#";

        private readonly IGitRepositoryProvider gitRepositoryProvider;

		public BranchConnectionStringTransformer(IGitRepositoryProvider gitRepositoryProvider)
		{
			this.gitRepositoryProvider = gitRepositoryProvider;
		}

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
			return originalDbName.Replace(BranchNamePlaceholder, repositoryBranch);
		}
	}
}