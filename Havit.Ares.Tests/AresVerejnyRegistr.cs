namespace Havit.Ares.Tests;

[TestClass]
[TestCategory("Ares")]
public class AresVerejnyRegistTests
{
	public TestContext TestContext { get; set; }

	[TestMethod]
	public void AresService_GetVerejnyRejstrikDleIco_BadParamNull()
	{
		// Arrange
		AresService aresService = new AresService();

		// Assert
		Assert.ThrowsExactly<ArgumentNullException>(() =>
		{
			// Act
			aresService.GetVerejnyRejstrikDleIco(null);
		});
	}

	[TestMethod]
	[DataRow("")]
	[DataRow("123")]
	[DataRow("1234567890")]
	public void AresService_GetVerejnyRejstrikDleIco_BadParamLength(string ico)
	{
		// Arrange
		AresService aresService = new AresService();

		Assert.ThrowsExactly<ArgumentException>(() =>
		{
			// Act
			aresService.GetVerejnyRejstrikDleIco(ico);
		}, "Ico nemá předepsanou délku 8 znaků");
	}

	[TestMethod]
	public void AresService_GetVerejnyRejstrikDleIco_Basic()
	{
		// Arrange
		string ic = "27389731";

		// Act
		var VRSubjekt = new AresService().GetVerejnyRejstrikDleIco(ic);

		// Assert
		Assert.AreEqual(ic, VRSubjekt.Ico.First().Hodnota);
	}
	[TestMethod]
	public async Task AresService_GetVerejnyRejstrikDleIcoAsync_Basic()
	{
		// Arrange
		string ic = "25612697";

		// Act
		var VRSubjekt = await new AresService().GetVerejnyRejstrikDleIcoAsync(ic, TestContext.CancellationToken);

		// Assert
		Assert.IsNotNull(VRSubjekt);
		Assert.AreEqual("HAVIT, s.r.o.", VRSubjekt.ObchodniJmeno.First(x => x.DatumVymazu.Year == DateTime.MinValue.Year).Hodnota);
	}

	[TestMethod]
	public void AresService_GetVerejnyRejstrikDleIco_SubjektZanikl()
	{
		// Arrange
		string ic = "27732487";

		// Act
		var VRSubjekt = new AresService().GetVerejnyRejstrikDleIco(ic);

		// Assert
		Assert.IsNull(VRSubjekt);
	}

	[TestMethod]
	public void AresService_GetVerejnyRejstrikDleIco_GetZpusobJednani()
	{
		// Arrange
		string ic = "27389731";

		// Act
		var VRSubjekt = new AresService().GetVerejnyRejstrikDleIco(ic);
		string ZpusobJednani = VRSubjekt.StatutarniOrgany?.FirstOrDefault(x => x.DatumVymazu.Year == DateTime.MinValue.Year)?.ZpusobJednani?.FirstOrDefault(x => x.DatumVymazu.Year == DateTime.MinValue.Year)?.Hodnota;

		// Assert
		Assert.IsNotNull(ZpusobJednani);
	}


}

