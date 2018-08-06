using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Havit.Business.CodeMigrations.XmlComments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Business.CodeMigrations.Tests.XmlComments
{
	[TestClass]
	public class XmlCommentParserTests
	{
		[TestMethod]
		public void XmlCommentParser_ParseFile_ParsedFileHasCorrectNumberOfTypes()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			Assert.IsNotNull(xmlCommentFile);
			Assert.IsTrue(xmlCommentFile.Types.Count == 4);
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_ParsedFileHasClasses()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			Assert.IsNotNull(xmlCommentFile);
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Location"));
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person"));
			Assert.IsNotNull(xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.PersonLocation"));
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectNumberOfCommentTags()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			XmlCommentType personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person");
			Assert.AreEqual(1, personType.Tags.Count);
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectCommentTag()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			XmlCommentType personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person");
			Assert.AreEqual(1, personType.Tags.Count);
			Assert.AreEqual("summary", personType.Tags[0].Name);
			Assert.AreEqual("Person object", personType.Tags[0].Content.Trim());
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectNumberOfProperties()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			XmlCommentType personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person");
			Assert.AreEqual(4, personType.Properties.Count);
		}

		[TestMethod]
		public void XmlCommentParser_ParseFile_PersonClassHasCorrectProperties()
		{
			var parser = new XmlCommentParser();

			var xmlCommentFile = parser.ParseFile(ParseXmlFile());

			XmlCommentType personType = xmlCommentFile.Types.FirstOrDefault(t => t.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person");
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person.Id"));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person.FirstName"));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person.LastName"));
			Assert.IsNotNull(personType.Properties.FirstOrDefault(p => p.Name == "Havit.Business.CodeMigrations.Tests.XmlComments.Model.Person.Birthday"));
		}

		private static XDocument ParseXmlFile()
		{
			try
			{
				return XDocument.Parse(XmlCommentsData.Havit_Business_CodeMigrations_Tests);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("Could not parse test XML file: " + ex.Message, ex);
			}
		}
	}
}