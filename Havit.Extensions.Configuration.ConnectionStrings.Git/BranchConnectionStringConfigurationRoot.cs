using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Havit.Data.Configuration.Git.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Havit.Extensions.Configuration.ConnectionStrings.Git
{
	public class BranchConnectionStringConfigurationRoot : IConfigurationRoot
	{
		private readonly IConfigurationRoot configurationRoot;
		private readonly IFileProvider fileProvider;
		private readonly IGitRepositoryProvider gitRepositoryProvider;

		public BranchConnectionStringConfigurationRoot(IConfigurationRoot configurationRoot,
			IFileProvider fileProvider,
			IGitRepositoryProvider gitRepositoryProvider)
		{
			this.configurationRoot = configurationRoot;
			this.fileProvider = fileProvider;
			this.gitRepositoryProvider = gitRepositoryProvider;
		}

		public IConfigurationSection GetSection(string key)
		{
			return new ExtensibleConfigurationSection(this, key);
		}

		public IEnumerable<IConfigurationSection> GetChildren()
		{
			return configurationRoot.GetChildren();
		}

		public IChangeToken GetReloadToken()
		{
			return configurationRoot.GetReloadToken();
		}

		public string this[string key]
        {
            get
            {
                if (IsConnectionStringKey(key))
                {
                    string connectionString = configurationRoot[key];
                    if (connectionString == null)
                    {
                        throw new ArgumentException(
                            $"Specified connection string ('{key}') not found, cannot transform it using current Git branch." +
                            " Please make sure configuration is correctly set up.", nameof(key));
                    }

                    return TransformConnectionString(connectionString);
                }

                return configurationRoot[key];
            }
            set => configurationRoot[key] = value;
        }

        public void Reload()
		{
			configurationRoot.Reload();
		}

		public IEnumerable<IConfigurationProvider> Providers => configurationRoot.Providers;

		private bool IsConnectionStringKey(string key) => key.StartsWith("ConnectionStrings:");

		private string TransformConnectionString(string connectionString)
		{
			var appSettingsDirectory = GetAppSettingsDirectory();

			return new BranchConnectionStringTransformer(gitRepositoryProvider).ChangeDatabaseName(connectionString, appSettingsDirectory);
		}

		private string GetAppSettingsDirectory()
		{
			return new FileInfo(fileProvider.GetDirectoryContents("").First().PhysicalPath).DirectoryName;
		}
	}
}