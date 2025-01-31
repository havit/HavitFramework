using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;

internal class DataSeedDependentOnItself : DataSeed<DefaultProfile>
{
	public override void SeedData()
	{
		// NOOP	
	}

	public override IEnumerable<Type> GetPrerequisiteDataSeeds()
	{
		yield return typeof(DataSeedDependentOnItself);
	}
}