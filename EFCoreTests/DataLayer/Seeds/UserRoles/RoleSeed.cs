using Havit.Data.Patterns.DataSeeds;
using Havit.EFCoreTests.Model;

namespace Havit.EFCoreTests.DataLayer.Seeds.UserRoles;

public class RoleSeed : DataSeed<UserRolesProfile>
{
	public override async Task SeedDataAsync(CancellationToken cancellationToken)
	{
		var data = new Role[]
		{
			new Role { Id = 1, Name = "Role 1" },
			new Role { Id = 2, Name = "Role 2" },
			new Role { Id = 3, Name = "Role 3" },
			new Role { Id = 4, Name = "Role 4" },
			new Role { Id = 5, Name = "Role 5" }
		};
		await SeedAsync(For(data).PairBy(item => item.Id), cancellationToken);
	}
}
