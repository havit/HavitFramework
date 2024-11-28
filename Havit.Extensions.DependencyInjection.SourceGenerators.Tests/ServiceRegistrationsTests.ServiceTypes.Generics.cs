using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_SerticeTypes_Generics()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace TestNamespace;

[Service<IMyService1, IMyService2, IMyService3, IMyService4>]
public class MyService : IMyService1, IMyService2, IMyService3, IMyService4 { }

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
