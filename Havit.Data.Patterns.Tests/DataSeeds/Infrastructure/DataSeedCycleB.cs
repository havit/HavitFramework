using System;
using System.Collections.Generic;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;

internal class DataSeedCycleB : DataSeed<DefaultProfile>
    {
	public override void SeedData()
	{
		// NOOP
	}

	public override IEnumerable<Type> GetPrerequisiteDataSeeds()
	{
		yield return typeof(DataSeedCycleB);
	}
}
