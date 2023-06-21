using Havit.Services.Sftp.FileStorage;
using Havit.Text.RegularExpressions;
using Renci.SshNet;
using System;
using System.IO;
using System.Linq;
using System.Threading;

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
