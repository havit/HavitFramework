using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Havit.Data.SqlTypes;

namespace HavitTestConsoleApplication
{
	using System.Data.SqlTypes;
	using System.Diagnostics;
	using System.IO;

	class Program
	{		
		static void Main(string[] args)
		{
			//TestSplit();
			System.Diagnostics.Stopwatch sw = new Stopwatch();
			sw.Start();
			TestMicroCollections();
			//TestSmallCollections();
			//TestLargeCollections();
			sw.Stop();
		}

		//private static void TestSplit()
		//{
		//	string[] s1 = null;
		//	string[] s2 = null;
		//	string[] s3 = null;
		//	string[] s4 = null;
		//	char[] separators = new char[] { '|' };
		//	for (int i = 0; i < 1000000; i++)
		//	{
		//		s1 = "|1234".Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//		s2 = "|1234".Split(separators, StringSplitOptions.None);
		//		s3 = "|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234".Split(separators, StringSplitOptions.RemoveEmptyEntries);
		//		s4 = "|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234|1234".Split(separators, StringSplitOptions.None);				
		//	}
		//	Console.WriteLine(s1.Length + s2.Length + s3.Length + s4.Length);
		//}

		private static void TestMicroCollections()
		{
			Test(5000000, Enumerable.Range(1, 1).Select(i => (SqlInt32)i).ToArray());
		}

		private static void TestSmallCollections()
		{
			Test(1000000, Enumerable.Range(1, 5).Select(i => (SqlInt32)i).ToArray());
		}

		private static void TestLargeCollections()
		{
			Test(100, Enumerable.Range(1, 50000).Select(i => (SqlInt32)i).ToArray());
		}

		private static void Test(int repeatCount, SqlInt32[] collectionData)
		{
			SqlInt32ArrayAggregate aggregate = new SqlInt32ArrayAggregate();

			for (int i = 0; i < repeatCount; i++)
			{
				aggregate.Init();

				for (int j = 0; j < collectionData.Length; j++)
				{
					aggregate.Accumulate(collectionData[j]);
				}

				SqlInt32Array result = aggregate.Terminate();
				
				using (MemoryStream ms = new MemoryStream())
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					result.Write(writer);
				}
			}
		}

	}
}
