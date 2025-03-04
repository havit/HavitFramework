
namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IRelicsCleaner
{
	Task CleanRelicsAsync(CancellationToken cancellationToken);
}