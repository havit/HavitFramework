using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_GenericAttribute()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace Havit.TestProject.Services.ServiceTypes.Generics;

[Service<IMyService1, IMyService2, IMyService3, IMyService4>]
public class MyService : IMyService1, IMyService2, IMyService3, IMyService4 { }

public interface IMyService1 { }
public interface IMyService2 { }
public interface IMyService3 { }
public interface IMyService4 { }
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
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.Generics.IMyService1, Havit.TestProject.Services.ServiceTypes.Generics.MyService>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.Generics.IMyService2, Havit.TestProject.Services.ServiceTypes.Generics.MyService>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.Generics.IMyService3, Havit.TestProject.Services.ServiceTypes.Generics.MyService>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.Generics.IMyService4, Havit.TestProject.Services.ServiceTypes.Generics.MyService>();
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
