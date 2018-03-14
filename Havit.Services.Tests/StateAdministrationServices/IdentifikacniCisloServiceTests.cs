using Havit.Services.StateAdministration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Tests.StateAdministrationServices
{
	[TestClass]
	public class IdentifikacniCisloServiceTests
	{
		[TestMethod]
		public void IdentifikacniCisloValidateTest()
		{
			Assert.IsTrue(IdentifikacniCisloServices.Validate("25612697"));  // HAVIT
			Assert.IsTrue(IdentifikacniCisloServices.Validate("73381543"));  // JK
			Assert.IsTrue(IdentifikacniCisloServices.Validate("64509061"));  // IČO, na které si z Edenredu stěžují, že neprocházelo
			Assert.IsTrue(IdentifikacniCisloServices.Validate("69663963"));  // problematické podle stránek s algoritmem (http://phpfashion.com/jak-overit-platne-ic-a-rodne-cislo)
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612690"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612691"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612692"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612693"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612694"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612695"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612696"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612698"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("25612699"));
			Assert.IsFalse(IdentifikacniCisloServices.Validate("0"));
		}
	}
}
