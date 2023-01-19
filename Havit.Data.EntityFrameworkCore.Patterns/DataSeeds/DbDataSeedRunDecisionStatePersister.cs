using System.Linq;
using Havit.Data.EntityFrameworkCore.Model;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;
using Havit.Data.Patterns.DataSeeds;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds
{
	/// <summary>
	/// Spravuje stav pro implementace DataSeedRunDecision.
	/// Ukládá stav do databáze jako entitu DataSeed (tabulka __DataSeed). Používá záznam s Id = 1.
	/// </summary>
	public class DbDataSeedRunDecisionStatePersister : IDataSeedRunDecisionStatePersister
	{
		private readonly IDbContextFactory dbContextFactory;
		private readonly IDbDataSeedTransactionContext dbDataSeedTransactionContext;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbDataSeedRunDecisionStatePersister(IDbContextFactory dbContextFactory, IDbDataSeedTransactionContext dbDataSeedTransactionContext)
		{
			this.dbContextFactory = dbContextFactory;
			this.dbDataSeedTransactionContext = dbDataSeedTransactionContext;
		}

		/// <summary>
		/// Přečte aktuální stav.
		/// Není-li dosud evidován, vrací null.
		/// </summary>
		/// <param name="profileName">Název profilu, jehož stav je čten.</param>
		/// <returns>Aktuální stav</returns>
		/// <remarks>
		/// Stav je držen ve třídě DataSeed (tabulka __DataSeed) v záznamu s Id = 1.
		/// </remarks>
		public string ReadCurrentState(string profileName)
		{
			using (var dbContext = dbContextFactory.CreateDbContext())
			{
				if (dbDataSeedTransactionContext.CurrentTransaction != null)
				{
					dbDataSeedTransactionContext.ApplyCurrentTransactionTo(dbContext);
				}

				DataSeedVersion dataSeedVersion = GetDataSeedVersion(dbContext, profileName);
				return dataSeedVersion?.Version;
			}
		}

		/// <summary>
		/// Zapíše aktuální stav do databáze (vč. provedení dbContext.SaveChanges).
		/// </summary>
		/// <param name="profileName">Název profilu, ke kterému je zapisován stav.</param>
		/// <param name="currentState">Aktuální stav k zapsání.</param>
		/// <remarks>
		/// Stav je držen ve třídě DataSeed (tabulka __DataSeed) v záznamu s Id = 1.
		/// </remarks>
		public void WriteCurrentState(string profileName, string currentState)
		{
			using (var dbContext = dbContextFactory.CreateDbContext())
			{
				if (dbDataSeedTransactionContext.CurrentTransaction != null)
				{
					dbDataSeedTransactionContext.ApplyCurrentTransactionTo(dbContext);
				}

				DataSeedVersion dataSeedVersion = GetDataSeedVersion(dbContext, profileName);
				if (dataSeedVersion == null)
				{
					dataSeedVersion = new DataSeedVersion { ProfileName = profileName };
					dbContext.Set<DataSeedVersion>().Add(dataSeedVersion);
				}
				dataSeedVersion.Version = currentState;
				dbContext.SaveChanges();
			}
		}

		private DataSeedVersion GetDataSeedVersion(IDbContext dbContext, string profileName)
		{
			IDbSet<DataSeedVersion> dbSet = dbContext.Set<DataSeedVersion>();
			return dbSet.FindTracked(profileName) ?? dbSet.AsQueryable().SingleOrDefault(dataSeedVersion => dataSeedVersion.ProfileName == profileName);
		}
	}
}
