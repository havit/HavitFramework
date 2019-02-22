using System.Collections.Generic;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.Patterns.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny.
	/// Podporováno je zřetězení (subjekt => subjekt.Adresa.Zeme.Svetadil) vč. varianty s kolekcemi, kdy je třeba použít AllItems (subjekt => subjekt.Adresy.AllItems().Zeme).
	/// Rozšiřuje DbDataLoader o paměť, jaké objekty a jaké vlastnosti byly načteny.
	/// </summary>
	public class DbDataLoaderWithLoadedPropertiesMemory : DbDataLoader
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbDataLoaderWithLoadedPropertiesMemory(IDbContext dbContext, IPropertyLoadSequenceResolver propertyLoadSequenceResolver, IPropertyLambdaExpressionManager lambdaExpressionManager, IEntityCacheManager entityCacheManager, IEntityKeyAccessor entityKeyAccessor) : base(dbContext, propertyLoadSequenceResolver, lambdaExpressionManager, entityCacheManager, entityKeyAccessor)
		{
		}

		/// <summary>
		/// Vrací true, pokud je vlastnost objektu již načtena.
		/// Předpokládá, že každá vlastnost každého objektu, na který se ptáme, bude načtena. Proto si ji uloží do paměti a při opakovaném přístupu vrací true, pokud máme u daného objektu danou vlastnost zapamatovanou.
		/// Pro vlastnosti objektů, které nejsou v paměti, volá bázovou třídu.
		/// </summary>
		protected override bool IsEntityPropertyLoaded<TEntity>(TEntity entity, string propertyName, bool isPropertyCollection)
		{
			LoadedEntityProperty loadedEntityProperty = new LoadedEntityProperty(entity, propertyName);
			return loadedEntityProperties.Add(loadedEntityProperty)
				? base.IsEntityPropertyLoaded(entity, propertyName, isPropertyCollection) // klíč nebyl v kolekci -> vlastnost ještě nemusela být načtena
				: true; // klíč byl v kolekci, již jsme načítali
		}
		private readonly HashSet<LoadedEntityProperty> loadedEntityProperties = new HashSet<LoadedEntityProperty>();
	
		internal sealed class LoadedEntityProperty
		{
			public object Entity { get; }
			public string PropertyName { get; }

			public LoadedEntityProperty(object entity, string propertyName)
			{
				this.Entity = entity;
				this.PropertyName = propertyName;
			}

			public override bool Equals(object obj)
			{
				LoadedEntityProperty objLoadedEntityProperty = obj as LoadedEntityProperty;
				if (objLoadedEntityProperty == null)
				{
					return false;
				}

				return (this.Entity == objLoadedEntityProperty.Entity) && (this.PropertyName == objLoadedEntityProperty.PropertyName);
			}

			public override int GetHashCode()
			{
				return Entity.GetHashCode() ^ PropertyName.GetHashCode();
			}
		}
	}
}
