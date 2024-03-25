using Havit.Ares.Ares;

namespace Havit.Ares.Tests.Ares;

[TestClass]
public class AresCiselnikTests
{
	[TestMethod]
	public void AresCiselnik_GetValue_SoudVr()
	{
		// Arrange
		AresCiselnik ciselnikRejstrikovySoud = new AresCiselnik("vr", "SoudVr");

		// Act
		string soud = ciselnikRejstrikovySoud.GetValue("MSPH");

		// Assert
		Assert.AreEqual("Městský soud v Praze", soud);
	}

	[TestMethod]
	public void AresCiselnik_GetValue_FinancniUrad()
	{
		// Arrange
		AresCiselnik ciselnikFinancniUred = new AresCiselnik("ufo", "FinancniUrad");

		// Act
		string uzemniPracoviste = ciselnikFinancniUred.GetValue("101");

		// Assert
		Assert.AreEqual("Územní pracoviště v Prachaticích", uzemniPracoviste);
	}

	[TestMethod]
	public void AresCiselnik_GetValue_PravniForma()
	{
		// Arrange
		AresCiselnik ciselnikPravniForma = new AresCiselnik("res", "PravniForma");

		// Act
		string pravniForma101 = ciselnikPravniForma.GetValue("101");
		string pravniForma102 = ciselnikPravniForma.GetValue("102");

		// Assert
		Assert.AreEqual("Fyzická osoba podnikající dle živnostenského zákona", pravniForma101);
		Assert.AreEqual("Fyzická osoba podnikající dle živnostenského zákona zapsaná v obchodním rejstříku", pravniForma102);
	}

	[TestMethod]
	public void AresCiselnik_GetValue_MissingCode()
	{
		// Arrange
		AresCiselnik CiselnikRejstrikovySoud = new AresCiselnik("ufo", "FinancniUrad");

		// Act
		string soud = CiselnikRejstrikovySoud.GetValue("___");
		string soudNull = CiselnikRejstrikovySoud.GetValue(null);
		string soudEmpty = CiselnikRejstrikovySoud.GetValue("");

		// Asert
		Assert.AreEqual(AresCiselnik.UnknownValue, soud);
		Assert.AreEqual(AresCiselnik.UnknownValue, soudNull);
		Assert.AreEqual(AresCiselnik.UnknownValue, soudEmpty);
	}
}
