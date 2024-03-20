namespace Havit.Ares.Tests;

[TestClass]
public class AresCiselnikTests
{
	[TestMethod]
	public void AresService_AresCiselnik_SoudVr()
	{
		// Arrange
		AresCiselnik ciselnikRejstrikovySoud = new AresCiselnik("vr", "SoudVr");

		// Act
		string soud = ciselnikRejstrikovySoud.GetValue("MSPH");

		// Assert
		Assert.AreEqual(soud, "Městský soud v Praze");
	}

	[TestMethod]
	public void AresService_AresCiselnik_FinancniUrad()
	{
		// Arrange
		AresCiselnik ciselnikFinancniUred = new AresCiselnik("ufo", "FinancniUrad");

		// Act
		string uzemniPracoviste = ciselnikFinancniUred.GetValue("101");

		// Assert
		Assert.AreEqual(uzemniPracoviste, "Územní pracoviště v Prachaticích");
	}

	[TestMethod]
	public void AresService_AresCiselnik_PravniForma()
	{
		// Arrange
		AresCiselnik ciselnikPravniForma = new AresCiselnik("res", "PravniForma");

		// Act
		string pravniForma101 = ciselnikPravniForma.GetValue("101");
		string pravniForma102 = ciselnikPravniForma.GetValue("102");

		// Assert
		Assert.AreEqual(pravniForma101, "Fyzická osoba podnikající dle živnostenského zákona");
		Assert.AreEqual(pravniForma102, "Fyzická osoba podnikající dle živnostenského zákona zapsaná v obchodním rejstříku");
	}

	[TestMethod]
	public void AresService_AresCiselnik_FinancniUradBadCode()
	{
		// Arrange
		AresCiselnik CiselnikRejstrikovySoud = new AresCiselnik("ufo", "FinancniUrad");

		// Act
		string soud = CiselnikRejstrikovySoud.GetValue("___");
		string soudNull = CiselnikRejstrikovySoud.GetValue(null);
		string soudEmpty = CiselnikRejstrikovySoud.GetValue("");

		// Asert
		Assert.AreEqual(soud, AresCiselnik.UnknownValue);
		Assert.AreEqual(soudNull, AresCiselnik.UnknownValue);
		Assert.AreEqual(soudEmpty, AresCiselnik.UnknownValue);
	}


}
