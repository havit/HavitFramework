
namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IDataLayerGenerator
{
	Task GenerateAsync(CancellationToken cancellationToken);
}