using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Havit.Extensions.DependencyInjection.SourceGenerators.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.Analyzers;

internal static class ServiceRegistrationsCodeBuilder
{
	public static string GenerateCode(ImmutableArray<ServiceRegistrationEntry> serviceRegistrationEntries)
	{
		var serviceRegistrationEntriesByProfile = serviceRegistrationEntries.ToLookup(item => item.Profile);
		List<string> profileNames = serviceRegistrationEntriesByProfile.Select(item => item.Key).OrderBy(item => item).ToList();

		using (var sourceCodeWriter = new SourceCodeWriter())
		{
			sourceCodeWriter.WriteLine("using Microsoft.Extensions.DependencyInjection;");
			sourceCodeWriter.WriteNewLine();
			sourceCodeWriter.WriteLine("namespace XY; // TODO name");
			sourceCodeWriter.WriteNewLine();
			sourceCodeWriter.WriteLine("public static class ServiceCollectionExtensions");
			using (sourceCodeWriter.BeginWriteBlock())
			{
				sourceCodeWriter.WriteLine("public static IServiceCollection AddXY(IServiceCollection services, string profileName) // TODO Name");
				using (sourceCodeWriter.BeginWriteBlock())
				{
					bool first = true;
					foreach (string profileName in profileNames)
					{
						WriteProfile(sourceCodeWriter, profileName, serviceRegistrationEntriesByProfile[profileName], first);
						first = false;
					}
					string elseIf = first ? null : "else ";
					sourceCodeWriter.WriteLine($"{elseIf}throw new InvalidOperationException(\"Unknown profile name.\");");
				}
			}

			return sourceCodeWriter.ToString();
		}
	}

	private static void WriteProfile(SourceCodeWriter sourceCodeWriter, string profileName, IEnumerable<ServiceRegistrationEntry> profileServiceRegistrationEntries, bool isFirst)
	{
		string elseIf = isFirst ? null : "else ";
		sourceCodeWriter.WriteLine($"{elseIf}if (profileName == \"{profileName}\")");
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
				sourceCodeWriter.WriteLine($"services.Add{serviceRegistration.Lifetime}<{serviceType}>(sp => sp.GetService<{firstServiceType}>());");
			}
		}
	}
}
