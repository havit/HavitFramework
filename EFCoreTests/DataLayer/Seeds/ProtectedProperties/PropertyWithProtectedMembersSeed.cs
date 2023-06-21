using Havit.Data.Patterns.DataSeeds;
using Havit.EFCoreTests.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.EFCoreTests.DataLayer.Seeds.ProtectedProperties;

public class PropertyWithProtectedMembersSeed : DataSeed<ProtectedPropertiesProfile>
{
	public override void SeedData()
	{
		PropertyWithProtectedMembers property = new PropertyWithProtectedMembers();
		property.Id = 1;
		property.SetProtectedSetterValue(nameof(PropertyWithProtectedMembers.ProtectedSetterValue));
		property.SetProtectedValue("ProtectedValue");

		Seed(For(property).PairBy(item => item.Id));
	}
}
