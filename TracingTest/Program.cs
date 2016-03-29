using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Diagnostics;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TracingTest
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			ExceptionTracer.Default.SubscribeToUnhandledExceptions(false);

			throw new ApplicationException("Test na Havit.Diagnostics.ExceptionTracer a Havit.Diagnostics.SmtpTraceListener.");
		}
	}
}
