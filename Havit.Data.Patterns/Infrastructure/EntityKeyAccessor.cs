using System;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Infrastructure
{
	/// <summary>
	/// Třída pro získávání identifikátoru modelových objektů.
	/// </summary>
	public class EntityKeyAccessor<TEntity> : IEntityKeyAccessor<TEntity, int>
		where TEntity : class
	{
		/// <summary>
		/// Vrátí klíč entity.
		/// </summary>
		/// <param name="entity">Entita.</param>
		public int GetEntityKey(TEntity entity)
		{
			return ((dynamic)entity).Id;
		}
	}
}