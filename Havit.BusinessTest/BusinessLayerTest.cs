﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Havit.BusinessLayerTest;

namespace Havit.BusinessTest
{
	/// <summary>
	///This is a test class for Havit.Business.ActiveRecordBusinessObjectBase and is intended
	///to contain all Havit.Business.ActiveRecordBusinessObjectBase Unit Tests
	///</summary>
	[TestClass()]
	public class BusinessLayerTest
	{
		#region BusinessLayerTest_ZaporneID
		/// <summary>
		/// Základní test na funkčnost záporných ID.
		///</summary>
		[TestMethod()]
		public void BusinessLayerTest_ZaporneID()
		{
			Role role = Role.GetObject(-1);

			Assert.IsFalse(String.IsNullOrEmpty(role.Symbol));
            Assert.AreEqual(role.Symbol, Role.ZaporneID.Symbol);
		}
		#endregion

		#region BusinessLayerTest_NuloveID
		/// <summary>
		/// Základní test na funkčnost nulových ID.
		///</summary>
		[TestMethod()]
		public void BusinessLayerTest_NuloveID()
		{
			Role role = Role.GetObject(0);

			Assert.IsFalse(String.IsNullOrEmpty(role.Symbol));
			Assert.AreEqual(role.Symbol, Role.NuloveID.Symbol);
		}
		#endregion

		#region BusinessLayerTest_ZakazaneNoID
		/// <summary>
		/// Základní test na funkčnost zakázaného NoID.
		///</summary>
		[TestMethod()]
		[ExpectedException(typeof(InvalidOperationException))]
		public void BusinessLayerTest_ZakazaneNoID()
		{
			Role role = Role.GetObject(Role.NoID);
		}
		#endregion

		#region BusinessLayerTest_CyclicUpdateWithInsert
		/// <summary>
		/// Nový objekt ukládá cyklický graf, kde sám by měl být minimal-insertován od jiného objektu, který je Update.
		///</summary>
		[TestMethod()]
		public void BusinessLayerTest_CyclicUpdateWithInsert()
		{
			Subjekt s = Subjekt.CreateObject();
			s.Save();

			Komunikace k1 = Komunikace.CreateObject();
			k1.Subjekt = s;
			s.Komunikace.Add(k1);
			k1.Save();

			ObjednavkaSepsani o1 = ObjednavkaSepsani.CreateObject();
			k1.ObjednavkaSepsani = o1;
			k1.Save();

			Komunikace k2 = Komunikace.CreateObject();
			k2.Subjekt = s;
			s.Komunikace.Add(k2);

			o1.StornoKomunikace = k2;
			k2.Save();

			Assert.IsFalse(k2.IsDirty);
			Assert.IsFalse(k2.IsNew);
			Assert.IsFalse(o1.IsDirty);
		}
		#endregion


		#region TestContext
		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}
		#endregion

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion
	}


}
