using Microsoft.Extensions.DependencyInjection;

namespace Havit.Extensions.DependencyInjection.SourceGenerators;

internal static class ServiceAttributeConstants
{
	public const string ServiceAttributeNonGenericFullname = "Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute";
	public const string ServiceAttributeGeneric1Fullname = "Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute`1";
	public const string ServiceAttributeGeneric2Fullname = "Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute`2";
	public const string ServiceAttributeGeneric3Fullname = "Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute`3";
	public const string ServiceAttributeGeneric4Fullname = "Havit.Extensions.DependencyInjection.Abstractions.ServiceAttribute`4";

	public const string LifetimePropertyName = "Lifetime";
	public const string ProfilePropertyName = "Profile";
	public const string ServiceTypePropertyName = "ServiceType";
	public const string ServiceTypesPropertyName = "ServiceTypes";

	public const string DefaultProfile = "@DefaultProfile";

	public const ServiceLifetime DefaultLifetime = ServiceLifetime.Transient;
}
