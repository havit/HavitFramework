namespace Havit.Web.Tests;

[TestClass]
public class QueryStringBuilderTests
{
	[TestMethod]
	public void QueryStringBuilder_Add_Null_ThrowsException()
	{
		// Arrange
		QueryStringBuilder target = new QueryStringBuilder();

		// Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			// Act
			target.Add(null, "cokoliv");
		});
	}

	[TestMethod]
	public void QueryStringBuilder_Set_Null_ThrowsException()
	{
		// Arrange
		QueryStringBuilder target = new QueryStringBuilder();

		// Assert
		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			// Act
			target.Set(null, "cokoliv");
		});
	}

	[TestMethod]
	public void QueryStringBuilder_ToString_Simple()
	{
		QueryStringBuilder target = new QueryStringBuilder();
		target.Add("key1", "value1");
		target.Add("key2", "value2");

		string expected = "key1=value1&key2=value2";
		string actual;

		actual = target.ToString();

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void QueryStringBuilder_ToString_EncodeSpace()
	{
		QueryStringBuilder target = new QueryStringBuilder();
		target.Add("key", "value with space");

		string expected = "key=value+with+space";
		string actual;

		actual = target.ToString();

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void QueryStringBuilder_ToString_EncodeText()
	{
		QueryStringBuilder target = new QueryStringBuilder();
		target.Add("key", "ěščřžýáíéúů");

		string expected = "key=%C4%9B%C5%A1%C4%8D%C5%99%C5%BE%C3%BD%C3%A1%C3%AD%C3%A9%C3%BA%C5%AF".ToLower();
		string actual;

		actual = target.ToString();

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void QueryStringBuilder_ToString_EncodeAmpersand()
	{
		QueryStringBuilder target = new QueryStringBuilder();
		target.Add("key", "text1&text2");

		string expected = "key=text1%26text2";
		string actual;

		actual = target.ToString();

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void QueryStringBuilder_GetUrlWithQueryString_Simple()
	{
		QueryStringBuilder target = new QueryStringBuilder();
		target.Add("key", "value");

		string url = "foo.aspx";

		string expected = "foo.aspx?key=value";
		string actual;

		actual = target.GetUrlWithQueryString(url);

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void QueryStringBuilder_GetUrlWithQueryString_ExistingQueryStringSimple()
	{
		QueryStringBuilder target = new QueryStringBuilder();
		target.Add("key", "value");

		string url = "foo.aspx?key1=value1";

		string expected = "foo.aspx?key1=value1&key=value";
		string actual;

		actual = target.GetUrlWithQueryString(url);

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void QueryStringBuilder_GetUrlWithQueryString_ExistingQueryStringWithAmpersand()
	{
		QueryStringBuilder target = new QueryStringBuilder();
		target.Add("key", "value");

		string url = "foo.aspx?key1=value1&";

		string expected = "foo.aspx?key1=value1&key=value";
		string actual;

		actual = target.GetUrlWithQueryString(url);

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void QueryStringBuilder_GetUrlWithQueryString_UrlWithQuestionMark()
	{
		QueryStringBuilder target = new QueryStringBuilder();
		target.Add("key", "value");

		string url = "foo.aspx?";

		string expected = "foo.aspx?key=value";
		string actual;

		actual = target.GetUrlWithQueryString(url);

		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void QueryStringBuilder_FillFromString_Simple()
	{
		QueryStringBuilder target = new QueryStringBuilder();

		string queryString = "key1=value1&key2=value2";

		target.FillFromString(queryString);

		Assert.AreEqual("value1", target["key1"]);
		Assert.AreEqual("value2", target["key2"]);
		Assert.HasCount(2, target);
		Assert.AreEqual(queryString, target.ToString());
	}

	[TestMethod]
	public void QueryStringBuilder_FillFromString_UrlEncoded()
	{
		QueryStringBuilder target = new QueryStringBuilder();

		string queryString = "key=%C4%9B%C5%A1%C4%8D%C5%99%C5%BE%C3%BD%C3%A1%C3%AD%C3%A9%C3%BA%C5%AF";

		target.FillFromString(queryString);

		Assert.AreEqual("ěščřžýáíéúů", target["key"]);
	}

	[TestMethod]
	public void QueryStringBuilder_FillFromString_Ampersands()
	{
		QueryStringBuilder target = new QueryStringBuilder();

		string queryString = "&key1=value1&&key2=value2&";

		target.FillFromString(queryString);

		Assert.AreEqual("value1", target["key1"]);
		Assert.AreEqual("value2", target["key2"]);
		Assert.HasCount(2, target);
		Assert.AreEqual("key1=value1&key2=value2", target.ToString());
	}

	[TestMethod]
	public void QueryStringBuilder_FillFromString_EmptyValue()
	{
		QueryStringBuilder target = new QueryStringBuilder();

		string queryString = "key1=";

		target.FillFromString(queryString);

		Assert.AreEqual(String.Empty, target["key1"]);
		Assert.HasCount(1, target);
		Assert.AreEqual("key1=", target.ToString());
	}

	[TestMethod]
	public void QueryStringBuilder_FillFromString_EmptyInput()
	{
		QueryStringBuilder target = new QueryStringBuilder();

		string queryString = "";

		target.FillFromString(queryString);

		Assert.IsEmpty(target);
		Assert.AreEqual(String.Empty, target.ToString());
	}

	[TestMethod]
	public void QueryStringBuilder_FillFromString_NullInput()
	{
		QueryStringBuilder target = new QueryStringBuilder();

		string queryString = null;

		target.FillFromString(queryString);

		Assert.IsEmpty(target);
		Assert.AreEqual(String.Empty, target.ToString());
	}

	/// <summary>
	/// Test reprodukující chybu, při které se vracela URL adresa končící znakem '?' v okamžku, kdy je kolekce parametrů prázdná.
	/// </summary>
	[TestMethod]
	public void QueryStringBuilder_GetUrlQueryString_EmptyQuery()
	{
		// Arrange
		string url = "http://www.google.com";

		QueryStringBuilder qsb = new QueryStringBuilder();

		// Act
		string result = qsb.GetUrlWithQueryString(url);

		// Assert
		Assert.AreEqual(url, result);
	}
}
