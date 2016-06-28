using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;

using Havit.Services.Ares;
using System.ComponentModel;
using Havit;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;

using Havit.Services.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;
using HavitTestConsoleApplication.ServiceReference1;

namespace HavitTestConsoleApplication
{

	internal static class Program
	{
		#region Main
		private static void Main(string[] args)
		{
			ServiceReference1.WebServiceExceptionTestSoapClient client = new WebServiceExceptionTestSoapClient();
			client.DoException();
		}
		#endregion

	}
}
