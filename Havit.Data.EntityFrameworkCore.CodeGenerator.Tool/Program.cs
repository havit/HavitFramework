using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool;

public class Program
{
	public static async Task Main()
	{
		Console.WriteLine("HAVIT Entity Framework Core CodeGenerator Tool");
		Console.WriteLine("----------------------------------------------");

		if (!CodeGeneratorToolConfiguration.TryGetConfiguration(out var codeGeneratorToolConfiguration))
		{
			return;
		}

		FileInfo[] files = codeGeneratorToolConfiguration.EntityProjectDirectory
			.GetFiles(codeGeneratorToolConfiguration.EntityAssemblyName + ".dll", SearchOption.AllDirectories)
			.Where(file => !file.Name.EndsWith("Havit.Entity.dll"))
			.Where(file => !file.Name.Contains("ref"))
			.OrderByDescending(item => item.LastAccessTime)
			.ToArray();

		if (files.Length == 0)
		{
			Console.WriteLine($"Assembly {codeGeneratorToolConfiguration.EntityAssemblyName}.dll was not found.");
			return;
		}

		FileInfo applicationEntityAssemblyFileInfo = files.First();
		Console.WriteLine($"Using {applicationEntityAssemblyFileInfo.FullName}.");

		var entityDepsFile = new FileInfo(applicationEntityAssemblyFileInfo.FullName.Replace(".dll", ".deps.json"));
		if (!entityDepsFile.Exists)
		{
			Console.WriteLine($"Deps.json file for {applicationEntityAssemblyFileInfo.Name} not found in the {entityDepsFile.FullName} folder.");
			Console.WriteLine("Make sure the Entity project is properly built.");
			return;
		}

		var entityDependencyContext = new DependencyContextJsonReader().Read(entityDepsFile.OpenRead());
		Console.WriteLine($"Resolving assemblies using {entityDepsFile.FullName}");

		DirectoryInfo[] objDirectory = entityDepsFile.Directory.Parent.Parent.Parent.GetDirectories("obj");
		var assetsJsonFile = objDirectory[0].GetFiles("project.assets.json")[0];

		if (!assetsJsonFile.Exists)
		{
			Console.WriteLine($"project.assets.json for the Entity project not found in the {entityDepsFile.FullName} folder.");
			Console.WriteLine("Make sure the Entity project is properly built.");
			return;
		}

		var entityAssetsContext = new DependencyContextJsonReader().Read(assetsJsonFile.OpenRead());
		Console.WriteLine($"Resolving assemblies using {assetsJsonFile.FullName}");

		// Default AssemblyLoadContext has to be used, because CodeGenerator itself uses it (and currently it's not passed over as parameter).
		var assemblyLoadContext = AssemblyLoadContext.Default;

		var assemblyLoader = new DependencyContextAssemblyLoader(entityDependencyContext, entityAssetsContext, applicationEntityAssemblyFileInfo.Directory.FullName);
		assemblyLoader.RegisterResolvingEvent(assemblyLoadContext);
		Assembly assembly;
		try
		{
			assembly = assemblyLoadContext.LoadFromAssemblyName(new AssemblyName { Name = "Havit.Data.EntityFrameworkCore.CodeGenerator" });
		}
		catch (Exception ex)
		{
			Console.WriteLine("Failed to load Havit.Data.EntityFrameworkCore.CodeGenerator.");
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
			Console.WriteLine("Havit.Data.EntityFrameworkCore.CodeGenerator entry point (class) was not found.");
			return;
		}

		MethodInfo main = program.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);
		if (main == null)
		{
			Console.WriteLine("Havit.Data.EntityFrameworkCore.CodeGenerator entry point (method) was not found.");
			return;
		}

		Console.WriteLine("Starting CodeGenerator...");

		Task mainTask = (Task)main.Invoke(null, new object[]
		{
			new string[] { codeGeneratorToolConfiguration.SolutionDirectory.FullName, Path.GetFileNameWithoutExtension(applicationEntityAssemblyFileInfo.FullName) }
		});

		if (mainTask == null)
		{
			Console.WriteLine("Havit.Data.EntityFrameworkCore.CodeGenerator entry point (method) did not return a Task.");
			return;
		}

		await mainTask;
	}
}