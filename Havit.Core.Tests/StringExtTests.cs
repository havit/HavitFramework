using Havit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests
{
	[TestClass]
	public class StringExtTests
	{		
		[TestMethod]
		public void StringExt_NormalizeForUrl_BasicScenario()
		{
			// arrange
			string text = "Ahoj Máňo, jak se máš?";
			string expected = "ahoj-mano-jak-se-mas";

			// act
			var actual = StringExt.NormalizeForUrl(text);

			// assert
			Assert.AreEqual(expected, actual);
		}
	}
}
