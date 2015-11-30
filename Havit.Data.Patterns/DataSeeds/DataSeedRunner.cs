using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Patterns.DataSeeds;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Předpis služby pro provedení seedování dat.
	/// </summary>
	public class DataSeedRunner : IDataSeedRunner
	{
		private readonly IDataSeedPersister dataSeedPersister;
		private readonly Dictionary<Type, IDataSeed> dataSeedDictionary;
		private List<IDataSeed> completedDataSeeds;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="dataSeeds">Předpisy seedování objektů.</param>
		/// <param name="dataSeedPersister">Persister seedovaných objektů.</param>
		public DataSeedRunner(IEnumerable<IDataSeed> dataSeeds, IDataSeedPersister dataSeedPersister)
		{
			Contract.Requires(dataSeeds != null);
			Contract.Requires(dataSeedPersister != null);

			if (dataSeeds.Select(item => item.GetType()).Distinct().Count() != dataSeeds.Count())
			{
				throw new ArgumentException("Contains dataseed type duplicity.", nameof(dataSeeds));
			}

			this.dataSeedPersister = dataSeedPersister;
			this.dataSeedDictionary = dataSeeds.ToDictionary(item => item.GetType(), item => item);
		}

		/// <summary>
		/// Provede seedování dat dle předpisů a persistuje (uloží) je.
		/// </summary>
		public void SeedData()
		{
			completedDataSeeds = new List<IDataSeed>();
			Stack<IDataSeed> stack = new Stack<IDataSeed>();
			foreach (IDataSeed dataSeed in dataSeedDictionary.Values)
			{
				SeedService(dataSeed, stack);
			}
			completedDataSeeds = null;
		}

		/// <summary>
		/// Provede seedování jednoho předpisu. V této metodě dochází zejména k vyhodnocení závislostí předpisů a detekci cyklů závislostí.
		/// </summary>
		/// <param name="dataSeed">Předpis k seedování.</param>
		/// <param name="stack">Zásobník pro detekci cyklů závislostí.</param>
		private void SeedService(IDataSeed dataSeed, Stack<IDataSeed> stack)
		{
			// Already completed?
			if (completedDataSeeds.Contains(dataSeed))
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
				foreach (var prerequisiteType in prerequisiteTypes)
				{
					IDataSeed prerequisitedDbSeed;
					if (!dataSeedDictionary.TryGetValue(prerequisiteType, out prerequisitedDbSeed))
					{
						throw new InvalidOperationException(String.Format("Prerequisite {1} for data seed {0} was not found.", dataSeed.GetType().Name, prerequisiteType.Name));
					}
					SeedService(prerequisitedDbSeed, stack);
				}
			}

			stack.Pop(); // cycle detection

			// Seed
			Seed(dataSeed);

			completedDataSeeds.Add(dataSeed);
		}

		/// <summary>
		/// Vlastní provedení seedování dat (s persistencí).
		/// </summary>
		/// <param name="dataSeed">Předpis seedování dat.</param>
		private void Seed(IDataSeed dataSeed)
		{
			dataSeed.SeedData(dataSeedPersister);
		}

	}
}
