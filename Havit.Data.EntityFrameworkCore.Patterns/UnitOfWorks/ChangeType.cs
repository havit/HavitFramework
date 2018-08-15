namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks
{
	/// <summary>
	/// Typ změny.
	/// </summary>
	public enum ChangeType
	{
		/// <summary>
		/// Insert
		/// </summary>
		Insert,

		/// <summary>
		/// Update
		/// </summary>
		Update,

		/// <summary>
		/// Delete
		/// </summary>
		Delete
	}
}
