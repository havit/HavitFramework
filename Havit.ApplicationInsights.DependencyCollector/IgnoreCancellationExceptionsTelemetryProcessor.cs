using Havit.Core;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Havit.ApplicationInsights.DependencyCollector;

/// <summary>
/// Application Insights telemetry processor that filters out exception telemetry caused by cancellations.
/// </summary>
public class IgnoreCancellationExceptionsTelemetryProcessor : ITelemetryProcessor
{
	private ITelemetryProcessor _next;

	/// <summary>
	/// Constructor.
	/// </summary>
	public IgnoreCancellationExceptionsTelemetryProcessor(ITelemetryProcessor next)
	{
		_next = next;
	}

	/// <inheritdoc/>
	public void Process(ITelemetry item)
	{
		if ((item is ExceptionTelemetry exceptionTelemetry)
			&& (exceptionTelemetry.Exception != null)
			&& CancellationExceptionChecker.IsCancellationException(exceptionTelemetry.Exception))
		{
			// do not process the cancellation exception telemetry
			// NOOP
		}
		else
		{
			// all other telemetry is allowed to continue processing
			_next.Process(item);
		}
	}
}
