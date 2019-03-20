using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Havit.PayMuzo
{
	/// <summary>
	/// Třída s pomocnými metodami pro implementaci platebního systému PayMUZO.
	/// </summary>
	public static class PayMuzoHelper
	{
		/// <summary>
		/// Vrátí <see cref="PayMuzoRequestData"/> s daty pro request CREATE_ORDER. Následně lze podepsat a použít pro GET (QueryString) nebo POST (form).
		/// </summary>
		/// <param name="merchantNumber">Přidělené číslo obchodníka</param>
		/// <param name="orderNumber">Pořadové číslo objednávky, číslo musí být v každém požadavku od obchodníka unikátní.</param>
		/// <param name="amount">Částka. Požadovaný převod na nejmenší jednotky dané měny se provede.</param>
		/// <param name="currency">Měna (Identifikátor měny dle ISO 4217).</param>
		/// <param name="depositFlag">Udává, zda má být objednávka uhrazena automaticky. <c>false</c> = není požadována úhrada, <c>true</c> = je požadována úhrada.</param>
		/// <param name="returnUrl">Plná URL adresa obchodníka. (včetně specifikace protokolu – např. https:// ) Na tuto adresu bude odeslán výsledek požadavku. V případě chybného podpisu dat se chybové hlášení zasílá zpět do internetového prohlížeče, ze kterého tento požadavek přišel.</param>
		/// <param name="merchantOrderNumber">Identifikace objednávky pro obchodníka. Zobrazí se na výpisu z banky. V případě, že není zadáno (null), použije se hodnota ORDERNUMBER.</param>
		/// <param name="description">Popis nákupu (nepovinný). Obsah pole se přenáší do 3-D systému pro možnost následné kontroly držitelem karty během autentikace u Access Control Serveru vydavatelské banky. Pole musí obsahovat pouze ASCII znaky v rozsahu 0x20 – 0x7E.</param>
		/// <param name="merchantData">Libovolná data obchodníka (nepovinné), která jsou vrácena obchodníkovi v odpovědi v nezměněné podobě. Pole se používá pro uspokojení rozdílných požadavků jednotlivých e shopů. Pole musí obsahovat pouze ASCII znaky v rozsahu 0x20 – 0x7E. Pokud je nezbytné přenášet jiná data, potom je zapotřebí použít BASE64 kódování. (viz. Dodatek Base64). Pole nesmí obsahovat osobní údaje. Výsledná délka dat může být maximálně 30 B.</param>
		/// <returns><see cref="PayMuzoRequestData"/> s daty pro request CREATE_ORDER. Následně lze použít pro GET (QueryString) nebo POST (form), avšak je nutno zachovat kódování (již nedělat UrlEncode) a nesmí se změnit pořadí.</returns>
		public static PayMuzoRequestData CreateOrder(
			ulong merchantNumber,
			int orderNumber,
			decimal amount,
			PayMuzoCurrency currency,
			bool depositFlag,
			string returnUrl,
			int? merchantOrderNumber,
			string description,
			string merchantData)
		{
			// ******************************************************************
			// POZOR !!!! ZÁLEŽÍ NA POŘADÍ POLOŽEK !!! ROZHODUJÍCÍ PRO PODPIS !!!
			// ******************************************************************

			PayMuzoRequestData request = new PayMuzoRequestData();

			// 1. MERCHANTNUMBER
			if (merchantNumber <= 0ul)
			{
				throw new ArgumentOutOfRangeException("merchantNumber", "merchantNumber musí být kladné číslo");
			}
			request.Add("MERCHANTNUMBER", merchantNumber.ToString(CultureInfo.InvariantCulture));

			// 2. OPERATION
			request.Add("OPERATION", "CREATE_ORDER");

			// 3. ORDERNUMBER 
			if (orderNumber < 0)
			{
				throw new ArgumentOutOfRangeException("orderNumber", "orderNumber musí být nezáporné číslo");
			}
			request.Add("ORDERNUMBER", orderNumber.ToString(CultureInfo.InvariantCulture));

			// 4. AMOUNT
			if (currency == null)
			{
				throw new ArgumentNullException("currency");
			}
			if (amount <= 0)
			{
				throw new ArgumentOutOfRangeException("amount", "amount musí být kladné číslo");
			}
			int amountInSmallestUnitsOfCurrency = ((int)Math.Floor(amount * currency.SmallestUnits));
			request.Add("AMOUNT", amountInSmallestUnitsOfCurrency.ToString(CultureInfo.InvariantCulture));

			// 5. CURRENCY
			request.Add("CURRENCY", currency.NumericCode.ToString(CultureInfo.InvariantCulture));

			// 6. DEPOSITFLAG
			request.Add("DEPOSITFLAG", depositFlag ? "1" : "0");

			// 7. MERORDERNUM
			if (merchantOrderNumber != null)
			{
				if (merchantOrderNumber.Value < 0)
				{
					throw new ArgumentOutOfRangeException("merchantOrderNumber", "merchantOrderNumber musí být kladné číslo");
				}
				request.Add("MERORDERNUM", merchantOrderNumber.Value.ToString(CultureInfo.InvariantCulture));

			}

			// 8. URL
			if (String.IsNullOrEmpty(returnUrl))
			{
				throw new ArgumentException("returnUrl nesmí být null ani String.Empty", nameof(returnUrl));
			}
			request.Add("URL", returnUrl);

			// 9. DESCRIPTION
			if (!String.IsNullOrEmpty(description))
			{
				// pole smí obsahovat pouze ASCII znaku 0x20 - 0x7E
				for (int i = 0; i < description.Length; i++)
				{
					int numericValue = description[i];
					if ((numericValue > 0x7E) || (numericValue < 0x20))
					{
						throw new ArgumentException("description smí obsahovat pouze ASCII znaky 0x20 - 0x7E.", nameof(description));
					}
				}
				request.Add("DESCRIPTION", description);
			}

			// 10. MD
			if (!String.IsNullOrEmpty(merchantData))
			{
				// pole smí být maximálně 30B dlouhé
				if (merchantData.Length > 30)
				{
					throw new ArgumentException("Maximální velikost pole merchantData je 30 znaků.", nameof(merchantData));
				}

				// pole smí obsahovat pouze ASCII znaku 0x20 - 0x7E
				for (int i = 0; i < merchantData.Length; i++)
				{
					int numericValue = merchantData[i];
					if ((numericValue > 0x7E) || (numericValue < 0x20))
					{
						throw new ArgumentException("merchantData smí obsahovat pouze ASCII znaky 0x20 - 0x7E. Použijte BASE64 kódování.", nameof(merchantData));
					}
				}
				request.Add("MD", merchantData);
			}

			return request;
		}

		/// <summary>
		/// Podepíše data requestu a přidá podpis do requestu.
		/// </summary>
		/// <param name="requestData">request k podepsání</param>
		/// <param name="certificate">certifikát s veřejným i privátním klíčem</param>
		public static void AddDigestToRequest(PayMuzoRequestData requestData, X509Certificate2 certificate)
		{
			if (requestData == null)
			{
				throw new ArgumentNullException(nameof(requestData));
			}

			if (certificate == null)
			{
				throw new ArgumentNullException(nameof(certificate));
			}

			string rawData = requestData.GetPipedRawData();

			string digest = CreateDigest(rawData, certificate, true);

			requestData.Add("DIGEST", digest);
		}

		/// <summary>
		/// Podepíše data (pipe-delimitted) a vrátí podpis (již UrlEncoded!).
		/// </summary>
		/// <param name="data">data k podepsání</param>
		/// <param name="certificate">certifikát s privátním i veřejným klíčem</param>
		/// <param name="urlEncode">indikuje, zdali má být podpis UrlEncoded</param>
		/// <returns>podpis</returns>
		public static string CreateDigest(string data, X509Certificate2 certificate, bool urlEncode)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException(nameof(certificate));
			}
			if (String.IsNullOrEmpty(data))
			{
				throw new ArgumentException("Argument data nesmí být null ani String.Empty.", nameof(data));
			}

			RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PrivateKey;

			string digest = string.Empty;

			// hash má být SHA-1
			HashAlgorithm hashAlg = HashAlgorithm.Create("SHA1");

			// hash má být zakódován pomocí EMSA-PKCS1
			AsymmetricSignatureFormatter signFormatter = new RSAPKCS1SignatureFormatter(rsa);
			signFormatter.SetHashAlgorithm("SHA1");

			// podpis RSASSA-PKCS1
//			byte[] signedData = signFormatter.CreateSignature(hashAlg.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data)));
			byte[] signedData = signFormatter.CreateSignature(hashAlg.ComputeHash(System.Text.Encoding.GetEncoding(1250).GetBytes(data)));

			// výstup má být zakódován pomocí BASE64
			string base64EncodedData = Convert.ToBase64String(signedData);

			// podpis se má předávat UrlEncoded
			if (urlEncode)
			{
				string urlEncodedData = System.Web.HttpUtility.UrlEncode(base64EncodedData);

				digest = urlEncodedData;
			}
			else
			{
				digest = base64EncodedData;
			}

			// podpis je nutné před použitím ověřit
			if (!VerifyDigest(data, digest, certificate, urlEncode))
			{
				throw new CryptographicException("Vytvořený podpis se nepodařilo ověřit.");
			}

			// a hotovo
			return digest;
		}

		/// <summary>
		/// Ověří vygenerovaný podpis.
		/// </summary>
		/// <param name="data">data, z kterých byl podpis vytvořen</param>
		/// <param name="digest">podpis, který byl z dat vytvořen</param>
		/// <param name="certificate">certifikát s veřejným klíčem</param>
		/// <param name="urlEncoded">indikuje, zdali je podpis UrlEncoded</param>
		/// <returns><c>true</c>, pokud podpis odpovídá; jinak <c>false</c></returns>
		public static bool VerifyDigest(string data, string digest, X509Certificate2 certificate, bool urlEncoded)
		{
			// Pokud není zadán žádný podpis, není platný automaticky (mj. řeší podvrhy).
			// Také se stalo, že banka vrátila chybu Technical Reason jako důvod neprovedení platby a zpráva nebyla podepsána.
			if (String.IsNullOrEmpty(digest))
			{
				return false;
			}

			string urlDecodedDigest = digest;
			if (urlEncoded)
			{
				urlDecodedDigest = System.Web.HttpUtility.UrlDecode(digest);
			}

			// dekódujeme UrlDecode a BASE64
			byte[] decodedDigest = Convert.FromBase64String(urlDecodedDigest);

			RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)certificate.PublicKey.Key;

			// formater RSAPKCS1
			RSAPKCS1SignatureDeformatter pkcs1SignDef = new RSAPKCS1SignatureDeformatter(rsa);
			pkcs1SignDef.SetHashAlgorithm("SHA1");

			// hash SHA-1
			HashAlgorithm hashAlg = HashAlgorithm.Create("SHA1");
//			byte[] dataHash = hashAlg.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
			byte[] dataHash = hashAlg.ComputeHash(System.Text.Encoding.GetEncoding(1250).GetBytes(data));

			// verifikace
			return pkcs1SignDef.VerifySignature(dataHash, decodedDigest);
		}

		/// <summary>
		/// Z textového řetězce ve formátu query-stringu vyextrahuje raw-data.
		/// </summary>
		/// <param name="rawQueryString">textový řetězec ve formátu query-string</param>
		public static PayMuzoRequestData ExtractRawDataFromRawQueryString(string rawQueryString)
		{
			PayMuzoRequestData data = new PayMuzoRequestData();

			MatchCollection matches = Regex.Matches(rawQueryString, @"(?<=(\?|&))(?<NAME>\w+)=(?<VALUE>[^&]+)(?=(&|$))");
			foreach (Match match in matches)
			{
				if (String.Compare(match.Groups["NAME"].Value, "RESULTTEXT", true) == 0)
				{
					// RESULTTEXT je ve Windows-1250 a UrlEncoded
					data.Add(match.Groups["NAME"].Value, System.Web.HttpUtility.UrlDecode(match.Groups["VALUE"].Value, Encoding.GetEncoding(1250)));
				}
				else
				{
					data.Add(match.Groups["NAME"].Value, match.Groups["VALUE"].Value);
				}
			}
			return data;
		}
	}
}
