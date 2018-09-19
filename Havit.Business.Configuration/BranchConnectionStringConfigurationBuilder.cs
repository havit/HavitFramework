using System.Configuration;
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
		private readonly ICurrentGitRepositoryProvider currentGitRepositoryProvider;

		/// <summary>
		/// Creates instance of <see cref="BranchConnectionStringConfigurationBuilder"/>.
		/// </summary>
		public BranchConnectionStringConfigurationBuilder()
			: this(new WebCurrentGitRepositoryProvider())
		{
		}

		/// <summary>
		/// Creates instance of <see cref="BranchConnectionStringConfigurationBuilder"/> with custom <see cref="ICurrentGitRepositoryProvider"/> (usually for testing purposes).
		/// </summary>
		public BranchConnectionStringConfigurationBuilder(ICurrentGitRepositoryProvider currentGitRepositoryProvider)
		{
			this.currentGitRepositoryProvider = currentGitRepositoryProvider;
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
			var transformedSection = new ConnectionStringsSection();

			foreach (ConnectionStringSettings connString in section.ConnectionStrings)
			{
				var newConnString = TransformConnectionString(connString.ConnectionString);
				transformedSection.ConnectionStrings.Add(new ConnectionStringSettings(connString.Name, newConnString, connString.ProviderName));
			}

			return transformedSection;
		}

		internal string TransformConnectionString(string connectionString)
		{
			var match = Regex.Match(connectionString, "Initial Catalog=([^;]*)");
			if (!match.Success)
			{
				return connectionString;
			}
			return connectionString.Replace(match.Value, $"Initial Catalog={DetermineDatabaseName(match.Groups[1].Value)}");
		}

		private string DetermineDatabaseName(string originalDbName)
		{
			string repositoryBranch = currentGitRepositoryProvider.GetCurrentBranch();
			if (repositoryBranch == "master" || repositoryBranch == null)
			{
				return originalDbName;
			}
			return $"{originalDbName}_{repositoryBranch}";
		}
	}
}