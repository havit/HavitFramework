using System;
using System.Collections.Generic;
using System.Globalization;
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
			TestAggregateMicroCollections();
			TestAggregateSmallCollections();
			TestAggregateLargeCollections();

			TestSplitMicro();
			TestSplitSmall();
			TestSplitLarge();

		}

		private static void TestSplitMicro()
		{
			TestSplit(5000000, "1234567");
		}

		private static void TestSplitSmall()
		{
			TestSplit(1000000, "1234567|1234567|1234567|1234567|1234567");
		}
		
		private static void TestSplitLarge()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 50000; i++)
			{
				if (i > 0)
				{
					sb.Append("|");
				}
				sb.Append("123467");
			}

			TestSplit(100, sb.ToString());
		}

		private static void TestAggregateMicroCollections()
		{
			TestAggregate(5000000, Enumerable.Repeat(1234567, 1).Select(i => (SqlInt32)i).ToArray());
		}

		private static void TestAggregateSmallCollections()
		{
			TestAggregate(1000000, Enumerable.Repeat(1234567, 5).Select(i => (SqlInt32)i).ToArray());
		}

		private static void TestAggregateLargeCollections()
		{
			TestAggregate(100, Enumerable.Repeat(1234567, 50000).Select(i => (SqlInt32)i).ToArray());
		}

		private static void TestAggregate(int repeatCount, SqlInt32[] collectionData)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

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

			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine("{0}x {1}", repeatCount, sw.ElapsedMilliseconds / (decimal)repeatCount);
			Console.WriteLine();

		}

		private static void TestSplit(int repeatCount, string value)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			unchecked
			{
				for (int i = 0; i < repeatCount; i++)
				{
					var stringValues = value.Split(new char[] { '|' });
					int[] intValues = new int[stringValues.Length];

					for (int j = 0; j < stringValues.Length; j++)
					{
						intValues[j] = IntParseFast(stringValues[j]);
						//int.TryParse(stringValues[j], NumberStyles.None, NumberFormatInfo.InvariantInfo, out intValues[j]);
					}
				}
			}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
			Console.WriteLine("{0}x {1}", repeatCount, sw.ElapsedMilliseconds / (decimal)repeatCount);
			Console.WriteLine();
		}

		private static int IntParseFast(string value)
		{
			unchecked
			{
				
			int result = 0;
			bool negative = value.Length > 0 && (value[0] == '-');

			for (int i = negative ? 1 : 0; i < value.Length; i++)
			{
				//char letter = value[i];
				result = 10 * result + (value[i] - 48);
			}

			return negative ? -1 * result : result;
			}

		}

	}
}
