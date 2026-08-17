using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool;

public class Program
{
	private const string LaunchModeArgument = "--launch";
	private const string CodeGeneratorAssemblyName = "Havit.Data.EntityFrameworkCore.CodeGenerator";

	public static async Task Main(string[] args)
	{
		if ((args.Length == 3) && (args[0] == LaunchModeArgument))
		{
			// Launch mode: we are running as a child process started by the locator mode below (dotnet exec --depsfile ...).
			// Assembly resolution of the Entity project dependency graph is provided natively by the .NET host
			// (deps.json of the Entity project + additional probing paths pointing to NuGet package folders).
			await RunCodeGeneratorAsync(entityAssemblyPath: args[1], solutionDirectory: args[2]);
			return;
		}

		RunLocator();
	}

	private static void RunLocator()
	{
		Console.WriteLine("HAVIT Entity Framework Core CodeGenerator Tool");
		Console.WriteLine("----------------------------------------------");

		if (!CodeGeneratorToolConfiguration.TryGetConfiguration(out var codeGeneratorToolConfiguration))
		{
			Environment.ExitCode = 1;
			return;
		}

		ProjectAssetsInfo projectAssets = ReadProjectAssets(codeGeneratorToolConfiguration.EntityProjectDirectory);

		// Preferujeme výstupní složku odpovídající aktuálnímu target frameworku projektu (bin/<konfigurace>/<tfm>),
		// aby nevyhrávaly opuštěné výstupy po retargetingu nebo publish složky. V rámci preference řadíme podle
		// LastWriteTime (LastAccessTime je na NTFS často vypnutý a na APFS se chová odlišně, řazení by bylo nahodilé).
		FileInfo[] files = codeGeneratorToolConfiguration.EntityProjectDirectory
			.GetFiles(codeGeneratorToolConfiguration.EntityAssemblyName + ".dll", SearchOption.AllDirectories)
			.Where(file => !file.Name.EndsWith("Havit.Entity.dll"))
			.Where(file => !file.Name.Contains("ref"))
			.OrderByDescending(file => projectAssets.TargetFrameworks.Contains(file.Directory.Name, StringComparer.OrdinalIgnoreCase))
			.ThenByDescending(item => item.LastWriteTime)
			.ToArray();

		if (files.Length == 0)
		{
			Console.WriteLine($"Assembly {codeGeneratorToolConfiguration.EntityAssemblyName}.dll was not found.");
			Environment.ExitCode = 1;
			return;
		}

		FileInfo applicationEntityAssemblyFileInfo = files.First();
		Console.WriteLine($"Using {applicationEntityAssemblyFileInfo.FullName}.");

		var entityDepsFile = new FileInfo(Path.ChangeExtension(applicationEntityAssemblyFileInfo.FullName, ".deps.json"));
		if (!entityDepsFile.Exists)
		{
			Console.WriteLine($"Deps.json file for {applicationEntityAssemblyFileInfo.Name} not found in the {entityDepsFile.DirectoryName} folder.");
			Console.WriteLine("Make sure the Entity project is properly built.");
			Environment.ExitCode = 1;
			return;
		}
		Console.WriteLine($"Using dependency manifest {entityDepsFile.FullName}.");

		// Restart ourselves as a child process with the Entity project dependency graph provided to the .NET host.
		// The host then resolves all package assemblies (CodeGenerator, EF Core, ...) natively from NuGet package folders
		// and project reference outputs from the folder of the deps.json file.
		var startInfo = new ProcessStartInfo
		{
			FileName = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet",
			UseShellExecute = false
		};
		startInfo.ArgumentList.Add("exec");
		startInfo.ArgumentList.Add("--depsfile");
		startInfo.ArgumentList.Add(entityDepsFile.FullName);
		foreach (string packageFolder in projectAssets.PackageFolders)
		{
			startInfo.ArgumentList.Add("--additionalprobingpath");
			startInfo.ArgumentList.Add(packageFolder);
		}
		startInfo.ArgumentList.Add(typeof(Program).Assembly.Location);
		startInfo.ArgumentList.Add(LaunchModeArgument);
		startInfo.ArgumentList.Add(applicationEntityAssemblyFileInfo.FullName);
		startInfo.ArgumentList.Add(codeGeneratorToolConfiguration.SolutionDirectory.FullName);

		Console.WriteLine("Starting CodeGenerator...");

		using Process process = Process.Start(startInfo);
		process.WaitForExit();
		Environment.ExitCode = process.ExitCode;
	}

	/// <summary>
	/// Reads NuGet package folders (global packages folder + fallback folders) and target frameworks of the Entity project
	/// from obj/project.assets.json (reflects nuget.config overrides). Package folders fall back to the NUGET_PACKAGES
	/// environment variable or the default global packages folder, target frameworks are empty when unavailable.
	/// </summary>
	private static ProjectAssetsInfo ReadProjectAssets(DirectoryInfo entityBinDirectory)
	{
		ProjectAssetsInfo result = new ProjectAssetsInfo();

		try
		{
			var assetsFile = new FileInfo(Path.Combine(entityBinDirectory.Parent.FullName, "obj", "project.assets.json"));
			if (assetsFile.Exists)
			{
				using FileStream assetsStream = assetsFile.OpenRead();
				using JsonDocument assetsJson = JsonDocument.Parse(assetsStream);
				if (assetsJson.RootElement.TryGetProperty("packageFolders", out JsonElement packageFolders))
				{
					foreach (JsonProperty packageFolder in packageFolders.EnumerateObject())
					{
						result.PackageFolders.Add(packageFolder.Name);
					}
				}
				if (assetsJson.RootElement.TryGetProperty("project", out JsonElement project)
					&& project.TryGetProperty("frameworks", out JsonElement frameworks))
				{
					foreach (JsonProperty framework in frameworks.EnumerateObject())
					{
						result.TargetFrameworks.Add(framework.Name);
					}
				}
			}
		}
		catch (Exception exception) when (exception is IOException or JsonException)
		{
			// project.assets.json is only a better source of the information, not a required one - fall back below.
		}

		if (result.PackageFolders.Count == 0)
		{
			// Fallback pro případ, kdy project.assets.json není k dispozici (vyčištěný obj, nestandardní layout):
			// NUGET_PACKAGES je oficiální proměnná prostředí, kterou NuGet i .NET SDK respektují pro přemístění
			// globální package cache (typicky na CI agentech). Není-li nastavena, platí výchozí umístění
			// %USERPROFILE%\.nuget\packages (Windows), resp. ~/.nuget/packages (macOS/Linux).
			// Oproti packageFolders z assets.json zde chybí případné fallback folders a globalPackagesFolder
			// z nuget.config - to je akceptovaná mez fallbacku, primárním zdrojem zůstává assets.json.
			result.PackageFolders.Add(Environment.GetEnvironmentVariable("NUGET_PACKAGES")
				?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages"));
		}

		return result;
	}

	private sealed class ProjectAssetsInfo
	{
		public List<string> PackageFolders { get; } = new List<string>();
		public List<string> TargetFrameworks { get; } = new List<string>();
	}

	private static async Task RunCodeGeneratorAsync(string entityAssemblyPath, string solutionDirectory)
	{
		string entityDirectory = Path.GetDirectoryName(entityAssemblyPath);

		// Safety net for assemblies the host cannot resolve natively: probe the Entity project output folder
		// (project reference outputs are always copied there).
		AssemblyLoadContext.Default.Resolving += (context, assemblyName) =>
		{
			string candidatePath = Path.Combine(entityDirectory, assemblyName.Name + ".dll");
			return File.Exists(candidatePath) ? context.LoadFromAssemblyPath(candidatePath) : null;
		};

		// The Entity assembly itself is not listed in any probing location known to the host - preload it from the known path,
		// so that the subsequent Assembly.Load(entityAssemblyName) in CodeGenerator finds it already loaded.
		try
		{
			AssemblyLoadContext.Default.LoadFromAssemblyPath(entityAssemblyPath);
		}
		catch (Exception exception)
		{
			Console.WriteLine($"Failed to load the Entity assembly {entityAssemblyPath}.");
			Console.WriteLine(exception.Message);
			Environment.ExitCode = 1;
			return;
		}

		Assembly assembly;
		try
		{
			assembly = Assembly.Load(new AssemblyName { Name = CodeGeneratorAssemblyName });
		}
		catch (Exception exception)
		{
			Console.WriteLine($"Failed to load {CodeGeneratorAssemblyName}.");
			Console.WriteLine($"Make sure the {CodeGeneratorAssemblyName} package is referenced by the Entity project and the project is properly built.");
			Console.WriteLine(exception);
			Environment.ExitCode = 1;
			return;
		}

		Type program = assembly.GetType($"{CodeGeneratorAssemblyName}.Program");
		if (program == null)
		{
			Console.WriteLine($"{CodeGeneratorAssemblyName} entry point (class) was not found.");
			Environment.ExitCode = 1;
			return;
		}

		MethodInfo main = program.GetMethod("Main", BindingFlags.Static | BindingFlags.Public);
		if (main == null)
		{
			Console.WriteLine($"{CodeGeneratorAssemblyName} entry point (method) was not found.");
			Environment.ExitCode = 1;
			return;
		}

		Task mainTask = (Task)main.Invoke(null, new object[]
		{
			new string[] { solutionDirectory, Path.GetFileNameWithoutExtension(entityAssemblyPath) }
		});

		if (mainTask == null)
		{
			Console.WriteLine($"{CodeGeneratorAssemblyName} entry point (method) did not return a Task.");
			Environment.ExitCode = 1;
			return;
		}

		await mainTask;
	}
}
