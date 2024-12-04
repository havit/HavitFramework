using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Extensions.DependencyInjection.SourceGenerators.Infrastructure;

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
					if (first)
					{
						sourceCodeWriter.WriteLine($"throw new System.InvalidOperationException(\"Unknown profile name.\");");
					}
					else
					{
						sourceCodeWriter.WriteLine($"else");
						using (sourceCodeWriter.BeginWriteBlock())
						{
							sourceCodeWriter.WriteLine($"throw new System.InvalidOperationException(\"Unknown profile name.\");");
						}

						sourceCodeWriter.WriteNewLine();
						sourceCodeWriter.WriteLine("return services;");

					}
				}
			}

			return sourceCodeWriter.ToString();
		}
	}

	private static string GetMethodName(string rootNamespace)
	{
		string namespaceSegment = rootNamespace?.Split('.').Last() ?? "";
		return $"Add{namespaceSegment}ProjectServices";
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
				var registrationsWithMissingServiceTypeJoined = String.Join(", ", registrationsWithMissingServiceType.OrderBy(item => item.ImplementationType).Select(item => item.ImplementationType));
				sourceCodeWriter.WriteLine($"throw new System.InvalidOperationException(\"Type(s) {registrationsWithMissingServiceTypeJoined} implement(s) no interface to register.\");");
			}
			else
			{
				foreach (var serviceRegistration in profileServiceRegistrationEntries.OrderBy(item => item.ImplementationType).ThenBy(item => item.ServiceTypes.FirstOrDefault()))
				{
					WriteServiceRegistration(sourceCodeWriter, serviceRegistration);
				}
			}
		}
	}

	private static void WriteServiceRegistration(SourceCodeWriter sourceCodeWriter, ServiceRegistrationEntry serviceRegistration)
	{
		if ((serviceRegistration.ServiceTypes.Length == 1) || (serviceRegistration.Lifetime == "Transient"))
		{
			foreach (var serviceType in serviceRegistration.ServiceTypes)
			{
				sourceCodeWriter.WriteLine($"services.Add{serviceRegistration.Lifetime}<{serviceType}, {serviceRegistration.ImplementationType}>();");
			}
		}
		else
		{
			var firstServiceType = serviceRegistration.ServiceTypes.First();
			sourceCodeWriter.WriteLine($"services.Add{serviceRegistration.Lifetime}<{firstServiceType}, {serviceRegistration.ImplementationType}>();");
			foreach (var serviceType in serviceRegistration.ServiceTypes.Skip(1))
			{
				sourceCodeWriter.WriteLine($"services.Add{serviceRegistration.Lifetime}<{serviceType}>(sp => ({serviceType})sp.GetService<{firstServiceType}>());");
			}
		}
	}
}
