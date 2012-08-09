using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Business.Query;

namespace Havit.BusinessTest
{
	/// <summary>
	/// Summary description for TextConditionTest
	/// </summary>
	[TestClass]
	public class TextConditionTest
	{

		#region Test infrastructure
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

		[TestMethod]
		public void IsWildcardMatchTest()
		{
			Assert.IsTrue(TextCondition.IsWildcardMatch("kolo", "kolo"));
			Assert.IsTrue(TextCondition.IsWildcardMatch("kolo", "kolotoč"));
			Assert.IsFalse(TextCondition.IsWildcardMatch("kolo", "okolo"));

			Assert.IsTrue(TextCondition.IsWildcardMatch("k*o", "kolo"));
			Assert.IsTrue(TextCondition.IsWildcardMatch("k*lo", "kolo"));
			Assert.IsTrue(TextCondition.IsWildcardMatch("k*olo", "kolo"));
			Assert.IsFalse(TextCondition.IsWildcardMatch("k*o", "kolotoč"));
			Assert.IsFalse(TextCondition.IsWildcardMatch("k*o", "okolo"));

			Assert.IsTrue(TextCondition.IsWildcardMatch("k.lo", "k.lo"));
			Assert.IsTrue(TextCondition.IsWildcardMatch("k?lo", "k?lo"));
			Assert.IsFalse(TextCondition.IsWildcardMatch("k.lo", "kolo"));
			Assert.IsFalse(TextCondition.IsWildcardMatch("k?lo", "kolo"));			
		}
	}
}
