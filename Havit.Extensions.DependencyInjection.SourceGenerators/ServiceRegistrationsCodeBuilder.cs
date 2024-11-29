using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Extensions.DependencyInjection.SourceGenerators.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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
				sourceCodeWriter.WriteLine($"public static IServiceCollection {methodName}(IServiceCollection services, string profileName)");
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
		sourceCodeWriter.WriteLine($"{elseIf}if (profileName == \"{profileName.Replace("\"", "\\\"")}\")");
		using (sourceCodeWriter.BeginWriteBlock())
		{
			foreach (var serviceRegistration in profileServiceRegistrationEntries.OrderBy(item => item.ImplementationType).ThenBy(item => item.ServiceTypes.FirstOrDefault()))
			{
				WriteServiceRegistration(sourceCodeWriter, serviceRegistration);
			}
		}
	}

	private static void WriteServiceRegistration(SourceCodeWriter sourceCodeWriter, ServiceRegistrationEntry serviceRegistration)
	{
		if (serviceRegistration.ServiceTypes.Length == 0)
		{
			sourceCodeWriter.WriteLine($"#warning no registration found");
		}
		else if ((serviceRegistration.ServiceTypes.Length == 1) || (serviceRegistration.Lifetime == ServiceLifetime.Transient))
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
