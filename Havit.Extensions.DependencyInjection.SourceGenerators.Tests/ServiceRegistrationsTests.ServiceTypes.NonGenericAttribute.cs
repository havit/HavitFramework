﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_NonGenericAttribute()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.TestProject.Services.ServiceTypes.NonGenerics;

[Service]
public partial class MyService1 { }
public partial class MyService1 : IMyService1 { }

[Service(ServiceType = typeof(IMyService2))]
public class MyService2 : IMyService2 { }

[Service(ServiceTypes = [typeof(IMyService3), typeof(IMyService4)])]
public class MyService3 : IMyService3, IMyService4 { }

[Service(Lifetime = ServiceLifetime.Singleton, ServiceTypes = [typeof(IMyService5), typeof(IMyService6)])]
public class MyService5 : IMyService5, IMyService6 { }

public interface IMyService1 { }
public interface IMyService2 { }
public interface IMyService3 { }
public interface IMyService4 { }
public interface IMyService5 { }
public interface IMyService6 { }
";

		// TODO: doplnit
		const string expectedOutput = @"using Microsoft.Extensions.DependencyInjection;

namespace Havit.TestProject.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServicesProjectServices(this IServiceCollection services, params string[] profileNames)
	{
		foreach (string profileName in profileNames)
		{
			AddServicesProjectServices(services, profileName);
		}
		return services;
	}

	public static IServiceCollection AddServicesProjectServices(this IServiceCollection services, string profileName = Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)
	{
		if (profileName == Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)
		{
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService1, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService1>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService2, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService2>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService3, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService3>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService4, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService3>();
			services.AddSingleton<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService5, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService5>();
			services.AddSingleton<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService6>(sp => (Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService6)sp.GetService<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService5>());
		}

		return services;
	}
}
";

		await VerifyGeneratorAsync(input, expectedOutput);
	}

	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_NonGenericAttribute_MissingRegistration()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.TestProject.Services.ServiceTypes.NonGenerics;

[Service]
public partial class MyService1: IMyService2 { }

public interface IMyService2 { }
";

		// TODO: doplnit
		const string expectedOutput = @"using Microsoft.Extensions.DependencyInjection;

namespace Havit.TestProject.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServicesProjectServices(this IServiceCollection services, params string[] profileNames)
	{
		foreach (string profileName in profileNames)
		{
			AddServicesProjectServices(services, profileName);
		}
		return services;
	}

	public static IServiceCollection AddServicesProjectServices(this IServiceCollection services, string profileName = Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)
	{
		if (profileName == Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)
		{
			throw new System.InvalidOperationException(""Type(s) Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService1 implement(s) no interface to register."");
		}

		return services;
	}
}
";

		await VerifyGeneratorAsync(input, expectedOutput);
	}

	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_NonGenericAttribute_DifferentNamespaces()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.TestProject.Services.ServiceTypes.NonGenerics
{
	[Service]
	public partial class MyService1: Havit.TestProject.Contracts.IMyService1 { }
}

namespace Havit.TestProject.Contracts
{
	public interface IMyService1 { }
}

";

		// TODO: doplnit
		const string expectedOutput = @"using Microsoft.Extensions.DependencyInjection;

namespace Havit.TestProject.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServicesProjectServices(this IServiceCollection services, params string[] profileNames)
	{
		foreach (string profileName in profileNames)
		{
			AddServicesProjectServices(services, profileName);
		}
		return services;
	}

	public static IServiceCollection AddServicesProjectServices(this IServiceCollection services, string profileName = Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)
	{
		if (profileName == Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute.DefaultProfile)
		{
			services.AddTransient<Havit.TestProject.Contracts.IMyService1, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService1>();
		}

		return services;
	}
}
";

		await VerifyGeneratorAsync(input, expectedOutput);
	}
}
