using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Ares.Tests;

[TestClass]
public class PlatceDphTests
{

	[TestMethod]
	[TestCategory("Ares_DPH")]
	public void PlatceDphService_GetPlatceDph_NoExists()
	{
		string dic = "CZ11100011";
		PlatceDphService platceDphService = new PlatceDphService();
		PlatceDphResponse platceDphResponse = platceDphService.GetPlatceDph(dic);
		Assert.IsNull(platceDphResponse);
	}

	[DataTestMethod]
	[DataRow("CZ27389731", true)]
	[DataRow("CZ25836595", false)]
	[TestCategory("Ares_DPH")]
	public async Task AresService_GetPlatceDph_Async(string dic, bool isSpolehlivy)
	{
		// act
		PlatceDphResponse platceDphResponse = await new PlatceDphService().GetPlatceDphAsync(dic);

		// Assert
		Assert.IsNotNull(platceDphResponse);
		Assert.AreEqual(platceDphResponse.Dic, dic);
		Assert.AreEqual(platceDphResponse.IsSpolehlivy, isSpolehlivy);
	}

	[TestMethod]
	[TestCategory("Ares_DPH")]
	public async Task AresService_GetPlatceDph_O2()
	{
		// Arrange
		string dic = "CZ60193336";

		// Act
		PlatceDphResponse platceDphResponse = await new PlatceDphService().GetPlatceDphAsync(dic);

		// Assert
		Assert.IsNotNull(platceDphResponse);
		Assert.AreEqual(platceDphResponse.Dic, dic);
		Assert.IsTrue(platceDphResponse.CislaUctu.Count() > 10);
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
	[TestCategory("Ares_DPH")]
	[ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
	public void AresService_GetPlatceDph_BadInput(string dic)
	{
		// Act
		new PlatceDphService().GetPlatceDph(dic);

		// Assert -> Must be Exception
	}
}
