using System.Collections.Immutable;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using Havit.Extensions.DependencyInjection.Analyzers;
using Havit.Extensions.DependencyInjection.SourceGenerators.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

[TestClass]
public partial class ServiceRegistrationsTests
{
	private static readonly ReferenceAssemblies Reference = ReferenceAssemblies
		.Net
		.Net80
		.AddPackages(ImmutableArray.Create(
			new PackageIdentity("Havit.Extensions.DependencyInjection.Abstractions", "2.0.20"),
			new PackageIdentity("Microsoft.Extensions.DependencyInjection", "8.0.0"),
			new PackageIdentity("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")));

	private static async Task VerifyGeneratorAsync(string sourceInput, string expectedSourceOutput, CancellationToken cancellationToken = default)
	{
		var test = new Microsoft.CodeAnalysis.CSharp.Testing.CSharpSourceGeneratorTest<SourceGeneratorAdapter<ServiceRegistrationsGenerator>, Microsoft.CodeAnalysis.Testing.DefaultVerifier>
		{
			ReferenceAssemblies = Reference,
			TestState =
			{
				Sources = { sourceInput }
			}
		};

		if (expectedSourceOutput != null)
		{
			test.TestState.GeneratedSources.Add((typeof(SourceGeneratorAdapter<ServiceRegistrationsGenerator>), ServiceRegistrationsGenerator.GeneratedOutputFileName, expectedSourceOutput));
		}

		await test.RunAsync(cancellationToken);
	}
}
