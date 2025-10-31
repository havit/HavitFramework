namespace Havit.Ares.Tests;

[TestClass]
[TestCategory("Ares")]
public class AresServicesTests
{
	[TestMethod]
	public void AresService_GetEkonomickeSubjektyDleIco_BadParamNull()
	{
		// Arrange
		AresService aresService = new AresService();

		// Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
		{
			// Act
			aresService.GetEkonomickeSubjektyDleIco(null);
		});
	}

	[TestMethod]
	[DataRow("")]
	[DataRow("123")]
	[DataRow("1234567890")]
	public void AresService_GetEkonomickeSubjektyDleIco_BadParamLength(string ico)
	{
		// Arrange
		AresService aresService = new AresService();

		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			// Act
			aresService.GetEkonomickeSubjektyDleIco(ico);
		}, "Ico nemá předepsanou délku 8 znaků");
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

	// Padal na to program, tak jsem udělal tento test, ale 15.5.2024 někdo doplnil adresu. Tím se stal zbytečným
	[Ignore]
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
		Assert.IsGreaterThan(30, ekonomickeSubjekty.PocetCelkem);
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
	public void AresService_GetEkonomickeSubjektyDleObchodnihoJmena_PrazdneObchodniJmeno()
	{
		// Arrange
		AresService aresService = new AresService();

		// Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
		{
			// Act
			aresService.GetEkonomickeSubjektyDleObchodnihoJmena(null);
		});
	}

	[TestMethod]
	public async Task AresService_GetEkonomickeSubjektyDleObchodnihoJmenaAsync_PrazdneObchodniJmeno()
	{
		// Arrange
		AresService aresService = new AresService();

		await Assert.ThrowsExactlyAsync<ArgumentException>(async () =>
		{
			// Act
			await aresService.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("");
		});
	}

	[TestMethod]
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

