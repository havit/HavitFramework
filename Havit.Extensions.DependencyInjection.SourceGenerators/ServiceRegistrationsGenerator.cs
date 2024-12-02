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

namespace Havit.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// This class generates service registration code for dependency injection
/// based on custom attributes applied to classes in the project.
/// </summary>
[Generator]
public class ServiceRegistrationsGenerator : IIncrementalGenerator
{
	internal const string GeneratedOutputFileName = "ServiceCollectionExtension.g.cs";

	/// <inheritdoc />
	public void Initialize(IncrementalGeneratorInitializationContext initializationContext)
	{
		// přečteme data z atributů (máme jich několik - negenerické a generické s různým počtem generických parametrů
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationNonGenericProvider = GetServiceRegistrationEntriesValuesProvider(initializationContext, typeof(ServiceAttribute), ExtractServiceTypesFromNamedParameters);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationGeneric1Provider = GetServiceRegistrationEntriesValuesProvider(initializationContext, typeof(ServiceAttribute<>), ExtractServiceTypesFromGenericArguments);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationGeneric2Provider = GetServiceRegistrationEntriesValuesProvider(initializationContext, typeof(ServiceAttribute<,>), ExtractServiceTypesFromGenericArguments);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationGeneric3Provider = GetServiceRegistrationEntriesValuesProvider(initializationContext, typeof(ServiceAttribute<,,>), ExtractServiceTypesFromGenericArguments);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationGeneric4Provider = GetServiceRegistrationEntriesValuesProvider(initializationContext, typeof(ServiceAttribute<,,,>), ExtractServiceTypesFromGenericArguments);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> allServiceRegistrationsProvider = serviceRegistrationNonGenericProvider.Concat(serviceRegistrationGeneric1Provider).Concat(serviceRegistrationGeneric2Provider).Concat(serviceRegistrationGeneric3Provider).Concat(serviceRegistrationGeneric4Provider);

		// přečteme konfiguraci projektu
		IncrementalValueProvider<BuildConfiguration> buildConfigurationProvider = initializationContext.AnalyzerConfigOptionsProvider.Select((analyzerConfig, _) =>
		{
			analyzerConfig.GlobalOptions.TryGetValue("build_property.RootNamespace", out string rootNamespace);

			return new BuildConfiguration
			{
				RootNamespace = rootNamespace
			};
		});

		// sloučíme data z atributů a konfiguraci projektu do dat pro generátor kódu
		var serviceRegistrationsGeneratorDataProvider = allServiceRegistrationsProvider.Combine(buildConfigurationProvider).Select((data, _) => new ServiceRegistrationsGeneratorData
		{
			ServiceRegistrationEntries = data.Left,
			BuildConfiguration = data.Right
		});

		// vygenerujeme kód
		initializationContext.RegisterSourceOutput(serviceRegistrationsGeneratorDataProvider, static (sourceContext, source) =>
		{
			GenerateSourceCode(source, sourceContext);
		});
	}

	private IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> GetServiceRegistrationEntriesValuesProvider(
		IncrementalGeneratorInitializationContext initializationContext,
		Type attributeType,
		Func<INamedTypeSymbol, AttributeData, string[]> serviceTypeReader)
	{
		return initializationContext
			.SyntaxProvider
			.ForAttributeWithMetadataName(
				fullyQualifiedMetadataName: attributeType.FullName,
				predicate: static (node, _) => node is ClassDeclarationSyntax,
				transform: (context, _) => GetServiceRegistrationEntries(context, serviceTypeReader))
			.SelectMany((items, _) => items)
			.Collect();
	}

	private static IEnumerable<ServiceRegistrationEntry> GetServiceRegistrationEntries(GeneratorAttributeSyntaxContext context, Func<INamedTypeSymbol, AttributeData, string[]> serviceTypeReader)
	{
		if (context.TargetNode is not ClassDeclarationSyntax classDeclarationSyntax)
		{
			yield break;
		}

		INamedTypeSymbol classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
		string classTypeName = classSymbol.ToDisplayString();

		foreach (AttributeData attribute in context.Attributes)
		{
			yield return new ServiceRegistrationEntry
			{
				ImplementationType = classTypeName,
				Lifetime = ExtractLifetime(attribute),
				Profile = ExtractProfile(attribute),
				ServiceTypes = serviceTypeReader(classSymbol, attribute)
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

	private static string[] ExtractServiceTypesFromNamedParameters(INamedTypeSymbol classSymbol, AttributeData attributeData)
	{
		List<string> result = new List<string>();

		KeyValuePair<string, TypedConstant> serviceTypeArgument = attributeData.NamedArguments.SingleOrDefault(a => a.Key == nameof(ServiceAttribute.ServiceType));
		if (!EqualityComparer<KeyValuePair<string, TypedConstant>>.Default.Equals(serviceTypeArgument, default)
			&& serviceTypeArgument.Value.Kind == TypedConstantKind.Type)
		{
			result.Add(((INamedTypeSymbol)serviceTypeArgument.Value.Value).ToDisplayString());
		}

		KeyValuePair<string, TypedConstant> serviceTypesArgument = attributeData.NamedArguments.SingleOrDefault(a => a.Key == nameof(ServiceAttribute.ServiceTypes));
		if (!EqualityComparer<KeyValuePair<string, TypedConstant>>.Default.Equals(serviceTypesArgument, default) && serviceTypesArgument.Value.Kind != TypedConstantKind.Error)
		{
			result.AddRange(
				serviceTypesArgument.Value.Values
				.Where(value => value.Kind == TypedConstantKind.Type)
				.Where(value => ((ISymbol)value.Value).Kind != SymbolKind.ErrorType)
				.Select(value => ((INamedTypeSymbol)value.Value).ToDisplayString()));
		}

		if (result.Count == 0)
		{
			// když není určen žádný typ (což je 99%+ scénářů), zkusíme najít interface mezi implementovanými interfaces
			string className = classSymbol.Name;
			string interfaceCandidateName = "I" + className;

			INamedTypeSymbol interfaceType = classSymbol.AllInterfaces.OrderBy(item => item.ToDisplayString()).FirstOrDefault(interfaceTypeSymbol => interfaceTypeSymbol.Name == interfaceCandidateName);
			if (interfaceType != null)
			{
				result.Add(interfaceType.ToDisplayString());
			}
		}

		return result.ToArray();
	}

	private static string[] ExtractServiceTypesFromGenericArguments(INamedTypeSymbol classSymbol, AttributeData attributeData)
	{
		return attributeData.AttributeClass.TypeArguments
			.Where(item => item.Kind != SymbolKind.ErrorType)
			.Select(typeArgument => typeArgument.ToDisplayString())
			.ToArray();
	}

	private static void GenerateSourceCode(ServiceRegistrationsGeneratorData serviceRegistrationsGeneratorData, SourceProductionContext context)
	{
		if (!serviceRegistrationsGeneratorData.ServiceRegistrationEntries.Any())
		{
			return;
		}

		context.AddSource(GeneratedOutputFileName, SourceText.From(ServiceRegistrationsCodeBuilder.GenerateCode(serviceRegistrationsGeneratorData), Encoding.UTF8));
	}

}
