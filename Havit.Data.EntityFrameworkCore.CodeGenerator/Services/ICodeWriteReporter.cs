
namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface ICodeWriteReporter
{
	List<string> GetWrittenFiles();
	void ReportWriteFile(string filename);
}