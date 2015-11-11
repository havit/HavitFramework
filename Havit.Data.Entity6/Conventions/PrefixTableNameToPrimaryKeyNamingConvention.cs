using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Havit.Data.Entity.Conventions
{
	/// <summary>
	/// Doplňuje název tabulky na začátek názvů sloupců primárního klíče (bez vazbových tabulek).
	/// </summary>
	public class PrefixTableNameToPrimaryKeyNamingConvention : IStoreModelConvention<EdmProperty>
	{
		/// <summary>
		/// Aplikuje konvenci na model.
		/// </summary>
		public void Apply(EdmProperty item, DbModel model)
		{
			EntityType parentEntityType = item.DeclaringType as EntityType;

			if (parentEntityType != null)
			{
				EntityType entityTypeCSpace = model.ConceptualModel.EntityTypes.FirstOrDefault(i => (i.Name == parentEntityType.Name));
				bool isEntity = entityTypeCSpace != null; 

				if (isEntity // eliminujeme vztahové tabulky
					&& (parentEntityType.KeyMembers.Contains(item))) // omezíme se jen na primární klíče
				{
					item.Name = parentEntityType.Name + item.Name;
				}
			}
		}
	}
}