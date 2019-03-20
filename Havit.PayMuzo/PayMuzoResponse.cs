using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Třída pro strong-type reprezentaci odpovědí z PayMUZO.
	/// </summary>
	public abstract class PayMuzoResponse
	{
		/// <summary>
		/// Podpis.
		/// </summary>
		public string Digest
		{
			get
			{
				return _digest;
			}
			protected set
			{
				_digest = value;
			}
		}
		private string _digest;

		/// <summary>
		/// Primární návratový kód.
		/// </summary>
		public PayMuzoPrimaryReturnCode PrimaryReturnCode
		{
			get { return _primaryReturnCode; }
			protected set { _primaryReturnCode = value; }
		}
		private PayMuzoPrimaryReturnCode _primaryReturnCode;

		/// <summary>
		/// Sekundární návratový kód.
		/// </summary>
		public PayMuzoSecondaryReturnCode SecondaryReturnCode
		{
			get { return _secondaryReturnCode; }
			protected set { _secondaryReturnCode = value; }
		}
		private PayMuzoSecondaryReturnCode _secondaryReturnCode;

		/// <summary>
		/// Data v normalizované podobě (správné pořadí, všechny parametry).
		/// </summary>
		public PayMuzoRequestData NormalizedRawData
		{
			protected set { _normalizedRawData = value; }
			get { return _normalizedRawData; }
		}
		private PayMuzoRequestData _normalizedRawData;

		/// <summary>
		/// Gets a value indicating whether this instance is digest URL encoded.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is digest URL encoded; otherwise, <c>false</c>.
		/// </value>
		public abstract bool IsDigestUrlEncoded { get; }

		/// <summary>
		/// Ověří správnost podpisu.
		/// </summary>
		/// <param name="certificate">certifikát, vůči kterému má být podpis ověřen (veřejný klíč PayMUZO)</param>
		/// <returns><c>true, pokud je podpis korektní</c>, jinak <c>false</c></returns>
		public bool VerifyDigest(X509Certificate2 certificate)
		{
			return PayMuzoHelper.VerifyDigest(this.NormalizedRawData.GetPipedRawData(), this.Digest, certificate, this.IsDigestUrlEncoded);
		}
	}
}
