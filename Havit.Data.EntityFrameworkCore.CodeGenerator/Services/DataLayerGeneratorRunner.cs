namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public class DataLayerGeneratorRunner(IEnumerable<IDataLayerGenerator> _dataLayerGenerators)
	: IDataLayerGeneratorRunner
{
	public async Task RunAsync(CancellationToken cancellationToken = default)
	{
		Console.WriteLine($"Generating code...");
		await Parallel.ForEachAsync(_dataLayerGenerators, async (dataLayerGenerator, _) => await dataLayerGenerator.GenerateAsync(cancellationToken));
	}
}