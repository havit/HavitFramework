using System;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Infrastructure
{
	/// <summary>
	/// Třída pro získávání identifikátoru modelových objektů.
	/// </summary>
	public class EntityKeyAccessor : IEntityKeyAccessor<ILanguage, int>
	{
		/// <summary>
		/// Vrátí klíč entity.
		/// </summary>
		/// <param name="entity">Entita.</param>
		public int GetEntityKey(ILanguage entity)
		{
			return ((dynamic)entity).Id;
		}
	}
}