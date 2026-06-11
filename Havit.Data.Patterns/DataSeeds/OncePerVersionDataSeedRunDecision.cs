using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Rozhoduje, že k spuštění seedování dat dojde jen jednou pro každou verzi aplikace.
/// Verze aplikace je primárně získávána z FileVersion a data posledního zápisu assembly; není-li dostupné <see cref="System.Reflection.Assembly.Location"/>
/// (typicky single-file publish), použije se jako fallback MVID (Module Version Id) assembly.
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
	/// Vrací aktuální stav assembly obsahujících data seedy.
	/// </summary>
	/// <remarks>
	/// Pro každou assembly se stav skládá z jejího názvu a:
	/// <list type="bullet">
	///   <item><description>je-li dostupné <see cref="System.Reflection.Assembly.Location"/>, z FileVersion (ProductVersion) a času posledního zápisu souboru assembly;</description></item>
	///   <item><description>není-li <see cref="System.Reflection.Assembly.Location"/> dostupné (typicky single-file publish, kde vrací prázdný řetězec), z MVID (Module Version Id) assembly.</description></item>
	/// </list>
	/// MVID je GUID, který do assembly vkládá kompilátor; u deterministických buildů (výchozí nastavení .NET SDK) jde fakticky o hash vstupů kompilace,
	/// takže se při rebuildu beze změny zdrojů nezmění (a seedování se v takovém případě znovu nespustí).
	/// </remarks>
	protected internal string GetState(List<Type> dataSeedTypes)
	{
		return String.Join(",", dataSeedTypes.Select(item => item.Assembly).Distinct().OrderBy(item => item.FullName).Select(GetAssemblyState));
	}

	private static string GetAssemblyState(System.Reflection.Assembly assembly)
	{
		string location = assembly.Location;
		if (!String.IsNullOrEmpty(location))
		{
			// Klasický deploy: identita + FileVersion + čas posledního zápisu souboru assembly.
			return String.Join("|", assembly.GetName().Name, System.Diagnostics.FileVersionInfo.GetVersionInfo(location).ProductVersion, new System.IO.FileInfo(location).LastWriteTimeUtc.ToString("O"));
		}
		else
		{
			// Single-file publish (Location je prázdné): identita + MVID assembly.
			return String.Join("|", assembly.GetName().Name, assembly.ManifestModule.ModuleVersionId.ToString());
		}
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
