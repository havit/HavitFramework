using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments
{
    /// <summary>
    /// Tests for <see cref="XmlCommentParser"/>. See class model (in Model namespace) being used.
    /// </summary>
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
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectNumberOfCommentTags()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			Assert.AreEqual(1, personType.Tags.Count);
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectCommentTag()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			Assert.AreEqual(1, personType.Tags.Count);
			Assert.AreEqual("summary", personType.Tags[0].Name);
			Assert.AreEqual("Person object", personType.Tags[0].Content.Trim());
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectNumberOfProperties()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			Assert.AreEqual(4, personType.Properties.Count);
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectProperties()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == typeof(Model.Person).GetProperty(nameof(Model.Person.Id)).FullName()));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == typeof(Model.Person).GetProperty(nameof(Model.Person.FirstName)).FullName()));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == typeof(Model.Person).GetProperty(nameof(Model.Person.LastName)).FullName()));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == typeof(Model.Person).GetProperty(nameof(Model.Person.Birthday)).FullName()));
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassPropertyHasCorrectSummary()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			XmlCommentMember firstNameProperty = personType.Properties.FirstOrDefault(p => p.Name == typeof(Model.Person).GetProperty(nameof(Model.Person.FirstName)).FullName());
			Assert.AreEqual("First name", firstNameProperty.Summary.Trim());
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectMethods()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());
			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			Assert.IsNotNull(personType.Methods.FirstOrDefault(p => p.Name == typeof(Model.Person).GetMethod(nameof(Model.Person.GetFullName)).FullName()));
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassMethodHasCorrectSummary()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			XmlCommentMember getFullNameMethod = personType.Methods.FirstOrDefault(p => p.Name == typeof(Model.Person).GetMethod(nameof(Model.Person.GetFullName)).FullName());
			Assert.AreEqual("Concatenates first and last name of the person.", getFullNameMethod.Summary.Trim());
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassMethodHasReturnsTag()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			XmlCommentMember getFullNameMethod = personType.Methods.FirstOrDefault(p => p.Name == typeof(Model.Person).GetMethod(nameof(Model.Person.GetFullName)).FullName());
			Assert.IsNotNull(getFullNameMethod.Tags.FirstOrDefault(tag => tag.Name == "returns"), "Person.GetFullName method does not contain 'returns' tag");
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassMethodHasCorrectReturnsTag()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Person).FullName);
			XmlCommentMember getFullNameMethod = personType.Methods.FirstOrDefault(p => p.Name == typeof(Model.Person).GetMethod(nameof(Model.Person.GetFullName)).FullName());
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

			var loginAccountType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.LoginAccount).FullName);
			XmlCommentMember userNameProperty = loginAccountType.Properties.FirstOrDefault(p => 
				p.Name == typeof(Model.LoginAccount).GetProperty(nameof(Model.LoginAccount.Username)).FullName());
			Assert.AreEqual("LoginAccount's user name", userNameProperty.Summary.Trim());
		}

		/// <summary>
		/// Scenario with extra whitespace - new lines. (Bug 42144)
		/// </summary>
		[TestMethod]
		public void XmlCommentParser_ParseFile_LocationClassDescriptionPropertyHasTrimmedSummary()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			var locationType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == typeof(Model.Location).FullName);
			XmlCommentMember descriptionProperty = locationType.Properties.FirstOrDefault(p => 
				p.Name == typeof(Model.Location).GetProperty(nameof(Model.Location.Description)).FullName());
			Assert.AreEqual(@"Summary tag with
new lines and whitespace.", descriptionProperty.Summary);
		}
	}
}