using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_SerticeTypes_NonGenerics()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace TestNamespace;

[Service]
public partial class MyService0: IMyService1 { }

[Service]
public partial class MyService1 { }
public partial class MyService1 : IMyService1 { }

[Service(ServiceType = typeof(IMyService2)]
public class MyService2 : IMyService2 { }

[Service(ServiceTypes = [typeof(IMyService3), typeof(IMyService4)])]
public class MyService3 : IMyService3, IMyService4 { }

[Service(Lifetime = ServiceLifetime.Singleton, ServiceTypes = [typeof(IMyService5), typeof(IMyService6)])]
public class MyService5 : IMyService5, IMyService6 { }

public interface IMyService1 { }
public interface IMyService2 { }
public interface IMyService3 { }
public interface IMyService4 { }
";

		// TODO: doplnit
		const string expectedOutput = @" some code";

		await VerifyGeneratorAsync(input, expectedOutput);
	}
}
