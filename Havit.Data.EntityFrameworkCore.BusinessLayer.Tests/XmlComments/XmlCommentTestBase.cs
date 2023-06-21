using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments;

public abstract class XmlCommentTestBase
{
	protected static XDocument ParseXmlFile()
	{
		try
		{
			return XDocument.Parse(File.ReadAllText(GetXmlCommentsFileFromAssembly(typeof(XmlCommentParserTests).Assembly)));
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("Could not parse test XML file: " + ex.Message, ex);
		}
	}

	private static string GetXmlCommentsFileFromAssembly(Assembly assembly)
	{
		var assemblyFile = new FileInfo(new Uri(assembly.Location).LocalPath);
		var xmlFile = $"{Path.GetFileNameWithoutExtension(assemblyFile.Name)}.xml";
		return assemblyFile.Directory?.GetFiles(xmlFile).FirstOrDefault()?.FullName;
	}
}