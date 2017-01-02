namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Spravuje stav pro implementace DataSeedRunDecision.
	/// </summary>
	public interface IDataSeedRunDecisionStatePersister
	{
		/// <summary>
		/// Přečte aktuální stav.
		/// </summary>
		/// <returns>Aktuální stav</returns>
		string ReadCurrentState();

		/// <summary>
		/// Zapíše aktuální stav.
		/// </summary>
		/// <param name="currentState">Aktuální stav k zapsání.</param>
		void WriteCurrentState(string currentState);
	}
}