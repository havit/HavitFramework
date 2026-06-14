using System.Globalization;
using System.Text;

namespace Havit.Tests;

[TestClass]
public class StringExtTests
{
	[TestMethod]
	public void StringExt_NormalizeForUrl_BasicScenario()
	{
		// arrange
		string text = "Ahoj Máňo, jak se máš?";
		string expected = "ahoj-mano-jak-se-mas";

		// act
		var actual = StringExt.NormalizeForUrl(text);

		// assert
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void StringExt_RemoveDiacritics()
	{
		Assert.AreEqual("escrzyaie", StringExt.RemoveDiacritics("ěščřžýáíé"));
		Assert.AreEqual("Prilis zlutoucky kun upel dabelske ody", StringExt.RemoveDiacritics("Příliš žluťoučký kůň úpěl ďábelské ódy"));
	}

	[TestMethod]
	public void StringExt_RemoveDiacritics_ResultIsInFormC()
	{
		string result = StringExt.RemoveDiacritics("Příliš žluťoučký kůň");

		// výsledek musí být v normalizační formě C (znaky nesmí zůstat dekomponované)
		Assert.AreEqual(result.Normalize(NormalizationForm.FormC), result);
		Assert.IsTrue(result.IsNormalized(NormalizationForm.FormC));
	}

	[TestMethod]
	public void StringExt_IntToHex_ReturnsLowercase()
	{
		Assert.AreEqual('0', StringExt.IntToHex(0));
		Assert.AreEqual('9', StringExt.IntToHex(9));
		Assert.AreEqual('a', StringExt.IntToHex(10));
		Assert.AreEqual('f', StringExt.IntToHex(15));
	}

	[TestMethod]
	public void StringExt_NormalizeForUrl_IsCultureInsensitive()
	{
		// arrange
		CultureInfo originalCulture = CultureInfo.CurrentCulture;
		try
		{
			// turecká kultura převádí 'I' na tečkované/netečkované varianty mimo A-Za-z
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR");

			// act
			var actual = StringExt.NormalizeForUrl("Istanbul");

			// assert
			Assert.AreEqual("istanbul", actual);
		}
		finally
		{
			CultureInfo.CurrentCulture = originalCulture;
		}
	}
}
