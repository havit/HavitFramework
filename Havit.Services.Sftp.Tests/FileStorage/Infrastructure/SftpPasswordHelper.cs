using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Havit.Services.Sftp.Tests.FileStorage.Infrastructure;

public static class SftpPasswordHelper
{
	public static string GetPasswordForPrimaryAccount() => GetPassword("Sftp--Primary--Password");
	public static string GetPasswordForSecondaryAccount() => GetPassword("Sftp--Secondary--Password");

	private static string GetPassword(string section)
	{
		// Při buildu v Azure máme k dispozici environment variables, resp. variable group 002.HFW-HavitFramework.
		// Tato variable group je napojená na Azure Key Vault HavitFrameworkConfigKV.
		string password = System.Environment.GetEnvironmentVariable(section);

		// Při lokálním vývoji nemáme environment variables, ale připojíme se k načtení hesel ke Azure Key Vaultu.
		// Přístupové údaje jsou řešeny pomocí DefaultAzureCredential, zde tedy obvykle přes Visual Studio sredentials (Visual Studio: Help, Register Visual Studio)
		if (password is null)
		{
			var client = new SecretClient(new Uri("https://HavitFrameworkConfigKV.vault.azure.net"), new DefaultAzureCredential());
			try
			{
				KeyVaultSecret secret = client.GetSecret(section);
				password = secret.Value;
			}
			catch (Azure.RequestFailedException)
			{
				// NOOP (daný secret neexistuje)
			}
		}

		if (password is null)
		{
			throw new InvalidOperationException("Couldn't find SFTP password in the configuration.");
		}

		return password;
	}
}
