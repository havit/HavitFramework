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

using Havit.Services.Ares;
using System.ComponentModel;

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
			
			//var service = new Havit.Services.Ares.AresService("73381543");
			//var response = service.GetData();		
			//Console.WriteLine(response);

			TestConverter();

		}

		private static void TestConverter()
		{
			Havit.ComponentModel.UniversalTypeConverter.ConvertTo(null, typeof(int?));
			Stopwatch sw = new Stopwatch();
			sw.Start();

			var data = new object[] { 1, (int?)1, (float)0, (float?)0, 0M, (decimal?)0M, "5", "A", "", ListSortDirection.Ascending, null };
			var targetTypes = new Type[] { typeof(int), typeof(int?), typeof(float), typeof(float?), typeof(decimal), typeof(decimal?), typeof(string), typeof(ListSortDescription) };
			data.Join(targetTypes, x => 1, y => 1, (x, y) => new { Value = x, TargetType = y }).Distinct().ToList().ForEach(item =>
				{
					Type targetType = item.TargetType;
					if (item.TargetType.IsGenericType && item.TargetType.GetGenericTypeDefinition() == typeof(Nullable<>).GetGenericTypeDefinition())
					{
						targetType = Nullable.GetUnderlyingType(targetType);
						Console.Write("{1} ({0}) --> Nullable<{2}>: ", item.Value, item.Value == null ? "null" : item.Value.GetType().Name, targetType.Name);
					}
					else
					{
						Console.Write("{1} ({0}) --> {2}: ", item.Value, item.Value == null ? "null" : item.Value.GetType().Name, targetType.Name);
					}

					try
					{
						if (item.Value == null)
						{
							Console.Write("null ");
						}
						else if (targetType.IsAssignableFrom(item.Value.GetType()))
						{
							Console.Write("assignable ");
						}
						else if (item.Value.GetType().IsPrimitive && targetType.IsPrimitive /* && item.Value.GetType().IsPrimitive*/)
						{
							Console.Write("changetype ");
							System.Convert.ChangeType(item.Value, targetType);
						}
						else
						{
							Console.Write("converter ");
							TypeConverter converter = TypeDescriptor.GetConverter(targetType);
							if (converter.CanConvertFrom(item.Value.GetType()))
							{
								converter.ConvertFrom(item.Value);								
							}
							else
							{
								converter = TypeDescriptor.GetConverter(item.Value);
								if (converter.CanConvertTo(targetType))
								{
									converter.ConvertTo(item.Value, targetType);
								}
								else
								{
									throw new ApplicationException();
								}
							}
						}
						//						System.Convert.ChangeType(item.X, item.Y.GetType());
						Console.Write("OK");
					}
					catch (Exception)
					{
						Console.Write("exception");						
					}
					Console.WriteLine();
				});

			//for (int i = 0; i < 1000000; i++)
			//{
				
			//	Console.WriteLine(System.Convert.ChangeType(5M, typeof(string)));
			//	//Console.WriteLine(typeof(int).IsAssignableFrom(typeof(int)));				
			//	TypeDescriptor.GetConverter(typeof(float)).ConvertFrom("5");
			//}
			sw.Stop();
			Console.WriteLine(sw.ElapsedMilliseconds);
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
