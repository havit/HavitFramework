using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;
using Havit.Diagnostics.Contracts;
using Havit.Services;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Předpis služby pro provedení seedování dat.
	/// </summary>
	public class DataSeedRunner : IDataSeedRunner
	{
		private readonly List<IDataSeed> dataSeeds;
		private readonly IDataSeedRunDecision dataSeedRunDecision;
		private readonly IServiceFactory<IDataSeedPersister> dataSeedPersisterFactory;

	    /// <summary>
	    /// Konstruktor.
	    /// </summary>
	    /// <param name="dataSeeds">Předpisy seedování objektů.</param>
	    /// <param name="dataSeedRunDecision">Služba vracející, zda se má dataseed spustit. Lze takto spouštění potlačit (např. pokud již bylo spuštěno).</param>
	    /// <param name="dataSeedPersister">Persister seedovaných objektů.</param>
	    public DataSeedRunner(IEnumerable<IDataSeed> dataSeeds, IDataSeedRunDecision dataSeedRunDecision, IServiceFactory<IDataSeedPersister> dataSeedPersisterFactory)
	    {
	        Contract.Requires(dataSeeds != null);
	        Contract.Requires(dataSeedRunDecision != null);
	        Contract.Requires(dataSeedPersisterFactory != null);

	        this.dataSeeds = dataSeeds.ToList();

	        if (this.dataSeeds.Select(item => item.GetType()).Distinct().Count() != this.dataSeeds.Count())
	        {
	            throw new ArgumentException("Contains dataseed type duplicity.", nameof(dataSeeds));
	        }

	        this.dataSeedRunDecision = dataSeedRunDecision;
	        this.dataSeedPersisterFactory = dataSeedPersisterFactory;
	    }

	    /// <summary>
		/// Provede seedování dat daného profilu.
		/// </summary>
		public void SeedData<TDataSeedProfile>(bool forceRun = false)
            where TDataSeedProfile : IDataSeedProfile, new()
	    {
            SeedData(typeof(TDataSeedProfile), forceRun);
		}

        /// <summary>
        /// Provede seedování dat daného profilu.
        /// </summary>
	    public void SeedData(Type dataSeedProfileType, bool forceRun = false)
	    {
	        SeedProfileWithPrequisites(dataSeedProfileType, forceRun, new Stack<Type>(), new List<Type>());
	    }

        /// <summary>
        /// Provede seedování profilu s prerequisitami. Řeší detekci cyklů závislostí, atp.
        /// </summary>
        private void SeedProfileWithPrequisites(Type profileType, bool forceRun, Stack<Type> profileTypesStack, List<Type> completedProfileTypes)
	    {
            // Already completed
	        if (completedProfileTypes.Contains(profileType))
	        {
	            return;
	        }

            // Cycle?
            if (profileTypesStack.Contains(profileType))
	        {
	            List<Type> cycle = profileTypesStack.ToList().SkipWhile(type => type != profileType).ToList();
	            cycle.Add(profileType);
	            string cycleMessage = String.Join(" -> ", cycle.Select(type => type.Name));

	            throw new InvalidOperationException($"DataSeed profiles contains a cycle ({cycleMessage}).");
	        }

	        profileTypesStack.Push(profileType); // cycle detection

	        IDataSeedProfile profile = Activator.CreateInstance(profileType) as IDataSeedProfile;
	        if (profile == null)
	        {
	            throw new InvalidOperationException($"Profile type {profileType.FullName} does not implement IDataSeedProfile interface.");
	        }

            foreach (Type prerequisiteProfileType in profile.GetPrerequisiteProfiles())
            {
	            SeedProfileWithPrequisites(prerequisiteProfileType, forceRun, profileTypesStack, completedProfileTypes);
	        }

	        profileTypesStack.Pop(); // cycle detection

            SeedProfile(profile, profileType, forceRun);

	        completedProfileTypes.Add(profileType);
        }

        /// <summary>
        /// Provede seedování jednoho profilu, již bez prerequisite.
        /// Zda se má seedování profilu skutečně spustit rozhoduje dataSeedRunDecision.
        /// </summary>
	    private void SeedProfile(IDataSeedProfile profile, Type profileType, bool forceRun)
	    {
	        List<IDataSeed> dataSeedsInProfile = dataSeeds.Where(item => item.ProfileType == profileType).ToList();
	        List<Type> dataSeedsInProfileTypes = dataSeedsInProfile.Select(item => item.GetType()).ToList();

            if (forceRun || dataSeedRunDecision.ShouldSeedData(profile, dataSeedsInProfileTypes))
	        {
	            // seed profile
	            Dictionary<Type, IDataSeed> dataSeedsInProfileByType = dataSeedsInProfile.ToDictionary(item => item.GetType(), item => item);
	            List<IDataSeed> completedDataSeeds = new List<IDataSeed>();

	            Stack<IDataSeed> dataSeedsStack = new Stack<IDataSeed>();
	            foreach (IDataSeed dataSeed in dataSeedsInProfile)
	            {
	                SeedService(dataSeed, dataSeedsStack, dataSeedsInProfileByType, completedDataSeeds);
	            }

	            dataSeedRunDecision.SeedDataCompleted(profile, dataSeedsInProfileTypes);
	        }
	    }

        /// <summary>
        /// Provede seedování jednoho předpisu. V této metodě dochází zejména k vyhodnocení závislostí předpisů a detekci cyklů závislostí.
        /// </summary>
        /// <param name="dataSeed">Předpis k seedování.</param>
        /// <param name="stack">Zásobník pro detekci cyklů závislostí.</param>
        /// <param name="dataSeedsInProfileByType">Index dataseedů dle typu pro dohledávání závislosí. Obsahuje instance dataseedů v aktuálně seedovaném profilu.</param>
        /// <param name="completedDataSeedsInProfile">Seznam již proběhlých dataseedů v daném profilu. Pro neopakování dataseedů, které jsou jako závislosti</param>
        private void SeedService(IDataSeed dataSeed, Stack<IDataSeed> stack, Dictionary<Type, IDataSeed> dataSeedsInProfileByType, List<IDataSeed> completedDataSeedsInProfile)
		{
			// Already completed?
			if (completedDataSeedsInProfile.Contains(dataSeed))
			{
				return;
			}

			// Cycle?
			if (stack.Contains(dataSeed))
			{
				List<IDataSeed> cycle = stack.ToList().SkipWhile(type => type != dataSeed).ToList();
				cycle.Add(dataSeed);
				string cycleMessage = String.Join(" -> ", cycle.Select(type => type.GetType().Name));

				throw new InvalidOperationException(String.Format("DataSeed prerequisites contains a cycle ({0}).", cycleMessage));
			}

			// Prerequisites
			stack.Push(dataSeed); // cycle detection

			IEnumerable<Type> prerequisiteTypes = dataSeed.GetPrerequisiteDataSeeds();
			if (prerequisiteTypes != null)
			{
				foreach (Type prerequisiteType in prerequisiteTypes)
				{
					IDataSeed prerequisitedDbSeed;
					if (!dataSeedsInProfileByType.TryGetValue(prerequisiteType, out prerequisitedDbSeed)) // neumožňujeme jako závislost použít předpis seedování dat z jiného profilu
					{
						throw new InvalidOperationException(String.Format("Prerequisite {1} for data seed {0} was not found.", dataSeed.GetType().Name, prerequisiteType.Name));
					}
					SeedService(prerequisitedDbSeed, stack, dataSeedsInProfileByType, completedDataSeedsInProfile);
				}
			}

			stack.Pop(); // cycle detection

			// Seed
			Seed(dataSeed);

			completedDataSeedsInProfile.Add(dataSeed);
		}

		/// <summary>
		/// Vlastní provedení seedování dat (s persistencí).
		/// </summary>
		/// <param name="dataSeed">Předpis seedování dat.</param>
		private void Seed(IDataSeed dataSeed)
		{
			dataSeedPersisterFactory.ExecuteAction(dataSeedPersister => dataSeed.SeedData(dataSeedPersister));
		}

		/// <summary>
		/// Provede seedování dat.
		/// </summary>
		void IDataSeedRunner.SeedData<TDataSeedProfile>()            
		{
			this.SeedData<TDataSeedProfile>();
		}

	    /// <summary>
	    /// Provede seedování dat.
	    /// </summary>
	    void IDataSeedRunner.SeedData(Type dataSeedProfileType)
	    {
	        this.SeedData(dataSeedProfileType);
	    }

    }
}
