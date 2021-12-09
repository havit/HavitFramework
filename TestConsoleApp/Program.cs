using Havit.Business;
using Havit.BusinessLayerTest;
using System;
using System.Threading;

namespace TestConsoleApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			using (var ims = new IdentityMapScope())
			{
				Console.WriteLine(Subjekt.GetObject(8).Nazev);
			}

			using (new IdentityMapScope())
			{
				Console.WriteLine(Subjekt.GetObject(8).Nazev);
			}

		}
	}
}
