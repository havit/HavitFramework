﻿using System.Xml.Linq;

namespace Havit.Business.BusinessLayerGenerator.Csproj;

public class NetCoreCsprojFile : CsprojFile
{
	public NetCoreCsprojFile(string filename, string generatorIdentifier, XDocument content)
		: base(filename, generatorIdentifier, content)
	{
	}

	protected override XNamespace MSBuildNamespace { get; } = XNamespace.None;

	protected override XElement FindPositionForEmbeddedResources()
	{
		XElement itemGroup = Content.Root.Elements("ItemGroup").FirstOrDefault(element => element.Elements("EmbeddedResource").Any());
		if (itemGroup == null)
		{
			var packageReference = Content.Root.Elements("ItemGroup").FirstOrDefault(element => element.Elements("PackageReference").Any())?.ElementsBeforeSelf().FirstOrDefault();
			itemGroup = new XElement("ItemGroup");
			packageReference.AddAfterSelf(
				new XText("\n"),
				new XText("  "),
				new XText("\n"),
				new XText("  "),
				itemGroup);
		}

		return itemGroup;
	}

	public override void Ensures(string filename)
	{
		// new .csproj file does not need explicitly list files to compile in Compile ItemGroup
	}

	protected override void SaveChangesCore()
	{
		File.WriteAllText(Filename, Content.ToString().Trim());
	}
}