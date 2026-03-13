using OpenTelemetry.Resources;

namespace Havit.Extensions.OpenTelemetry.Azure;

/// <summary>
/// Resource detektor nastavuje hodnotu pro "service.name".
/// V Application Insights se propíše do Cloud Role Name. 
/// </summary>
public class ServiceNameResourceDetector : IResourceDetector
{
	private readonly string _serviceName;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ServiceNameResourceDetector(string serviceName)
	{
		ArgumentException.ThrowIfNullOrEmpty(serviceName);
		_serviceName = serviceName;
	}

	/// <inheritdoc />
	public Resource Detect()
	{
		return new Resource([new KeyValuePair<string, object>("service.name", _serviceName)]);
	}
}