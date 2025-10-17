using Havit.Diagnostics;

namespace TracingTest;

internal static class Program
{
	private static void Main()
	{
		ExceptionTracer.Default.SubscribeToUnhandledExceptions();

		throw new ApplicationException("Test na Havit.Diagnostics.ExceptionTracer a Havit.Diagnostics.SmtpTraceListener.");
	}
}
