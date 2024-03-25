using System.Diagnostics;
using System.Drawing;
using Havit.Diagnostics.Contracts;
using Havit.Ares;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Ares.Tests;

[TestClass]
public class AresServicesTests
{
	[TestMethod]
	[TestCategory("Ares")]
	[ExpectedException(typeof(ArgumentNullException))]
	public void AresService_GetEkonomickeSubjektyFromIco_BadParamNull()
	{
		// Act
		new AresService().GetEkonomickeSubjektyDleIco(null);
		// Assert -> must be Exception
	}

	[DataTestMethod]
	[DataRow("")]
	[DataRow("123")]
	[DataRow("1234567890")]
	[TestCategory("Ares")]
	[ExpectedException(typeof(ArgumentException), "Ico nemá předepsanou délku 8 znaků")]
	public void AresService_GetEkonomickeSubjektyFromIco_BadParamLength(string ico)
	{
		// Act
		new AresService().GetEkonomickeSubjektyDleIco(ico);
		// Assert -> must be Exception
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetEkonomickeSubjektyFromIco_Basic()
	{
		// Arrange
		string ic = "27389731";

		// Act
		var ekonomickySubjekt = new AresService().GetEkonomickeSubjektyDleIco(ic);

		// Assert
		Assert.AreEqual(ic, ekonomickySubjekt.EkonomickySubjektAres.Ico);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetEkonomickeSubjektyFromIcoAsync_BasicAsync()
	{
		// Arrange
		string ic = "25612697";

		// Act
		var ekonomickySubjekt = await new AresService().GetEkonomickeSubjektyDleIcoAsync(ic);

		// Assert
		Assert.IsNotNull(ekonomickySubjekt);
		Assert.AreEqual(ekonomickySubjekt.EkonomickySubjektAres.ObchodniJmeno, "HAVIT, s.r.o.");
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetEkonomickeSubjektyFromIcoAsync_NemaSidlo()
	{
		// Arrange
		string ic = "25601458";

		// Act
		var ekonomickySubjekt = await new AresService().GetEkonomickeSubjektyDleIcoAsync(ic);

		// Assert
		Assert.IsNotNull(ekonomickySubjekt);
		Assert.AreEqual(ekonomickySubjekt.EkonomickySubjektAres.Sidlo, null);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetEkonomickeSubjektyFromIco_SubjektZanikl()
	{
		// Arrange
		string ic = "27732487";

		// Act
		var ekonomickySubjekt = new AresService().GetEkonomickeSubjektyDleIco(ic);

		// Assert
		Assert.IsNull(ekonomickySubjekt);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetEkonomickeSubjektyFromObchodniJmeno_Basic()
	{
		// Arrange
		string obchodniJmeno = "ORCA";

		// Act
		var ekonomickeSubjekty = new AresService().GetEkonomickeSubjektyDleObchodnihoJmena(obchodniJmeno);

		// Assert
		Assert.IsTrue(ekonomickeSubjekty.PocetCelkem > 30);
	}


	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetEkonomickeSubjektyFromObchodniJmenoAsync_Basic()
	{
		// Arrange
		string ObchodniJmeno = "HAVIT";

		// Act
		var ekonomickeSubjekty = await new AresService().GetEkonomickeSubjektyDleObchodnihoJmenaAsync(ObchodniJmeno);

		// Assert
		Assert.IsTrue(ekonomickeSubjekty.Items.Any(x => x.EkonomickySubjektAres.ObchodniJmeno == "HAVIT, s.r.o." && x.EkonomickySubjektAres.Ico == "25612697"));
	}


	[TestMethod]
	[TestCategory("Ares")]
	[ExpectedException(typeof(ArgumentNullException), "Contract failed: ObchodniJmeno = Null")]
	public void AresService_GetEkonomickeSubjektyFromObchodniJmeno_BadParamNull()
	{
		// Act
		new AresService().GetEkonomickeSubjektyDleObchodnihoJmena(null);
		// Assert -> must be Exception
	}

	[TestMethod]
	[TestCategory("Ares")]
	[ExpectedException(typeof(ArgumentException))]
	public async Task AresService_GetEkonomickeSubjektyFromObchodniJmenoAsync_BadParamEmpty()
	{
		// Act
		var aresResult = await new AresService().GetEkonomickeSubjektyDleObchodnihoJmenaAsync("");
		// Assert -> must be Exception
	}

	[DataTestMethod]
	[DataRow("jedna, dva, tři", "jedna\r\n dva\n tři")]
	[DataRow("jedna a půl\n", "jednaapůl")]
	[DataRow(null, null)]
	[DataRow(null, "")]
	[TestCategory("Ares")]
	public void AresService_IsAddressEqual(string addresaDorucovaci, string adresaSidlo)
	{
		// Act / Assert
		Assert.IsTrue(new AresService().IsAddressEqual(addresaDorucovaci, adresaSidlo));
	}

}

