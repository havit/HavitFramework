using System;
using System.Linq;
using System.Xml.Linq;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services
{
	public class ProjectFactory
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
				var csprojFile = new System.IO.DirectoryInfo(folderOrCsprojPath).EnumerateFiles("*.csproj").SingleOrDefault();
				if (csprojFile == null)
                {
					throw new InvalidOperationException($"No csproj found in {folderOrCsprojPath}.");
                }
				csprojPath = csprojFile.FullName;
			}

			XDocument content = XDocument.Load(csprojPath, LoadOptions.PreserveWhitespace);
			if (IsDotNetCoreProject(content))
			{
				return new NetCoreProject(csprojPath, content);
			}

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"{csprojPath} has an old csproj format, use the new csproj format.");
			Console.ResetColor();
			return new LegacyProject(csprojPath, content);
		}

		private bool IsDotNetCoreProject(XDocument content)
		{
			return content.Root.Attribute("Sdk") != null;
		}
	}
}