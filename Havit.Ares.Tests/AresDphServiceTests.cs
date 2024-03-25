using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Diagnostics.Contracts;

namespace Havit.Ares.Tests;

[TestClass]
public class AresDphServiceTests
{
	[TestMethod]
	[TestCategory("AresDph")]
	public void AresDphService_GetAresAndPlatceDph_Basic()
	{
		// Arrange
		string ic = "27389731";
		// 		ic = "46003703";		Ico ktere vzniklo dnes (4.3.2024) a jeste nema Pravni formu (resp = 100 unknown).
		ic = "45000115";

		// Act
		AresDphResponse aresDphResponse = new AresDphService().GetAresAndPlatceDph(ic);

		// Assert
		Assert.AreEqual(ic, aresDphResponse.EkonomickySubjektItem.EkonomickySubjektAres.Ico);
	}


	[TestMethod]
	[TestCategory("AresDph")]
	public void AresService_GetAresAndPlatceDph_OsvcNeplatce()
	{
		// Arrange
		string ic = "42081181";     //  DIC "CZ6006060126", není plátce DPH

		// Act
		AresDphResponse aresDphResponse = new AresDphService().GetAresAndPlatceDph(ic);

		// Assert
		Assert.AreEqual(aresDphResponse.EkonomickySubjektItem.EkonomickySubjektAres.Ico, "42081181");
		Assert.AreEqual(aresDphResponse.PlatceDphElement.Dic, "6006060126");
	}
}
