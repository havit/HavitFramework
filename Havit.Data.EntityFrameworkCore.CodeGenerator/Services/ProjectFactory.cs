using System;
using System.Xml.Linq;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services
{
	public class ProjectFactory
	{
		public IProject Create(string csprojPath)
		{
			XDocument content = XDocument.Load(csprojPath, LoadOptions.PreserveWhitespace);
			if (IsDotNetCoreProject(content))
			{
				return new NetCoreProject(csprojPath, content);
			}

			return new LegacyProject(csprojPath, content);
		}

		private bool IsDotNetCoreProject(XDocument content)
		{
			return content.Root.Attribute("Sdk") != null;
		}
	}
}