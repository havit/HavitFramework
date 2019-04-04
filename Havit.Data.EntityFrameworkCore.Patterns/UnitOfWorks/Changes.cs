using System.Linq;

namespace Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks
{
	/// <summary>
	/// Změny objektů v UnitOfWork.
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

		/// <summary>
		/// Vrátí všechny změněné objekty (bez ohledu na způsob změny).
		/// </summary>
		public object[] GetAllChanges()
		{
			return Inserts.Concat(Updates).Concat(Deletes).ToArray();
		}
	}
}