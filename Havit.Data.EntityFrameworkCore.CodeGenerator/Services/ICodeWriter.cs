
namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface ICodeWriter
{
	Task SaveAsync(string filename, string content, OverwriteBahavior overwriteBahavior, CancellationToken cancellationToken = default);
}