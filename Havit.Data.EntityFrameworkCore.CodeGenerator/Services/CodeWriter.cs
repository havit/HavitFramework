using System.Runtime.InteropServices;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

/// <summary>
/// Třída CodeWriter zajišťuje generování kódu. Zajišťuje odsazení, v budoucnu může zajišťovat ochranu před maximální délkou řádky kódu, apod.
/// </summary>
public class CodeWriter : ICodeWriter
{
	private static readonly byte[] Utf8ByteOrderMask = Encoding.UTF8.GetPreamble();

	private readonly ICodeWriteReporter _codeWriteReporter;

	public CodeWriter(ICodeWriteReporter codeWriteReporter)
	{
		_codeWriteReporter = codeWriteReporter;
	}

	/// <summary>
	/// Zapíšeme obsah do souboru (jen tehdy, pokud se neliší od současného obsahu souboru).
	/// </summary>
	public async Task SaveAsync(string filename, string content, OverwriteBehavior overwriteBehavior, CancellationToken cancellationToken = default)
	{
		_codeWriteReporter.ReportWriteFile(filename);

		// Normalizujeme konce řádků poskytnutého obsahu.
		// Content obsahuje konce řádků ve formátu CRLF (Windows), protože takto přichází z *.tt šablon.
		// Pro MacOS a Linux převedeme Windows-style konce řádků (CRLF) na Unix-style (LF).
		content = NormalizePlatformSpecificLineEndings(content);

		string directory = Path.GetDirectoryName(filename);
		string requestedFilename = Path.GetFileName(filename);

		// Na Windows může existovat soubor s názvem, který se liší jen velikostí písmen (např. "file.cs" a "FILE.cs").
		// Jediným průchodem adresářem zjistíme, zda soubor existuje (bez ohledu na velikost písmen) a jaký je jeho skutečný název na disku.
		// Pokud se skutečný název liší jen velikostí písmen, přejmenujeme soubor na požadovaný název (case sensitivity). Názvy složek se neřeší.
		string actualFilenameOnDisk = GetFilenameOnDisk(directory, requestedFilename);
		bool existsCaseInsensitive = (actualFilenameOnDisk != null);
		if (existsCaseInsensitive && !String.Equals(actualFilenameOnDisk, requestedFilename, StringComparison.CurrentCulture))
		{
			// přejmenuje soubor na správný název (bez ohledu na název složky)
			File.Move(filename, filename);
		}

		// Soubor přepíšeme, pokud se jeho obsah liší, nebo pokud nemá UTF-8 BOM (chceme jej doplnit).
		// Pokud soubor neexistuje, čtení neprovádíme (zápis je stejně potřeba).
		bool needsWrite = true;
		if (existsCaseInsensitive)
		{
			// Jediným čtením souboru zjistíme současně shodu obsahu i přítomnost UTF-8 BOM.
			(bool sameContent, bool hasByteOrderMask) = await GetFileStateAsync(filename, content, cancellationToken);
			needsWrite = !sameContent || !hasByteOrderMask;
		}

		if (needsWrite && ((overwriteBehavior == OverwriteBehavior.OverwriteWhenFileAlreadyExists) || !existsCaseInsensitive))
		{
			if (!String.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			await File.WriteAllTextAsync(filename, content, Encoding.UTF8, cancellationToken);
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
	/// Vrací skutečný název souboru na disku (jak je fyzicky uložen, s ohledem na velikost písmen) ve složce <paramref name="directory"/>,
	/// který odpovídá <paramref name="filename"/> bez ohledu na velikost písmen. Pokud takový soubor neexistuje, vrací <c>null</c>.
	/// Case sensitivita názvu složky se neřeší.
	/// </summary>
	private static string GetFilenameOnDisk(string directory, string filename)
	{
		if (String.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			return null;
		}

		return new DirectoryInfo(directory)
			.EnumerateFiles(filename)
			.Select(file => file.Name)
			.FirstOrDefault(name => String.Equals(name, filename, StringComparison.CurrentCultureIgnoreCase));
	}

	/// <summary>
	/// Jedním čtením souboru zjistí, zda se jeho obsah shoduje s <paramref name="content"/> (případný úvodní UTF-8 BOM se ignoruje, stejně jako to dělá File.ReadAllText)
	/// a zda soubor obsahuje UTF-8 byte order mask.
	/// </summary>
	private static async Task<(bool SameContent, bool HasByteOrderMask)> GetFileStateAsync(string filename, string content, CancellationToken cancellationToken)
	{
		byte[] fileBytes = await File.ReadAllBytesAsync(filename, cancellationToken);

		bool hasByteOrderMask = (fileBytes.Length >= Utf8ByteOrderMask.Length)
			&& fileBytes.AsSpan(0, Utf8ByteOrderMask.Length).SequenceEqual(Utf8ByteOrderMask);

		int contentOffset = hasByteOrderMask ? Utf8ByteOrderMask.Length : 0;
		string fileContent = Encoding.UTF8.GetString(fileBytes, contentOffset, fileBytes.Length - contentOffset);

		return (String.Equals(fileContent, content, StringComparison.Ordinal), hasByteOrderMask);
	}
}
