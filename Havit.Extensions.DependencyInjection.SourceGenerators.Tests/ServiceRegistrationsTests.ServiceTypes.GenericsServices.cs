﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_GenericServices()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.TestProject.Services.ServiceTypes.Generics;

[Service]
public class MyService1<T> : IMyService1<T> { }

[Service(ServiceType = typeof(IMyService2<>))]
public class MyService2<T> : IMyService2<T> { }

[Service<IMyService3<int>>]
public class MyService3 : IMyService3<int> { }

public interface IMyService1<T> { }
public interface IMyService2<T> { }
public interface IMyService3<T> { }
";

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
			services.AddTransient(typeof(Havit.TestProject.Services.ServiceTypes.Generics.IMyService1<>), typeof(Havit.TestProject.Services.ServiceTypes.Generics.MyService1<>));
			services.AddTransient(typeof(Havit.TestProject.Services.ServiceTypes.Generics.IMyService2<>), typeof(Havit.TestProject.Services.ServiceTypes.Generics.MyService2<>));
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.Generics.IMyService3<int>, Havit.TestProject.Services.ServiceTypes.Generics.MyService3>();
		}
		else
		{
			throw new System.InvalidOperationException(""Unknown profile name."");
		}

		return services;
	}
}
";

		await VerifyGeneratorAsync(input, expectedOutput);
	}
}
