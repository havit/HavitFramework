using System.Collections.Immutable;
using Havit.Extensions.DependencyInjection.SourceGenerators.Tests.Infrastructure;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

[TestClass]
public partial class ServiceRegistrationsTests
{
	private static readonly ReferenceAssemblies Reference = ReferenceAssemblies
		.Net
		.Net80
		.AddPackages(ImmutableArray.Create(
			new PackageIdentity("Microsoft.Extensions.DependencyInjection", "8.0.0"),
			new PackageIdentity("Microsoft.Extensions.Hosting.Abstractions", "8.0.0")));

	public TestContext TestContext { get; set; }

	private static async Task VerifyGeneratorAsync(string sourceInput, string expectedSourceOutput, List<DiagnosticResult> expectedDiagnostics = null, CancellationToken cancellationToken = default)
	{
		var test = new Microsoft.CodeAnalysis.CSharp.Testing.CSharpSourceGeneratorTest<SourceGeneratorAdapter<ServiceRegistrationsGenerator>, Microsoft.CodeAnalysis.Testing.DefaultVerifier>
		{
			ReferenceAssemblies = Reference,
			SolutionTransforms =
			{
				(solution, projectId) =>
				{
					var project = solution.GetProject(projectId);
					project = project.AddMetadataReference(MetadataReference.CreateFromFile(typeof(Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute).Assembly.Location));

					return project.Solution;
				}
			},
			TestState =
			{
				Sources = { sourceInput },
				AnalyzerConfigFiles =
				{
					("/.editorconfig", $@"""
						is_global=true
						build_property.RootNamespace = Havit.TestProject.Services
						""")
				}
			}
		};

		if (expectedSourceOutput != null)
		{
			test.TestState.GeneratedSources.Add((typeof(SourceGeneratorAdapter<ServiceRegistrationsGenerator>), ServiceRegistrationsGenerator.GeneratedOutputFileName, expectedSourceOutput));
		}

		if (expectedDiagnostics != null)
		{
			test.ExpectedDiagnostics.AddRange(expectedDiagnostics);
		}

		await test.RunAsync(cancellationToken);
	}
}
