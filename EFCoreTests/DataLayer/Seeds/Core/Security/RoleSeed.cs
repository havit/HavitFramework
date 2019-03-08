using Havit.Data.Patterns.DataSeeds;
using Havit.EFCoreTests.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.EFCoreTests.DataLayer.Seeds.Core
{
	public class RoleSeed : DataSeed<CoreProfile>
	{
		public override void SeedData()
		{
            Role[] roles = Enumerable.Range(1, 1000).Select(i => new Role { Id = i, Name = "Role " + i }).ToArray();
            Seed(For(roles).PairBy(role => role.Id));
		}
	}

}
