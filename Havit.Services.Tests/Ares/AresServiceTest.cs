using Havit.Services.Ares;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Services.Tests.Ares
{
	[TestClass]
	public class AresServiceTest
	{
		#region GetAresBasicDataTest
		[TestMethod]
		public void GetAresBasicDataTest()
		{
			string ico = "28186796"; // "25612697"
			AresService service = new AresService(ico);
			service.Timeout = 60 * 1000; /* 60 sec */
			AresData basicAresResult = service.GetData(AresRegistr.Basic);
			Assert.AreEqual(basicAresResult.SubjektZanikl, false);
		}
		#endregion

		#region GetAresBasicDataSubjektZaniklTest
		[TestMethod]
		public void GetAresBasicDataSubjektZaniklTest()
		{
			string ico = "27732487"; // "25612697"
			AresService service = new AresService(ico);
			service.Timeout = 60 * 1000; /* 60 sec */
			AresData basicAresResult = service.GetData(AresRegistr.Basic);
			Assert.AreEqual(basicAresResult.SubjektZanikl, true);
		}
		#endregion

		#region GetAresObchodniRejstrikDataTest
		[TestMethod]
		public void GetAresObchodniRejstrikDataTest()
		{
			string ico = "28186796";
			AresService service = new AresService(ico);
			service.Timeout = 60 * 1000; /* 60 sec */
			var data = service.GetData(AresRegistr.ObchodniRejstrik);
			Assert.AreEqual(data.SubjektZanikl, false);
		}
		#endregion

		#region GetAresObchodniRejstrikSubjektZaniklTest
		[TestMethod]
		public void GetAresObchodniRejstrikSubjektZaniklTest()
		{
			string ico = "27732487";
			AresService service = new AresService(ico);
			service.Timeout = 60 * 1000; /* 60 sec */
			var data = service.GetData(AresRegistr.ObchodniRejstrik);
			Assert.AreEqual(data.SubjektZanikl, true);
		}
		#endregion

		#region GetBasicDataTest
		[TestMethod]
		public void GetBasicData_ReadsDic_Test()
		{
			string ico = "25612697";
			AresService service = new AresService(ico);
			service.Timeout = 60 * 1000; /* 60 sec */
			AresData data = service.GetData(AresRegistr.Basic);
			Assert.AreEqual("CZ25612697", data.Dic);
		}
		#endregion

		#region GetAresPrehledSubjektuFirmaTest
		[TestMethod]
		public void GetAresPrehledSubjektuFirmaTest()
		{
			string name = "ASSECO";
			AresPrehledSubjektuService service = new AresPrehledSubjektuService();
			service.Timeout = 60 * 1000; /* 60 sec */
			AresPrehledSubjektuResult result = service.GetData(name);
			Assert.IsTrue(result.Data.Count > 1);
		}
		#endregion

		#region GetAresPrehledSubjektuFyzickaOsobaTest
		[TestMethod]
		public void GetAresPrehledSubjektuFyzickaOsobaTest()
		{
			string name = "vožice";
			string obec = "vožice";
			AresPrehledSubjektuService service = new AresPrehledSubjektuService();
			service.Timeout = 60 * 1000; /* 60 sec */
			AresPrehledSubjektuResult result = service.GetData(name, obec);
			Assert.IsTrue(result.Data.Count > 10);
		}
		#endregion

		#region GetStandardDataTest
		[TestMethod]
		public void GetStandardDataTest_Havit()
		{
			const string Havit = "HAVIT, s.r.o.";
			AresStandardService service = new AresStandardService();
			var result = service.GetData(Havit);
			Assert.AreEqual(1, result.Data.Count);
			Assert.IsFalse(result.PrilisMnohoVysledku);
			Assert.AreEqual("25612697", result.Data[0].Ico);
			Assert.AreEqual(Havit, result.Data[0].Nazev);
		}

		[TestMethod]
		public void GetStandardDataTest_Msfest()
		{
			AresStandardService service = new AresStandardService();
			var result = service.GetData("ms fest");
			Assert.AreEqual(1, result.Data.Count);
			Assert.IsFalse(result.PrilisMnohoVysledku);
			Assert.AreEqual("01251554", result.Data[0].Ico);
			Assert.AreEqual("MS Fest, z.s.", result.Data[0].Nazev);
		}
		#endregion

		#region GetStandardDataTest_Empty
		[TestMethod]
		public void GetStandardDataTest_Empty()
		{
			AresStandardService service = new AresStandardService();
			var result = service.GetData("weiojgwhjigwnmgerjgwrejgw"); // hledám nějaký nesmysl
			Assert.AreEqual(0, result.Data.Count);
		}
		#endregion
	}
}
