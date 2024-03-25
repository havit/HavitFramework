namespace Havit.Ares.Tests;

[TestClass]
[TestCategory("Ares")]
public class AresServicesTests
{
	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void AresService_GetEkonomickeSubjektyDleIco_BadParamNull()
	{
		// Act
		new AresService().GetEkonomickeSubjektyDleIco(null);

		// Assert by method attribute
	}

	[DataTestMethod]
	[DataRow("")]
	[DataRow("123")]
	[DataRow("1234567890")]
	[ExpectedException(typeof(ArgumentException), "Ico nemá předepsanou délku 8 znaků")]
	public void AresService_GetEkonomickeSubjektyDleIco_BadParamLength(string ico)
	{
		// Act
		new AresService().GetEkonomickeSubjektyDleIco(ico);

		// Assert by method attribute
	}

	[TestMethod]
	public void AresService_GetEkonomickeSubjektyDleIco_Basic()
	{
		// Arrange
		string ic = "27389731";

		// Act
		var ekonomickySubjekt = new AresService().GetEkonomickeSubjektyDleIco(ic);

		// Assert
		Assert.AreEqual(ic, ekonomickySubjekt.EkonomickySubjektAres.Ico);
	}

	[TestMethod]
	public async Task AresService_GetEkonomickeSubjektyDleIcoAsync_Basic()
	{
		// Arrange
		string ic = "25612697";

		// Act
		var ekonomickySubjekt = await new AresService().GetEkonomickeSubjektyDleIcoAsync(ic);

		// Assert
		Assert.IsNotNull(ekonomickySubjekt);
		Assert.AreEqual("HAVIT, s.r.o.", ekonomickySubjekt.EkonomickySubjektAres.ObchodniJmeno);
	}

	[TestMethod]
	public async Task AresService_GetEkonomickeSubjektyDleIcoAsync_NemaSidlo()
	{
		// Arrange
		string ic = "25601458";

		// Act
		var ekonomickySubjekt = await new AresService().GetEkonomickeSubjektyDleIcoAsync(ic);

		// Assert
		Assert.IsNotNull(ekonomickySubjekt);
		Assert.IsNull(ekonomickySubjekt.EkonomickySubjektAres.Sidlo);
	}

	[TestMethod]
	public void AresService_GetEkonomickeSubjektyDleIco_SubjektZanikl()
	{
		// Arrange
		string ic = "27732487";

		// Act
		var ekonomickySubjekt = new AresService().GetEkonomickeSubjektyDleIco(ic);

		// Assert
		Assert.IsNull(ekonomickySubjekt);
	}

	[TestMethod]
	public void AresService_GetEkonomickeSubjektyDleObchodnihoJmena_Basic()
	{
		// Arrange
		string obchodniJmeno = "ORCA";

		// Act
		var ekonomickeSubjekty = new AresService().GetEkonomickeSubjektyDleObchodnihoJmena(obchodniJmeno);

		// Assert
		Assert.IsTrue(ekonomickeSubjekty.PocetCelkem > 30);
	}


	[TestMethod]
	public async Task AresService_GetEkonomickeSubjektyDleObchodnihoJmenaAsync_Basic()
	{
		// Arrange
		string ObchodniJmeno = "HAVIT";

		// Act
		var ekonomickeSubjekty = await new AresService().GetEkonomickeSubjektyDleObchodnihoJmenaAsync(ObchodniJmeno);

		// Assert
		Assert.IsTrue(ekonomickeSubjekty.Items.Any(x => x.EkonomickySubjektAres.ObchodniJmeno == "HAVIT, s.r.o." && x.EkonomickySubjektAres.Ico == "25612697"));
	}


	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void AresService_GetEkonomickeSubjektyDleObchodnihoJmena_PrazdneObchodniJmeno()
	{
		// Act
		new AresService().GetEkonomickeSubjektyDleObchodnihoJmena(null);

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(ArgumentException))]
	public async Task AresService_GetEkonomickeSubjektyDleObchodnihoJmenaAsync_PrazdneObchodniJmeno()
	{
		// Act
		_ = await new AresService().GetEkonomickeSubjektyDleObchodnihoJmenaAsync("");

		// Assert by method attribute
	}

	[DataTestMethod]
	[DataRow("jedna, dva, tři", "jedna\r\n dva\n tři")]
	[DataRow("jedna a půl\n", "jednaapůl")]
	[DataRow(null, null)]
	[DataRow(null, "")]
	public void AresService_IsAddressEqual(string addresaDorucovaci, string adresaSidlo)
	{
		// Act + Assert
		Assert.IsTrue(AresService.IsAddressEqual(addresaDorucovaci, adresaSidlo));
	}

}

