using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;

/// <summary>
/// Poskytuje seznam kolekcí a one-to-one referencí referencující danou entitu.
/// </summary>
public class ReferencingNavigationsService : IReferencingNavigationsService
{
	private readonly IReferencingNavigationsStorage _referencingCollectionsStorage;
	private readonly IDbContext _dbContext;

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public ReferencingNavigationsService(IReferencingNavigationsStorage referencingCollectionsStorage, IDbContext dbContext)
	{
		_referencingCollectionsStorage = referencingCollectionsStorage;
		_dbContext = dbContext;
	}

	/// <inheritdoc />
	public List<ReferencingNavigation> GetReferencingNavigations(IEntityType entityType)
	{
		Debug.Assert(entityType != null);

		if (_referencingCollectionsStorage.Value == null)
		{
			lock (_referencingCollectionsStorage)
			{
				if (_referencingCollectionsStorage.Value == null)
				{
					_referencingCollectionsStorage.Value = _dbContext.Model
						.GetEntityTypes() // získáme entity types
										  // z každého EntityType vezmeme EntityType a připojíme ReferencingCollections (klidně prázdný seznam)
						.Select(entityType => new
						{
							EntityType = entityType,
							// abychom vzali referencující kolekce, vezmeme cizí klíče a z nich "opačný směr", tj. kolekci (neřešíme vazbu 1:1)
							ReferencingCollections = GetReferencingNavigations_OneToOneReference(entityType)
								.Concat(GetReferencingNavigations_OneToManyCollections(entityType))
								.Concat(GetReferencingNavigations_ManyToManyCollections(entityType))
								.ToList()
						})
						.ToFrozenDictionary(item => item.EntityType, item => item.ReferencingCollections);
				}
			}
		}

		if (_referencingCollectionsStorage.Value.TryGetValue(entityType, out var result))
		{
			return result;
		}
		else
		{
			throw new InvalidOperationException(String.Format("EntityType {0} is not a supported type.", entityType.Name));
		}
	}

	/// <summary>
	/// Hledáme, jaké entity svými kolekcemi odkazují na předaný entityType (když dojde ke změně entityType, koho musíme invalidovat v cache).
	/// Např. pokud je na vstupu entityType třídy FakturaItem, vrátí ReferencingNavigation s entity type "Faktura" a navigation property name "Items".
	/// </summary>
	private List<ReferencingNavigation> GetReferencingNavigations_OneToManyCollections(IEntityType entityType)
	{
		return entityType.GetNavigations()
			.Where(navigation => navigation.Inverse?.IsCollection ?? false) // Hledáme navigace vedoucí na entityType, proto musíme použít Inverse
			.Select(navigation =>
			{
				var property = navigation.ForeignKey.Properties.Single();
				return new ReferencingNavigation
				{
					EntityType = navigation.ForeignKey.PrincipalEntityType.ClrType,
					NavigationPropertyName = navigation.ForeignKey.PrincipalToDependent.Name,
					SourceEntityForeignKeyProperty = property
				};
			}).ToList();
	}

	/// <summary>
	/// Hledáme, jaké entity svými referencemi odkazují jakožto one-to-one vazbou na předaný entityType (když dojde ke změně entityType, koho musíme invalidovat v cache).
	/// </summary>
	private List<ReferencingNavigation> GetReferencingNavigations_OneToOneReference(IEntityType entityType)
	{
		return entityType.GetNavigations()
			.Where(navigation =>
				(!navigation.IsCollection
				&& (navigation.Inverse != null) // a zároveň máme protistranu,
				&& !navigation.Inverse.IsCollection) // která je referencí (není kolekcí)
				&& (navigation.ForeignKey.DeclaringEntityType == entityType)) // předchozí je splněno pro oba směry navigace, potřebujeme vybrat ten správný směr navigace
			.Select(navigation =>
			{
				var property = navigation.ForeignKey.Properties.Single();
				return new ReferencingNavigation
				{
					EntityType = navigation.Inverse.DeclaringEntityType.ClrType,
					NavigationPropertyName = navigation.Inverse.Name,
					SourceEntityForeignKeyProperty = property
				};
			})
			.ToList();
	}

	/// <summary>
	/// Hledáme, jaké entity svými kolekcemi odkazují na předaný entityType (když dojde ke změně entityType, koho musíme invalidovat v cache).
	/// Např. pokud je na vstupu entityType pro skip navigation "User_Role" (tj. reprezentace ManyToMany vztahu bez třídy),
	/// vrátí ReferencingNavigation s entity type "User" a navigation property name "Roles".
	/// </summary>
	private List<ReferencingNavigation> GetReferencingNavigations_ManyToManyCollections(IEntityType entityType)
	{
		// Poznámka k modelu.
		// Vztah ManyToMany je reprezentován jako SkipNavigation. Jenže pomocí SkipNavigation se dozvíme o vztahu mezi EntityA a EntityB,
		// ovšem bez vztahové "tabulky" mezi EntityA a EntityB. Entitu této vztahové "tabulky" nemá ani Navigations, ani SkipNavigations (což dává rozum).
		// Proto volíme řešení, kdy se orientujeme podle cizích klíčů z vztahové "tabulky" (= entityType).		
		return entityType.GetForeignKeys()
			// Chceme jen takové cizí klíče, které k sobě mají na druhé straně (jedinou) skip navigaci. Tak poznáme, že náš cizí klíč je reprezentací skip navigace.
			.Where(foreignKey => foreignKey.GetReferencingSkipNavigations().SingleOrDefault() != null)
			// JK: Jenže EF nám vrací i skip navigace tříd, které se účastní vztahu, ale které nemají kolekci (a IsCollection je true).
			// Nevím, jestli nerozumím dobře modelu, nebo je to bug EF Core. Předpokládám však, že kolekce musí být nějak definovaná v kódu jakožto property
			// (neuvažujeme jiný způsob), tak si existenci property ověříme pomocí PropertyInfo.
			// Další výskyt téhož: viz NavigationTargetTypeService.
			.Where(foreignKey => foreignKey.GetReferencingSkipNavigations().Single().PropertyInfo != null)
			.Select(foreignKey =>
			{
				var property = foreignKey.Properties.Single();
				var skipNavigation = foreignKey.GetReferencingSkipNavigations().Single();
				return new ReferencingNavigation
				{
					EntityType = skipNavigation.DeclaringEntityType.ClrType,
					NavigationPropertyName = skipNavigation.Name,
					SourceEntityForeignKeyProperty = property
				};
			})
			.ToList();
	}
}

