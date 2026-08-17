using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool;

public class Program
{
	private const string LaunchModeArgument = "--launch";
	private const string CodeGeneratorAssemblyName = "Havit.Data.EntityFrameworkCore.CodeGenerator";
	private const string NetCoreAppFrameworkName = "Microsoft.NETCore.App";

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
			// Vynecháváme referenční assemblies (bin/<konfigurace>/<tfm>/ref[int]) - jsou metadata-only a nelze je spustit.
			.Where(file => !string.Equals(file.Directory.Name, "ref", StringComparison.OrdinalIgnoreCase)
				&& !string.Equals(file.Directory.Name, "refint", StringComparison.OrdinalIgnoreCase))
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

		if (projectAssets.CodeGeneratorReferenced == false)
		{
			Console.WriteLine($"The {CodeGeneratorAssemblyName} package is not referenced by the Entity project.");
			Console.WriteLine($"Add the {CodeGeneratorAssemblyName} package reference to the Entity project and rebuild it.");
			Environment.ExitCode = 1;
			return;
		}

		// The Entity project may depend on shared frameworks beyond Microsoft.NETCore.App (typically Microsoft.AspNetCore.App,
		// e.g. via a transitive FrameworkReference from an identity-related package). Assemblies provided by shared frameworks
		// are not listed in deps.json, so the child process host must load the same frameworks as the Entity project itself,
		// otherwise the framework-provided assemblies fail to resolve (FileNotFoundException).
		string entityRuntimeConfigFile = Path.ChangeExtension(applicationEntityAssemblyFileInfo.FullName, ".runtimeconfig.json");
		string temporaryRuntimeConfigFile = null;
		if (File.Exists(entityRuntimeConfigFile))
		{
			Console.WriteLine($"Using runtime configuration {entityRuntimeConfigFile}.");
		}
		else
		{
			// runtimeconfig.json is generated for class libraries only when GenerateRuntimeConfigurationFiles is set
			// (e.g. by Microsoft.EntityFrameworkCore.Design.props for its direct referencers) - fall back to a generated one.
			entityRuntimeConfigFile = temporaryRuntimeConfigFile = TryWriteTemporaryRuntimeConfig(projectAssets, applicationEntityAssemblyFileInfo);
		}

		// Restart ourselves as a child process with the Entity project dependency graph provided to the .NET host.
		// The host then resolves all package assemblies (CodeGenerator, EF Core, ...) natively from NuGet package folders
		// and project reference outputs from the folder of the deps.json file.
		var startInfo = new ProcessStartInfo
		{
			FileName = GetDotnetHostPath(),
			UseShellExecute = false
		};
		startInfo.ArgumentList.Add("exec");
		startInfo.ArgumentList.Add("--depsfile");
		startInfo.ArgumentList.Add(entityDepsFile.FullName);
		if (entityRuntimeConfigFile != null)
		{
			startInfo.ArgumentList.Add("--runtimeconfig");
			startInfo.ArgumentList.Add(entityRuntimeConfigFile);
		}
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

		if (temporaryRuntimeConfigFile != null)
		{
			try
			{
				File.Delete(temporaryRuntimeConfigFile);
			}
			catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
			{
				// best effort - opuštěný soubor v temp složce nezpůsobí žádnou škodu
			}
		}
	}

	/// <summary>
	/// Returns the dotnet host to launch the child process with. Prefers the official DOTNET_HOST_PATH environment
	/// variable, then the host of the current process (a framework-dependent dotnet tool runs under the dotnet muxer;
	/// a global tool shim does not, then the condition does not match), and falls back to "dotnet" from PATH.
	/// </summary>
	private static string GetDotnetHostPath()
	{
		string dotnetHostPath = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH");
		if (!string.IsNullOrEmpty(dotnetHostPath))
		{
			return dotnetHostPath;
		}

		string processPath = Environment.ProcessPath;
		if (string.Equals(Path.GetFileNameWithoutExtension(processPath), "dotnet", StringComparison.OrdinalIgnoreCase))
		{
			return processPath;
		}

		return "dotnet";
	}

	/// <summary>
	/// Writes a temporary runtimeconfig.json for the Entity project so that the child process host loads
	/// the shared frameworks required by the Entity project (beyond Microsoft.NETCore.App provided by default).
	/// Returns null when no additional shared framework is referenced (the default host configuration is then sufficient)
	/// or when the required information is not available (missing project.assets.json, unsupported target framework moniker).
	/// </summary>
	private static string TryWriteTemporaryRuntimeConfig(ProjectAssetsInfo projectAssets, FileInfo entityAssemblyFileInfo)
	{
		List<string> additionalFrameworks = projectAssets.FrameworkReferences
			.Where(framework => !string.Equals(framework, NetCoreAppFrameworkName, StringComparison.OrdinalIgnoreCase))
			.Order(StringComparer.OrdinalIgnoreCase)
			.ToList();
		if (additionalFrameworks.Count == 0)
		{
			return null;
		}

		// Stejná preference jako při výběru assembly - složka výstupu odpovídající target frameworku projektu.
		string targetFramework = projectAssets.TargetFrameworks.Find(tfm => string.Equals(tfm, entityAssemblyFileInfo.Directory.Name, StringComparison.OrdinalIgnoreCase))
			?? projectAssets.TargetFrameworks.FirstOrDefault();

		// Z TFM odvozujeme verzi frameworků stejně jako .NET SDK při generování runtimeconfig.json pro class libraries
		// (net10.0 → 10.0.0). Platform-specific TFM (net10.0-windows) ořežeme na verzi, ostatní tvary (netstandard2.0) vzdáváme.
		if ((targetFramework == null)
			|| !targetFramework.StartsWith("net", StringComparison.OrdinalIgnoreCase)
			|| !Version.TryParse(targetFramework["net".Length..].Split('-')[0], out Version frameworkVersion)
			|| (frameworkVersion.Major < 5))
		{
			return null;
		}
		string frameworkVersionString = $"{frameworkVersion.Major}.{frameworkVersion.Minor}.0";

		var runtimeConfig = new
		{
			runtimeOptions = new
			{
				tfm = targetFramework,
				frameworks = new[] { NetCoreAppFrameworkName }.Concat(additionalFrameworks)
					.Select(framework => new { name = framework, version = frameworkVersionString })
					.ToArray()
			}
		};

		string temporaryRuntimeConfigFile = Path.Combine(Path.GetTempPath(), $"efcodegenerator.{Path.GetRandomFileName()}.runtimeconfig.json");
		File.WriteAllText(temporaryRuntimeConfigFile, JsonSerializer.Serialize(runtimeConfig));

		Console.WriteLine($"Using generated runtime configuration for shared frameworks: {string.Join(", ", additionalFrameworks)}.");

		return temporaryRuntimeConfigFile;
	}

	/// <summary>
	/// Reads NuGet package folders (global packages folder + fallback folders), target frameworks and shared framework
	/// references of the Entity project from obj/project.assets.json (reflects nuget.config overrides). Package folders
	/// fall back to the NUGET_PACKAGES environment variable or the default global packages folder, target frameworks
	/// and framework references are empty when unavailable.
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

						// Přímé FrameworkReference projektu (vč. implicitní Microsoft.NETCore.App).
						if (framework.Value.TryGetProperty("frameworkReferences", out JsonElement projectFrameworkReferences))
						{
							foreach (JsonProperty frameworkReference in projectFrameworkReferences.EnumerateObject())
							{
								result.FrameworkReferences.Add(frameworkReference.Name);
							}
						}
					}
				}
				if (assetsJson.RootElement.TryGetProperty("targets", out JsonElement targets))
				{
					// FrameworkReference přinesené (i tranzitivně) balíčky - např. Duende.IdentityServer → Microsoft.AspNetCore.App.
					foreach (JsonProperty target in targets.EnumerateObject())
					{
						foreach (JsonProperty library in target.Value.EnumerateObject())
						{
							if (library.Value.TryGetProperty("frameworkReferences", out JsonElement libraryFrameworkReferences))
							{
								foreach (JsonElement frameworkReference in libraryFrameworkReferences.EnumerateArray())
								{
									if (frameworkReference.ValueKind == JsonValueKind.String)
									{
										result.FrameworkReferences.Add(frameworkReference.GetString());
									}
								}
							}
						}
					}
				}
				if (assetsJson.RootElement.TryGetProperty("libraries", out JsonElement libraries))
				{
					// Klíče mají tvar "<název>/<verze>", CodeGenerator zde figuruje jako package i jako project(ová reference).
					result.CodeGeneratorReferenced = libraries.EnumerateObject()
						.Any(library => library.Name.StartsWith(CodeGeneratorAssemblyName + "/", StringComparison.OrdinalIgnoreCase));
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

		/// <summary>
		/// Sdílené frameworky (FrameworkReference) v grafu závislostí Entity projektu - přímé i přinesené balíčky.
		/// </summary>
		public HashSet<string> FrameworkReferences { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		/// Indikace, zda je CodeGenerator v grafu závislostí Entity projektu. Null = nepodařilo se zjistit (chybějící/nečitelný project.assets.json).
		/// </summary>
		public bool? CodeGeneratorReferenced { get; set; }
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

		try
		{
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
		catch (Exception exception) when (GetAssemblyLoadFileNotFoundException(exception) is not null)
		{
			// Scénář resolvingu assemblies, se kterým zatím nepočítáme - čitelná hláška místo surového stack trace.
			FileNotFoundException fileNotFoundException = GetAssemblyLoadFileNotFoundException(exception);
			Console.WriteLine(fileNotFoundException.Message);
			Console.WriteLine("The assembly was not resolved from the Entity project dependency graph (deps.json), NuGet package folders or shared frameworks.");
			Console.WriteLine("Make sure the Entity project is properly built. If the problem persists, report the scenario to HAVIT.");
			Environment.ExitCode = 1;
		}
	}

	/// <summary>
	/// Returns the inner FileNotFoundException when the exception represents an assembly load failure
	/// (reflection invocation wraps exceptions in TargetInvocationException). Returns null for other exceptions,
	/// including FileNotFoundException for ordinary files - an assembly load failure carries the assembly
	/// display name (with Version=...) in FileName, an ordinary missing file just a path.
	/// </summary>
	private static FileNotFoundException GetAssemblyLoadFileNotFoundException(Exception exception)
	{
		FileNotFoundException fileNotFoundException = ((exception as TargetInvocationException)?.InnerException as FileNotFoundException) ?? (exception as FileNotFoundException);
		return (fileNotFoundException?.FileName?.Contains("Version=") == true) ? fileNotFoundException : null;
	}
}
