using System.Security.Cryptography;

namespace Havit.Services.FileStorage.Infrastructure;

/// <summary>
/// InternalCryptoStream spolu s InternalCryptoTransform řeší situaci:
/// 1. Čteme data z CryptoStreamu
/// 2. Nedočteme do konce
/// 3. Je zavolán dispose
/// 4. Dispose volá TransformFinalBlock
/// 5. TransformFinalBlock vyhodí výjimku CryptographicException: Invalid padding...
/// 
/// Řešení spočívá v oznámení CryptoTransformu (z Dispose), že má ignorovat výjimky v TransformFinalBlock.
/// To však pouze při dešifrování a v režimu čtení (viz použití tříd).
/// </summary>
internal class InternalCryptoTransform : ICryptoTransform
{
	private readonly ICryptoTransform transform;
	private bool ignoreTransformFinalBlockExceptions = false;

	public int InputBlockSize => transform.InputBlockSize;

	public int OutputBlockSize => transform.OutputBlockSize;

	public bool CanTransformMultipleBlocks => transform.CanTransformMultipleBlocks;

	public bool CanReuseTransform => transform.CanReuseTransform;

	public InternalCryptoTransform(ICryptoTransform transform)
	{
		this.transform = transform;
	}

	public void IgnoreTransformFinalBlockExceptions()
	{
		ignoreTransformFinalBlockExceptions = true;
	}

	public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
	{
		return transform.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
	}

	public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
	{
		try
		{
			return transform.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
		}
		catch (CryptographicException) when (ignoreTransformFinalBlockExceptions)
		{
			return new byte[] { };
		}

	}

	public void Dispose()
	{
		transform.Dispose();
	}

}