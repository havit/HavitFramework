﻿using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Extensions.DependencyInjection.SourceGenerators.Infrastructure;
using Microsoft.CodeAnalysis;

namespace Havit.Extensions.DependencyInjection.SourceGenerators;

internal static class ServiceRegistrationsCodeBuilder
{
	public static string GenerateCode(ServiceRegistrationsGeneratorData serviceRegistrationsGeneratorData)
	{
		var serviceRegistrationEntriesByProfile = serviceRegistrationsGeneratorData.ServiceRegistrationEntries.ToLookup(item => item.Profile);
		List<string> profileNames = serviceRegistrationEntriesByProfile.Select(item => item.Key).OrderBy(item => item).ToList();

		using (var sourceCodeWriter = new SourceCodeWriter())
		{
			sourceCodeWriter.WriteLine("using Microsoft.Extensions.DependencyInjection;");
			sourceCodeWriter.WriteNewLine();

			string rootNamespace = serviceRegistrationsGeneratorData.BuildConfiguration.RootNamespace;
			if (!string.IsNullOrEmpty(rootNamespace))
			{
				sourceCodeWriter.WriteLine($"namespace {rootNamespace};");
				sourceCodeWriter.WriteNewLine();
			}
			sourceCodeWriter.WriteLine("public static class ServiceCollectionExtensions");
			using (sourceCodeWriter.BeginWriteBlock())
			{
				string methodName = GetMethodName(rootNamespace);

				sourceCodeWriter.WriteLine($"public static IServiceCollection {methodName}(this IServiceCollection services, params string[] profileNames)");
				using (sourceCodeWriter.BeginWriteBlock())
				{
					sourceCodeWriter.WriteLine("foreach (string profileName in profileNames)");
					using (sourceCodeWriter.BeginWriteBlock())
					{
						sourceCodeWriter.WriteLine($"{methodName}(services, profileName);");
					}
					sourceCodeWriter.WriteLine("return services;");
				}
				sourceCodeWriter.WriteNewLine();

				sourceCodeWriter.WriteLine($"public static IServiceCollection {methodName}(this IServiceCollection services, string profileName = Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)");
				using (sourceCodeWriter.BeginWriteBlock())
				{
					bool first = true;
					foreach (string profileName in profileNames)
					{
						WriteProfile(sourceCodeWriter, profileName, serviceRegistrationEntriesByProfile[profileName], first);
						first = false;
					}
					if (!first)
					{
						sourceCodeWriter.WriteNewLine();
					}
					sourceCodeWriter.WriteLine("return services;");
				}
			}

			return sourceCodeWriter.ToString();
		}
	}

	private static string GetMethodName(string rootNamespace)
	{
		string namespaceSegment = rootNamespace?.Split('.').Last() ?? "";
		return $"Add{namespaceSegment}ByServiceAttribute";
	}

	private static void WriteProfile(SourceCodeWriter sourceCodeWriter, string profileName, IEnumerable<ServiceRegistrationEntry> profileServiceRegistrationEntries, bool isFirst)
	{
		string elseIf = isFirst ? null : "else ";
		if (profileName == ServiceAttributeConstants.DefaultProfile)
		{
			sourceCodeWriter.WriteLine($"{elseIf}if (profileName == Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)");
		}
		else
		{
			sourceCodeWriter.WriteLine($"{elseIf}if (profileName == \"{profileName.Replace("\"", "\\\"")}\")");
		}

		using (sourceCodeWriter.BeginWriteBlock())
		{
			if (profileServiceRegistrationEntries.Any(item => item.ServiceTypes.Length == 0))
			{
				var registrationsWithMissingServiceType = profileServiceRegistrationEntries.Where(item => item.ServiceTypes.Length == 0).ToList();
				var registrationsWithMissingServiceTypeJoined = String.Join(", ", registrationsWithMissingServiceType.Select(item => item.ImplementationType.ToDisplayString()).OrderBy(item => item));
				sourceCodeWriter.WriteLine($"throw new System.InvalidOperationException(\"Type(s) {registrationsWithMissingServiceTypeJoined} implement(s) no interface to register.\");");
			}
			else
			{
				foreach (var serviceRegistration in profileServiceRegistrationEntries.OrderBy(item => item.ImplementationType.ToDisplayString()).ThenBy(item => item.ServiceTypes.Select(serviceType => serviceType.ToDisplayString()).FirstOrDefault()))
				{
					WriteServiceRegistration(sourceCodeWriter, serviceRegistration);
				}
			}
		}
	}

	private static void WriteServiceRegistration(SourceCodeWriter sourceCodeWriter, ServiceRegistrationEntry serviceRegistration)
	{
		void WriteServiceWithLifetime(INamedTypeSymbol serviceType, INamedTypeSymbol implementationType)
		{
			// Situace:
			// (1) service type IService<>, implementation type Service<T> -> chceme implementation type Service<> (použití z [Service(ServiceType= typeof(IService<>)])
			// (2) service type IService<>, implementation type Service -> je chybou, nelze použít
			// (3) service type IService<T>, implementation type Service<T> -> chceme service type IService<>, implementation type Service<> (použití z [ServiceAttribute])
			// (4) service type IService<T>, implementation type Service -> je chybou, nelze použít
			// (5) service type IService<int>, implementation type Service -> chceme, jak je - service type IService<int>, implementation type Service
			// (6) service type IService<int>, implementation type Service<T> -> je chybou, nelze použít (museli bychom registrovat pod Service<int>)

			bool useTypeOfArguments = serviceType.IsUnboundGenericType || implementationType.IsGenericType; // (1), (2), (3), (4)
			string serviceTypeDisplayString = serviceType.IsGenericType && implementationType.IsGenericType // (3), (6)
				? serviceType.ConstructUnboundGenericType().ToDisplayString()
				: serviceType.ToDisplayString();

			string implementationTypeDisplayString = serviceType.IsGenericType && implementationType.IsGenericType // (1), (3), (6)
				? implementationType.ConstructUnboundGenericType().ToDisplayString()
				: implementationType.ToDisplayString();

			if (useTypeOfArguments)
			{
				sourceCodeWriter.WriteLine($"services.Add{serviceRegistration.Lifetime}(typeof({serviceTypeDisplayString}), typeof({implementationTypeDisplayString}));");
			}
			else
			{
				sourceCodeWriter.WriteLine($"services.Add{serviceRegistration.Lifetime}<{serviceTypeDisplayString}, {implementationTypeDisplayString}>();");
			}


		}

		if ((serviceRegistration.ServiceTypes.Length == 1) || (serviceRegistration.Lifetime == "Transient"))
		{
			foreach (INamedTypeSymbol serviceType in serviceRegistration.ServiceTypes)
			{
				WriteServiceWithLifetime(serviceType, serviceRegistration.ImplementationType);
			}
		}
		else
		{
			INamedTypeSymbol firstServiceType = serviceRegistration.ServiceTypes.First();
			WriteServiceWithLifetime(firstServiceType, serviceRegistration.ImplementationType);
			foreach (INamedTypeSymbol serviceType in serviceRegistration.ServiceTypes.Skip(1))
			{
				// varianty s generickými parametry nejsou pro další parametry aktuálně řešeny.
				sourceCodeWriter.WriteLine($"services.Add{serviceRegistration.Lifetime}<{serviceType.ToDisplayString()}>(sp => ({serviceType.ToDisplayString()})sp.GetService<{firstServiceType.ToDisplayString()}>());");
			}
		}
	}
}
