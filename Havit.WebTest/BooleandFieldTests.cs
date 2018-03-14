using Havit.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HavitTest
{
	[TestClass]
	public class BooleandFieldTests
	{
		[TestMethod]
		public void FormatDataValueTest()
		{
			BooleanField bf = new BooleanField();
			bf.EmptyText = "empty";
			bf.TrueText = "True";
			bf.FalseText = "False";

			Assert.AreEqual(bf.FormatDataValue(true), bf.TrueText);
			Assert.AreEqual(bf.FormatDataValue(false), bf.FalseText);
			Assert.AreEqual(bf.FormatDataValue(bf.EmptyText), bf.EmptyText);
		}
	}
}
