using System.Threading.Tasks;
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
