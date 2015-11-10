using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Diagnostics;
using System.Diagnostics;

namespace TracingTest
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			ExceptionTracer.Default.SubscribeToUnhandledExceptions(false);

			//SmtpTraceListener listener = new SmtpTraceListener("devmail@havit.cz");

			//TraceSource source = new TraceSource(ExceptionTracer.DefaultTraceSourceName);
			//source.Listeners.Add(listener);

			throw new ApplicationException("Test na Havit.Diagnostics.ExceptionTracer a Havit.Diagnostics.SmtpTraceListerner.");
		}
	}
}
