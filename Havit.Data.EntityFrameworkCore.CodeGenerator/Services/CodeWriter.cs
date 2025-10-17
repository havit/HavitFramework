using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

/// <summary>
/// Třída CodeWriter zajišťuje generování kódu. Zajišťuje odsazení, v budoucnu může zajišťovat ochranu před maximální délkou řádky kódu, apod.
/// </summary>
public class CodeWriter : ICodeWriter
{
	private readonly ICodeWriteReporter _codeWriteReporter;

	public CodeWriter(ICodeWriteReporter codeWriteReporter)
	{
		_codeWriteReporter = codeWriteReporter;
	}

	/// <summary>
	/// Zapíšeme obsah do souboru (jen tehdy, pokud se neliší od současného obsahu souboru).
	/// </summary>
	public async Task SaveAsync(string filename, string content, OverwriteBahavior overwriteBahavior, CancellationToken cancellationToken = default)
	{
		_codeWriteReporter.ReportWriteFile(filename);

		// Normalizujeme konce řádků poskytnutého obsahu.
		// Content obsahuje konce řádků ve formátu CRLF (Windows), protože takto přichází z *.tt šablon.
		// Pro MacOS a Linux převedeme Windows-style konce řádků (CRLF) na Unix-style (LF).
		content = NormalizePlatformSpecificLineEndings(content);

		// Na windows může existovat soubor s názvem, který se liší jen velikostí písmen (např. "file.csv" a "FILE.csv").
		// Abychom předešli problémům níže s tím, zda AlreadyExistsTheSameAsync má vrátit true nebo false, provedeme zde kontrolu, zda soubor existuje
		// s a bez ohledu na case-sensitivitu a případně soubor přejmenujeme na správný název (case sensitivity).
		// Názvy složek však nejsou řešeny.
		bool existsCaseInsensitive = FileExistsCaseInsensitive(filename);

		bool existsCaseSensitive = existsCaseInsensitive && FileExistsCaseSensitive(filename); // exists: nemuže existovat case sensitive, pokud vůbec neexistuje
		if (existsCaseInsensitive && !existsCaseSensitive)
		{
			// přejmenuje soubor na správný název (bez ohledu na název složky)
			System.IO.File.Move(filename, filename);
			existsCaseSensitive = true;
		}
		Debug.Assert(existsCaseInsensitive == existsCaseSensitive);

		if (!(await AlreadyExistsTheSameAsync(filename, content, cancellationToken)) || !HasByteOrderMask(filename))
		{
			string directory = Path.GetDirectoryName(filename);
			if (!String.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			if ((overwriteBahavior == OverwriteBahavior.OverwriteWhenFileAlreadyExists) || !existsCaseInsensitive)
			{
				await File.WriteAllTextAsync(filename, content, Encoding.UTF8, cancellationToken);
			}
		}
	}

	/// <summary>
	/// Normalizuje konce řádků poskytnutého obsahu. Předpokládá se, že na vstupu jsou konce řádků ve formátu CRLF (Windows), protože takto přichází z *.tt šablon.
	/// Na platformě Windows zachová původní konce řádků (CRLF), na ostatních platformách (Linux, Mac) převede Windows-style konce řádků (CRLF) na Unix-style (LF).
	/// </summary>
	private static string NormalizePlatformSpecificLineEndings(string content)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return content;
		}

		// Na ostatních platformách (Linux, Mac) normalizujeme konce řádků na LF (Unix style).
		return content.Replace("\r\n", "\n");
	}

	/// <summary>
	/// Vrací true, pokud již existuje soubor se stejným jménem a obsahem.
	/// </summary>
	private async Task<bool> AlreadyExistsTheSameAsync(string filename, string content, CancellationToken cancellationToken = default)
	{
		return (File.Exists(filename) && (await File.ReadAllTextAsync(filename, cancellationToken) == content));
	}

	/// <summary>
	/// Vrací true, pokud soubor již existuje a má UTF-8 byte order mask.
	/// </summary>
	private bool HasByteOrderMask(string filename)
	{
		byte[] utf8BOM = Encoding.UTF8.GetPreamble();
		if (!File.Exists(filename) || (new FileInfo(filename).Length < utf8BOM.Length))
		{
			return false;
		}

		byte[] fileBOM;
		using (FileStream fileStream = new FileStream(filename, FileMode.Open))
		{
			using (BinaryReader reader = new BinaryReader(fileStream))
			{
				fileBOM = reader.ReadBytes(utf8BOM.Length);
			}
		}
		return utf8BOM.SequenceEqual(fileBOM);
	}

	/// <summary>
	/// Vrací true, pokud existuje soubor se stejným názvem case-sensitive. Case sensitivita složky se neřeší.
	/// </summary>
	public static bool FileExistsCaseInsensitive(string fullPath)
	{
		string filename = Path.GetFileName(fullPath);
		return new DirectoryInfo(Path.GetDirectoryName(fullPath)).EnumerateFiles(filename).Any(file => String.Equals(file.Name, filename, StringComparison.CurrentCultureIgnoreCase));
	}

	/// <summary>
	/// Vrací true, pokud existuje soubor se stejným názvem case-sensitive. Case sensitivita složky se neřeší.
	/// </summary>
	public static bool FileExistsCaseSensitive(string fullPath)
	{
		string filename = Path.GetFileName(fullPath);
		return new DirectoryInfo(Path.GetDirectoryName(fullPath)).EnumerateFiles(filename).Any(file => String.Equals(file.Name, filename, StringComparison.CurrentCulture));
	}
}
