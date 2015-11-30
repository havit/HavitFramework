using System;
using System.Collections.Generic;
using System.Linq;
using Havit.Data.Patterns.DataSeeds;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.Seeds
{
	public class DataSeedRunner : IDataSeedRunner
	{
		private readonly IDataSeedPersister dataSeedPersister;
		private readonly Dictionary<Type, IDataSeed> dataSeedDictionary;
		private List<IDataSeed> completedDataSeeds;

		public DataSeedRunner(IEnumerable<IDataSeed> dataSeeds, IDataSeedPersister dataSeedPersister)
		{
			Contract.Requires(dataSeeds != null);
			Contract.Requires(dataSeedPersister != null);

			this.dataSeedPersister = dataSeedPersister;
			this.dataSeedDictionary = dataSeeds.ToDictionary(item => item.GetType(), item => item);
		}

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

		private void Seed(IDataSeed dataSeed)
		{
			dataSeed.SeedData(dataSeedPersister);
		}

	}
}
