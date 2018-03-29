using System;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Infrastructure
{
	// TODO JK: Odstranit, přesunuto (zkopírováno) do EF6.Patterns.

	/// <summary>
	/// Služba pro získávání primárního klíče modelových objektů.
	/// </summary>
	public class EntityKeyAccessor<TEntity> : IEntityKeyAccessor<TEntity, int>
		where TEntity : class
	{
		/// <summary>
		/// Vrátí hodnotu primárního klíče entity.
		/// </summary>
		/// <param name="entity">Entita.</param>
		public int GetEntityKey(TEntity entity)
		{
			return ((dynamic)entity).Id;
		}

		/// <summary>
		/// Vrátí název vlastnosti, která je primárním klíčem.
		/// </summary>
		public string GetEntityKeyPropertyName()
		{
			return "Id";
		}
	}
}