using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Havit.Extensions.DependencyInjection.Abstractions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.Analyzers;

// TODO: Komentář
/// <summary>
/// TODO
/// </summary>
[Generator]
public class ServiceRegistrationsGenerator : IIncrementalGenerator
{
	internal const string GeneratedOutputFileName = "ServiceCollectionExtension.g.cs";

	/// <inheritdoc />
	public void Initialize(IncrementalGeneratorInitializationContext initializationContext)
	{
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationEntriesProvider = initializationContext
			.SyntaxProvider
			.ForAttributeWithMetadataName(
				fullyQualifiedMetadataName: typeof(ServiceAttribute).FullName,
				predicate: static (node, _) => node is ClassDeclarationSyntax,
				transform: static (ctx, _) => GetServiceGenerations_ServiceAttribute0(ctx))
			.SelectMany((fields, _) => fields)
			.Collect();

		initializationContext.RegisterSourceOutput(serviceRegistrationEntriesProvider, static (sourceContext, source) =>
		{
			GenerateSourceCode(source, sourceContext);
		});

	}

	private static IEnumerable<ServiceRegistrationEntry> GetServiceGenerations_ServiceAttribute0(GeneratorAttributeSyntaxContext context)
	{
		if (context.TargetNode is not ClassDeclarationSyntax classDeclarationSyntax) yield break;

		var classSymbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
		var classTypeName = classSymbol.ToDisplayString();

		foreach (AttributeData attribute in context.Attributes)
		{
			yield return new ServiceRegistrationEntry
			{
				ImplementationType = classTypeName,
				Lifetime = ExtractLifetime(attribute),
				Profile = ExtractProfile(attribute),
				//ServiceTypes = ExtractServiceTypes0(attribute)
			};
		}
	}

	private static ServiceLifetime ExtractLifetime(AttributeData attributeData)
	{
		ServiceLifetime defaultLifetime = ServiceLifetime.Transient;

		KeyValuePair<string, TypedConstant> lifetimeArgument = attributeData.NamedArguments.SingleOrDefault(a => a.Key == nameof(ServiceAttribute.Lifetime));
		if (EqualityComparer<KeyValuePair<string, TypedConstant>>.Default.Equals(lifetimeArgument, default))
		{
			return defaultLifetime;
		}

		if (lifetimeArgument.Value.Kind == TypedConstantKind.Error)
		{
			return defaultLifetime;
		}

		return (ServiceLifetime)lifetimeArgument.Value.Value;
	}

	private static string ExtractProfile(AttributeData attributeData)
	{
		KeyValuePair<string, TypedConstant> profileArgument = attributeData.NamedArguments.SingleOrDefault(a => a.Key == nameof(ServiceAttribute.Profile));
		if (EqualityComparer<KeyValuePair<string, TypedConstant>>.Default.Equals(profileArgument, default))
		{
			return ServiceAttribute.DefaultProfile;
		}

		if (profileArgument.Value.Kind == TypedConstantKind.Error)
		{
			return ServiceAttribute.DefaultProfile;
		}

		return (string)profileArgument.Value.Value;
	}

	//foreach (var attributeListSyntax in classDeclaration.AttributeLists)

	//{
	//	foreach (var attributeSyntax in attributeListSyntax.Attributes)
	//	{
	//		if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
	//		{
	//			continue;
	//		}

	//		var fullyQualifiedAttributeName = attributeSymbol.ContainingType.ToString();

	//		if (RegistrationTypes.TryGetValue(fullyQualifiedAttributeName, out var registrationType))
	//		{
	//			var symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(classDeclaration);
	//			var typeName = symbol.ToDisplayString();

	//			var attributeData = symbol.GetFirstAutoRegisterAttribute(fullyQualifiedAttributeName);

	//			string[] registerAs;
	//			string serviceKey = string.Empty;

	//			if (attributeData?.AttributeConstructor?.Parameters.Length > 0 &&
	//				attributeData?.AttributeConstructor?.Parameters.Any(a => a.Name == SERVICE_KEY) is true)
	//			{
	//				serviceKey = attributeData?.ConstructorArguments.First().Value?.ToString();
	//			}

	//			if (attributeData?.AttributeConstructor?.Parameters.Length > 0 && attributeData.GetIgnoredTypeNames(ONLY_REGISTER_AS) is { Length: > 0 } onlyRegisterAs)
	//			{
	//				registerAs = symbol!
	//				.AllInterfaces
	//				.Select(x => x.ToDisplayString())
	//				.Where(x => onlyRegisterAs.Contains(x))
	//				.ToArray();
	//			}
	//			else
	//			{
	//				registerAs = symbol!
	//				.Interfaces
	//				.Select(x => x.ToDisplayString())
	//				.Where(x => !IgnoredInterfaces.Contains(x))
	//				.ToArray();
	//			}

	//			return new AutoRegisteredClass(
	//				typeName,
	//				registrationType,
	//				registerAs,
	//				serviceKey);
	//		}
	//	}

	private static void GenerateSourceCode(ImmutableArray<ServiceRegistrationEntry> serviceRegistrations, SourceProductionContext context)
	{
		if (!serviceRegistrations.Any())
		{
			return;
		}
		// TODO: Název metody?
		// TODO: Profily?

		var formatted = string.Join("\r\n", serviceRegistrations.Select(c => "// " + c.GetCode()));
		var output = formatted;
		context.AddSource(GeneratedOutputFileName, SourceText.From(output, Encoding.UTF8));

	}

	internal class ServiceRegistrationEntry
	{
		public string[] ServiceTypes { get; set; }
		public string ImplementationType { get; set; }
		public string Profile { get; set; }
		public ServiceLifetime Lifetime { get; set; }

		public string GetCode()
		{
			return $"services.Add{Lifetime}<{ServiceTypes?.FirstOrDefault()}, {ImplementationType}>(); // {Profile}";
		}
	}
}
