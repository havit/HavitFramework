using Havit.Diagnostics.Contracts;
using Renci.SshNet;

namespace Havit.Services.Sftp.FileStorage
{
	/// <summary>
	/// Parametry pro SftpStorageService.
	/// </summary>
	public class SftpStorageServiceOptions
	{
		/// <summary>
		/// Konfigurace připojení k cílovému SFTP serveru.
		/// </summary>
		/// <remarks>
		/// Díky typu ConnectionInfo publikujeme závislost na použité knihovně, vzhledem ke scope knihovny a vzhledem k možnostem,
		/// které to přináší pro možnost konfigurace připojení, toto necháváme jako OK.
		/// </remarks>
		public ConnectionInfo ConnectionInfo { get; set; }
	}
}
