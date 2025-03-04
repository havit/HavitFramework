namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

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
