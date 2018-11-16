using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments
{
	public class XmlCommentParser
	{
		public XmlCommentFile ParseFile(XDocument document)
		{
			Contract.Requires<ArgumentNullException>(document != null);

			Dictionary<string, XmlCommentType> types = document.Root.XPathSelectElements("/doc/members/member[starts-with(@name,'T:')]", new XmlNamespaceManager(new NameTable()))
				.Select(e => new { Name = e.Attribute("name").Value.Substring(2), Element = e })
				.ToDictionary(e => e.Name, e => ParseXmlCommentType(e.Element, e.Name));
			IEnumerable<XElement> properties = document.XPathSelectElements("doc/members/member[starts-with(@name,'P:')]");

			foreach (XElement propertyElement in properties)
			{
				var propertyFullName = propertyElement.Attribute("name").Value.Substring(2);

				var xmlPropertyType = new XmlCommentMember(propertyFullName);
				xmlPropertyType.Tags.AddRange(propertyElement.Elements().Select(e => new XmlMemberTag(e.Name.LocalName, e.Value)));

				int lastDotIndex = xmlPropertyType.Name.LastIndexOf('.');
				if (lastDotIndex == -1)
				{
					Console.WriteLine($"Property '{propertyFullName}' (XML) has not any matching any parent type, skipping");
					continue;
				}

				var currentTypeCandidate = propertyFullName.Substring(0, lastDotIndex);

				if (types.TryGetValue(currentTypeCandidate, out XmlCommentType xmlCommentType))
				{
					xmlCommentType.Properties.Add(xmlPropertyType);
				}
			}

			XmlCommentFile xmlCommentFile = new XmlCommentFile();
			xmlCommentFile.Types.AddRange(types.Values);
			return xmlCommentFile;
		}

		private static XmlCommentType ParseXmlCommentType(XElement element, string name)
		{
			var xmlCommentType = new XmlCommentType(name);

			IEnumerable<XmlMemberTag> tags = element.Elements().Select(e => new XmlMemberTag(e.Name.LocalName, e.Value));
			xmlCommentType.Tags.AddRange(tags);

			return xmlCommentType;
		}
	}
}