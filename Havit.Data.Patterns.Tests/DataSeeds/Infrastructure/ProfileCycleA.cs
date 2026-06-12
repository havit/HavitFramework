using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;

/// <summary>
/// Profil tvořící cyklus s <see cref="ProfileCycleB"/> (A -> B -> A) pro ověření detekce cyklu profilů.
/// </summary>
public class ProfileCycleA : DataSeedProfile
{
	public override IEnumerable<Type> GetPrerequisiteProfiles()
	{
		yield return typeof(ProfileCycleB);
	}
}
