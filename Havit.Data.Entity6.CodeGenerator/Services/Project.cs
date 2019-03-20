using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Havit.Data.Entity.Patterns.SoftDeletes;

namespace Havit.Data.Entity.CodeGenerator.Services
{
	[DebuggerDisplay("{Filename}")]
	public class Project
	{
		private readonly XNamespace MSBuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";
		private const string CodeGeneratorIdentifier = "HavitEntityCodeGenerator";

		private bool contentChanged = false;
		private readonly XDocument content;
		private readonly List<string> usedFilenames = new List<string>();

		public string Filename
		{
			get;
			private set;
		}

		public Project(string filename)
		{
			this.Filename = filename;
			content = XDocument.Load(filename, LoadOptions.PreserveWhitespace);
			content.Changed += Content_Changed;
		}

		private void Content_Changed(object sender, XObjectChangeEventArgs e)
		{
			contentChanged = true;
		}

		public virtual void AddOrUpdate(string filename)
		{
			filename = NormalizeForProject(filename);

			lock (usedFilenames)
			{
				usedFilenames.Add(filename);
			}

			XElement itemElement;

			if (fileNamesWithElements == null)
			{
				lock (content)
				{
					if (fileNamesWithElements == null)
					{
						fileNamesWithElements = content.Root.Elements(MSBuildNamespace + "ItemGroup").Elements(MSBuildNamespace + "Compile").ToDictionary(element => (string)element.Attributes("Include").Single(), element => element, StringComparer.InvariantCultureIgnoreCase);
					}
				}
			}
			fileNamesWithElements.TryGetValue(filename, out itemElement);
			//itemElement = content.Root.Elements(MSBuildNamespace + "ItemGroup").Elements(MSBuildNamespace + "Compile").Where(element => element.Attributes("Include").Any(attribute => String.Equals(filename, (string)attribute, StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault();

			// abychom do kódu dostali metadata generátoru, tak položku, která jej nemá, odstraníme (a následně přidáme spolu s ní)
			//if (itemElement != null)
			//{
			//	if (itemElement.Elements(MSBuildNamespace + CodeGeneratorIdentifier).Count() == 0)
			//	{
			//		this.RemoveWithNextWhitespace(itemElement);
			//		itemElement = null;
			//	}
			//}

			if (itemElement == null)
			{
				lock (content)
				{
					// najdeme první element ItemGroup obsahující sub-element Compile.
					XElement itemgroup = content.Root.Elements(MSBuildNamespace + "ItemGroup").Where(element => element.Elements(MSBuildNamespace + "Compile").Count() > 0).FirstOrDefault();

					if (itemgroup != null)
					{
						// za poslední element dáme konec řádku, odsazení a element
						itemgroup.Elements().Last().AddAfterSelf(
							new XText("\n"),
							new XText("    "),
							new XElement(MSBuildNamespace + "Compile",
								new XAttribute("Include", filename),
								new XText("\n"),
								new XText("      "),
								new XElement(MSBuildNamespace + "SubType", "Code"),
								new XText("\n"),
								new XText("      "),
								new XElement(MSBuildNamespace + CodeGeneratorIdentifier, "1"),
								new XText("\n"),
								new XText("    ")));
					}
				}
			}
		}
		private Dictionary<string, XElement> fileNamesWithElements;

		//#region Remove
		//public virtual void Remove(string filename)
		//{
		//	// Odstraníme všechny ItemGroupy, které obsahují jakýkoli element Compile s atributem Include rovným názvu souboru.
		//	// protože odstranění za sebou nechá whitespaces (díky LoadOptions.PreserveWhitespaces), odstraníme element vč. následucího whitespace
		//	content.Root.Elements(MSBuildNamespace + "ItemGroup").Elements(MSBuildNamespace + "Compile").Where(element => element.Attributes("Include").Any(attribute => attribute.Value == filename)).ToList().ForEach(element => this.RemoveWithNextWhitespace(element));
		//}
		//#endregion

		public void RemoveUnusedGeneratedFiles()
		{
			lock (content)
			{
				GetUnusedGeneratedFilesElements().ForEach(element =>
				{	
					this.RemoveWithNextWhitespace(element);
				});
			}
		}

		private List<XElement> GetUnusedGeneratedFilesElements()
		{
			lock (usedFilenames)
			{
				// najdeme všechny elementy Compile v ItemGroup, které mají sub-element dle CodeGeneratorIdentifier, ale nejsou v seznamu generovaných souborů
				return content.Root
					.Elements(MSBuildNamespace + "ItemGroup")
					.Elements(MSBuildNamespace + "Compile")
					.Where(element =>
						(element.Elements(MSBuildNamespace + CodeGeneratorIdentifier).Count() > 0)
						&& !element.Attributes("Include").Any(attribute => usedFilenames.Contains(attribute.Value)))
					.ToList();
			}
		}

		public string[] GetUnusedGeneratedFiles()
		{
			lock (content)
			{
				return GetUnusedGeneratedFilesElements().Select(item => Path.Combine(GetProjectRootPath(), item.Attribute("Include").Value)).ToArray();
			}
		}

		public virtual void SaveChanges()
		{
			if (contentChanged)
			{
				content.Save(Filename);
			}
		}

		private void RemoveWithNextWhitespace(XElement element)
		{
			List<XNode> textNodes
				= element.NodesAfterSelf().TakeWhile(node => node is XText).ToList();
			if (element.ElementsAfterSelf().Any())
			{
				// Easy case, remove following text nodes.
				textNodes.ForEach(node => node.Remove());
			}
			else
			{
				// Remove trailing whitespace.
				textNodes.Cast<XText>().TakeWhile(text => !text.Value.Contains("\n"))
						 .ToList().ForEach(text => text.Remove());
				// Fetch text node containing newline, if any.
				XText newLineTextNode
					= element.NodesAfterSelf().OfType<XText>().FirstOrDefault();
				if (newLineTextNode != null)
				{
					string value = newLineTextNode.Value;
					if (value.Length > 1)
					{
						// Composite text node, trim until newline (inclusive).
						newLineTextNode.AddAfterSelf(
							new XText(value.Substring(value.IndexOf('\n') + 1)));
					}
					// Remove original node.
					newLineTextNode.Remove();
				}
			}
			element.Remove();
		}

		public string GetProjectRootNamespace()
		{
			if (_projectRootNamespace == null)
			{
				lock (content)
				{
					if (_projectRootNamespace == null)
					{
						_projectRootNamespace = (string)content.Root
							.Elements(MSBuildNamespace + "PropertyGroup")
							.Elements(MSBuildNamespace + "RootNamespace")
							.First();
					}
				}
			}
			return _projectRootNamespace;
		}
		private string _projectRootNamespace = null;

		public string GetProjectRootPath()
		{
			return System.IO.Path.GetDirectoryName(Filename);
		}

		private string NormalizeForProject(string filename)
		{
			string projectRootPath = GetProjectRootPath();
			if (filename.StartsWith(projectRootPath))
			{
				return filename.Substring(projectRootPath.Length).TrimStart('\\');
			}
			else
			{
				return filename;
			}
		}

		public static Project GetByFolder(string folder)
		{
			var files = System.IO.Directory.GetFiles(folder, "*.csproj");
			if (files.Length == 1)
			{
				return new Project(files[0]);
			}
			return null;
		}

	}
}
