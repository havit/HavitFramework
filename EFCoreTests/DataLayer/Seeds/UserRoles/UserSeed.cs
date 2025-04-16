using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.Repositories;
using Havit.EFCoreTests.DataLayer.Repositories;
using Havit.EFCoreTests.Model;

namespace Havit.EFCoreTests.DataLayer.Seeds.UserRoles;

public class UserSeed(IRoleRepository _roleRepository) : DataSeed<UserRolesProfile>
{
	public override async Task SeedDataAsync(CancellationToken cancellationToken)
	{
		var data = new User[]
		{
			new User { Id = new Guid("e0ffc5d7-aba7-4ace-a2ca-cbd264d370c6"), Username = "User 1" },
			new User { Id = new Guid("825a83a3-eef0-406c-937f-a1b8ea2e22b6"), Username = "User 2" },
			new User { Id = new Guid("9be0b789-94f1-4122-b634-bb183efe55f9"), Username = "User 3" },
			new User { Id = new Guid("5cae6436-1217-4f55-abb0-d693e59252d1"), Username = "User 4" },
			new User { Id = new Guid("3216d2d9-388e-4bc3-b114-ebd207558459"), Username = "User 5" },
		};

		await SeedAsync(For(data).PairBy(item => item.Id).BeforeSave(beforeSaveArgs =>
		{
			//if (beforeSaveArgs.IsNew)
			{
				beforeSaveArgs.PersistedEntity.PrimaryRoles = new List<Role>();
				beforeSaveArgs.PersistedEntity.SecondaryRoles = new List<Role>();
				beforeSaveArgs.PersistedEntity.AdditionalRoles = new List<Role>();

				beforeSaveArgs.PersistedEntity.PrimaryRoles.Add(_roleRepository.GetObject(1));
				beforeSaveArgs.PersistedEntity.SecondaryRoles.Add(_roleRepository.GetObject(2));
				beforeSaveArgs.PersistedEntity.AdditionalRoles.Add(_roleRepository.GetObject(3));
			}
		}), cancellationToken);
	}

	public override IEnumerable<Type> GetPrerequisiteDataSeeds()
	{
		yield return typeof(RoleSeed);
	}
}
