using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System;

namespace Havit.Services.Azure.Tests.FileStorage.Infrastructure
{
	public static class AzureStorageConnectionStringHelper
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
					.AddAzureKeyVault(new Uri("https://HavitFrameworkConfigKV.vault.azure.net"), new DefaultAzureCredential())
					.Build();
				connectionString = config.GetConnectionString("AzureStorage");
			}

			if (connectionString is null)
			{
				throw new InvalidOperationException("Couldn't find Azure storage connection string in the configuration.");
			}

			return connectionString;
		});
	}
}
