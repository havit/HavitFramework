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
		AresService service = new AresService();
		service.GetEkonomickeSubjektyDleIco(null);
		Assert.AreEqual(false, true);
	}

	[DataTestMethod]
	[DataRow("")]
	[DataRow("123")]
	[DataRow("1234567890")]
	[TestCategory("Ares")]
	[ExpectedException(typeof(ArgumentException), "Ico nemá předepsanou délku 8 znaků")]
	public void AresService_GetEkonomickeSubjektyFromIco_BadParamLength(string ico)
	{
		AresService service = new AresService();
		service.GetEkonomickeSubjektyDleIco(ico);
		Assert.AreEqual(false, true);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetEkonomickeSubjektyFromIco_Basic()
	{
		string ic = "27389731";
		AresService service = new AresService();
		var ekonomickySubjekt = service.GetEkonomickeSubjektyDleIco(ic);
		Assert.AreEqual(ic, ekonomickySubjekt.EkonomickySubjektAres.Ico);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetEkonomickeSubjektyFromIco_BasicAsync()
	{
		string ic = "25612697";
		AresService service = new AresService();
		var ekonomickySubjekt = await service.GetEkonomickeSubjektyDleIcoAsync(ic);
		Assert.IsNotNull(ekonomickySubjekt);
		Assert.AreEqual(ekonomickySubjekt.EkonomickySubjektAres.ObchodniJmeno, "HAVIT, s.r.o.");
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetEkonomickeSubjektyFromIco_NemaSidlo()
	{
		string ic = "25601458";
		AresService service = new AresService();
		var ekonomickySubjekt = await service.GetEkonomickeSubjektyDleIcoAsync(ic);
		Assert.IsNotNull(ekonomickySubjekt);
		Assert.AreEqual(ekonomickySubjekt.EkonomickySubjektAres.Sidlo, null);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetEkonomickeSubjektyFromIco_SubjektZanikl()
	{
		string ic = "27732487";
		AresService service = new AresService();
		var ekonomickySubjekt = service.GetEkonomickeSubjektyDleIco(ic);
		Assert.IsNull(ekonomickySubjekt);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetEkonomickeSubjektyFromObchodniJmeno_Basic()
	{
		string obchodniJmeno = "ORCA";
		AresService service = new AresService();
		var ekonomickeSubjekty = service.GetEkonomickeSubjektyDleObchodnihoJmena(obchodniJmeno);
		Assert.IsTrue(ekonomickeSubjekty.PocetCelkem > 30);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetEkonomickeSubjektyFromObchodniJmeno_BasicAsync()
	{
		string ObchodniJmeno = "ORCA";
		AresService service = new AresService();
		var ekonomickeSubjekty = await service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync(ObchodniJmeno);
		Assert.IsTrue(ekonomickeSubjekty.PocetCelkem > 30);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetEkonomickeSubjektyFromObchodniJmeno_BasicAsync2()
	{
		string ObchodniJmeno = "HAVIT";
		AresService service = new AresService();
		var ekonomickeSubjekty = await service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync(ObchodniJmeno);
		Assert.IsTrue(ekonomickeSubjekty.Items.Any(x => x.EkonomickySubjektAres.ObchodniJmeno == "HAVIT, s.r.o." && x.EkonomickySubjektAres.Ico == "25612697"));
	}


	[TestMethod]
	[TestCategory("Ares")]
	[ExpectedException(typeof(ArgumentNullException), "Contract failed: ObchodniJmeno = Null")]
	public void AresService_GetEkonomickeSubjektyFromObchodniJmeno_BadParamNull()
	{
		AresService service = new AresService();
		service.GetEkonomickeSubjektyDleObchodnihoJmena(null);
	}

	[TestMethod]
	[TestCategory("Ares")]
	[ExpectedException(typeof(ArgumentException))]
	public async Task AresService_GetEkonomickeSubjektyFromObchodniJmeno_BadParamEmpty()
	{
		AresService service = new AresService();
		var aresResult = await service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("");
	}


	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetAresAndPlatceDph_Basic()
	{
		string ic = "27389731";
		// 		ic = "46003703";		Ico ktere vzniklo dnes (4.3.2024) a jeste nema Pravni formu (resp = 100 unknown).
		ic = "45000115";
		AresService service = new AresService();
		AresDphResponse aresDphResult = service.GetAresAndPlatceDph(ic);
		Assert.AreEqual(ic, aresDphResult.AresElement.Ico);
	}


	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetAresAndPlatceDph_OSVNeplatce()
	{
		string ic = "42081181";     //  DIC "CZ6006060126", nemá účet
		AresService service = new AresService();
		AresDphResponse aresDphResult = service.GetAresAndPlatceDph(ic);
		Assert.AreEqual(aresDphResult.AresElement.Ico, "42081181");
		Assert.AreEqual(aresDphResult.PlatceDphElement.Dic, "6006060126");
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_GetPlatceDph_NoExists()
	{
		string dic = "CZ11100011";
		AresService service = new AresService();
		PlatceDphResponse platceDPH = service.GetPlatceDph(dic);
		Assert.IsFalse(platceDPH.IsNalezeno);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetPlatceDph_Async()
	{
		string dic = "CZ27389731";
		//		dic = "CZ25836595";			// nespolehlivý

		AresService service = new AresService();
		PlatceDphResponse platceDPH = await service.GetPlatceDphAsync(dic);
		Assert.IsTrue(platceDPH.IsNalezeno);
		Assert.AreEqual(platceDPH.Dic, dic);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task AresService_GetPlatceDph_O2()
	{
		string dic = "CZ60193336";
		AresService service = new AresService();
		PlatceDphResponse platceDPH = await service.GetPlatceDphAsync(dic);
		Assert.IsTrue(platceDPH.IsNalezeno);
		Assert.AreEqual(platceDPH.Dic, dic);
		Assert.IsTrue(platceDPH.CislaUctu.Count() > 10);
	}

	[DataTestMethod]
	[DataRow(null)]
	[DataRow("")]
	[DataRow("123")]
	[DataRow("1234567")]
	[DataRow("CZ3456789")]
	[DataRow("CZ345678912")]
	[DataRow("CZ<DELETE>")]
	[DataRow("<X12345678")]
	[TestCategory("Ares")]
	[ExpectedException(typeof(ArgumentException), AllowDerivedTypes = true)]
	public void AresService_GetPlatceDph_BadInput(string dic)
	{
		AresService service = new AresService();
		service.GetPlatceDph(dic);
	}


	[TestMethod]
	[TestCategory("Ares")]
	public void AresService_AresCiselnik_PravniForma()
	{
		AresCiselnik CiselnikPravniForma = new AresCiselnik("res", "PravniForma");
		string pravniForma101 = CiselnikPravniForma.GetValue("101");
		string pravniForma102 = CiselnikPravniForma.GetValue("102");
		Assert.AreEqual(pravniForma101, "Fyzická osoba podnikající dle živnostenského zákona");
		Assert.AreEqual(pravniForma102, "Fyzická osoba podnikající dle živnostenského zákona zapsaná v obchodním rejstříku");
	}


	/*
	// [TestMethod]
	[TestCategory("Ares")]
	public void TestZatezovy()
	{
		AresService service = new AresService();
		for (long i = 25601458; i < 25610000; i++)
		{
			string strIco = i.ToString("D8");
			int sum = 0;
			for (int j = 0; j < 7; j++)
			{
				sum += int.Parse(strIco[j].ToString()) * (8 - j);
			}
			sum = sum % 11;
			int lastNumber = int.Parse(strIco[7].ToString());
			if ((lastNumber == 0 && sum == 1) || (lastNumber == 1 && sum == 0) || (lastNumber == 11 - sum))
			{
				try
				{
					AresDphResponse aresDphResult = service.GetAresAndPlatceDph(strIco);
					string AresDphInfo = $"{strIco} ";
					if (aresDphResult.AresElement == null)
					{
						AresDphInfo += "No Exists.";
					}
					else
					{
						AresDphInfo += $"{aresDphResult.AresElement.ObchodniJmeno}  -  ";
						if (aresDphResult.PlatceDphElement == null)
						{
							AresDphInfo += "Není plátce DPH";
						}
						else
						{
							AresDphInfo += "DPH: " + aresDphResult.PlatceDphElement.Dic;
							AresDphInfo += " spolehlivý: " + aresDphResult.PlatceDphElement.isSpolehlivy.ToString();
						}
						AresDphInfo += aresDphResult.AresElement.AresExtension.SpisovaZnackaFull + "; " + aresDphResult.AresElement.AresExtension.FinancniUradText + "; " + aresDphResult.AresElement.AresExtension.PravniFormaText;
						System.Diagnostics.Debug.WriteLine(AresDphInfo);
					}
				}
				catch (ApiException ex)
				{
					var result = ((Havit.Services.Ares.ApiException<Havit.Services.Ares.Chyba>)ex).Result;
					Debug.WriteLine(result.Kod + "-" + result.SubKod);
					Debug.WriteLine(result.Popis);
				}
				catch (ContractException ex)
				{
					Debug.WriteLine(strIco + ". Chybné vstupní parametry. " + ex.Message);
				}
				catch (AresDphException ex)
				{
					Debug.WriteLine(strIco + ". DPH Error: " + ex.Code.ToString() + " - " + ex.Message);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(strIco + ". Jiná chyba. " + ex.Message);
				}
			}
			else
			{
				// System.Diagnostics.Debug.WriteLine($".");
			}
		}
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void TestSynchro()
	{
		AresService service = new AresService();
		DateTime start = DateTime.Now;
		service.GetEkonomickeSubjektyFromObchodniJmeno("OOO");
		service.GetEkonomickeSubjektyFromObchodniJmeno("OR");
		service.GetEkonomickeSubjektyFromObchodniJmeno("ORC");
		service.GetEkonomickeSubjektyFromObchodniJmeno("ORCA");
		double total = (DateTime.Now - start).TotalMilliseconds;
		System.Diagnostics.Debug.WriteLine("Synchronni volani " + total.ToString());
	}


	[TestMethod]
	[TestCategory("Ares")]
	public async Task TestParalel1()
	{
		AresService service = new AresService();
		DateTime start = DateTime.Now;
		var t1 = service.GetEkonomickeSubjektyFromObchodniJmenoAsync("OOOO");
		var t2 = service.GetEkonomickeSubjektyFromObchodniJmenoAsync("OR");
		var t3 = service.GetEkonomickeSubjektyFromObchodniJmenoAsync("ORC");
		var t4 = service.GetEkonomickeSubjektyFromObchodniJmenoAsync("ORCA");
		await t1; await t2; await t3; await t4;
		double total = (DateTime.Now - start).TotalMilliseconds;
		System.Diagnostics.Debug.WriteLine("Asynchronni volani Postupne " + total.ToString());
		Assert.AreEqual(true, true);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public async Task TestParalel2()
	{
		AresService service = new AresService();
		DateTime start = DateTime.Now;
		var t1 = service.GetEkonomickeSubjektyFromObchodniJmenoAsync("OOOO");
		var t2 = service.GetEkonomickeSubjektyFromObchodniJmenoAsync("OR");
		var t3 = service.GetEkonomickeSubjektyFromObchodniJmenoAsync("ORC");
		var t4 = service.GetEkonomickeSubjektyFromObchodniJmenoAsync("ORCA");
		await Task.WhenAll(t1, t2, t3, t4);
		double total = (DateTime.Now - start).TotalMilliseconds;
		System.Diagnostics.Debug.WriteLine("Asynchronni volani Najednou " + total.ToString());
		Assert.AreEqual(true, true);
	}

	[TestMethod]
	[TestCategory("Ares")]
	public void ZpracovaniChyby()
	{
		try
		{
			AresService service = new AresService();
			var t1 = service.GetEkonomickeSubjektyFromObchodniJmeno("O");
			Assert.AreEqual(true, true);
		}
		catch (ApiException ex)
		{
			var result = ((Havit.Services.Ares.ApiException<Havit.Services.Ares.Chyba>)ex).Result;
			Debug.WriteLine(result.Kod + "-" + result.SubKod);
			Debug.WriteLine(result.Popis);
		}
		catch (AresDphException ex)
		{
			Debug.WriteLine(ex.Code + "  - " + ex.Message);
		}

		catch (ContractException ex)
		{
			Debug.WriteLine("Chybné vstupní parametry. " + ex.Message);
		}
		catch (Exception ex)
		{
			Debug.WriteLine("Jiná chyba. " + ex.Message);
		}
	}
	*/
}

