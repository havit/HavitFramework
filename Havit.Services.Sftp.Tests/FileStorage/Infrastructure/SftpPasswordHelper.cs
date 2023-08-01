using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace Havit.Services.Sftp.Tests.FileStorage.Infrastructure;

public static class SftpPasswordHelper
{
	public static string GetPasswordForPrimaryAccount() => GetPassword("Sftp:Primary:Password");
	public static string GetPasswordForSecondaryAccount() => GetPassword("Sftp:Secondary:Password");

	private static string GetPassword(string section)
	{
		var config = new ConfigurationBuilder()
			.AddEnvironmentVariables()
			.Build();
		string password = config.GetValue<string>(section);

		if (password is null)
		{
			config = new ConfigurationBuilder()
				.AddAzureKeyVault(new Uri("https://HavitFrameworkConfigKV.vault.azure.net"), new DefaultAzureCredential())
				.Build();
			password = config.GetValue<string>(section);
		}

		if (password is null)
		{
			throw new InvalidOperationException("Couldn't find SFTP password in the configuration.");
		}

		return password;
	}
}
