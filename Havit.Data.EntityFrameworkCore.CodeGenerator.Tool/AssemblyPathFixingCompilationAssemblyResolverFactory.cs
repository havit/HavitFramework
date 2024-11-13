﻿using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Tool;

public class AssemblyPathFixingCompilationAssemblyResolverFactory
{
	public static readonly ICompilationAssemblyResolver[] DefaultResolvers =
	{
		new ReferenceAssemblyPathResolver(),
		new PackageCompilationAssemblyResolver()
	};

	private readonly CompositeCompilationAssemblyResolver _innerResolver;

	public AssemblyPathFixingCompilationAssemblyResolverFactory(string appBasePath)
	{
		_innerResolver = new CompositeCompilationAssemblyResolver(
			new[] { new AppBaseCompilationAssemblyResolver(appBasePath) }
				.Concat(DefaultResolvers).ToArray());
	}

	public AssemblyPathFixingCompilationAssemblyResolver Create(DependencyContext dependencyContext)
		=> new AssemblyPathFixingCompilationAssemblyResolver(dependencyContext, _innerResolver);
}