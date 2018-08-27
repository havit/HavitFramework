namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	/// <summary>
	/// Základný interface pre DB Injections.
	/// </summary>
	public interface IDbInjection
	{
		/// <summary>
		/// Názov databázového objektu, ktorý je spravovaný touto <see cref="IDbInjection"/>.
		/// </summary>
        string ObjectName { get; }
	}
}