namespace Havit.Services.Sftp.Tests.FileStorage.Internal;

internal class SftpUserInfo
{
	public string Username { get; }

	public string Password => Username;

	public SftpUserInfo(string username)
	{
		Username = username;
	}
}

