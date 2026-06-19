namespace Havit.Core.Tests;

[TestClass]
public class EnumExtTests
{
	[TestMethod]
	public void EnumExt_GetDescription_ReturnsDescription()
	{
		Assert.AreEqual("red", EnumExt.GetDescription(typeof(Colors), Colors.Red));
		Assert.AreEqual("blue", EnumExt.GetDescription(typeof(Colors), Colors.Blue));
	}

	[TestMethod]
	public void EnumExt_GetDescription_MissingDescriptionAttributeReturnsEmptyString()
	{
		Assert.AreEqual("", EnumExt.GetDescription(typeof(Colors), Colors.Green));
	}

	[TestMethod]
	public void EnumExt_GetDescription_UndefinedValueReturnsEmptyString()
	{
		Assert.AreEqual("", EnumExt.GetDescription(typeof(Colors), 999));
	}

	private enum Colors
	{
		[System.ComponentModel.Description("red")]
		Red,

		[System.ComponentModel.Description("blue")]
		Blue,

		Green
	}
}
