using System.Xml.Linq;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

public class ProjectFactory : IProjectFactory
{
	public IProject Create(string folderOrCsprojPath)
	{
		string csprojPath;
		if (folderOrCsprojPath.EndsWith(".csproj"))
		{
			csprojPath = folderOrCsprojPath;
		}
		else
		{
			var csprojFile = new DirectoryInfo(folderOrCsprojPath).EnumerateFiles("*.csproj").SingleOrDefault();
			if (csprojFile == null)
			{
				throw new InvalidOperationException($"No csproj found in {folderOrCsprojPath}.");
			}
			csprojPath = csprojFile.FullName;
		}

		XDocument content = XDocument.Load(csprojPath, LoadOptions.PreserveWhitespace);
		if (IsDotNetCoreProject(content))
		{
			return new Project(csprojPath, content);
		}

		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"{csprojPath} has an old csproj format, use the new csproj format.");
		return null;
	}

	private bool IsDotNetCoreProject(XDocument content)
	{
		return content.Root.Attribute("Sdk") != null;
	}
}