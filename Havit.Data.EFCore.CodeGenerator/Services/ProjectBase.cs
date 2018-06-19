using System.Linq;
using System.Xml.Linq;

namespace Havit.Data.Entity.CodeGenerator.Services
{
	public abstract class ProjectBase : IProject
	{
		protected XDocument Content { get; }

		public string Filename { get; }

		protected ProjectBase(string filename, XDocument content)
		{
			Filename = filename;
			Content = content;
		}

		public abstract void AddOrUpdate(string filename);

		public abstract string[] GetUnusedGeneratedFiles();

		public abstract void RemoveUnusedGeneratedFiles();

		public abstract void SaveChanges();

		public abstract string GetProjectRootNamespace();

		public string GetProjectRootPath()
		{
			return System.IO.Path.GetDirectoryName(Filename);
		}

		protected string GetProjectRootNamespaceCore(XNamespace @namespace)
		{
			return (string)Content.Root
				.Elements(@namespace + "PropertyGroup")
				.Elements(@namespace + "RootNamespace")
				.FirstOrDefault();
		}
	}
}