﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_Lifetimes()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.TestProject.Services.Lifetimes;

[Service]
public class MyDefaultService : IMyDefaultService { }
public interface IMyDefaultService { }

[Service(Lifetime = ServiceLifetime.Scoped)]
public class MyScopedService : IMyScopedService { }
public interface IMyScopedService { }

[Service(Lifetime = ServiceLifetime.Singleton)]
public class MySingletonService : IMySingletonService { }
public interface IMySingletonService { }

[Service(Lifetime = ServiceLifetime.Transient)]
public class MyTransientService : IMyTransientService { }
public interface IMyTransientService { }
";

		// TODO: doplnit
		const string expectedOutput = @"using Microsoft.Extensions.DependencyInjection;

namespace Havit.TestProject.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServicesByServiceAttribute(this IServiceCollection services, params string[] profileNames)
	{
		foreach (string profileName in profileNames)
		{
			AddServicesByServiceAttribute(services, profileName);
		}
		return services;
	}

	public static IServiceCollection AddServicesByServiceAttribute(this IServiceCollection services, string profileName = Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)
	{
		if (profileName == Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)
		{
			services.AddTransient<Havit.TestProject.Services.Lifetimes.IMyDefaultService, Havit.TestProject.Services.Lifetimes.MyDefaultService>();
			services.AddScoped<Havit.TestProject.Services.Lifetimes.IMyScopedService, Havit.TestProject.Services.Lifetimes.MyScopedService>();
			services.AddSingleton<Havit.TestProject.Services.Lifetimes.IMySingletonService, Havit.TestProject.Services.Lifetimes.MySingletonService>();
			services.AddTransient<Havit.TestProject.Services.Lifetimes.IMyTransientService, Havit.TestProject.Services.Lifetimes.MyTransientService>();
		}

		return services;
	}
}
";

		await VerifyGeneratorAsync(input, expectedOutput);
	}
}
