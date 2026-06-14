namespace Havit;

/// <summary>
/// Mathematical functions, constants, and various other utilities.
/// The class provides static methods and constants, it is non-instantiable.
/// </summary>
public static class MathExt
{
	/// <summary>
	/// Returns true if the specified number is even.
	/// </summary>
	/// <param name="d">number</param>
	/// <returns>true if the number is even</returns>
	public static bool IsEven(double d)
	{
		return ((d % 2) == 0);
	}

	/// <summary>
	/// Returns true if the specified number is odd.
	/// </summary>
	/// <param name="d">number</param>
	/// <returns>true if the number is odd</returns>
	public static bool IsOdd(double d)
	{
		return !IsEven(d);
	}

	/// <summary>
	/// Verifies whether the specified string is an integer.
	/// </summary>
	/// <remarks>
	/// Accepts an optional leading sign (+/-) followed by one or more ASCII digits (equivalent to the pattern <c>^[-+]?\d+$</c> restricted to ASCII digits, which is what integers actually parse as).<br/>
	/// If the text is null or empty, it returns false.
	/// </remarks>
	/// <param name="text">verified string</param>
	/// <returns>true if the text is an integer; otherwise, false</returns>
	public static bool IsInteger(string text)
	{
		// Manual scan instead of a regex - this is often called in tight loops and avoids the Regex machinery (cache lookup, Match state allocation).
		if (String.IsNullOrEmpty(text))
		{
			return false;
		}

		int i = 0;
		if ((text[0] == '+') || (text[0] == '-'))
		{
			i = 1;
		}

		if (i == text.Length)
		{
			// sign only, no digits
			return false;
		}

		for (; i < text.Length; i++)
		{
			char c = text[i];
			if ((c < '0') || (c > '9'))
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>
	/// Rounds a number to the nearest multiple of another number.
	/// When the number is exactly halfway between two multiples, it is rounded to the even multiple (banker's rounding, <see cref="MidpointRounding.ToEven" />).
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest multiple</returns>
	public static double RoundToMultiple(double d, double multiple)
	{
		return Math.Round(d / multiple) * multiple;
	}

	/// <summary>
	/// Rounds a number to the nearest multiple of another number.
	/// When the number is exactly halfway between two multiples, it is rounded to the even multiple (banker's rounding, <see cref="MidpointRounding.ToEven" />).
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest multiple</returns>
	public static int RoundToMultiple(double d, int multiple)
	{
		return (int)Math.Round(d / multiple) * multiple;
	}

	/// <summary>
	/// Rounds a number to the nearest multiple of another number.
	/// When the number is exactly halfway between two multiples, it is rounded to the even multiple (banker's rounding, <see cref="MidpointRounding.ToEven" />).
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest multiple</returns>
	public static decimal RoundToMultiple(decimal d, decimal multiple)
	{
		return Math.Round(d / multiple) * multiple;
	}

	/// <summary>
	/// Rounds a number to the nearest higher multiple of another number.
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest higher multiple</returns>
	public static double CeilingToMultiple(double d, double multiple)
	{
		return Math.Ceiling(d / multiple) * multiple;
	}

	/// <summary>
	/// Rounds a number to the nearest higher multiple of another number.
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest higher multiple</returns>
	public static int CeilingToMultiple(double d, int multiple)
	{
		return (int)Math.Ceiling(d / multiple) * multiple;
	}

	/// <summary>
	/// Rounds a number to the nearest higher multiple of another number.
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest higher multiple</returns>
	public static decimal CeilingToMultiple(decimal d, decimal multiple)
	{
		return Math.Ceiling(d / multiple) * multiple;
	}

	/// <summary>
	/// Rounds a number to the nearest lower multiple of another number.
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest lower multiple</returns>
	public static double FloorToMultiple(double d, double multiple)
	{
		return Math.Floor(d / multiple) * multiple;
	}

	/// <summary>
	/// Rounds a number to the nearest lower multiple of another number.
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest lower multiple</returns>
	public static int FloorToMultiple(double d, int multiple)
	{
		return (int)Math.Floor(d / multiple) * multiple;
	}

	/// <summary>
	/// Rounds a number to the nearest lower multiple of another number.
	/// </summary>
	/// <param name="d">number to round</param>
	/// <param name="multiple">number to round to its multiple</param>
	/// <returns>number rounded to the nearest lower multiple</returns>
	public static decimal FloorToMultiple(decimal d, decimal multiple)
	{
		return Math.Floor(d / multiple) * multiple;
	}

	/// <summary>
	/// Returns the largest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the largest of the values</returns>
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
	/// Returns the largest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the largest of the values</returns>
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
	/// Returns the largest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the largest of the values</returns>
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
	/// Returns the largest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the largest of the values</returns>
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
	/// Returns the largest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the largest of the values</returns>
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
	/// Returns the smallest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the smallest of the values</returns>
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
	/// Returns the smallest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the smallest of the values</returns>
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
	/// Returns the smallest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the smallest of the values</returns>
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
	/// Returns the smallest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the smallest of the values</returns>
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
	/// Returns the smallest of the specified numbers.
	/// </summary>
	/// <param name="values">numbers to compare</param>
	/// <returns>the smallest of the values</returns>
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
