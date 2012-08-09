using System;
using System.Text.RegularExpressions;
using Havit.Text.RegularExpressions;

namespace Havit
{
	/// <summary>
	/// Matematické funkce, konstanty a rùzné další pomùcky.
	/// Tøída poskytuje statické metody a konstanty, je neinstanèní.
	/// </summary>
	public sealed class MathExt
	{
		#region IsEven, IsOdd
		/// <summary>
		/// Vrátí true, je-li zadané èíslo sudé.
		/// </summary>
		/// <param name="d">èíslo</param>
		/// <returns>true, je-li èíslo sudé</returns>
		public static bool IsEven(double d)
		{
			return ((d % 2) == 0);
		}


		/// <summary>
		/// Vrátí true, je-li zadané èíslo liché.
		/// </summary>
		/// <param name="d">èíslo</param>
		/// <returns>true, je-li èíslo liché</returns>
		public static bool IsOdd(double d)
		{
			return !IsEven(d);
		}
		#endregion

		#region IsInteger
		/// <summary>
		/// Ovìøí, zda-li je zadaný textový øetìzec celým èíslem.
		/// </summary>
		/// <remarks>
		/// Ovìøuje se vùèi regulárnímu výrazu <see cref="Havit.Text.RegularExpressions.RegexPatterns.Integer"/>.<br/>
		/// Pokud je text null, vrátí false.
		/// </remarks>
		/// <param name="text">ovìøovaný textový øetìzec</param>
		/// <returns>true, je-li text celým èíslem; jinak false</returns>
		public static bool IsInteger(string text)
		{
			return ((text != null) && Regex.IsMatch(text, RegexPatterns.Integer));
		}
		#endregion

		#region RoundToMultiple, CeilingToMultiple, FloorToMultiple
		/// <summary>
		/// Zaokrouhlí (aritmeticky) èíslo na nejbližší násobek (multiple) jiného èísla.
		/// </summary>
		/// <param name="d">èíslo k zaohrouhlení</param>
		/// <param name="multiple">èíslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>èíslo zaokrouhlené (aritmeticky) na nejbliží násobek (multiple)</returns>
		public static double RoundToMultiple(double d, double multiple)
		{
			return Math.Round(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí (aritmeticky) èíslo na nejbližší násobek (multiple) jiného èísla.
		/// </summary>
		/// <param name="d">èíslo k zaohrouhlení</param>
		/// <param name="multiple">èíslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>èíslo zaokrouhlené (aritmeticky) na nejbliží násobek (multiple)</returns>
		public static int RoundToMultiple(double d, int multiple)
		{
			return (int)Math.Round(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí èíslo na nejbližší vyšší násobek (multiple) jiného èísla.
		/// </summary>
		/// <param name="d">èíslo k zaohrouhlení</param>
		/// <param name="multiple">èíslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>èíslo zaokrouhlené na nejbliží vyšší násobek (multiple)</returns>
		public static double CeilingToMultiple(double d, double multiple)
		{
			return Math.Ceiling(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí èíslo na nejbližší vyšší násobek (multiple) jiného èísla.
		/// </summary>
		/// <param name="d">èíslo k zaohrouhlení</param>
		/// <param name="multiple">èíslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>èíslo zaokrouhlené na nejbliží vyšší násobek (multiple)</returns>
		public static int CeilingToMultiple(double d, int multiple)
		{
			return (int)Math.Ceiling(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí èíslo na nejbližší nižší násobek (multiple) jiného èísla.
		/// </summary>
		/// <param name="d">èíslo k zaohrouhlení</param>
		/// <param name="multiple">èíslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>èíslo zaokrouhlené na nejbliží nižší násobek (multiple)</returns>
		public static double FloorToMultiple(double d, double multiple)
		{
			return Math.Floor(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí èíslo na nejbližší nižší násobek (multiple) jiného èísla.
		/// </summary>
		/// <param name="d">èíslo k zaohrouhlení</param>
		/// <param name="multiple">èíslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>èíslo zaokrouhlené na nejbliží nižší násobek (multiple)</returns>
		public static int FloorToMultiple(double d, int multiple)
		{
			return (int)Math.Floor(d / multiple) * multiple;
		}
		#endregion

		#region Max(params), Min(params)
		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static int Max(params int[] values)
		{
			int result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				if (values[i] > result)
				{
					result = values[i];
				}
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static double Max(params double[] values)
		{
			double result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				result = Math.Max(result, values[i]);
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static float Max(params float[] values)
		{
			float result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				result = Math.Max(result, values[i]);
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static decimal Max(params decimal[] values)
		{
			decimal result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				result = Math.Max(result, values[i]);
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static byte Max(params byte[] values)
		{
			byte result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				if (values[i] > result)
				{
					result = values[i];
				}
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static int Min(params int[] values)
		{
			int result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				if (values[i] < result)
				{
					result = values[i];
				}
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static double Min(params double[] values)
		{
			double result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				result = Math.Min(result, values[i]);
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static float Min(params float[] values)
		{
			float result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				result = Math.Min(result, values[i]);
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static decimal Min(params decimal[] values)
		{
			decimal result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				result = Math.Min(result, values[i]);
			}
			return result;
		}

		/// <summary>
		/// Vrátí nejvìtší ze zadaných èísel.
		/// </summary>
		/// <param name="values">èísla k porovnání</param>
		/// <returns>nejvìtší z values</returns>
		public static byte Min(params byte[] values)
		{
			byte result = values[0];
			int length = values.Length;
			for (int i = 1; i < length; i++)
			{
				if (values[i] < result)
				{
					result = values[i];
				}
			}
			return result;
		}
		#endregion

		#region private constructor
		/// <summary>
		/// Prázdný private constructor zamezující vytvoøení instance tøídy.
		/// </summary>
		private MathExt() {}
		#endregion
	}
}
