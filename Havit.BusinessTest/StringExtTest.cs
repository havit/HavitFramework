using Havit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Havit.BusinessTest
{

    /// <summary>
    /// This is a test class for StringExtTest and is intended
    /// to contain all StringExtTest Unit Tests
    /// </summary>
	[TestClass]
	public class StringExtTest
	{

		#region Test infrastructure
		private TestContext testContextInstance;

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
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

		#region NormalizeForUrlTest
		/// <summary>
		/// A test for NormalizeForUrl
		/// </summary>
		[TestMethod]
		public void NormalizeForUrlTest()
		{
			string text = "Ahoj Máňo, jak se máš?";
			string expected = "ahoj-mano-jak-se-mas";
			string actual;
			actual = StringExt.NormalizeForUrl(text);
			Assert.AreEqual(expected, actual);
		}
		#endregion
	}
}
