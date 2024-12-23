namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public interface IDataLayerGeneratorRunner
{
	Task RunAsync(CancellationToken cancellationToken = default);
}