using System.Security.Cryptography;
using Havit.Diagnostics.Contracts;

namespace Havit.Services.FileStorage;

/// <summary>
/// AES šifrování file storage.
/// </summary>
public class AesEncryptionOption : EncryptionOptions, IDisposable
{
	private readonly Aes aes;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public AesEncryptionOption(byte[] key, byte[] iv)
	{
		Contract.Requires(key != null);
		Contract.Requires(iv != null);

		aes = new AesManaged();
		aes.Key = key;
		aes.IV = iv;
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="keyAndIV">Serializovaný (Base64 string) klíč a IV. Předpokládá se délka klíče 32 bytů a délke IV 16 bytů.</param>
	/// <remarks>
	/// keyAndIV lze získat statickou metodou <see cref="CreateRandomKeyAndIvAsBase64String"/>.
	/// </remarks>
	public AesEncryptionOption(string keyAndIV)
	{
		Contract.Requires(!String.IsNullOrEmpty(keyAndIV));

		byte[] bytes = Convert.FromBase64String(keyAndIV);
		Contract.Requires(bytes.Length == (32 /* Key */ + 16 /* IV */));

		byte[] key = bytes.Take(32).ToArray();
		byte[] iv = bytes.Skip(32).Take(16).ToArray();

		aes = new AesManaged();
		aes.Key = key;
		aes.IV = iv;
	}

	/// <summary>
	/// Vrátí encryptor.
	/// </summary>
	public override ICryptoTransform CreateEncryptor()
	{
		return aes.CreateEncryptor();
	}

	/// <summary>
	/// Vrátí decryptor.
	/// </summary>
	public override ICryptoTransform CreateDecryptor()
	{
		return aes.CreateDecryptor();
	}

	/// <summary>
	/// Uvolní použité zdroje.
	/// </summary>
	public void Dispose()
	{
		aes.Dispose();
	}

	/// <summary>
	/// Vrátí nový náhodný klíč a náhodný IV (spojí je do jednoho pole bytů a serializuje jako Base64).
	/// Klíč je dlouhý 32 bytes, IV 16 bytes.
	/// </summary>
	public static string CreateRandomKeyAndIvAsBase64String()
	{
		using (Aes aes = new AesManaged())
		{
			aes.GenerateKey();
			aes.GenerateIV();
			Contract.Assert(aes.Key.Length == 32);
			Contract.Assert(aes.IV.Length == 16);
			return Convert.ToBase64String(aes.Key.Concat(aes.IV).ToArray());
		}
	}
}
