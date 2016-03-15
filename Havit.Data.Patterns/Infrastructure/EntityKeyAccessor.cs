using System;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.Infrastructure
{
	public class EntityKeyAccessor : IEntityKeyAccessor<ILanguage, int>
	{
		public int GetEntityKey(ILanguage entity)
		{
			return ((dynamic)entity).Id;
		}
	}
}