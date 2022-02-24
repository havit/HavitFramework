using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System;

namespace Havit.Services.Azure.Tests.FileStorage.Infrastructure
{
    internal static class AzureStorageConnectionStringHelper
    {
		public static string GetConnectionString() => AzureBlobStorageConnectionStringLazy.Value;

		private static Lazy<string> AzureBlobStorageConnectionStringLazy = new Lazy<string>(() =>
		{
			var config = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.Build();
			string connectionString = config.GetConnectionString("AzureStorage");

			if (connectionString is null)
			{
				config = new ConfigurationBuilder()
					.AddAzureKeyVault("https://HavitFrameworkConfigKV.vault.azure.net", new DefaultKeyVaultSecretManager())
					.Build();
				connectionString = config.GetConnectionString("AzureStorage");
			}

			if (connectionString is null)
			{
				throw new InvalidOperationException("Couldn't find Azure storage connection string in configuration.");
			}

			return connectionString;
		});
	}
}
