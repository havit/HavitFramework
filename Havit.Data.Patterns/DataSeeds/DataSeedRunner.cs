using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Zajišťuje spuštění seedování databáze.
/// </summary>
public class DataSeedRunner : IDataSeedRunner
{
	private readonly List<IDataSeed> _dataSeeds;
	private readonly IDataSeedRunDecision _dataSeedRunDecision;
	private readonly IDataSeedPersisterFactory _dataSeedPersisterFactory;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	/// <param name="dataSeeds">Předpisy seedování objektů.</param>
	/// <param name="dataSeedRunDecision">Služba vracející, zda se má dataseed spustit. Lze takto spouštění potlačit (např. pokud již bylo spuštěno).</param>
	/// <param name="dataSeedPersisterFactory">Persister seedovaných objektů (factory).</param>
	/// <remarks>
	/// Revize použití s ohledem na https://github.com/volosoft/castle-windsor-ms-adapter/issues/32:
	/// DbDataSeedPersister je registrován jako transientní se závislostí DbContext, který je v tomto případě rovněž transientní.
	/// Proto se v tomto případě factory IServiceFactory&lt;IDataSeedPersister&gt; popsaná issue netýká.
	/// </remarks>
	public DataSeedRunner(IEnumerable<IDataSeed> dataSeeds, IDataSeedRunDecision dataSeedRunDecision, IDataSeedPersisterFactory dataSeedPersisterFactory)
	{
		Contract.Requires<ArgumentNullException>(dataSeeds != null, nameof(dataSeeds));
		Contract.Requires<ArgumentNullException>(dataSeedRunDecision != null, nameof(dataSeedRunDecision));
		Contract.Requires<ArgumentNullException>(dataSeedPersisterFactory != null, nameof(dataSeedPersisterFactory));

		this._dataSeeds = dataSeeds.ToList();

		if (this._dataSeeds.Select(item => item.GetType()).Distinct().Count() != this._dataSeeds.Count())
		{
			throw new ArgumentException("Contains dataseed type duplicity.", nameof(dataSeeds));
		}

		this._dataSeedRunDecision = dataSeedRunDecision;
		this._dataSeedPersisterFactory = dataSeedPersisterFactory;
	}

	/// <summary>
	/// Provede seedování dat daného profilu.
	/// </summary>
	public virtual void SeedData<TDataSeedProfile>(bool forceRun = false)
		where TDataSeedProfile : IDataSeedProfile, new()
	{
		SeedData(typeof(TDataSeedProfile), forceRun);
	}

	/// <summary>
	/// Provede seedování dat daného profilu.
	/// </summary>
	public virtual async Task SeedDataAsync<TDataSeedProfile>(bool forceRun = false, CancellationToken cancellationToken = default)
		where TDataSeedProfile : IDataSeedProfile, new()
	{
		await SeedDataAsync(typeof(TDataSeedProfile), forceRun, cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Provede seedování dat daného profilu.
	/// </summary>
	public virtual void SeedData(Type dataSeedProfileType, bool forceRun = false)
	{
		SeedProfileWithPrerequisites(dataSeedProfileType, forceRun, new Stack<Type>(), new HashSet<Type>());
	}

	/// <summary>
	/// Provede seedování dat daného profilu.
	/// </summary>
	public virtual async Task SeedDataAsync(Type dataSeedProfileType, bool forceRun = false, CancellationToken cancellationToken = default)
	{
		await SeedProfileWithPrerequisitesAsync(dataSeedProfileType, forceRun, new Stack<Type>(), new HashSet<Type>(), cancellationToken).ConfigureAwait(false);
	}

	/// <summary>
	/// Provede seedování profilu s prerequisitami. Řeší detekci cyklů závislostí, atp.
	/// </summary>
	private void SeedProfileWithPrerequisites(Type profileType, bool forceRun, Stack<Type> profileTypesStack, HashSet<Type> completedProfileTypes)
	{
		// Already completed
		if (completedProfileTypes.Contains(profileType))
		{
			return;
		}

		SeedProfileWithPrerequisites_DetectProfileCycle(profileType, profileTypesStack);

		profileTypesStack.Push(profileType); // cycle detection

		IDataSeedProfile profile = Activator.CreateInstance(profileType) as IDataSeedProfile;
		if (profile == null)
		{
			throw new InvalidOperationException($"Profile type {profileType.FullName} does not implement IDataSeedProfile interface.");
		}

		foreach (Type prerequisiteProfileType in profile.GetPrerequisiteProfiles())
		{
			SeedProfileWithPrerequisites(prerequisiteProfileType, forceRun, profileTypesStack, completedProfileTypes);
		}

		profileTypesStack.Pop(); // cycle detection

		SeedProfile(profile, profileType, forceRun);

		completedProfileTypes.Add(profileType);
	}

	/// <summary>
	/// Provede seedování profilu s prerequisitami. Řeší detekci cyklů závislostí, atp.
	/// </summary>
	private async Task SeedProfileWithPrerequisitesAsync(Type profileType, bool forceRun, Stack<Type> profileTypesStack, HashSet<Type> completedProfileTypes, CancellationToken cancellationToken)
	{
		// Already completed
		if (completedProfileTypes.Contains(profileType))
		{
			return;
		}

		// Cycle?
		SeedProfileWithPrerequisites_DetectProfileCycle(profileType, profileTypesStack);

		profileTypesStack.Push(profileType); // cycle detection

		IDataSeedProfile profile = Activator.CreateInstance(profileType) as IDataSeedProfile;
		if (profile == null)
		{
			throw new InvalidOperationException($"Profile type {profileType.FullName} does not implement IDataSeedProfile interface.");
		}

		foreach (Type prerequisiteProfileType in profile.GetPrerequisiteProfiles())
		{
			await SeedProfileWithPrerequisitesAsync(prerequisiteProfileType, forceRun, profileTypesStack, completedProfileTypes, cancellationToken).ConfigureAwait(false);
		}

		profileTypesStack.Pop(); // cycle detection

		await SeedProfileAsync(profile, profileType, forceRun, cancellationToken).ConfigureAwait(false);

		completedProfileTypes.Add(profileType);
	}

	/// <summary>
	/// Detekuje případný cyklus v profilech, na kterých je seed závislý.
	/// </summary>
	private static void SeedProfileWithPrerequisites_DetectProfileCycle(Type profileType, Stack<Type> profileTypesStack)
	{
		if (profileTypesStack.Contains(profileType))
		{
			// Stack se enumeruje od vrcholu (naposledy vložený) k dnu, proto bereme prvky až po kořen cyklu a obrátíme je do chronologického pořadí (kořen -> ... -> uzávěr).
			List<Type> cycle = profileTypesStack.TakeWhile(type => type != profileType).ToList();
			cycle.Add(profileType);
			cycle.Reverse();
			cycle.Add(profileType);
			string cycleMessage = String.Join(" -> ", cycle.Select(type => type.Name));

			throw new InvalidOperationException($"DataSeed profiles contains a cycle ({cycleMessage}).");
		}
	}

	/// <summary>
	/// Provede seedování jednoho profilu, již bez prerequisite.
	/// Zda se má seedování profilu skutečně spustit rozhoduje dataSeedRunDecision.
	/// </summary>
	private void SeedProfile(IDataSeedProfile profile, Type profileType, bool forceRun)
	{
		Dictionary<Type, IDataSeed> dataSeedsInProfileByType = _dataSeeds.Where(item => item.ProfileType == profileType).ToDictionary(item => item.GetType(), item => item);
		List<Type> dataSeedsInProfileTypes = dataSeedsInProfileByType.Keys.ToList();

		if (forceRun || _dataSeedRunDecision.ShouldSeedData(profile, dataSeedsInProfileTypes))
		{
			// seed profile
			HashSet<IDataSeed> completedDataSeeds = new HashSet<IDataSeed>();

			Stack<IDataSeed> dataSeedsStack = new Stack<IDataSeed>();
			// Seedy seřadíme deterministicky (dle názvu typu), aby pořadí nezáviselo na pořadí registrace v DI containeru a bylo reprodukovatelné.
			// Skutečné závislosti mezi seedy je nutné vyjádřit přes GetPrerequisiteDataSeeds (na to se toto řazení nespoléhá).
			foreach (IDataSeed dataSeed in dataSeedsInProfileByType.Values.OrderBy(dataSeed => dataSeed.GetType().FullName, StringComparer.Ordinal))
			{
				SeedService(dataSeed, dataSeedsStack, profile, dataSeedsInProfileByType, completedDataSeeds);
			}

			_dataSeedRunDecision.SeedDataCompleted(profile, dataSeedsInProfileTypes);
		}
	}

	/// <summary>
	/// Provede seedování jednoho profilu, již bez prerequisite.
	/// Zda se má seedování profilu skutečně spustit rozhoduje dataSeedRunDecision.
	/// </summary>
	private async Task SeedProfileAsync(IDataSeedProfile profile, Type profileType, bool forceRun, CancellationToken cancellationToken)
	{
		Dictionary<Type, IDataSeed> dataSeedsInProfileByType = _dataSeeds.Where(item => item.ProfileType == profileType).ToDictionary(item => item.GetType(), item => item);
		List<Type> dataSeedsInProfileTypes = dataSeedsInProfileByType.Keys.ToList();

		if (forceRun || await _dataSeedRunDecision.ShouldSeedDataAsync(profile, dataSeedsInProfileTypes, cancellationToken).ConfigureAwait(false))
		{
			// seed profile
			HashSet<IDataSeed> completedDataSeeds = new HashSet<IDataSeed>();

			Stack<IDataSeed> dataSeedsStack = new Stack<IDataSeed>();
			// Seedy seřadíme deterministicky (dle názvu typu), aby pořadí nezáviselo na pořadí registrace v DI containeru a bylo reprodukovatelné.
			// Skutečné závislosti mezi seedy je nutné vyjádřit přes GetPrerequisiteDataSeeds (na to se toto řazení nespoléhá).
			foreach (IDataSeed dataSeed in dataSeedsInProfileByType.Values.OrderBy(dataSeed => dataSeed.GetType().FullName, StringComparer.Ordinal))
			{
				await SeedServiceAsync(dataSeed, dataSeedsStack, profile, dataSeedsInProfileByType, completedDataSeeds, cancellationToken).ConfigureAwait(false);
			}

			await _dataSeedRunDecision.SeedDataCompletedAsync(profile, dataSeedsInProfileTypes, cancellationToken).ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Provede seedování jednoho předpisu. V této metodě dochází zejména k vyhodnocení závislostí předpisů a detekci cyklů závislostí.
	/// </summary>
	/// <param name="dataSeed">Předpis k seedování.</param>
	/// <param name="stack">Zásobník pro detekci cyklů závislostí.</param>
	/// <param name="profile">Profil, který je seedován.</param>
	/// <param name="dataSeedsInProfileByType">Index dataseedů dle typu pro dohledávání závislostí. Obsahuje instance dataseedů v aktuálně seedovaném profilu.</param>
	/// <param name="completedDataSeedsInProfile">Seznam již proběhlých dataseedů v daném profilu. Pro neopakování dataseedů, které jsou jako závislosti</param>
	private void SeedService(IDataSeed dataSeed, Stack<IDataSeed> stack, IDataSeedProfile profile, Dictionary<Type, IDataSeed> dataSeedsInProfileByType, HashSet<IDataSeed> completedDataSeedsInProfile)
	{
		// Already completed?
		if (completedDataSeedsInProfile.Contains(dataSeed))
		{
			return;
		}

		// Cycle?
		SeedService_DetectServiceCycle(dataSeed, stack);

		// Prerequisites
		stack.Push(dataSeed); // cycle detection

		IEnumerable<Type> prerequisiteTypes = dataSeed.GetPrerequisiteDataSeeds();
		if (prerequisiteTypes != null)
		{
			foreach (Type prerequisiteType in prerequisiteTypes)
			{
				IDataSeed prerequisitedDbSeed = SeedService_GetPrerequisite(dataSeed, profile, dataSeedsInProfileByType, prerequisiteType);
				SeedService(prerequisitedDbSeed, stack, profile, dataSeedsInProfileByType, completedDataSeedsInProfile);
			}
		}

		stack.Pop(); // cycle detection

		// Seed
		Seed(dataSeed);

		completedDataSeedsInProfile.Add(dataSeed);
	}

	/// <summary>
	/// Provede seedování jednoho předpisu. V této metodě dochází zejména k vyhodnocení závislostí předpisů a detekci cyklů závislostí.
	/// </summary>
	/// <param name="dataSeed">Předpis k seedování.</param>
	/// <param name="stack">Zásobník pro detekci cyklů závislostí.</param>
	/// <param name="profile">Profil, který je seedován.</param>
	/// <param name="dataSeedsInProfileByType">Index dataseedů dle typu pro dohledávání závislostí. Obsahuje instance dataseedů v aktuálně seedovaném profilu.</param>
	/// <param name="completedDataSeedsInProfile">Seznam již proběhlých dataseedů v daném profilu. Pro neopakování dataseedů, které jsou jako závislosti</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	private async Task SeedServiceAsync(IDataSeed dataSeed, Stack<IDataSeed> stack, IDataSeedProfile profile, Dictionary<Type, IDataSeed> dataSeedsInProfileByType, HashSet<IDataSeed> completedDataSeedsInProfile, CancellationToken cancellationToken)
	{
		// Already completed?
		if (completedDataSeedsInProfile.Contains(dataSeed))
		{
			return;
		}

		// Cycle?
		SeedService_DetectServiceCycle(dataSeed, stack);

		// Prerequisites
		stack.Push(dataSeed); // cycle detection

		IEnumerable<Type> prerequisiteTypes = dataSeed.GetPrerequisiteDataSeeds();
		if (prerequisiteTypes != null)
		{
			foreach (Type prerequisiteType in prerequisiteTypes)
			{
				IDataSeed prerequisiteDbSeed = SeedService_GetPrerequisite(dataSeed, profile, dataSeedsInProfileByType, prerequisiteType);
				await SeedServiceAsync(prerequisiteDbSeed, stack, profile, dataSeedsInProfileByType, completedDataSeedsInProfile, cancellationToken).ConfigureAwait(false);
			}
		}

		stack.Pop(); // cycle detection

		// Seed
		await SeedAsync(dataSeed, cancellationToken).ConfigureAwait(false);

		completedDataSeedsInProfile.Add(dataSeed);
	}

	/// <summary>
	/// Detekuje případný cyklus v seedech, na kterých je tento seed závislý.
	/// </summary>
	private static void SeedService_DetectServiceCycle(IDataSeed dataSeed, Stack<IDataSeed> stack)
	{
		if (stack.Contains(dataSeed))
		{
			// Stack se enumeruje od vrcholu (naposledy vložený) k dnu, proto bereme prvky až po kořen cyklu a obrátíme je do chronologického pořadí (kořen -> ... -> uzávěr).
			List<IDataSeed> cycle = stack.TakeWhile(type => type != dataSeed).ToList();
			cycle.Add(dataSeed);
			cycle.Reverse();
			cycle.Add(dataSeed);
			string cycleMessage = String.Join(" -> ", cycle.Select(type => type.GetType().Name));

			throw new InvalidOperationException(String.Format("DataSeed prerequisites contains a cycle ({0}).", cycleMessage));
		}
	}

	/// <summary>
	/// Vyhledá závislý data seed, ev. vyhodí výjimku, pokud není nalezen.
	/// </summary>
	private static IDataSeed SeedService_GetPrerequisite(IDataSeed dataSeed, IDataSeedProfile profile, Dictionary<Type, IDataSeed> dataSeedsInProfileByType, Type prerequisiteType)
	{
		IDataSeed prerequisiteDbSeed;
		if (!dataSeedsInProfileByType.TryGetValue(prerequisiteType, out prerequisiteDbSeed)) // neumožňujeme jako závislost použít předpis seedování dat z jiného profilu
		{
			throw new InvalidOperationException(String.Format("Prerequisite {0} for data seed {1} was not found. Data seeds and prerequisites must be in the same profile ({2}).",
				prerequisiteType.Name,
				dataSeed.GetType().Name,
				profile.ProfileName));
		}

		return prerequisiteDbSeed;
	}

	/// <summary>
	/// Vlastní provedení seedování dat (s persistencí).
	/// </summary>
	/// <param name="dataSeed">Předpis seedování dat.</param>
	private void Seed(IDataSeed dataSeed)
	{
		IDataSeedPersister dataSeedPersister = _dataSeedPersisterFactory.CreateService();
		dataSeedPersister.AttachDataSeed(dataSeed);
		try
		{
			// Voláme obě metody (sync i async), protože potomek DataSeed<TProfile> může overridnout libovolnou z nich.
			// Výchozí implementace SeedDataAsync vrací Task.CompletedTask, takže pokud potomek overridnul jen SeedData, async volání je NOOP.
			dataSeed.SeedData(dataSeedPersister);
			var task = dataSeed.SeedDataAsync(dataSeedPersister, CancellationToken.None);
			if (!task.IsCompleted)
			{
				// Chybné použití (programátorská chyba k odhalení při ladění): synchronní runner byl použit na data seed,
				// jehož SeedDataAsync vrací nedokončený task. Task úmyslně nedokončujeme a ve finally se uvolní jeho persister,
				// takže opuštěný task by pracoval s persisterem uvolněným "pod rukama". To je akceptováno - v korektně napsané
				// aplikaci tato situace nikdy nenastane a výjimka slouží jen jako development-time guard (viz SeedAsyncFromSyncSeedDataException).
				throw new SeedAsyncFromSyncSeedDataException($"Async data seeds are supported only for async {nameof(DataSeedRunner)} methods. It means you need to run data seeding using {nameof(IDataSeedRunner)}.{nameof(IDataSeedRunner.SeedDataAsync)} method when {nameof(DataSeed<DefaultProfile>)} implements (overrides) {nameof(DataSeed<DefaultProfile>.SeedDataAsync)} method.");
			}
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
			task.GetAwaiter().GetResult(); // task je dokončen, jen propagace případných výjimek
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits
		}
		finally
		{
			_dataSeedPersisterFactory.ReleaseService(dataSeedPersister);
		}
	}

	/// <summary>
	/// Vlastní provedení seedování dat (s persistencí).
	/// </summary>
	/// <param name="dataSeed">Předpis seedování dat.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	private async Task SeedAsync(IDataSeed dataSeed, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();

		IDataSeedPersister dataSeedPersister = _dataSeedPersisterFactory.CreateService();
		dataSeedPersister.AttachDataSeed(dataSeed);
		try
		{
			dataSeed.SeedData(dataSeedPersister);
			await dataSeed.SeedDataAsync(dataSeedPersister, cancellationToken).ConfigureAwait(false);
		}
		finally
		{
			_dataSeedPersisterFactory.ReleaseService(dataSeedPersister);
		}
	}
}
