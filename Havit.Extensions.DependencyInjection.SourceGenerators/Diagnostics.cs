using Havit.Diagnostics.Common;
using Microsoft.CodeAnalysis;

namespace Havit.Extensions.DependencyInjection.SourceGenerators;

internal static class Diagnostics
{
	public static readonly DiagnosticDescriptor ServiceAttributeCannotDetermineServiceType = new DiagnosticDescriptor(
		id: DiagnosticIdentifiers.ServiceAttributeCannotDetermineServiceTypeId,
		title: "ServiceType not determined",
		messageFormat: "ServiceAttribute on class '{0}' cannot determine service type to register",
		category: "Usage",
		defaultSeverity: DiagnosticSeverity.Warning,
		isEnabledByDefault: true,
		description: "This diagnostic ensures that all ServiceAttributes can determine service type."
	);
}
