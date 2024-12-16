using Havit.Services.StateAdministration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Tests.StateAdministrationServices;

[TestClass]
public class RodneCisloServiceTests
{
	[DataTestMethod]
	[DataRow(true, "7801233540")] // problematické, ale správné, viz http://phpfashion.com/jak-overit-platne-ic-a-rodne-cislo
	[DataRow(true, "780123/3540")] // problematické, ale správné, viz http://phpfashion.com/jak-overit-platne-ic-a-rodne-cislo
	[DataRow(false, "780123//3540")] // chybný formát
	[DataRow(false, "80123//3540")] // chybný formát
	[DataRow(true, "7909173030")]
	[DataRow(true, "790917/3030")]
	[DataRow(false, "7909173/030")] // chybný formát
	[DataRow(false, "79091/73030")] // chybný formát

	//[DataRow(true, "9950010004")] 
	//[DataRow(true, "995001/0004")]

	[DataRow(false, "404040404")]
	[DataRow(false, "404040/404")]
	[DataRow(false, "40404/4040")] // chybný formát
	[DataRow(false, "40404//4040")] // chybný formát
	[DataRow(false, "7909172990")]
	[DataRow(false, "7909172991")]
	[DataRow(false, "7909172992")]
	[DataRow(false, "7909172993")]
	[DataRow(false, "7909172994")]
	[DataRow(false, "7909172995")]
	[DataRow(false, "7909172996")]
	[DataRow(false, "7909172998")]
	[DataRow(false, "7909172999")]
	[DataRow(false, "790917/2990")]
	[DataRow(false, "790917/2991")]
	[DataRow(false, "790917/2992")]
	[DataRow(false, "790917/2993")]
	[DataRow(false, "790917/2994")]
	[DataRow(false, "790917/2995")]
	[DataRow(false, "790917/2996")]
	[DataRow(false, "790917/2998")]
	[DataRow(false, "790917/2999")]
	[DataRow(false, "991601/0004")]
	[DataRow(false, "990132/0004")]
	[DataRow(true, "000229/0002")] // přestupný rok
	[DataRow(false, "990229/0002")] // nepřestupný rok
	[DataRow(true, "992311/4927")]
	[DataRow(true, "017112/7154")] // přičteno 70 u měsíce
	[DataRow(true, "992301/0009")] // přičteno 20 u měsíce
	[DataRow(false, "992301/0008")]
	[DataRow(true, "505822/332")] // před rokem 1954
	public void RodneCisloServices_Validate_Samples(bool expected, string rodneCislo)
	{
		var result = RodneCisloServices.Validate(rodneCislo);
		Assert.AreEqual(expected, result);
	}

	[TestMethod]
	public void RodneCisloServices_Validate_NullValidatesFalse()
	{
		Assert.IsFalse(RodneCisloServices.Validate(null));
	}

	[TestMethod]
	public void RodneCisloServices_Validate_EmptyValidatesFalse()
	{
		Assert.IsFalse(RodneCisloServices.Validate(string.Empty));
	}

	[TestMethod]
	public void RodneCisloServices_Validate_WhitespaceValidatesFalse()
	{
		Assert.IsFalse(RodneCisloServices.Validate(" "));
	}
}
