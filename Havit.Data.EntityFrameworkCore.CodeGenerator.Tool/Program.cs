using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("HAVIT Entity Framework Core CodeGenerator Tool");
			Console.WriteLine("----------------------------------------------");

			DirectoryInfo solutionDirectory = new DirectoryInfo(Environment.CurrentDirectory);// @"D:\Dev\002.HFW-NewProjectTemplate";
			while (Directory.GetFiles(solutionDirectory.FullName, "*.sln", SearchOption.TopDirectoryOnly).Length == 0)
			{
				if (solutionDirectory.Root.FullName == solutionDirectory.FullName)
				{
					Console.WriteLine("Solution file (*.sln) was not found.");
					return;
				}
				solutionDirectory = solutionDirectory.Parent;
			}

			var entityBinDirectory = new DirectoryInfo(Path.Combine(solutionDirectory.FullName, "Entity", "bin"));
			if (!entityBinDirectory.Exists)
			{
				Console.WriteLine($"Bin directory for project Entity not found ({entityBinDirectory}).");
				Console.WriteLine("Make sure the project Entity is properly built.");
				return;
			}

			FileInfo[] files = entityBinDirectory
				.GetFiles("*.Entity.dll", SearchOption.AllDirectories)
				.Where(file => !file.Name.EndsWith("Havit.Entity.dll"))
				.Where(file => !file.Name.Contains("ref"))
				.OrderByDescending(item => item.LastAccessTime)
				.ToArray();

			if (files.Length == 0)
			{
				Console.WriteLine("Assembly *.Entity.dll was not found.");
				return;
			}

			FileInfo applicationEntityAssembly = files.First();
			DirectoryInfo workingFolder = applicationEntityAssembly.Directory;
			Console.WriteLine($"Using folder {workingFolder}.");

			var dataLayerDepsFile = new FileInfo(applicationEntityAssembly.FullName.Replace(".Entity.dll", ".Entity.deps.json"));
			if (!dataLayerDepsFile.Exists)
			{
				Console.WriteLine($"Deps.json file for *.Entity.dll not found: {dataLayerDepsFile.GetRelativePath(solutionDirectory)}");
				Console.WriteLine("Make sure Entity project is properly built.");
				return;
			}

			var dataLayerDependencyContext = new DependencyContextJsonReader().Read(dataLayerDepsFile.OpenRead());
			Console.WriteLine($"Resolving assemblies using {dataLayerDepsFile.GetRelativePath(solutionDirectory)}");

			DirectoryInfo[] objDirectory = dataLayerDepsFile.Directory.Parent.Parent.Parent.GetDirectories("obj");
			var assetsJsonFile = objDirectory[0].GetFiles("project.assets.json")[0];

			if (!assetsJsonFile.Exists)
			{
				Console.WriteLine($"project.assets.json for Entity not found: {dataLayerDepsFile.GetRelativePath(solutionDirectory)}");
				Console.WriteLine("Make sure Entity project is properly built.");
				return;
			}

			var dataLayerAssetsContext = new DependencyContextJsonReader().Read(assetsJsonFile.OpenRead());
			Console.WriteLine($"Resolving assemblies using {assetsJsonFile.GetRelativePath(solutionDirectory)}");

			// Default AssemblyLoadContext has to be used, because CodeGenerator itself uses it (and currently it's not passed over as parameter).
			var assemblyLoadContext = AssemblyLoadContext.Default;
			var assemblyLoader = new DependencyContextAssemblyLoader(dataLayerDependencyContext, dataLayerAssetsContext, workingFolder.FullName);
			assemblyLoader.RegisterResolvingEvent(assemblyLoadContext);

			Assembly assembly;
			try
			{
				assembly = assemblyLoadContext.LoadFromAssemblyName(new AssemblyName { Name = "Havit.Data.EntityFrameworkCore.CodeGenerator" });
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to load Havit.Data.EntityFrameworkCore.CodeGenerator");
				Console.WriteLine(ex);
				throw;
			}

			if (assembly == null)
			{
				Console.WriteLine("Assembly Havit.Data.EntityFrameworkCore.CodeGenerator could not be loaded.");
				return;
			}

			Type program = assembly.GetType("Havit.Data.EntityFrameworkCore.CodeGenerator.Program");
			if (program == null)
			{
				Console.WriteLine("CodeGenerator entry point (class) was not found.");
				return;
			}

			MethodInfo main = program.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);
			if (program == null)
			{
				Console.WriteLine("CodeGenerator entry point (method) was not found.");
				return;
			}

			Console.WriteLine("Starting CodeGenerator...");
			main.Invoke(null, new object[]
			{
				new string[] { solutionDirectory.FullName, Path.GetFileNameWithoutExtension(applicationEntityAssembly.FullName) }
			});
		}
	}
}