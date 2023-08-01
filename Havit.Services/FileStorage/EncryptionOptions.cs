using System.Security.Cryptography;

namespace Havit.Services.FileStorage;

/// <summary>
/// Parametry šifrování file storage.
/// </summary>
public abstract class EncryptionOptions
{
	/// <summary>
	/// Vrátí encryptor.
	/// </summary>
	public abstract ICryptoTransform CreateEncryptor();

	/// <summary>
	/// Vrátí decryptor.
	/// </summary>
	public abstract ICryptoTransform CreateDecryptor();
}
