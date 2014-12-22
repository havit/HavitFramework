using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Havit.Services.StateAdministration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.ServicesTest.StateAdministrationServices
{
	[TestClass]
	public class RodneCisloServiceTest
	{
		[TestMethod]
		public void RodneCisloValidateTest()
		{
            Assert.IsTrue(RodneCisloServices.Validate("7801233540")); // problematické, ale správné, viz http://phpfashion.com/jak-overit-platne-ic-a-rodne-cislo
			Assert.IsTrue(RodneCisloServices.Validate("780123/3540")); // problematické, ale správné, viz http://phpfashion.com/jak-overit-platne-ic-a-rodne-cislo
			Assert.IsFalse(RodneCisloServices.Validate("780123//3540")); // chybný formát
			Assert.IsFalse(RodneCisloServices.Validate("80123//3540"));	// chybný formát
			Assert.IsTrue(RodneCisloServices.Validate("7909173030")); 
            Assert.IsTrue(RodneCisloServices.Validate("790917/3030")); 
            Assert.IsFalse(RodneCisloServices.Validate("7909173/030")); // chybný formát
            Assert.IsFalse(RodneCisloServices.Validate("79091/73030"));	// chybný formát
			//Assert.IsTrue(RodneCisloServices.Validate("9950010004")); 
			//Assert.IsTrue(RodneCisloServices.Validate("995001/0004"));
			Assert.IsTrue(RodneCisloServices.Validate("404040404")); // správný formát, nelze validovat
			Assert.IsTrue(RodneCisloServices.Validate("404040/404")); // správný formát, nelze validovat
			Assert.IsFalse(RodneCisloServices.Validate("40404/4040")); // chybný formát
			Assert.IsFalse(RodneCisloServices.Validate("40404//4040"));	// chybný formát
			Assert.IsFalse(RodneCisloServices.Validate("7909172990"));
			Assert.IsFalse(RodneCisloServices.Validate("7909172991"));
			Assert.IsFalse(RodneCisloServices.Validate("7909172992"));
			Assert.IsFalse(RodneCisloServices.Validate("7909172993"));
			Assert.IsFalse(RodneCisloServices.Validate("7909172994"));
			Assert.IsFalse(RodneCisloServices.Validate("7909172995"));
			Assert.IsFalse(RodneCisloServices.Validate("7909172996"));
			Assert.IsFalse(RodneCisloServices.Validate("7909172998"));
			Assert.IsFalse(RodneCisloServices.Validate("7909172999"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2990"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2991"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2992"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2993"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2994"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2995"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2996"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2998"));
			Assert.IsFalse(RodneCisloServices.Validate("790917/2999"));

			Assert.IsFalse(RodneCisloServices.Validate("991601/0004"));
			Assert.IsFalse(RodneCisloServices.Validate("990132/0004"));
			Assert.IsTrue(RodneCisloServices.Validate("000229/0002")); // přestupný rok
			Assert.IsFalse(RodneCisloServices.Validate("990229/0002")); // nepřestupný rok
		}
	}
}
