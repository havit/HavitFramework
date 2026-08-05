using Havit.Data.EntityFrameworkCore.Model;
using Havit.Data.Patterns.DataSeeds;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;

/// <summary>
/// Spravuje stav pro implementace DataSeedRunDecision.
/// Ukládá stav do databáze jako entitu DataSeed (tabulka __DataSeed). Používá záznam s Id = 1.
/// </summary>
public class DbDataSeedRunDecisionStatePersister : IDataSeedRunDecisionStatePersister
{
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public DbDataSeedRunDecisionStatePersister(IDbContext dbContext)
	{
		_dbContext = dbContext;
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
		DataSeedVersion dataSeedVersion = GetDataSeedVersion(_dbContext, profileName);
		return dataSeedVersion?.Version;
	}

	/// <summary>
	/// Přečte aktuální stav.
	/// Není-li dosud evidován, vrací null.
	/// </summary>
	/// <param name="profileName">Název profilu, jehož stav je čten.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>Aktuální stav</returns>
	/// <remarks>
	/// Stav je držen ve třídě DataSeed (tabulka __DataSeed) v záznamu s Id = 1.
	/// </remarks>
	public async Task<string> ReadCurrentStateAsync(string profileName, CancellationToken cancellationToken = default)
	{
		DataSeedVersion dataSeedVersion = await GetDataSeedVersionAsync(_dbContext, profileName, cancellationToken).ConfigureAwait(false);
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
		DataSeedVersion dataSeedVersion = GetDataSeedVersion(_dbContext, profileName);
		if (dataSeedVersion == null)
		{
			dataSeedVersion = new DataSeedVersion { ProfileName = profileName };
			_dbContext.Set<DataSeedVersion>().Add(dataSeedVersion);
		}
		dataSeedVersion.Version = currentState;
		_dbContext.SaveChanges();
	}

	/// <summary>
	/// Zapíše aktuální stav do databáze (vč. provedení dbContext.SaveChangesAsync).
	/// </summary>
	/// <param name="profileName">Název profilu, ke kterému je zapisován stav.</param>
	/// <param name="currentState">Aktuální stav k zapsání.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <remarks>
	/// Stav je držen ve třídě DataSeed (tabulka __DataSeed) v záznamu s Id = 1.
	/// </remarks>
	public async Task WriteCurrentStateAsync(string profileName, string currentState, CancellationToken cancellationToken = default)
	{
		DataSeedVersion dataSeedVersion = await GetDataSeedVersionAsync(_dbContext, profileName, cancellationToken).ConfigureAwait(false);
		if (dataSeedVersion == null)
		{
			dataSeedVersion = new DataSeedVersion { ProfileName = profileName };
#pragma warning disable VSTHRD103 // Call async methods when in an async method
			_dbContext.Set<DataSeedVersion>().Add(dataSeedVersion);
#pragma warning restore VSTHRD103 // Call async methods when in an async method
		}
		dataSeedVersion.Version = currentState;
		await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
	}

	private DataSeedVersion GetDataSeedVersion(IDbContext dbContext, string profileName)
	{
		IDbSet<DataSeedVersion> dbSet = dbContext.Set<DataSeedVersion>();
		return dbSet.FindTracked(profileName) ?? dbSet.AsQueryable(QueryTagBuilder.CreateTag(this.GetType(), nameof(GetDataSeedVersion))).SingleOrDefault(dataSeedVersion => dataSeedVersion.ProfileName == profileName);
	}

	private async Task<DataSeedVersion> GetDataSeedVersionAsync(IDbContext dbContext, string profileName, CancellationToken cancellationToken)
	{
		IDbSet<DataSeedVersion> dbSet = dbContext.Set<DataSeedVersion>();
		return dbSet.FindTracked(profileName) ?? await dbSet.AsQueryable(QueryTagBuilder.CreateTag(this.GetType(), nameof(GetDataSeedVersionAsync))).SingleOrDefaultAsync(dataSeedVersion => dataSeedVersion.ProfileName == profileName, cancellationToken).ConfigureAwait(false);
	}
}
