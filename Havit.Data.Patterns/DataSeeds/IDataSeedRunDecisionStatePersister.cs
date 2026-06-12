namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Spravuje stav pro implementace DataSeedRunDecision.
/// </summary>
public interface IDataSeedRunDecisionStatePersister
{
	/// <summary>
	/// Přečte aktuální stav.
	/// </summary>
	/// <returns>Aktuální stav</returns>
	/// <param name="profileName">Název profilu, jehož stav je čten.</param>
	string ReadCurrentState(string profileName);

	/// <summary>
	/// Přečte aktuální stav.
	/// </summary>
	/// <returns>Aktuální stav</returns>
	/// <param name="profileName">Název profilu, jehož stav je čten.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	Task<string> ReadCurrentStateAsync(string profileName, CancellationToken cancellationToken = default);

	/// <summary>
	/// Zapíše aktuální stav.
	/// </summary>
	/// <param name="profileName">Název profilu, ke kterému je stav zapisován.</param>
	/// <param name="currentState">Aktuální stav k zapsání.</param>
	void WriteCurrentState(string profileName, string currentState);

	/// <summary>
	/// Zapíše aktuální stav.
	/// </summary>
	/// <param name="profileName">Název profilu, ke kterému je stav zapisován.</param>
	/// <param name="currentState">Aktuální stav k zapsání.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	Task WriteCurrentStateAsync(string profileName, string currentState, CancellationToken cancellationToken = default);
}