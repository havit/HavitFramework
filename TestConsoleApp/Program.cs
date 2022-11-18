using Havit.Services.Sftp.FileStorage;
using Renci.SshNet;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace TestConsoleApp
{
	public class Program
	{
		public static void Main()
		{
			// jak získat tento soubor?
			// a) stáhnout puttygen, spustit
			// b) načíst *.ppk přes menu Conversions, Import Key
			// c) exportovat přes Conversion, Export OpenSSH key (nikoliv new format, nikoliv ssh.com!)

			using var service = new SftpStorageService(new SftpStorageServiceOptions { ConnectionInfo = new ConnectionInfo("sfteu.xerox.com", "ESPChester", new Renci.SshNet.PrivateKeyAuthenticationMethod("ESPChester", new PrivateKeyFile("D:\\Temp\\ESPChester.new"))) });
			foreach (var fileInfo in service.EnumerateFiles())
			{
				Console.WriteLine(fileInfo.Name);
			}

		}
	}
}
