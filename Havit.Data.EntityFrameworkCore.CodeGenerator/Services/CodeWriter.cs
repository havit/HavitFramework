using System.Text;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

/// <summary>
/// Třída CodeWriter zajišťuje generování kódu. Zajišťuje odsazení, v budoucnu může zajišťovat ochranu před maximální délkou řádky kódu, apod.
/// </summary>
public class CodeWriter
{
	private readonly IProject project;

	public CodeWriter(IProject project)
	{
		this.project = project;
	}

	/// <summary>
	/// Zapíšeme obsah do souboru (jen tehdy, pokud se neliší od současného obsahu souboru).
	/// </summary>
	public Task SaveAsync(string filename, string content, bool canOverwriteExistingFile = true, CancellationToken cancellationToken = default)
	{
		// TODO Vyměnit za asynchroní implementaci
		Save(filename, content, canOverwriteExistingFile);
		return Task.CompletedTask;
	}

	public void Save(string filename, string content, bool canOverwriteExistingFile = true)
	{
		if (!this.AlreadyExistsTheSame(filename, content) || !this.HasByteOrderMask(filename))
		{
			string directory = Path.GetDirectoryName(filename);
			if (!String.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			bool exists = File.Exists(filename);
			if (canOverwriteExistingFile || !exists)
			{
				File.WriteAllText(filename, content, Encoding.UTF8);
			}
		}
	}

	/// <summary>
	/// Vrací true, pokud již existuje soubor se stejným jménem a obsahem.
	/// </summary>
	public bool AlreadyExistsTheSame(string filename, string content)
	{
		return (File.Exists(filename) && (File.ReadAllText(filename) == content));
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
