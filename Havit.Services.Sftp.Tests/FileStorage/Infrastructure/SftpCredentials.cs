namespace Havit.Services.Sftp.Tests.FileStorage.Infrastructure;

internal class SftpCredentials
{
	public string Username { get; set; }

	public string Password => _passworLazy.Value;
	private Lazy<string> _passworLazy;

	public SftpCredentials(string username, string passwordSecretName)
	{
		Username = username;
		_passworLazy = new Lazy<string>(() => SftpPasswordHelper.GetPassword(passwordSecretName), LazyThreadSafetyMode.ExecutionAndPublication);
	}
}

