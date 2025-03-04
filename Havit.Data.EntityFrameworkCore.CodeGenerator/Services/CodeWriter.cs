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
		if (!(await AlreadyExistsTheSameAsync(filename, content, cancellationToken)) || !HasByteOrderMask(filename))
		{
			string directory = Path.GetDirectoryName(filename);
			if (!String.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			bool exists = File.Exists(filename);
			if ((overwriteBahavior == OverwriteBahavior.OverwriteWhenFileAlreadyExists) || !exists)
			{
				await File.WriteAllTextAsync(filename, content, Encoding.UTF8, cancellationToken);
			}
		}
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
}
