using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Havit.Services.Azure.Tests.FileStorage.Infrastructure;

public static class AzureStorageConnectionStringHelper
{
	public static string GetConnectionString() => AzureBlobStorageConnectionStringLazy.Value;

	private static Lazy<string> AzureBlobStorageConnectionStringLazy = new Lazy<string>(() =>
	{
		string connectionString = System.Environment.GetEnvironmentVariable("ConnectionStrings_AzureStorage");

		if (connectionString is null)
		{
			var client = new SecretClient(new Uri("https://HavitFrameworkConfigKV.vault.azure.net"), new DefaultAzureCredential());

			try
			{
				KeyVaultSecret secret = client.GetSecret("ConnectionStrings--AzureStorage");
				connectionString = secret.Value;
			}
			catch (global::Azure.RequestFailedException)
			{
				// NOOP (daný secret neexistuje)
			}
		}

		if (connectionString is null)
		{
			throw new InvalidOperationException("Couldn't find Azure storage connection string in the configuration.");
		}

		return connectionString;
	});
}
