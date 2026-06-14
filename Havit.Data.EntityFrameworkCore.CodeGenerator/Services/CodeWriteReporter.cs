namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

/// <summary>
/// Eviduje soubory, které byly v rámci běhu vygenerovány.
/// Pozor: jde o soubory, které <em>měly být</em> vygenerovány (tj. byly nahlášeny CodeWriterem), ne nutně o soubory, které byly fyzicky zapsány na disk.
/// CodeWriter totiž nahlásí i soubor, jehož obsah se nezměnil (a tedy se fyzicky nepřepisuje). Je to záměr: RelicsCleaner z tohoto seznamu pozná,
/// které existující soubory jsou stále aktuální, a nesmí je tedy smazat jako pozůstatek.
/// </summary>
public class CodeWriteReporter : ICodeWriteReporter
{
	private List<string> writtenFiles = new List<string>();

	public void ReportWriteFile(string filename)
	{
		lock (writtenFiles)
		{
			writtenFiles.Add(filename);
		}
	}

	public List<string> GetWrittenFiles()
	{
		lock (writtenFiles)
		{
			return writtenFiles.ToList(); /* copy */
		}
	}
}
