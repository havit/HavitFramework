﻿using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;

/// <summary>
/// Minimal implementation of XML documentation file. Supports only features required by EF Core + Business Layer integration.
///
/// <para>
///     See tests for this class, which demonstrate supported features while operating on XML files generated by compiler.
/// </para>
/// </summary>
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
			var xmlMember = ParseXmlMember(propertyElement);
			var (xmlCommentType, isShadowType) = GetMatchingParentType(xmlMember, types);
			if (xmlCommentType == null)
			{
				continue;
			}
			if (isShadowType)
			{
				types[xmlCommentType.Name] = xmlCommentType;
			}

			xmlCommentType.Properties.Add(xmlMember);
		}

		IEnumerable<XElement> methods = document.XPathSelectElements("doc/members/member[starts-with(@name,'M:')]");

		foreach (XElement methodElement in methods)
		{
			var xmlMember = ParseXmlMember(methodElement);
			var (xmlCommentType, isShadowType) = GetMatchingParentType(xmlMember, types);
			if (xmlCommentType == null)
			{
				continue;
			}

			if (isShadowType)
			{
				types[xmlCommentType.Name] = xmlCommentType;
			}

			xmlCommentType.Methods.Add(xmlMember);
		}

		XmlCommentFile xmlCommentFile = new XmlCommentFile();
		xmlCommentFile.Types.AddRange(types.Values);
		return xmlCommentFile;
	}

	private static (XmlCommentType, bool) GetMatchingParentType(XmlCommentMember xmlMemberType, Dictionary<string, XmlCommentType> types)
	{
		int lastDotIndex = xmlMemberType.Name.LastIndexOf('.');
		if (lastDotIndex == -1)
		{
			Console.WriteLine($"Member '{xmlMemberType.Name}' (XML) has not any matching any parent type, skipping");
			return (null, false);
		}

		var currentTypeCandidate = xmlMemberType.Name.Substring(0, lastDotIndex);

		bool isShadowType = false;
		if (!types.TryGetValue(currentTypeCandidate, out XmlCommentType xmlParentType))
		{
			// create "shadow type" for this method (and any other down the road). This type won't have any tags and clients should be aware of this.
			xmlParentType = new XmlCommentType(currentTypeCandidate);
			isShadowType = true;
		}

		return (xmlParentType, isShadowType);
	}

	private static XmlCommentMember ParseXmlMember(XElement element)
	{
		var methodFullName = element.Attribute("name").Value.Substring(2);

		var xmlMemberType = new XmlCommentMember(methodFullName);
		xmlMemberType.Tags.AddRange(element.Elements().Select(e => new XmlMemberTag(e.Name.LocalName, ParseTagValue(e))));
		return xmlMemberType;
	}

	private static XmlCommentType ParseXmlCommentType(XElement element, string name)
	{
		var xmlCommentType = new XmlCommentType(name);

		IEnumerable<XmlMemberTag> tags = element.Elements().Select(e => new XmlMemberTag(e.Name.LocalName, ParseTagValue(element)));
		xmlCommentType.Tags.AddRange(tags);

		return xmlCommentType;
	}

	private static string ParseTagValue(XElement element)
	{
		var lines = element.Value
			.Replace("\r", "")
			.Split('\n')
			.Select(line => line.Trim())
			.Where(line => line.Length > 0);

		return string.Join(Environment.NewLine, lines);
	}
}