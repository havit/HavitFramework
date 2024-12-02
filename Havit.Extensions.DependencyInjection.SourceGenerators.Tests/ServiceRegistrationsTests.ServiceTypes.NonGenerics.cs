using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_NonGenerics()
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
	public static IServiceCollection AddServicesProjectServices(IServiceCollection services, string profileName)
	{
		if (profileName == ""@DefaultProfile"")
		{
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService1, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService1>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService2, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService2>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService3, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService3>();
			services.AddTransient<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService4, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService3>();
			services.AddSingleton<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService5, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService5>();
			services.AddSingleton<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService6>(sp => (Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService6)sp.GetService<Havit.TestProject.Services.ServiceTypes.NonGenerics.IMyService5>());
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

	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_NonGenerics_MissingRegistration()
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
	public static IServiceCollection AddServicesProjectServices(IServiceCollection services, string profileName)
	{
		if (profileName == ""@DefaultProfile"")
		{
			throw new System.InvalidOperationException(""Type(s) Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService1 implement(s) no interface to register."");
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

	[TestMethod]
	public async Task ServiceRegistration_ServiceTypes_NonGenerics_DifferentNamespaces()
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
	public static IServiceCollection AddServicesProjectServices(IServiceCollection services, string profileName)
	{
		if (profileName == ""@DefaultProfile"")
		{
			services.AddTransient<Havit.TestProject.Contracts.IMyService1, Havit.TestProject.Services.ServiceTypes.NonGenerics.MyService1>();
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
