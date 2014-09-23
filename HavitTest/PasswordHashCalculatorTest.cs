using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Havit.Security;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HavitTest
{
	/// <summary>
	/// Ověřuje správnost implementace třídy PasswordHashCalculator.
	/// </summary>
	[TestClass]
	public class PasswordHashCalculatorTest
	{
		#region GetSha512HashStringTest
		/// <summary>
		/// Ověřmuje správnost metody HashCalculator.ComputeSHA512HashString.
		/// </summary>
		[TestMethod]
		public void GetSha512HashStringTest()
		{
			// Expected hodnoty jsou získány z SQL Serveru pomocí HASHBYTES:
			// (pozor na N v parametru metody!)
			// SELECT HASHBYTES('SHA2_512', N'password')
			Assert.AreEqual("A7C6BF7982F92ED197AF475F194D3B750D0B26C0D555F3AAD6E00849320062CE4B0628038360725174962662E7BAE8E484ADF2AD20B20D2D3CA67EE3F7B9F856", PasswordHashCalculator.ComputeSHA512HashString("password"), true);
			Assert.AreEqual("ABE5E1A3FE41F5DA8F3E6C5B54D1ADB6073DF5D17BB8E3A575077F2A9D1AF2A6B22F50AA00336C35D0977EC4006272D6C7F8FF586FB4FCD9CA035894332D067F", PasswordHashCalculator.ComputeSHA512HashString("password", "password"), true);
			Assert.AreEqual("ACDDA85B31ACA524AF221BF8BB635583167414180D55985294A64E48E82796627CCCAE6CA4A3C3B2B568478B0265FE62753C37119B899BE7E632D434C8B2A54E", PasswordHashCalculator.ComputeSHA512HashString("password", "salt"), true);
			Assert.AreEqual("B3684C13DA380E2112EEFF968D845F98D390B4BC7910CB67D1F145E43BF2352F6202A09A2892285BC3BEFF03299823A3B0DB5B05BC9FFD0EC477B22AECCC4616", PasswordHashCalculator.ComputeSHA512HashString("1234", "5"), true);
			Assert.AreEqual("B2BC16A0106B7FF8EB014DBD210E9A370B967EFF38503A658D44D6A0DA95A60F7A73B4E65F93315AD34EB51E6695A087FB58E4E7A96AB7652D9BC42343B3F2AA", PasswordHashCalculator.ComputeSHA512HashString("123", "45"), true);
			Assert.AreEqual("1A57A2F34DFE0004A9CFE69DC2813A57ADCF84EBF5D50DA377D99329661E416541B8F67B5F81474A8C184D729B8CB876E1DCFAAB59ABE4A4B15DCC98549AD177", PasswordHashCalculator.ComputeSHA512HashString("12", "345"), true);
		}
		#endregion
	}
}
