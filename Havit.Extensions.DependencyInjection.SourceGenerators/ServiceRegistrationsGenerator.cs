using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationNonGenericProvider = GetServiceRegistrationEntriesValuesProvider(initializationContext, ServiceAttributeConstants.ServiceAttributeNonGenericFullname, ExtractServiceTypesFromNamedParameters);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationGeneric1Provider = GetServiceRegistrationEntriesValuesProvider(initializationContext, ServiceAttributeConstants.ServiceAttributeGeneric1Fullname, ExtractServiceTypesFromGenericArguments);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationGeneric2Provider = GetServiceRegistrationEntriesValuesProvider(initializationContext, ServiceAttributeConstants.ServiceAttributeGeneric2Fullname, ExtractServiceTypesFromGenericArguments);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationGeneric3Provider = GetServiceRegistrationEntriesValuesProvider(initializationContext, ServiceAttributeConstants.ServiceAttributeGeneric3Fullname, ExtractServiceTypesFromGenericArguments);
		IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> serviceRegistrationGeneric4Provider = GetServiceRegistrationEntriesValuesProvider(initializationContext, ServiceAttributeConstants.ServiceAttributeGeneric4Fullname, ExtractServiceTypesFromGenericArguments);
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
			ReportDiagnostic(source, sourceContext);
			GenerateSourceCode(source, sourceContext);
		});
	}

	private IncrementalValueProvider<ImmutableArray<ServiceRegistrationEntry>> GetServiceRegistrationEntriesValuesProvider(
		IncrementalGeneratorInitializationContext initializationContext,
		string attributeTypeFullname,
		Func<INamedTypeSymbol, AttributeData, INamedTypeSymbol[]> serviceTypeReader)
	{
		return initializationContext
			.SyntaxProvider
			.ForAttributeWithMetadataName(
				fullyQualifiedMetadataName: attributeTypeFullname,
				predicate: static (node, _) => node is ClassDeclarationSyntax,
				transform: (context, cancellationToken) => GetServiceRegistrationEntries(context, serviceTypeReader, cancellationToken))
			.SelectMany((items, _) => items)
			.Collect();
	}

	private static IEnumerable<ServiceRegistrationEntry> GetServiceRegistrationEntries(GeneratorAttributeSyntaxContext context, Func<INamedTypeSymbol, AttributeData, INamedTypeSymbol[]> serviceTypeReader, CancellationToken cancellationToken)
	{
		if (context.TargetNode is not ClassDeclarationSyntax classDeclarationSyntax)
		{
			yield break;
		}

		INamedTypeSymbol classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken);

		foreach (AttributeData attribute in context.Attributes)
		{
			yield return new ServiceRegistrationEntry
			{
				ImplementationType = classSymbol,
				Lifetime = ExtractLifetime(attribute),
				Profile = ExtractProfile(attribute),
				ServiceTypes = serviceTypeReader(classSymbol, attribute)
			};
		}
	}

	private static string ExtractLifetime(AttributeData attributeData)
	{
		KeyValuePair<string, TypedConstant> lifetimeArgument = attributeData.NamedArguments.SingleOrDefault(a => a.Key == ServiceAttributeConstants.LifetimePropertyName);
		if (EqualityComparer<KeyValuePair<string, TypedConstant>>.Default.Equals(lifetimeArgument, default))
		{
			return ServiceAttributeConstants.DefaultLifetime;
		}

		if (lifetimeArgument.Value.Kind == TypedConstantKind.Error)
		{
			return ServiceAttributeConstants.DefaultLifetime;
		}

		int value = (int)lifetimeArgument.Value.Value; // získáme hodnotu lifetime, ovšem jako int hodnotu podkladového int
		return lifetimeArgument.Value.Type.GetMembers() // tak budeme dohledávat v podkladovém enumu mezi hodnotami
			.Where(item => (int)((IFieldSymbol)item).ConstantValue == value) // tu, jejíž hodnota odpovídá
			.Select(item => item.Name)
			.FirstOrDefault();
	}

	private static string ExtractProfile(AttributeData attributeData)
	{
		KeyValuePair<string, TypedConstant> profileArgument = attributeData.NamedArguments.SingleOrDefault(a => a.Key == ServiceAttributeConstants.ProfilePropertyName);
		if (EqualityComparer<KeyValuePair<string, TypedConstant>>.Default.Equals(profileArgument, default))
		{
			return ServiceAttributeConstants.DefaultProfile;
		}

		if (profileArgument.Value.Kind == TypedConstantKind.Error)
		{
			return ServiceAttributeConstants.DefaultProfile;
		}

		return (string)profileArgument.Value.Value;
	}

	private static INamedTypeSymbol[] ExtractServiceTypesFromNamedParameters(INamedTypeSymbol classSymbol, AttributeData attributeData)
	{
		List<INamedTypeSymbol> result = new List<INamedTypeSymbol>();

		KeyValuePair<string, TypedConstant> serviceTypeArgument = attributeData.NamedArguments.SingleOrDefault(a => a.Key == ServiceAttributeConstants.ServiceTypePropertyName);
		bool hasServiceTypeArgument = !EqualityComparer<KeyValuePair<string, TypedConstant>>.Default.Equals(serviceTypeArgument, default);
		if (hasServiceTypeArgument && (serviceTypeArgument.Value.Kind == TypedConstantKind.Type))
		{
			result.Add(((INamedTypeSymbol)serviceTypeArgument.Value.Value));
		}

		KeyValuePair<string, TypedConstant> serviceTypesArgument = attributeData.NamedArguments.SingleOrDefault(a => a.Key == ServiceAttributeConstants.ServiceTypesPropertyName);
		bool hasServiceTypesArgument = !EqualityComparer<KeyValuePair<string, TypedConstant>>.Default.Equals(serviceTypesArgument, default);
		if (hasServiceTypesArgument && serviceTypesArgument.Value.Kind != TypedConstantKind.Error)
		{
			result.AddRange(
				serviceTypesArgument.Value.Values
				.Where(value => value.Kind == TypedConstantKind.Type)
				.Where(value => ((ISymbol)value.Value).Kind != SymbolKind.ErrorType)
				.Select(value => ((INamedTypeSymbol)value.Value)));
		}

		if (!hasServiceTypeArgument && !hasServiceTypesArgument)
		{
			// když není určen žádný typ (což je 99%+ scénářů), zkusíme najít interface mezi implementovanými interfaces
			string className = classSymbol.Name;
			string interfaceCandidateName = "I" + className;

			INamedTypeSymbol interfaceType = classSymbol.AllInterfaces.OrderBy(item => item.ToDisplayString()).FirstOrDefault(interfaceTypeSymbol => interfaceTypeSymbol.Name == interfaceCandidateName);
			if (interfaceType != null)
			{
				result.Add(interfaceType);
			}
		}

		return result.ToArray();
	}

	private static INamedTypeSymbol[] ExtractServiceTypesFromGenericArguments(INamedTypeSymbol classSymbol, AttributeData attributeData)
	{
		return attributeData.AttributeClass.TypeArguments
			.Where(item => item.Kind != SymbolKind.ErrorType)
			.Select(typeArgument => (INamedTypeSymbol)typeArgument)
			.ToArray();
	}

	private static void ReportDiagnostic(ServiceRegistrationsGeneratorData source, SourceProductionContext sourceContext)
	{
		foreach (var serviceRegistration in source.ServiceRegistrationEntries)
		{
			if (serviceRegistration.ServiceTypes.Length == 0)
			{
				sourceContext.ReportDiagnostic(Diagnostic.Create(Diagnostics.ServiceAttributeCannotDetermineServiceType, serviceRegistration.ImplementationType.Locations.First(), serviceRegistration.ImplementationType.ToDisplayString()));
			}
		}
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
