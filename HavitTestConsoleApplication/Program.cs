using System;
using System.Collections.Generic;
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

namespace HavitTestConsoleApplication
{

	internal class Program
	{
		#region Main
		private static void Main(string[] args)
		{
			ActiveDirectoryServices ads = new ActiveDirectoryServices(@"CMEDC\lookup", "@cm3n0v4!!$");
			foreach (string groupmember in ads.GetGroupMembers("CMEDC\\cz-ss", false, true))
			{
				Console.WriteLine(groupmember);
			}
		}
		#endregion

	}
}
