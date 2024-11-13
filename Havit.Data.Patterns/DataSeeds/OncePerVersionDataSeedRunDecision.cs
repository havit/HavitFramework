using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Rozhoduje, že k spuštění seedování dat dojde jen jednou pro každou verzi aplikace.
/// Verze aplikace je získávána z FileVersion a data posledního zápisu assembly.
/// </summary>
public class OncePerVersionDataSeedRunDecision : IDataSeedRunDecision
{
	private readonly IDataSeedRunDecisionStatePersister _dataSeedRunDecisionStatePersister;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public OncePerVersionDataSeedRunDecision(IDataSeedRunDecisionStatePersister dataSeedRunDecisionStatePersister)
	{
		this._dataSeedRunDecisionStatePersister = dataSeedRunDecisionStatePersister;
	}

	/// <summary>
	/// Vrací aktuální stav dle assembly.
	/// </summary>
	protected internal string GetState(List<Type> dataSeedTypes)
	{
		return String.Join(",", dataSeedTypes.Select(item => item.Assembly).Distinct().OrderBy(item => item.FullName).Select(assembly =>
			String.Join("|", assembly.GetName().Name, System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion, new System.IO.FileInfo(assembly.Location).LastWriteTimeUtc.ToString("O"))));
	}

	/// <summary>
	/// Vrací true, pokud persister obsahuje jinou hodnotu než aktuální stav.
	/// </summary>
	public bool ShouldSeedData(IDataSeedProfile profile, List<Type> dataSeedTypes)
	{
		return GetState(dataSeedTypes) != _dataSeedRunDecisionStatePersister.ReadCurrentState(profile.ProfileName);
	}

	/// <summary>
	/// Nastaví do persisteru aktuální stav.
	/// </summary>
	public void SeedDataCompleted(IDataSeedProfile profile, List<Type> dataSeedTypes)
	{
		_dataSeedRunDecisionStatePersister.WriteCurrentState(profile.ProfileName, GetState(dataSeedTypes));
	}
}
