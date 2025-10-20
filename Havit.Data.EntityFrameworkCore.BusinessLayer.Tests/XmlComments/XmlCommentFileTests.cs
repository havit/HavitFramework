using Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model;
using Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments;

[TestClass]
public class XmlCommentFileTests : XmlCommentTestBase
{
	[TestMethod]
	public void XmlCommentFile_FindMethod_FindMethodReturnsCorrectXmlCommentMember()
	{
		var parser = new XmlCommentParser();
		var xmlCommentFile = parser.ParseFile(ParseXmlFile());

		XmlCommentMember xmlMethodMember = xmlCommentFile.FindMethod(typeof(Person).GetMethod(nameof(Person.GetFullName)));

		XmlCommentMember expectedXmlMethodMember = xmlCommentFile.Types
			.FirstOrDefault(t => t.Name == typeof(Person).FullName)?
			.Methods.FirstOrDefault(m => m.Name == typeof(Person).FullName + "." + nameof(Person.GetFullName));

		Assert.IsNotNull(xmlMethodMember);
		Assert.AreSame(expectedXmlMethodMember, xmlMethodMember);
	}
}