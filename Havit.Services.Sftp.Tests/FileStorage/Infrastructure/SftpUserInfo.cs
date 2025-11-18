namespace Havit.Services.Sftp.Tests.FileStorage.Infrastructure;

internal class SftpUserInfo
{
	public string Username { get; }

	public string Password => Username;

	public int UID { get; }

	public int Gid => UID;

	public string HomeDirectory => "/home/" + Username;

	public SftpUserInfo(string username, int uid)
	{
		Username = username;
		UID = uid;
	}
}

