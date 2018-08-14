using Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments
{
	[TestClass]
	public class XmlCommentMemberTests
	{
		[TestMethod]
		public void XmlCommentMember_SummaryProperty_ReturnsValueOfSummaryTag()
		{
			const string PropertySummary = "This is my fabulous property!";

			var xmlCommentMember = new XmlCommentMember("Hello.World.MyProperty")
			{
				Tags =
				{
					new XmlMemberTag("summary", PropertySummary)
				}
			};

			Assert.AreEqual(PropertySummary, xmlCommentMember.Summary);
		}
	}
}