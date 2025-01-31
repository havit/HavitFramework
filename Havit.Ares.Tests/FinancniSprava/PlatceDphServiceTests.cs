using Havit.Ares.FinancniSprava;

namespace Havit.Ares.Tests.FinancniSprava;

[TestClass]
[TestCategory("Ares_DPH")]
public class PlatceDphServiceTests
{
	[TestMethod]
	public void PlatceDphService_GetPlatceDph_DoesNotExist()
	{
		// Arrange
		string dic = "CZ11100011";
		PlatceDphService platceDphService = new PlatceDphService();

		// Act
		PlatceDphResult platceDphResult = platceDphService.GetPlatceDph(dic);

		// Assert
		Assert.IsNull(platceDphResult);
	}

	[DataTestMethod]
	[DataRow("CZ27389731", false)]
	[DataRow("CZ25836595", true)]
	public async Task PlatceDphService_GetPlatceDphAsync_Nespolehlivy(string dic, bool isNespolehlivy)
	{
		// Arrange
		PlatceDphResult platceDphResult = await new PlatceDphService().GetPlatceDphAsync(dic);

		// Assert
		Assert.IsNotNull(platceDphResult);
		Assert.AreEqual(dic, platceDphResult.Dic);
		Assert.AreEqual(isNespolehlivy, platceDphResult.IsNespolehlivy);
	}

	[TestMethod]
	public async Task AresService_GetPlatceDphAsync_ViceUctu()
	{
		// Arrange
		string dic = "CZ60193336";

		// Act
		PlatceDphResult platceDphResult = await new PlatceDphService().GetPlatceDphAsync(dic);

		// Assert
		Assert.IsNotNull(platceDphResult);
		Assert.AreEqual(dic, platceDphResult.Dic);
		Assert.IsTrue(platceDphResult.CislaUctu.Count() > 10);
	}

	[DataTestMethod]
	[DataRow(null)]
	[DataRow("")]
	[DataRow("123")]
	[DataRow("1234567")]
	[DataRow("CZ3456789")]
	[DataRow("CZ34567890122")]
	[DataRow("CZ<DELETE>")]
	[DataRow("<X12345678")]
	[DataRow("<<12345678")]
	[DataRow("123456780912")]
	[DataRow("123456780>")]
	[ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
	public void AresService_GetPlatceDph_BadInput(string dic)
	{
		// Act
		new PlatceDphService().GetPlatceDph(dic);

		// Assert by method attribute
	}
}
