using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Havit.Data.Entity.Internal;
using Havit.Data.Entity.ModelConfiguration.Edm;

namespace Havit.Data.Entity.Conventions;

/// <summary>
/// Zajišťuje existenci indexu na sloupcích cizího klíče.
/// Oproti vestavěné convention v EF podporuje více indexů na jednom sloupci a IDX_ + název tabulky (třídy) na začátku názvu indexu.
/// </summary>
public class ForeignKeyIndexConvention : IStoreModelConvention<AssociationType>
{
	/// <summary>
	/// Aplikuje konvenci na model.
	/// </summary>
	public void Apply(AssociationType association, DbModel model)
	{
		AssociationType associantionCSpace = model.ConceptualModel.AssociationTypes.FirstOrDefault(item => (item.GetSourceEnd().Name == association.Name) || (item.GetTargetEnd().Name == association.Name));
		bool isManyToMany = associantionCSpace != null && associantionCSpace.IsManyToMany();

		if (association.IsForeignKey && !isManyToMany)
		{
			EdmProperty[] foreignKeyIndexProperties = association.Constraint.ToProperties.Where(property => !property.DeclaringType.IsConventionSuppressed(typeof(ForeignKeyIndexConvention)) && !property.IsConventionSuppressed(typeof(ForeignKeyIndexConvention))).ToArray();
			if (foreignKeyIndexProperties.Any()) // jen pokud je nějaký klíč, nad kterým budeme tvořit index
			{
				// ManyToMany můžeme mít definovanou ještě pomocí dekompozice do dvou OneToMany vazeb.
				// Ani pro takovou nechceme tvořit indexy. Neřešíme jiné scénáře, v našich scénářích použití by se neměly vyskytovat.
				EntityType parentEntityType = foreignKeyIndexProperties.First().DeclaringType as EntityType;
				if ((parentEntityType != null) && (parentEntityType.KeyMembers.Count > 1))
				{
					// NOOP
				}
				else
				{
					IndexHelper.AddIndex(foreignKeyIndexProperties);
				}
			}
		}
	}
}