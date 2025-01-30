using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Havit.ApplicationInsights.DependencyCollector;

/// <summary>
/// Ignoruje (neposílá) ty requesty, které jsou úspěšně zpracovanými CORS requesty (identifikace CORS requestu je z RequestTelemetry.Name, jehož hodnota musí začínat "OPTIONS ").
/// </summary>
public class IgnoreCorsRequests : ITelemetryProcessor
{
	private readonly ITelemetryProcessor next;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public IgnoreCorsRequests(ITelemetryProcessor next)
	{
		this.next = next;
	}

	/// <inheritdoc />
	public void Process(ITelemetry item)
	{
		if (ExcludeTelemetry(item))
		{
			return;
		}

		next.Process(item);
	}

	/// <summary>
	/// Vrací true, pokud má dojít k ignorování předané telemetrie.
	/// Pro telemetrie, které nejsou requesty, pro telemetrie, které nejsou úspěšné vrací false (neignorujeme je).
	/// </summary>
	private bool ExcludeTelemetry(ITelemetry item)
	{
		RequestTelemetry requestTelemetry = item as RequestTelemetry;

		// it is not a reqeest telemetry
		if (requestTelemetry == null)
		{
			return false; // do not exclude
		}

		// it is failed request, we want to know what happened
		if (requestTelemetry.Success == false)
		{
			return false; // do not exclude
		}

		// successful cors requests
		if (IsOptionsRequest(requestTelemetry))
		{
			return true; // exclude
		}

		return false; // do not exclude
	}

	private bool IsOptionsRequest(RequestTelemetry requestTelemetry)
	{
		return requestTelemetry.Name.StartsWith("OPTIONS ");
	}
}
