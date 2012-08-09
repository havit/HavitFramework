using Havit.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Havit.WebTest
{
	
	
	/// <summary>
	///This is a test class for HttpServerUtilityExtTest and is intended
	///to contain all HttpServerUtilityExtTest Unit Tests
	///</summary>
	[TestClass()]
	public class HttpServerUtilityExtTest
	{
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

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion
		#endregion

		#region ResolveUrlTest_Standard
		/// <summary>
		///A test for ResolveUrl
		///</summary>
		[TestMethod()]
		public void ResolveUrlTest_Standard()
		{
			string appPath = "http://www.havit.cz/app";
			string url = "~/path";
			string expected = "http://www.havit.cz/app/path";
			string actual;
			actual = HttpServerUtilityExt.ResolveUrl(appPath, url);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region ResolveUrlTest_SlashEndingAppPath
		/// <summary>
		///A test for ResolveUrl
		///</summary>
		[TestMethod()]
		public void ResolveUrlTest_SlashEndingAppPath()
		{
			string appPath = "http://www.havit.cz/app/";
			string url = "~/path";
			string expected = "http://www.havit.cz/app/path";
			string actual;
			actual = HttpServerUtilityExt.ResolveUrl(appPath, url);
			Assert.AreEqual(expected, actual);
		}
		#endregion
	}
}
