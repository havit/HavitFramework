using System.Text;

namespace Havit.Tools.Win1250ToUtf8;

public static class Program
{
	public static void Main(string[] args)
	{
		bool requiresConversionToUtf8 = false;
		CommandLine.Utility.Arguments commandLineArguments = new CommandLine.Utility.Arguments(args);
		string patterns = commandLineArguments["patterns"];
		string mode = commandLineArguments["mode"];

		if (String.IsNullOrEmpty(patterns) || ((mode != "warn") && (mode != "convert")))
		{
			Console.WriteLine("Havit.Tools.Win1250ToUtf8.exe -patterns:*.txt;*.abc -mode:(warn|convert) [-key:true]");
			System.Environment.Exit(-2);
		}
		List<string> filenames = GetFilenames(System.Environment.CurrentDirectory, patterns);

		foreach (string filename in filenames)
		{
			if (filename.Contains(@"\_generated\")
				|| filename.Contains(@"\bin\")
				|| filename.Contains(@"\obj\")
				|| filename.Contains(@"\node_modules\")
				|| filename.Contains(@"\.vs\")
				|| filename.Contains(@"\.git\")
				|| filename.Contains(@"\.git\")
				|| filename.Contains(@"\.gitignore")
				|| filename.Contains(@"\.gitattributes\")
				|| filename.Contains(@".ps1")
				|| filename.Contains(@".sln")
				|| filename.Contains(@"package.json")
				|| filename.Contains(@"yarn.lock")
				|| filename.Contains(@"favicon.ico"))
			{
				continue;
			}

			if (RequiresConversionToUtf8(filename))
			{
				requiresConversionToUtf8 = true;
				Console.WriteLine(filename);
				if (mode == "convert")
				{
					ConvertToUtf8(filename);
				}
			}
		}


		// pokud jsme o to byli žádání z příkazové řádky, počkáme na stisk klávesy
		if (commandLineArguments["key"] == "true")
		{
			Console.WriteLine("Hotovo.");
			Console.ReadKey();
		}

		if ((mode == "warn") && requiresConversionToUtf8)
		{
			System.Environment.Exit(-1);
		}


	}

	private static List<string> GetFilenames(string path, string patterns)
	{
		string[] patternsArray = patterns.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

		List<string> filenames = new List<string>();

		foreach (string pattern in patternsArray)
		{
			string[] filenamesArray = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
			foreach (string filename in filenamesArray)
			{
				if (!filename.Contains(".svn") && !filenames.Contains(filename))
				{
					filenames.Add(filename);
				}
			}
		}
		return filenames;
	}

	private static bool RequiresConversionToUtf8(string filename)
	{
		byte[] preamble = UTF8Encoding.UTF8.GetPreamble();

		byte[] buffer = new byte[preamble.Length];
		using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 1024, FileOptions.None))
		{
			_ = fileStream.Read(buffer, 0, preamble.Length);
		}

		return !buffer.SequenceEqual(preamble);
	}

	private static void ConvertToUtf8(string filename)
	{
		string fileContent;

		using (TextReader reader = new StreamReader(filename, Encoding.GetEncoding(1250), true))
		{
			fileContent = reader.ReadToEnd();
		}

		using (TextWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
		{
			writer.Write(fileContent);
		}
	}
}