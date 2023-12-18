namespace Havit.Business.BusinessLayerGenerator.Settings;

public static class GeneratorStrategyExtensions
{
	public static bool IsEntityFrameworkGeneratedDatabaseStrategy(this GeneratorStrategy generatorStrategy)
	{
		return (generatorStrategy == GeneratorStrategy.HavitCodeFirst) || (generatorStrategy == GeneratorStrategy.HavitEFCoreCodeFirst);
	}

	public static bool IsAnyHavitStrategy(this GeneratorStrategy generatorStrategy)
	{
		return (generatorStrategy == GeneratorStrategy.Havit) || (generatorStrategy == GeneratorStrategy.HavitCodeFirst) || (generatorStrategy == GeneratorStrategy.HavitEFCoreCodeFirst);
	}

}
