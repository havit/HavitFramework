using System.Xml.Linq;
using Havit.Business.BusinessLayerGenerator.Csproj;

namespace Havit.Business.BusinessLayerGenerator.Csproj;

public class CsprojFileFactory
{
	public CsprojFile GetByFolder(string folder, string generatorIdentifier)
	{
		var files = System.IO.Directory.GetFiles(folder, "*.csproj");
		if (files.Length == 1)
		{
			return Create(files[0], generatorIdentifier);
		}
		return null;
	}

	public CsprojFile Create(string csprojPath, string generatorIdentifier)
	{
		XDocument content = XDocument.Load(csprojPath, LoadOptions.PreserveWhitespace);
		if (IsDotNetCoreProject(content))
		{
			return new NetCoreCsprojFile(csprojPath, generatorIdentifier, content);
		}

		return new CsprojFile(csprojPath, generatorIdentifier, content);
	}

	private bool IsDotNetCoreProject(XDocument content)
	{
		return content.Root.Attribute("Sdk") != null;
	}
}