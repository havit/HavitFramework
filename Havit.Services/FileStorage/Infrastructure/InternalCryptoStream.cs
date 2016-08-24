using System.IO;
using System.Security.Cryptography;

namespace Havit.Services.FileStorage.Infrastructure
{
	/// <summary>
	/// InternalCryptoStream spolu s InternalCryptoTransform řeší situaci:
	/// 1. Čteme data z CryptoStreamu
	/// 2. Nedočteme do konce
	/// 3. Je zavolán dispose
	/// 4. Dispose volá TransformFinalBlock
	/// 5. TransformFinalBlock vyhodí výjimku CryptographicException: Invalid padding...
	/// 
	/// Řešení spočívá v oznámení CryptoTransformu (z Dispose), že má ignorovat výjimky v TransformFinalBlock.
	/// To však pouze při dešifrování a v režimu čtení a pokud nejsme až na konci (viz použití tříd).
	/// </summary>
	internal class InternalCryptoStream : CryptoStream
	{
		private readonly ICryptoTransform cryptoTransform;

		private readonly CryptoStreamMode mode;

		public InternalCryptoStream(Stream stream, ICryptoTransform cryptoTransform, CryptoStreamMode mode) : base(stream, cryptoTransform, mode)
		{
			this.cryptoTransform = cryptoTransform;
			this.mode = mode;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (!disposed && (mode == CryptoStreamMode.Read) && (cryptoTransform is InternalCryptoTransform) && (this.CanRead) && (this.ReadByte() != -1))
				{
					((InternalCryptoTransform)cryptoTransform).IgnoreTransformFinalBlockExceptions();
					disposed = true;
				}
			}
			base.Dispose(disposing);
		}
		private bool disposed = false;
	}
}