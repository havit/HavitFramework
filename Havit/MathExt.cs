using System;
using System.Text.RegularExpressions;
using Havit.Text.RegularExpressions;

namespace Havit
{
	/// <summary>
	/// Matematické funkce, konstanty a různé další pomůcky.
	/// Třída poskytuje statické metody a konstanty, je neinstanční.
	/// </summary>
	public static class MathExt
	{
		/// <summary>
		/// Vrátí true, je-li zadané číslo sudé.
		/// </summary>
		/// <param name="d">číslo</param>
		/// <returns>true, je-li číslo sudé</returns>
		public static bool IsEven(double d)
		{
			return ((d % 2) == 0);
		}

		/// <summary>
		/// Vrátí true, je-li zadané číslo liché.
		/// </summary>
		/// <param name="d">číslo</param>
		/// <returns>true, je-li číslo liché</returns>
		public static bool IsOdd(double d)
		{
			return !IsEven(d);
		}

		/// <summary>
		/// Ověří, zdali je zadaný textový řetězec celým číslem.
		/// </summary>
		/// <remarks>
		/// Ověřuje se vůči regulárnímu výrazu <see cref="Havit.Text.RegularExpressions.RegexPatterns.Integer"/>.<br/>
		/// Pokud je text null, vrátí false.
		/// </remarks>
		/// <param name="text">ověřovaný textový řetězec</param>
		/// <returns>true, je-li text celým číslem; jinak false</returns>
		public static bool IsInteger(string text)
		{
			return ((text != null) && Regex.IsMatch(text, RegexPatterns.Integer));
		}

		/// <summary>
		/// Zaokrouhlí (aritmeticky) číslo na nejbližší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené (aritmeticky) na nejbliží násobek (multiple)</returns>
		public static double RoundToMultiple(double d, double multiple)
		{
			return Math.Round(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí (aritmeticky) číslo na nejbližší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené (aritmeticky) na nejbliží násobek (multiple)</returns>
		public static int RoundToMultiple(double d, int multiple)
		{
			return (int)Math.Round(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí (aritmeticky) číslo na nejbližší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené (aritmeticky) na nejbliží násobek (multiple)</returns>
		public static decimal RoundToMultiple(decimal d, decimal multiple)
		{
			return Math.Round(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí číslo na nejbližší vyšší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené na nejbliží vyšší násobek (multiple)</returns>
		public static double CeilingToMultiple(double d, double multiple)
		{
			return Math.Ceiling(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí číslo na nejbližší vyšší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené na nejbliží vyšší násobek (multiple)</returns>
		public static int CeilingToMultiple(double d, int multiple)
		{
			return (int)Math.Ceiling(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí číslo na nejbližší vyšší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené na nejbliží vyšší násobek (multiple)</returns>
		public static decimal CeilingToMultiple(decimal d, decimal multiple)
		{
			return Math.Ceiling(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí číslo na nejbližší nižší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené na nejbliží nižší násobek (multiple)</returns>
		public static double FloorToMultiple(double d, double multiple)
		{
			return Math.Floor(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí číslo na nejbližší nižší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené na nejbliží nižší násobek (multiple)</returns>
		public static int FloorToMultiple(double d, int multiple)
		{
			return (int)Math.Floor(d / multiple) * multiple;
		}

		/// <summary>
		/// Zaokrouhlí číslo na nejbližší nižší násobek (multiple) jiného čísla.
		/// </summary>
		/// <param name="d">číslo k zaohrouhlení</param>
		/// <param name="multiple">číslo, na jehož násobek se má zaokrouhlit (multiple)</param>
		/// <returns>číslo zaokrouhlené na nejbliží nižší násobek (multiple)</returns>
		public static decimal FloorToMultiple(decimal d, decimal multiple)
		{
			return Math.Floor(d / multiple) * multiple;
		}

		/// <summary>
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
		/// Vrátí největší ze zadaných čísel.
		/// </summary>
		/// <param name="values">čísla k porovnání</param>
		/// <returns>největší z values</returns>
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
	}
}
