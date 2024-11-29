using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_Generics()
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
	public static IServiceCollection AddServicesProjectServices(IServiceCollection services, string profileName)
	{
		if (profileName == ""@DefaultProfile"")
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
