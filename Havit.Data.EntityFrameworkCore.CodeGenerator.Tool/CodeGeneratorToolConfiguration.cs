using System.Xml;
using System.Xml.Linq;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool
{
	/// <summary>
	/// Konfigurace používaná v CodeGeneratorTool.
	/// </summary>
	public class CodeGeneratorToolConfiguration
	{
		public required DirectoryInfo SolutionDirectory { get; set; }
		public required DirectoryInfo EntityProjectDirectory { get; set; }
		public string EntityAssemblyName { get; set; }

		public static bool TryGetConfiguration(out CodeGeneratorToolConfiguration result)
		{
			bool hasConfiguration = CodeGeneratorLimitedConfiguration.TryGetConfiguration(out var codeGeneratorLimitedConfiguration);

			DirectoryInfo solutionDirectory;
			if (hasConfiguration && !String.IsNullOrEmpty(codeGeneratorLimitedConfiguration.SolutionDirectory))
			{
				// pokud máme SolutionDirectory v konfiguraci, použijeme jej 
				solutionDirectory = new DirectoryInfo(codeGeneratorLimitedConfiguration.SolutionDirectory);
				if (!solutionDirectory.Exists)
				{
					Console.WriteLine($"Solution folder {solutionDirectory.FullName} does not exist.");
					result = null;
					return false;
				}
			}
			else
			{
				// pokud nemáme SolutionDirectory v konfiguraci, procházíme složkami nahoru, než najdeme *.sln
				solutionDirectory = new DirectoryInfo(Environment.CurrentDirectory);// @"D:\Dev\002.HFW-NewProjectTemplate";
				while ((Directory.GetFiles(solutionDirectory.FullName, "*.sln", SearchOption.TopDirectoryOnly).Length == 0) && (Directory.GetFiles(solutionDirectory.FullName, "*.slnx", SearchOption.TopDirectoryOnly).Length == 0))
				{
					if (solutionDirectory.Root.FullName == solutionDirectory.FullName)
					{
						Console.WriteLine("Solution file (*.sln[x]) was not found.");
						result = null;
						return false;
					}
					solutionDirectory = solutionDirectory.Parent;
				}
			}

			DirectoryInfo entityBinDirectory;
			string entityAssemblyName;
			if (hasConfiguration && !String.IsNullOrEmpty(codeGeneratorLimitedConfiguration.EntityProjectPath))
			{
				// pokud máme EntityProjectPath v konfiguraci, použijeme jej
				FileInfo entityProjectPath = new System.IO.FileInfo(Path.Combine(solutionDirectory.FullName, codeGeneratorLimitedConfiguration.EntityProjectPath));
				if (!entityProjectPath.Exists)
				{
					Console.WriteLine($"Entity project file {entityProjectPath.FullName} does not exists.");
					result = null;
					return false;
				}

				entityBinDirectory = new DirectoryInfo(Path.Combine(entityProjectPath.Directory.FullName, "bin"));
				if (!entityBinDirectory.Exists)
				{
					Console.WriteLine($"Bin directory for project Entity {entityBinDirectory} does not exists.");
					Console.WriteLine("Make sure the Entity project is properly built.");

					result = null;
					return false;
				}

				entityAssemblyName = GetAssemblyNameFromCsproj(entityProjectPath);
			}
			else
			{
				// pokud nemáme EntityProjectPath v konfiguraci, použijeme default
				entityBinDirectory = new DirectoryInfo(Path.Combine(solutionDirectory.FullName, "Entity", "bin"));
				entityAssemblyName = GetAssemblyNameFromCsproj(new FileInfo(Path.Combine(entityBinDirectory.Parent.FullName, "Entity.csproj")));
			}

			result = new CodeGeneratorToolConfiguration
			{
				SolutionDirectory = solutionDirectory,
				EntityProjectDirectory = entityBinDirectory,
				EntityAssemblyName = entityAssemblyName
			};

			return true;
		}

		private static string GetAssemblyNameFromCsproj(FileInfo csprojFile)
		{
			var csprojXml = XDocument.Load(csprojFile.FullName);
			var assemblyNameElement = csprojXml.Root.Elements("PropertyGroup").Elements("AssemblyName").FirstOrDefault();
			if (assemblyNameElement != null)
			{
				return assemblyNameElement.Value;
			}
			else
			{
				return Path.GetFileNameWithoutExtension(csprojFile.Name);
			}
		}
	}
}
