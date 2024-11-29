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

namespace Havit.TestProject.Services.Profiles;

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

		const string expectedOutput = @"using Microsoft.Extensions.DependencyInjection;

namespace Havit.TestProject.Services;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServicesProjectServices(IServiceCollection services, string profileName)
	{
		if (profileName == ""@DefaultProfile"")
		{
			services.AddTransient<Havit.TestProject.Services.Profiles.IMyDefaultService, Havit.TestProject.Services.Profiles.MyDefaultService>();
		}
		else if (profileName == ""Profile1"")
		{
			services.AddTransient<Havit.TestProject.Services.Profiles.IMyProfile1Service, Havit.TestProject.Services.Profiles.MyProfile1Service>();
		}
		else if (profileName == ""Profile2"")
		{
			services.AddTransient<Havit.TestProject.Services.Profiles.IMyProfile2Service, Havit.TestProject.Services.Profiles.MyProfile2Service>();
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
