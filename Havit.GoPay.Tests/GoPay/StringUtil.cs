namespace Havit.Tests.GoPay;

internal static class StringUtil
{
	public static string[] Split(string input, char c)
	{
		int index = input.IndexOf(c);

		return (index == -1)
			? new[] { input }
			: new[] { input.Substring(0, index), input.Substring(index + 1) };

	}
}
