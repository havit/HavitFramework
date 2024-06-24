namespace Havit.AspNetCore.ExceptionMonitoring.Processors;

/// <summary>
/// Exception Monitoring Processor.
/// </summary>
public interface IExceptionMonitoringProcessor
{
	/// <summary>
	/// Zpracuje výjimku zaslanou do exception monitoringu.
	/// </summary>
	void ProcessException(Exception exception);
}
