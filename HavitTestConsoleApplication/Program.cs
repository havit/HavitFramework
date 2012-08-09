using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.BusinessLayerTest;

namespace HavitTestConsoleApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			SubjektCollection subjekty = Subjekt.GetAll();			
			for (int i = 0; i < 1000; i++)
			{
				Havit.Collections.SortHelper.PropertySort(subjekty, "Uzivatel.DisplayAs.Length");
			}
		}
	}
}
