using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Web.UI.WebControls;

namespace HavitTest
{
	[TestClass]
	public class BooleandFieldTest
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
