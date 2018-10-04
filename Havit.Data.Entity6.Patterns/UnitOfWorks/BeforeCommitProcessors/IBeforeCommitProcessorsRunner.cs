namespace Havit.Data.Entity.Patterns.UnitOfWorks.BeforeCommitProcessors
{
	/// <summary>
	/// Spouští registrované IBeforeCommitProcessory.	
	/// </summary>
	public interface IBeforeCommitProcessorsRunner
	{
		/// <summary>
		/// Spustí registrované IBeforeCommitProcessory.	
		/// </summary>
		void Run(Changes changes);
	}
}
