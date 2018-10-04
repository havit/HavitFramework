namespace Havit.Data.Entity.Patterns.UnitOfWorks
{
	/// <summary>
	/// Zmìny objektù v UnitOfWork.
	/// </summary>
	public class Changes
	{
		/// <summary>
		/// Registrované objekty pro Insert.
		/// </summary>
		public object[] Inserts { get; set; }

		/// <summary>
		/// Registrované objekty pro Update.
		/// </summary>
		public object[] Updates { get; set; }

		/// <summary>
		/// Registrované objekty pro Delete.
		/// </summary>
		public object[] Deletes { get; set; }
	}
}