using System.Text;

namespace Havit.Web.Tests;

[TestClass]
public class HttpUtilityExtTests
{
	[TestMethod]
	public void HttpUtilityExt_UrlEncodeNonAscii_NullOrEmpty()
	{
		Assert.IsNull(HttpUtilityExt.UrlEncodeNonAscii(null, Encoding.UTF8));
		Assert.AreEqual("", HttpUtilityExt.UrlEncodeNonAscii("", Encoding.UTF8));
	}

	[TestMethod]
	public void HttpUtilityExt_UrlEncodeNonAscii_AsciiTextIsNotChanged()
	{
		Assert.AreEqual("/path/page.aspx?a=1&b=x y", HttpUtilityExt.UrlEncodeNonAscii("/path/page.aspx?a=1&b=x y", Encoding.UTF8));
	}

	[TestMethod]
	public void HttpUtilityExt_UrlEncodeNonAscii_Utf8()
	{
		// á = C3 A1, ž = C5 BE
		Assert.AreEqual("/str%c3%a1nka?q=%c5%be&r=1", HttpUtilityExt.UrlEncodeNonAscii("/stránka?q=ž&r=1", Encoding.UTF8));
	}

	[TestMethod]
	public void HttpUtilityExt_UrlEncodeNonAscii_NullEncodingUsesUtf8()
	{
		Assert.AreEqual("%c5%be", HttpUtilityExt.UrlEncodeNonAscii("ž", null));
	}

	[TestMethod]
	public void HttpUtilityExt_UrlEncodeNonAscii_Windows1250()
	{
		// č = E8 ve Windows-1250
		Assert.AreEqual("?q=%e8", HttpUtilityExt.UrlEncodeNonAscii("?q=č", Encoding.GetEncoding(1250)));
	}

	[TestMethod]
	public void HttpUtilityExt_UrlEncodeNonAscii_LongText()
	{
		string text = String.Concat(Enumerable.Repeat("a=ž&", 1000));
		string expected = String.Concat(Enumerable.Repeat("a=%c5%be&", 1000));

		Assert.AreEqual(expected, HttpUtilityExt.UrlEncodeNonAscii(text, Encoding.UTF8));
	}

	[TestMethod]
	public void HttpUtilityExt_UrlEncodeBytesToBytesNonAscii_AsciiReturnsSameInstance()
	{
		byte[] bytes = Encoding.ASCII.GetBytes("abc");
		Assert.AreSame(bytes, HttpUtilityExt.UrlEncodeBytesToBytesNonAscii(bytes));
	}

	[TestMethod]
	public void HttpUtilityExt_UrlEncodeBytesToBytesNonAscii_EncodesNonAsciiBytes()
	{
		byte[] actual = HttpUtilityExt.UrlEncodeBytesToBytesNonAscii(new byte[] { 0x61, 0xC5, 0xBE });
		Assert.AreEqual("a%c5%be", Encoding.ASCII.GetString(actual));
	}
}
