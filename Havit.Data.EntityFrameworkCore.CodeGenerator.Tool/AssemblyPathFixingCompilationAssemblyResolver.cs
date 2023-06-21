using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool;

/// <summary>
/// Custom <see cref="ICompilationAssemblyResolver"/>, which determins real assemblies of <see cref="CompilationLibrary"/>, when resolving assembly paths.
/// </summary>
public class AssemblyPathFixingCompilationAssemblyResolver : ICompilationAssemblyResolver
{
	private readonly ICompilationAssemblyResolver innerResolver;

	public DependencyContext DependencyContext { get; }

	public AssemblyPathFixingCompilationAssemblyResolver(DependencyContext dependencyContext, ICompilationAssemblyResolver innerResolver)
	{
		DependencyContext = dependencyContext;
		this.innerResolver = innerResolver;
	}

	public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
	{
		// DependencyContextJsonReader does not properly fill metadata for CompilationLibrary (for packages)
		// (or rather CompilationLibrary.Assemblies does not contain assemblies which actually belong to the package).

		// This results in failure to find assemblies when resolving their paths in PackageCompilationAssemblyResolver,
		// which attempts to find assemblies in <userprofile>/.nuget/packages/<package>/<version>/ directory. This is not correct, since
		// assemblies are (usually) located in <userprofile>/.nuget/packages/<package>/<version>/lib/<framework>/ directory.

		// The workaround is to find runtime assemblies from list of runtime libraries (i.e. "targets" part deps.json / project.assets.json).
		// PackageCompilationAssemblyResolver is then able to properly resolve paths to assemblies belonging to packages.

		// This is only possible when DependencyContext is loaded from <project>/obj/project.assets.json, since only this file contains
		// all the necessary packages and assemblies used by the project. DependencyContext for project assemblies located
		// in <assembly>.deps.json (in bin directory) is used to resolve paths to project assemblies (see DependencyContextAssemblyLoader for more information).

		RuntimeLibrary runtimeLibrary = DependencyContext.RuntimeLibraries.FirstOrDefault(l => l.Name == library.Name);
		RuntimeAssetGroup ridGroup = runtimeLibrary?.RuntimeAssemblyGroups.FirstOrDefault(g => g.Runtime == DependencyContext.Target.Runtime);
		string[] runtimeAssemblies = ridGroup?.RuntimeFiles.Select(f => f.Path).ToArray();

		CompilationLibrary libraryToResolve = library;
		if (runtimeAssemblies != null)
		{
			libraryToResolve = new CompilationLibrary(library.Type,
				library.Name,
				library.Version,
				library.Hash,
				runtimeAssemblies,
				library.Dependencies,
				library.Serviceable,
				library.Path,
				library.HashPath);
		}

		return innerResolver.TryResolveAssemblyPaths(libraryToResolve, assemblies);
	}
}