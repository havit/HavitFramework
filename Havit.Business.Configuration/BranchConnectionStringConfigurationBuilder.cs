using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Havit.Business.Configuration
{
	/// <summary>
	/// Configuration Builder implementation that modifies connection strings according to current Git branch (i.e. changes DB name).
	///
	/// master -> original DB
	/// {branch_name} -> {original_DB}_{branch_name}
	/// </summary>
	public class BranchConnectionStringConfigurationBuilder : ConfigurationBuilder
	{
		private readonly IGitRepositoryProvider gitRepositoryProvider;

		/// <summary>
		/// Creates instance of <see cref="BranchConnectionStringConfigurationBuilder"/>.
		/// </summary>
		public BranchConnectionStringConfigurationBuilder()
			: this(new WebGitRepositoryProvider())
		{
		}

		/// <summary>
		/// Creates instance of <see cref="BranchConnectionStringConfigurationBuilder"/> with custom <see cref="IGitRepositoryProvider"/> (usually for testing purposes).
		/// </summary>
		public BranchConnectionStringConfigurationBuilder(IGitRepositoryProvider gitRepositoryProvider)
		{
			this.gitRepositoryProvider = gitRepositoryProvider;
		}

		/// <inheritdoc />
		public override ConfigurationSection ProcessConfigurationSection(ConfigurationSection configSection)
		{
			if (configSection is ConnectionStringsSection connectionStringsSection)
			{
				return ProcessConnectionStringsSection(connectionStringsSection);
			}

			return base.ProcessConfigurationSection(configSection);
		}

		private ConfigurationSection ProcessConnectionStringsSection(ConnectionStringsSection section)
		{
			string configPath = Path.GetDirectoryName(section.ElementInformation.Source);

			var transformedSection = new ConnectionStringsSection();

			foreach (ConnectionStringSettings connString in section.ConnectionStrings)
			{
				var newConnString = TransformConnectionString(connString.ConnectionString, configPath);
				transformedSection.ConnectionStrings.Add(new ConnectionStringSettings(connString.Name, newConnString, connString.ProviderName));
			}

			return transformedSection;
		}

		internal string TransformConnectionString(string connectionString, string configPath)
		{
			// TODO: consider using System.Data.SqlClient.SqlConnectionStringBuilder
			var match = Regex.Match(connectionString, "Initial Catalog=([^;]*)");
			if (!match.Success)
			{
				return connectionString;
			}
			return connectionString.Replace(match.Value, $"Initial Catalog={DetermineDatabaseName(match.Groups[1].Value, configPath)}");
		}

		private string DetermineDatabaseName(string originalDbName, string configPath)
		{
			string repositoryBranch = gitRepositoryProvider.GetBranch(configPath);
			if (repositoryBranch == "master" || repositoryBranch == null)
			{
				return originalDbName;
			}
			return $"{originalDbName}_{repositoryBranch}";
		}
	}
}