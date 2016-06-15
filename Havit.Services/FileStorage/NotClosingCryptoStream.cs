using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Zoufalost dle  http://stackoverflow.com/questions/19736631/can-a-cryptostream-leave-the-base-stream-open
	/// </summary>
	internal class NotClosingCryptoStream : CryptoStream
	{
		public NotClosingCryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode) : base(stream, transform, mode)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (!HasFlushedFinalBlock)
			{
				FlushFinalBlock();
			}

			base.Dispose(false);
		}
	}
}
