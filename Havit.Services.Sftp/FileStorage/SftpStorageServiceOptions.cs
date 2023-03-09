using System;
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
		///
		/// Příklad použití:
		/// using var service = new SftpStorageService(new SftpStorageServiceOptions { ConnectionInfoFunc = () => new ConnectionInfo("sfteu.xerox.com", "ESPChester", new Renci.SshNet.PrivateKeyAuthenticationMethod("ESPChester", new PrivateKeyFile("D:\\Temp\\ESPChester.new"))) });
		/// 
		/// jak získat tento soubor (ESPChester.new)?
		/// a) stáhnout puttygen, spustit
		/// b) načíst *.ppk přes menu Conversions, Import Key
		/// c) exportovat přes Conversion, Export OpenSSH key (nikoliv new format, nikoliv ssh.com!)
		/// </remarks>
		public Func<ConnectionInfo> ConnectionInfoFunc { get; set; }
	}
}
