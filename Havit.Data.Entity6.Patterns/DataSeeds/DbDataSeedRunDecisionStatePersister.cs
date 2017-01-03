using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Model;
using Havit.Data.Patterns.DataSeeds;

namespace Havit.Data.Entity.Patterns.DataSeeds
{
	/// <summary>
	/// Spravuje stav pro implementace DataSeedRunDecision.
	/// Ukládá stav do databáze jako entitu DataSeed (tabulka __DataSeed). Používá záznam s Id = 1.
	/// </summary>
	public class DbDataSeedRunDecisionStatePersister : IDataSeedRunDecisionStatePersister
	{
		private readonly IDbContext dbContext;

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbDataSeedRunDecisionStatePersister(IDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		/// <summary>
		/// Přečte aktuální stav.
		/// Není-li dosud evidován, vrací null.
		/// </summary>
		/// <returns>Aktuální stav</returns>
		/// <remarks>
		/// Stav je držen ve třídě DataSeed (tabulka __DataSeed) v záznamu s Id = 1.
		/// </remarks>
		public string ReadCurrentState()
		{
			Model.DataSeed dataSeed = dbContext.Set<Model.DataSeed>().SingleOrDefault(item => item.Id == 1);
			return dataSeed?.Version;
		}

		/// <summary>
		/// Zapíše aktuální stav do databáze (vč. provedení dbContext.SaveChanges).
		/// </summary>
		/// <param name="currentState">Aktuální stav k zapsání.</param>
		/// <remarks>
		/// Stav je držen ve třídě DataSeed (tabulka __DataSeed) v záznamu s Id = 1.
		/// </remarks>
		public void WriteCurrentState(string currentState)
		{
			Model.DataSeed dataSeed = dbContext.Set<Model.DataSeed>().SingleOrDefault(item => item.Id == 1);
			if (dataSeed == null)
			{
				dataSeed = new Model.DataSeed { Id = 1 };
				dbContext.Set<Model.DataSeed>().Add(dataSeed);
			}
			dataSeed.Version = currentState;
			dbContext.SaveChanges();
		}
	}
}
