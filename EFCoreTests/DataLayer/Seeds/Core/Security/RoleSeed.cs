using Havit.Data.Patterns.DataSeeds;
using Havit.EFCoreTests.Model.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.EFCoreTests.DataLayer.Seeds.Core
{
	public class RoleSeed : DataSeed<CoreProfile>
	{
		public override void SeedData()
		{
			Seed(For(new Role { Id = 1, Name = "Administrator" }).PairBy(role => role.Id));
		}
	}

}
