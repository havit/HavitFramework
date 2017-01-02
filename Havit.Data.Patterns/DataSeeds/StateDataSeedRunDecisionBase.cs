namespace Havit.Data.Patterns.DataSeeds
{
	/// <summary>
	/// Abstraktní předek řešící spuštění seedování dat dle (nějakého) stavu.
	/// </summary>
	public abstract class StateDataSeedRunDecisionBase : IDataSeedRunDecision
	{
		private readonly IDataSeedRunDecisionStatePersister dataSeedRunDecisionStatePersister;

		protected StateDataSeedRunDecisionBase(IDataSeedRunDecisionStatePersister dataSeedRunDecisionStatePersister)
		{
			this.dataSeedRunDecisionStatePersister = dataSeedRunDecisionStatePersister;
		}

		/// <summary>
		/// Vrací aktuální stav.
		/// </summary>
		protected internal abstract string GetCurrentState();

		/// <summary>
		/// Vrací true, pokud persister obsahuje jinou hodnotu než aktuální stav.
		/// </summary>
		public virtual bool ShouldSeedData()
		{
			return GetCurrentState() != dataSeedRunDecisionStatePersister.ReadCurrentState();
		}

		/// <summary>
		/// Nastaví do persisteru aktuální stav.
		/// </summary>
		public virtual void SeedDataCompleted()
		{
			dataSeedRunDecisionStatePersister.WriteCurrentState(GetCurrentState());
		}
	}
}