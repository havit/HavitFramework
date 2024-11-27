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
	public async Task ServiceRegistration_DoesNotProduceCodeWhenNoServiceAttribute()
	{
		await VerifyGeneratorAsync("public class MyService { }", null);
	}
}
