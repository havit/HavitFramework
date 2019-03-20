using Havit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests
{

    /// <summary>
    /// This is a test class for StringExtTest and is intended
    /// to contain all StringExtTest Unit Tests
    /// </summary>
	[TestClass]
	public class StringExtTests
	{		
		/// <summary>
		/// A test for NormalizeForUrl
		/// </summary>
		[TestMethod]
		public void StringExt_NormalizeForUrl()
		{
			string text = "Ahoj Máňo, jak se máš?";
			string expected = "ahoj-mano-jak-se-mas";
			string actual;
			actual = StringExt.NormalizeForUrl(text);
			Assert.AreEqual(expected, actual);
		}
	}
}
