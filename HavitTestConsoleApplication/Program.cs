using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Havit.Business;
using Havit.Data.SqlTypes;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;

namespace HavitTestConsoleApplication
{

	internal class Program
	{
		#region Main
		private static void Main(string[] args)
		{
			//TestAggregateMicroCollections();
			//TestAggregateSmallCollections();
			//TestAggregateLargeCollections();

			//Console.WriteLine("String split + parse");
			//TestSplitMicro();
			//TestSplitSmall();
			//TestSplitLarge();

			//Console.WriteLine("regex match + parse");
			//MatchCollection matches = Regex.Matches("<ID>1234567</ID>", "[-]\\d+", RegexOptions.Singleline | RegexOptions.Compiled);
			//TestRegexParseMicro();
			//TestRegexParseSmall();
			//TestRegexParseLarge();
		}
		#endregion

		#region TestRegexParseLarge
		private static void TestRegexParseLarge()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 50000; i++)
			{
				sb.Append("<ID>123467</ID>");
			}

			TestRegexParse(100, sb.ToString());
		}
		#endregion

		#region TestRegexParseSmall
		private static void TestRegexParseSmall()
		{
			TestRegexParse(1000000, "<ID>1234567</ID><ID>1234567</ID><ID>1234567</ID><ID>1234567</ID><ID>1234567</ID>");
		}
		#endregion

		#region TestRegexParseMicro
		private static void TestRegexParseMicro()
		{
			TestRegexParse(5000000, "<ID>1234567</ID>");
		}
		#endregion

		#region TestRegexParse
		private static void TestRegexParse(int repeatCount, string value)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			for (int i = 0; i < repeatCount; i++)
			{
				MatchCollection matches = Regex.Matches(value, "-?\\d+", RegexOptions.Singleline | RegexOptions.Compiled);
				int[] intValues = new int[matches.Count];

				for (int j = 0; j < matches.Count; j++)
				{
					intValues[j] = IntParseFast1(matches[j].Value);
				}
			}

			sw.Stop();
			Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
			Console.WriteLine("{0}x {1} ns", repeatCount, sw.ElapsedMilliseconds / (decimal)repeatCount * 1000);
			Console.WriteLine();

		}
		#endregion

		#region TestSplitMicro
		private static void TestSplitMicro()
		{
			TestSplit(5000000, "1234567|");
		}
		#endregion

		#region TestSplitSmall
		private static void TestSplitSmall()
		{
			TestSplit(1000000, "1234567|1234567|1234567|1234567|1234567|");
		}
		#endregion

		#region TestSplitLarge
		private static void TestSplitLarge()
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < 50000; i++)
			{
				sb.Append("123467|");
			}

			TestSplit(100, sb.ToString());
		}
		#endregion

		#region TestAggregateMicroCollections
		private static void TestAggregateMicroCollections()
		{
			TestAggregate(5000000, Enumerable.Repeat(1234567, 1).Select(i => (SqlInt32)i).ToArray());
		}
		#endregion

		#region TestAggregateSmallCollections
		private static void TestAggregateSmallCollections()
		{
			TestAggregate(1000000, Enumerable.Repeat(1234567, 5).Select(i => (SqlInt32)i).ToArray());
		}
		#endregion

		#region TestAggregateLargeCollections
		private static void TestAggregateLargeCollections()
		{
			TestAggregate(100, Enumerable.Repeat(1234567, 50000).Select(i => (SqlInt32)i).ToArray());
		}
		#endregion

		#region TestAggregate
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
			Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
			Console.WriteLine("{0}x {1} ns", repeatCount, sw.ElapsedMilliseconds / (decimal)repeatCount * 1000);
			Console.WriteLine();

		}
		#endregion

		#region TestSplit
		private static void TestSplit(int repeatCount, string value)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			char[] separator = new char[] { '|' };
			unchecked
			{
				for (int i = 0; i < repeatCount; i++)
				{
					var stringValues = value.TrimEnd(separator).Split(separator);
					int[] intValues = new int[stringValues.Length];

					for (int j = 0; j < stringValues.Length; j++)
					{
						intValues[j] = IntParseFast1(stringValues[j]);
						//int.TryParse(stringValues[j], NumberStyles.None, NumberFormatInfo.InvariantInfo, out intValues[j]);
					}
				}
			}
			sw.Stop();
			Console.WriteLine("{0} ms", sw.ElapsedMilliseconds);
			Console.WriteLine("{0}x {1} ns", repeatCount, sw.ElapsedMilliseconds / (decimal)repeatCount * 1000);
			Console.WriteLine();
		}
		#endregion

		#region IntParseFast1
		private static int IntParseFast1(string value)
		{
			unchecked
			{

				int result = 0;
				byte negative = (byte)(((value.Length > 0) && (value[0] == '-')) ? 1 : 0);

				byte l = (byte)value.Length;
				for (byte i = negative; i < l; i++)
				{
					//char letter = value[i];
					result = 10 * result + (value[i] - 48);
				}

				return negative == 0 ? result : -1 * result;
			}

		}
		#endregion

		#region IntParseFast2
		private static int IntParseFast2(string value)
		{
			unchecked
			{

				int result = 0;
				int negative = (byte)(((value.Length > 0) && (value[0] == '-')) ? 1 : 0);

				int l = value.Length;
				for (int i = negative; i < l; i++)
				{
					//char letter = value[i];
					result = 10 * result + (value[i] - 48);
				}

				return negative == 0 ? result : -1 * result;
			}

		}
		#endregion

	}
}
