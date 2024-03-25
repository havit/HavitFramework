/*
using Havit.Ares;
using Havit.Ares.FinancniSprava;
using Havit.Diagnostics.Contracts;

namespace Havit.Ares.Tests;

//[TestClass]
public class AresZatezovyTests
{
	[TestMethod]
	public async Task TestParalel1()
	{
		AresService service = new AresService();
		DateTime start = DateTime.Now;
		var t1 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("OOOO");
		var t2 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("OR");
		var t3 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("ORC");
		var t4 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("ORCA");
		await t1; await t2; await t3; await t4;
		double total = (DateTime.Now - start).TotalMilliseconds;
		System.Diagnostics.Debug.WriteLine("Asynchronni volani Postupne " + total.ToString());
		Assert.AreEqual(true, true);
	}

	[TestMethod]
	public async Task TestParalel2()
	{
		AresService service = new AresService();
		DateTime start = DateTime.Now;
		var t1 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("OOOO");
		var t2 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("OR");
		var t3 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("ORC");
		var t4 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("ORCA");
		await Task.WhenAll(t1, t2, t3, t4);
		double total = (DateTime.Now - start).TotalMilliseconds;
		System.Diagnostics.Debug.WriteLine("Asynchronni volani Najednou " + total.ToString());
		Assert.AreEqual(true, true);
	}


	[TestMethod]
	public void TestSynchro()
	{
		AresService service = new AresService();
		DateTime start = DateTime.Now;
		service.GetEkonomickeSubjektyDleObchodnihoJmena("OOO");
		service.GetEkonomickeSubjektyDleObchodnihoJmena("OR");
		service.GetEkonomickeSubjektyDleObchodnihoJmena("ORC");
		service.GetEkonomickeSubjektyDleObchodnihoJmena("ORCA");
		double total = (DateTime.Now - start).TotalMilliseconds;
		System.Diagnostics.Debug.WriteLine("Synchronni volani " + total.ToString());
	}

	[TestMethod]
	public void TestZatezovy_HledaniDleJmena()
	{
		string obchodniJmeno = "Orca";
		var ekonomickeSubjektyResult = new AresService().GetEkonomickeSubjektyDleObchodnihoJmena(obchodniJmeno, 1000);
		System.Diagnostics.Debug.WriteLine("Nalezeno " + ekonomickeSubjektyResult.Items.Count() + " items pro '" + obchodniJmeno + "'");
		foreach (var subject in ekonomickeSubjektyResult.Items)
		{
			string ico = subject.EkonomickySubjektAres.Ico;
			try
			{
				string AresDphInfo = ico;
				AresDphResponse aresDphResult = new AresDphService(new AresService(), new PlatceDphService()).GetAresAndPlatceDph(subject.EkonomickySubjektAres.Ico);
				if (aresDphResult.EkonomickySubjektItem == null)
				{
					AresDphInfo += "No Exists.";
				}
				else
				{
					AresDphInfo += $"{aresDphResult.EkonomickySubjektItem.EkonomickySubjektAres.ObchodniJmeno}  -  ";
					if (!aresDphResult.EkonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph)
					{
						AresDphInfo += "Není plátce DPH, ";
					}
					else
					{
						AresDphInfo += "DPH: " + aresDphResult.PlatceDphElement.Dic;
						AresDphInfo += " nespolehlivý: " + aresDphResult.PlatceDphElement.IsNespolehlivy.ToString() + ", ";
					}
					AresDphInfo += aresDphResult.EkonomickySubjektItem.EkonomickySubjektExtension.SpisovaZnackaFull + "; " + aresDphResult.EkonomickySubjektItem.EkonomickySubjektExtension.FinancniUradText + "; " + aresDphResult.EkonomickySubjektItem.EkonomickySubjektExtension.PravniFormaText;
					System.Diagnostics.Debug.WriteLine(AresDphInfo);
				}
			}
			catch (ApiException ex)
			{
				var result = ((ApiException<Chyba>)ex).Result;
				System.Diagnostics.Debug.WriteLine(result.Kod + "-" + result.SubKod);
				System.Diagnostics.Debug.WriteLine(result.Popis);
			}
			catch (ContractException ex)
			{
				System.Diagnostics.Debug.WriteLine(ico + ". Chybné vstupní parametry. " + ex.Message);
			}
			catch (PlatceDphException ex)
			{
				System.Diagnostics.Debug.WriteLine(ico + ". DPH Error: " + ex.Code.ToString() + " - " + ex.Message);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ico + ". Jiná chyba. " + ex.Message);
			}
		}
	}
	[TestMethod]
	public void TestZatezovy_NahodneIco()
	{
		AresDphService service = new AresDphService(new AresService(), new PlatceDphService());
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
			if (lastNumber == 0 && sum == 1 || lastNumber == 1 && sum == 0 || lastNumber == 11 - sum)
			{
				try
				{
					AresDphResponse aresDphResult = service.GetAresAndPlatceDph(strIco);
					string AresDphInfo = $"{strIco} ";
					if (aresDphResult.EkonomickySubjektItem == null)
					{
						AresDphInfo += "No Exists.";
					}
					else
					{
						AresDphInfo += $"{aresDphResult.EkonomickySubjektItem.EkonomickySubjektAres.ObchodniJmeno}  -  ";
						if (!aresDphResult.EkonomickySubjektItem.EkonomickySubjektExtension.IsPlatceDph)
						{
							AresDphInfo += "Není plátce DPH, ";
						}
						else
						{
							AresDphInfo += "DPH: " + aresDphResult.PlatceDphElement.Dic;
							AresDphInfo += " nespolehlivý: " + aresDphResult.PlatceDphElement.IsNespolehlivy.ToString() + ", ";
						}
						AresDphInfo += aresDphResult.EkonomickySubjektItem.EkonomickySubjektExtension.SpisovaZnackaFull + "; " + aresDphResult.EkonomickySubjektItem.EkonomickySubjektExtension.FinancniUradText + "; " + aresDphResult.EkonomickySubjektItem.EkonomickySubjektExtension.PravniFormaText;
						System.Diagnostics.Debug.WriteLine(AresDphInfo);
					}
				}
				catch (ApiException ex)
				{
					var result = ((ApiException<Chyba>)ex).Result;
					System.Diagnostics.Debug.WriteLine(result.Kod + "-" + result.SubKod);
					System.Diagnostics.Debug.WriteLine(result.Popis);
				}
				catch (ContractException ex)
				{
					System.Diagnostics.Debug.WriteLine(strIco + ". Chybné vstupní parametry. " + ex.Message);
				}
				catch (PlatceDphException ex)
				{
					System.Diagnostics.Debug.WriteLine(strIco + ". DPH Error: " + ex.Code.ToString() + " - " + ex.Message);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(strIco + ". Jiná chyba. " + ex.Message);
				}
			}
			else
			{
				// System.Diagnostics.Debug.WriteLine($".");
			}
		}
	}

	[TestMethod]
	public void ZpracovaniChyby()
	{
		try
		{
			AresService service = new AresService();
			var t1 = service.GetEkonomickeSubjektyDleObchodnihoJmenaAsync("O");
			Assert.AreEqual(true, true);
		}
		catch (ApiException ex)
		{
			var result = ((ApiException<Chyba>)ex).Result;
			System.Diagnostics.Debug.WriteLine(result.Kod + "-" + result.SubKod);
			System.Diagnostics.Debug.WriteLine(result.Popis);
		}
		catch (ContractException ex)
		{
			System.Diagnostics.Debug.WriteLine(". Chybné vstupní parametry. " + ex.Message);
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine(". Jiná chyba. " + ex.Message);
		}
	}
}
*/