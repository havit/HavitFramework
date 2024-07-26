namespace Havit.Ares.Tests;

[TestClass]
[TestCategory("Ares")]
public class AresVerejnyRegistTests
{
	[TestMethod]
	[ExpectedException(typeof(ArgumentNullException))]
	public void AresService_GetVerejnyRejstrikDleIco_BadParamNull()
	{
		// Act
		new AresService().GetVerejnyRejstrikDleIco(null);

		// Assert by method attribute
	}

	[DataTestMethod]
	[DataRow("")]
	[DataRow("123")]
	[DataRow("1234567890")]
	[ExpectedException(typeof(ArgumentException), "Ico nemá předepsanou délku 8 znaků")]
	public void AresService_GetVerejnyRejstrikDleIco_BadParamLength(string ico)
	{
		// Act
		new AresService().GetVerejnyRejstrikDleIco(ico);

		// Assert by method attribute
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
		var VRSubjekt = await new AresService().GetVerejnyRejstrikDleIcoAsync(ic);

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

