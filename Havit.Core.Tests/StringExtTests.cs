using Havit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests
{
	[TestClass]
	public class StringExtTests
	{		
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
