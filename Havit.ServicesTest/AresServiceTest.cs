using System;

using Havit.Services.Ares;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.ServicesTest
{
	[TestClass]
	public class AresServiceTest
	{
		[TestMethod]
		public void GetBasicDataTest()
		{
			string ico = "00569330";
			AresService service = new AresService(ico);
			service.Timeout = 60 * 1000; /* 60 sec */
			service.GetData(AresRegistr.Basic);
		}

		[TestMethod]
		public void GetObchodniRejstrikDataTest()
		{
			string ico = "00569330";
			AresService service = new AresService(ico);
			service.Timeout = 60 * 1000; /* 60 sec */
			service.GetData(AresRegistr.ObchodniRejstrik);
		}
	}
}
