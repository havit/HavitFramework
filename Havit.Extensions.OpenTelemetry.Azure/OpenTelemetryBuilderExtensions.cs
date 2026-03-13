using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;

namespace Havit.Extensions.OpenTelemetry.Azure;

/// <summary>
/// Extension metody pro konfiguraci OpenTelemetry s Azure Monitor exporterem.
/// </summary>
public static class OpenTelemetryBuilderExtensions
{
	/// <summary>
	/// Vrátí výchozí connection string pro Azure Application Insights /Azure Monitor exporter z konfigurace.
	/// Primárně, pokud je v konfiguraci přítomen klíč <c>APPLICATIONINSIGHTS_CONNECTION_STRING</c> (typicky environment variable), použije se jeho hodnota.
	/// Sekundárně se použije hodnota connection stringu <c>ApplicationInsights</c>.
	/// </summary>
	public static string GetDefaultApplicationInsightsConnectionString(IConfiguration configuration) =>
		configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING")
		?? configuration.GetConnectionString("ApplicationInsights");

	/// <summary>
	/// Zaregistruje Azure Monitor exporter s výchozím connection stringem, pokud existuje.
	/// Pokud connection string chybí, exporter se nezaregistruje a OpenTelemetry pipeline zůstane beze změny.
	/// </summary>
	/// <param name="openTelemetryBuilder">OpenTelemetry builder.</param>
	/// <param name="configuration">Konfigurace pro získání connection stringu.</param>
	/// <param name="configureAzureMonitor">Volitelná akce pro další konfiguraci Azure Monitor exporteru. Použije se pouze, pokud je connection string nastaven.</param>
	/// <param name="configureOpenTelemetry">Volitelná akce pro další konfiguraci open telemetry. Použije se pouze, pokud je connection string nastaven.</param>
	public static OpenTelemetryBuilder UseAzureMonitorIfConfigured(this OpenTelemetryBuilder openTelemetryBuilder, IConfiguration configuration, Action<AzureMonitorOptions> configureAzureMonitor = null, Action<OpenTelemetryBuilder> configureOpenTelemetry = null)
	{
		return openTelemetryBuilder.UseAzureMonitorIfConfigured(GetDefaultApplicationInsightsConnectionString(configuration), configureAzureMonitor, configureOpenTelemetry);
	}

	/// <summary>
	/// Zaregistruje Azure Monitor exporter s daným connection stringem.
	/// Pokud je connection string <c>null</c> nebo prázdný, exporter se nezaregistruje.
	/// </summary>
	/// <param name="openTelemetryBuilder">OpenTelemetry builder.</param>
	/// <param name="connectionString">Application Insights / Azure Monitor connection string.</param>
	/// <param name="configureAzureMonitor">Volitelná akce pro další konfiguraci Azure Monitor exporteru. Použije se pouze, pokud je connection string nastaven.</param>
	/// <param name="configureOpenTelemetry">Volitelná akce pro další konfiguraci open telemetry. Použije se pouze, pokud je connection string nastaven.</param>
	public static OpenTelemetryBuilder UseAzureMonitorIfConfigured(this OpenTelemetryBuilder openTelemetryBuilder, string connectionString, Action<AzureMonitorOptions> configureAzureMonitor = null, Action<OpenTelemetryBuilder> configureOpenTelemetry = null)
	{
		if (!String.IsNullOrEmpty(connectionString))
		{
			openTelemetryBuilder = openTelemetryBuilder.UseAzureMonitor(options =>
			{
				configureAzureMonitor?.Invoke(options);
				options.ConnectionString = connectionString;
			});
			configureOpenTelemetry?.Invoke(openTelemetryBuilder);
		}
		return openTelemetryBuilder;
	}
}
