
namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface ICodeWriter
{
	Task SaveAsync(string filename, string content, OverwriteBehavior overwriteBehavior, CancellationToken cancellationToken = default);
}