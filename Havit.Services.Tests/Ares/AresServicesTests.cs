using Havit.Services.Ares;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Tests.Ares;

[TestClass]
public class AresServicesTests
{
	//[TestMethod]
	[TestCategory("Integration")]
	public void AresService_Basic_SubjektZaniklFalse()
	{
		string ico = "28186796"; // "25612697"
		AresService service = new AresService(ico);
		service.Timeout = 60 * 1000; /* 60 sec */
		AresData basicAresResult = service.GetData(AresRegistr.Basic);
		Assert.AreEqual(false, basicAresResult.SubjektZanikl);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresService_Basic_SubjektZaniklTrue()
	{
		string ico = "27732487"; // "25612697"
		AresService service = new AresService(ico);
		service.Timeout = 60 * 1000; /* 60 sec */
		AresData basicAresResult = service.GetData(AresRegistr.Basic);
		Assert.AreEqual(true, basicAresResult.SubjektZanikl);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresService_ObchodniRejstrik_SubjektZaniklFalse()
	{
		string ico = "28186796";
		AresService service = new AresService(ico);
		service.Timeout = 60 * 1000; /* 60 sec */
		var data = service.GetData(AresRegistr.ObchodniRejstrik);
		Assert.AreEqual(false, data.SubjektZanikl);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresService_ObchodniRejstrik_SubjektZaniklTrue()
	{
		string ico = "27732487";
		AresService service = new AresService(ico);
		service.Timeout = 60 * 1000; /* 60 sec */
		var data = service.GetData(AresRegistr.ObchodniRejstrik);
		Assert.AreEqual(true, data.SubjektZanikl);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresService_Basic_ReadsDic()
	{
		string ico = "25612697";
		AresService service = new AresService(ico);
		service.Timeout = 60 * 1000; /* 60 sec */
		AresData data = service.GetData(AresRegistr.Basic);
		Assert.AreEqual("CZ25612697", data.Dic);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresPrehledSubjektuService_Nazev()
	{
		string name = "ASSECO";
		AresPrehledSubjektuService service = new AresPrehledSubjektuService();
		service.Timeout = 60 * 1000; /* 60 sec */
		AresPrehledSubjektuResult result = service.GetData(name);
		Assert.IsTrue(result.Data.Count > 1);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresPrehledSubjektu_FyzickaOsoba()
	{
		string name = "vožice";
		string obec = "vožice";
		AresPrehledSubjektuService service = new AresPrehledSubjektuService();
		service.Timeout = 60 * 1000; /* 60 sec */
		AresPrehledSubjektuResult result = service.GetData(name, obec);
		Assert.IsTrue(result.Data.Count > 10);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresStandardService_Havit()
	{
		const string Havit = "HAVIT, s.r.o.";
		AresStandardService service = new AresStandardService();
		var result = service.GetData(Havit);
		Assert.AreEqual(1, result.Data.Count);
		Assert.IsFalse(result.PrilisMnohoVysledku);
		Assert.AreEqual("25612697", result.Data[0].Ico);
		Assert.AreEqual(Havit, result.Data[0].Nazev);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresStandardService_Msfest()
	{
		AresStandardService service = new AresStandardService();
		var result = service.GetData("ms fest");
		Assert.AreEqual(1, result.Data.Count);
		Assert.IsFalse(result.PrilisMnohoVysledku);
		Assert.AreEqual("01251554", result.Data[0].Ico);
		Assert.AreEqual("MS Fest, z.s.", result.Data[0].Nazev);
	}

	//[TestMethod]
	[TestCategory("Integration")]
	public void AresStandardService_NotExists()
	{
		AresStandardService service = new AresStandardService();
		var result = service.GetData("weiojgwhjigwnmgerjgwrejgw"); // hledám nějaký nesmysl
		Assert.AreEqual(0, result.Data.Count);
	}
}