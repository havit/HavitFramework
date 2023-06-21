using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage;

/// <summary>
/// Výjimka při šifrování nebo dešifrování obsahu storage.
/// </summary>
public class EncryptDecryptFilesException : Exception
{
	private readonly string[] encryptedOrDecryptedFiles;

	private readonly string[] filesToEncryptOrDecrypt;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="message">Zpráva.</param>
	/// <param name="innerException">Inner exception.</param>
	/// <param name="encryptedOrDecryptedFiles">Soubory, které se podařilo šifrovat či dešifrovat.</param>
	/// <param name="filesToEncryptOrDecrypt">Soubory, které se nepodařilo šifrovat či dešifrovat. Nemuselo u nich dojít k žádné chybě, prostě na ně nedošla řada, neboť dříve došlo k této výjimce.</param>
	public EncryptDecryptFilesException(string message, Exception innerException, string[] encryptedOrDecryptedFiles, string[] filesToEncryptOrDecrypt) : base(message, innerException)
	{
		this.encryptedOrDecryptedFiles = encryptedOrDecryptedFiles;
		this.filesToEncryptOrDecrypt = filesToEncryptOrDecrypt;
	}

	/// <summary>
	/// Zobrazuje obsah výjimky jako human-readable text.
	/// </summary>
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder(base.ToString());
		if (encryptedOrDecryptedFiles != null)
		{
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("Encrypted or Decrypted files:");
			encryptedOrDecryptedFiles.ToList().ForEach(file => sb.AppendLine(file));
		}
		if (filesToEncryptOrDecrypt != null)
		{
			sb.AppendLine();
			sb.AppendLine();
			sb.AppendLine("All Files to Encrypt or Decrypt:");
			filesToEncryptOrDecrypt.ToList().ForEach(file => sb.AppendLine(file));
		}
		return sb.ToString();
	}
}
