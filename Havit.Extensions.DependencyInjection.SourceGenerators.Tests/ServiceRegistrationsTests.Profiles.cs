﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Extensions.DependencyInjection.SourceGenerators.Tests;

public partial class ServiceRegistrationsTests
{
	[TestMethod]
	public async Task ServiceRegistration_Profiles()
	{
		const string input = @"
using Microsoft.Extensions.DependencyInjection;
using Havit.Extensions.DependencyInjection.Abstractions;

namespace TestNamespace;

[Service]
public class MyDefaultService : IMyDefaultService { }
public interface IMyDefaultService { }

[Service(Profile = ""Profile1"")]
public class MyProfile1Service : IMyProfile1Service { }
public interface IMyProfile1Service { }

[Service(Profile = Constants.Profile2)]
public class MyProfile2Service : IMyProfile2Service { }
public interface IMyProfile2Service { }

internal static class Constants
{
	public const string Profile2 = ""Profile2"";
}
";

		// TODO: doplnit
		const string expectedOutput = @" some code";

		await VerifyGeneratorAsync(input, expectedOutput);
	}
}
