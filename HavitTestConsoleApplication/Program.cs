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

namespace HavitTestConsoleApplication
{

	internal class Program
	{
		#region Main
		private static void Main(string[] args)
		{
			ActiveDirectoryServices ads = new ActiveDirectoryServices();
			//			var groups = ads.GetUserCrossDomainMembership("HAVIT\\kanda", new string[] { "HAVIT\\devs", "HAVIT\\Domain Users"}, true);
			var groups = ads.GetUserDomainMembership("kanda");

				foreach (string group in groups)
				{
					Console.WriteLine(group);
				}
		}
		#endregion

	}
}
