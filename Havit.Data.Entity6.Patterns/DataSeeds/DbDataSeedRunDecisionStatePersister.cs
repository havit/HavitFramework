﻿using Havit.Data.Patterns.DataSeeds;

namespace Havit.Data.Entity.Patterns.DataSeeds;

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
	/// <param name="profileName">Název profilu, jehož stav je čten.</param>
	/// <returns>Aktuální stav</returns>
	/// <remarks>
	/// Stav je držen ve třídě DataSeed (tabulka __DataSeed) v záznamu s Id = 1.
	/// </remarks>
	public string ReadCurrentState(string profileName)
	{
		Model.DataSeedVersion dataSeedVersion = dbContext.Set<Model.DataSeedVersion>().SingleOrDefault(item => item.ProfileName == profileName);
		return dataSeedVersion?.Version;
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
		Model.DataSeedVersion dataSeedVersion = dbContext.Set<Model.DataSeedVersion>().SingleOrDefault(item => item.ProfileName == profileName);
		if (dataSeedVersion == null)
		{
			dataSeedVersion = new Model.DataSeedVersion { ProfileName = profileName };
			dbContext.Set<Model.DataSeedVersion>().Add(dataSeedVersion);
		}
		dataSeedVersion.Version = currentState;
		dbContext.SaveChanges();
	}
}
