using System;
using System.Linq;
using Havit.Extensions.DependencyInjection.Abstractions;
using Havit.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

namespace TestConsoleApp;

public class Program
{
	public static void Main()
	{
		string[] strings = { "a.txt", "b.txt", "c.TXt", "d.TXT" };
		foreach (string s in strings.Where(item => RegexPatterns.IsFileWildcardMatch(item.ToLower(), "*.TXT".ToLower())))
		{
			Console.WriteLine(s);
		}
	}
}
