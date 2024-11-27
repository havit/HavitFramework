using System.Collections.Immutable;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Havit.Extensions.DependencyInjection.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
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

namespace TestNamespace;

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
		const string expectedOutput = @" some code";

		await VerifyGeneratorAsync(input, expectedOutput);
	}
}
