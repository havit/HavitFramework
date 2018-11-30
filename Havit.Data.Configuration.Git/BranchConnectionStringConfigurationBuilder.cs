using Havit.Data.Configuration.Git.Core;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Havit.Data.Configuration.Git
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
			: this(new HeadFileGitRepositoryProvider())
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
			return new BranchConnectionStringTransformer(gitRepositoryProvider)
				.ChangeDatabaseName(connectionString, configPath);
		}
	}
}