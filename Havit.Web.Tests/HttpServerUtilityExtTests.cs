using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Web.Tests;

[TestClass]
public class HttpServerUtilityExtTests
{
	[TestMethod]
	public void ResolveUrlTest_Standard()
	{
		string appPath = "http://www.havit.cz/app";
		string url = "~/path";
		string expected = "http://www.havit.cz/app/path";
		string actual;
		actual = HttpServerUtilityExt.ResolveUrl(appPath, url);
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void ResolveUrlTest_SlashEndingAppPath()
	{
		string appPath = "http://www.havit.cz/app/";
		string url = "~/path";
		string expected = "http://www.havit.cz/app/path";
		string actual;
		actual = HttpServerUtilityExt.ResolveUrl(appPath, url);
		Assert.AreEqual(expected, actual);
	}
}