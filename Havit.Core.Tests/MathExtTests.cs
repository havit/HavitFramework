namespace Havit.Tests;

[TestClass]
public class MathExtTests
{
	[TestMethod]
	public void MathExt_IsInteger_ReturnsFalseForNull()
	{
		// arange
		string text = null;
		bool expected = false;

		// act
		var actual = Havit.MathExt.IsInteger(text);

		// assert
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void MathExt_IsInteger_ReturnsTrueForPositiveInteger()
	{
		// arrange
		string text = "156";
		bool expected = true;

		// act
		var actual = Havit.MathExt.IsInteger(text);

		// assert
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void MathExt_IsInteger_ReturnsTrueForNegativeInteger()
	{
		// arrange
		string text = "-156";
		bool expected = true;

		// act
		var actual = Havit.MathExt.IsInteger(text);

		// assert
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void MathExt_IsIntegerTest_ReturnsFalseForEmptyString()
	{
		// arrange
		string text = String.Empty;
		bool expected = false;

		// act
		var actual = Havit.MathExt.IsInteger(text);

		// assert
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void MathExt_IsIntegerTest_ReturnsFalseForInvalidValue()
	{
		// arrange
		string text = "10.";
		bool expected = false;

		// act
		var actual = Havit.MathExt.IsInteger(text);

		// assert
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void MathExt_RoundToMultiple()
	{
		// double
		Assert.AreEqual(10.0, MathExt.RoundToMultiple(11.0, 5.0));
		Assert.AreEqual(15.0, MathExt.RoundToMultiple(13.0, 5.0));

		// double, int multiple
		Assert.AreEqual(10, MathExt.RoundToMultiple(11.0, 5));
		Assert.AreEqual(15, MathExt.RoundToMultiple(13.0, 5));

		// decimal
		Assert.AreEqual(10M, MathExt.RoundToMultiple(11M, 5M));
		Assert.AreEqual(15M, MathExt.RoundToMultiple(13M, 5M));
		Assert.AreEqual(0.3M, MathExt.RoundToMultiple(0.26M, 0.1M));
	}

	[TestMethod]
	public void MathExt_RoundToMultiple_MidpointRoundsToEvenMultiple()
	{
		// dokumentované chování: hodnota přesně uprostřed mezi dvěma násobky se zaokrouhluje k sudému násobku (bankéřské zaokrouhlení)
		Assert.AreEqual(2.0, MathExt.RoundToMultiple(2.5, 1.0));
		Assert.AreEqual(4.0, MathExt.RoundToMultiple(3.5, 1.0));
		Assert.AreEqual(2, MathExt.RoundToMultiple(2.5, 1));
		Assert.AreEqual(0.2M, MathExt.RoundToMultiple(0.25M, 0.1M));
		Assert.AreEqual(0.4M, MathExt.RoundToMultiple(0.35M, 0.1M));
	}
}
