using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments
{
	[TestClass]
	public class XmlCommentParserTests : XmlCommentTestBase
	{
		[TestMethod]
		public void XmlCommentParser_ParseFile_ParsedFileHasCorrectTypes()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			Assert.IsNotNull(xmlCommentFile);
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Location).FullName));
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.PersonLocation).FullName));
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName));
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.LoginAccount).FullName));
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_ParsedFileHasClasses()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			Assert.IsNotNull(xmlCommentFile);
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Location"));
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person"));
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.PersonLocation"));
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectNumberOfCommentTags()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			Assert.AreEqual(1, personType.Tags.Count);
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectCommentTag()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			Assert.AreEqual(1, personType.Tags.Count);
			Assert.AreEqual("summary", personType.Tags[0].Name);
			Assert.AreEqual("Person object", personType.Tags[0].Content.Trim());
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectNumberOfProperties()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			Assert.AreEqual(4, personType.Properties.Count);
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectProperties()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.Id"));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.FirstName"));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.LastName"));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.Birthday"));
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassPropertyHasCorrectSummary()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			XmlCommentMember firstNameProperty = personType.Properties.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.FirstName");
			Assert.AreEqual("First name", firstNameProperty.Summary.Trim());
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectMethods()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			Assert.IsNotNull(personType.Methods.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.GetFullName"));
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassMethodHasCorrectSummary()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			XmlCommentMember getFullNameMethod = personType.Methods.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.GetFullName");
			Assert.AreEqual("Concatenates first and last name of the person.", getFullNameMethod.Summary.Trim());
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassMethodHasReturnsTag()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			XmlCommentMember getFullNameMethod = personType.Methods.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.GetFullName");
			Assert.IsNotNull(getFullNameMethod.Tags.FirstOrDefault(tag => tag.Name == "returns"), "Person.GetFullName method does not contain 'returns' tag");
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassMethodHasCorrectReturnsTag()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person");
			XmlCommentMember getFullNameMethod = personType.Methods.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.Person.GetFullName");
			Assert.AreEqual("Person's full name.", getFullNameMethod.Tags.FirstOrDefault(tag => tag.Name == "returns").Content.Trim());
		}

		/// <summary>
		/// Scenario with parent class having no XML comment (Bug 41564)
		/// </summary>
		[TestMethod]
		public void XmlCommentParser_ParseFile_LoginAccountClassPropertyHasCorrectSummary()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var loginAccountType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.LoginAccount");
			XmlCommentMember userNameProperty = loginAccountType.Properties.FirstOrDefault(p => p.Name == "Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model.LoginAccount.Username");
			Assert.AreEqual(userNameProperty.Summary.Trim(), "LoginAccount's user name");
		}
	}
}