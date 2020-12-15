using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.Patterns.DataLoaders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataLoaders
{
	public partial class DbDataLoader
	{
		/// <summary>
		/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
		/// </summary>
		private LoadPropertyInternalResult LoadReferencePropertyInternal<TEntity, TProperty>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TProperty : class
		{
			LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(propertyName, entities, out List<object> foreignKeysToLoad);

			if ((foreignKeysToLoad != null) && foreignKeysToLoad.Any()) // zůstalo nám, na co se ptát do databáze?
			{
				List<TProperty> loadedProperties = LoadReferencePropertyInternal_GetQuery<TProperty>(foreignKeysToLoad).ToList();
				LoadReferencePropertyInternal_StoreToCache(loadedProperties);
			}

			return LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(propertyName, entities);
		}

		/// <summary>
		/// Zajistí načtení vlastnosti, která je referencí (není kolkecí). Voláno reflexí.
		/// </summary>
		private async Task<LoadPropertyInternalResult> LoadReferencePropertyInternalAsync<TEntity, TProperty>(string propertyName, TEntity[] entities, CancellationToken cancellationToken /* no default */)
			where TEntity : class
			where TProperty : class
		{
			LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(propertyName, entities, out List<object> foreignKeysToLoad);

			if ((foreignKeysToLoad != null) && foreignKeysToLoad.Any()) // zůstalo nám, na co se ptát do databáze?
			{
				List<TProperty> loadedProperties = await LoadReferencePropertyInternal_GetQuery<TProperty>(foreignKeysToLoad).ToListAsync(cancellationToken).ConfigureAwait(false);
				LoadReferencePropertyInternal_StoreToCache(loadedProperties);
			}

			return LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(propertyName, entities);
		}

		private void LoadReferencePropertyInternal_GetFromCache<TEntity, TProperty>(string propertyName, TEntity[] entities, out List<object> foreignKeysToLoad)
			where TEntity : class
			where TProperty : class
		{
			List<TEntity> entitiesToLoadReference = entities.Where(entity => !IsEntityPropertyLoaded(entity, propertyName)).ToList();

			if (entitiesToLoadReference.Count == 0)
			{
				foreignKeysToLoad = null;
				return;
			}

            // získáme cizí klíč reprezentující referenci (Navigation)
            // TODO JK: Performance? Každý pokus o načtení z cache?            
            IProperty foreignKeyForReference = dbContext.Model.FindEntityType(typeof(TEntity)).FindNavigation(propertyName).ForeignKey.Properties.Single();

			// získáme klíče objektů, které potřebujeme načíst (z "běžných vlastností" nebo z shadow properties)
			// ignorujeme nenastavené reference (null)
			object[] foreignKeyValues = entitiesToLoadReference.Select(entity => dbContext.GetEntry(entity, true).CurrentValues[foreignKeyForReference]).Where(value => value != null).Distinct().ToArray();

			IDbSet<TProperty> dbSet = dbContext.Set<TProperty>();

			bool shouldFixup = false;
			foreignKeysToLoad = new List<object>(entitiesToLoadReference.Count);

			foreach (object foreignKeyValue in foreignKeyValues)
			{
				// Čistě teoreticky nemusela dosud proběhnout detekce změn (resp. fixup), proto se musíme podívat do identity map před tím,
				// než budeme řešit cache (cache by se mohla pokoušet o vytažení objektu, který je již v identity mapě a došlo by ke kolizi).
				// Spoléháme na provedení fixupu pomocí changetrackeru.

				// K této situaci dojde, pokud je objekt nejprve zaregistrován do changetrackeru (třeba přidáním do UnitOfWork/DbContextu)
				// a následně je mu nastavena hodnota cizího klíče (Id objektu), avšak nikoliv nastavení navigation property.
				TProperty trackedEntity = dbSet.FindTracked(foreignKeyValue);
				if (trackedEntity != null)
				{
					shouldFixup = true;					
				}
				else if (entityCacheManager.TryGetEntity<TProperty>(foreignKeyValue, out TProperty cachedEntity))
				{
					// NOOP
				}
				else // není ani v identity mapě, ani v cache, hledáme v databázi
				{
					foreignKeysToLoad.Add(foreignKeyValue);
				}
			}

			// Detekovali jsme, že některá z vlastností nebyla načtena, ale hodnota cizího klíče má hodnotu, která již je v identity mapě.
			// Proto provedeme fixup. AFAIK, implementačně nemáme lepší možnosti, než zavolat DetectChanges.
			// Teoreticky bychom měli detekci změn volat v ještě menší míře - maximálně jednou na volání Loadu, avšak pro minimální výskyt problému necháváme v této podobě.
			// Known issue: Property bude i nadále považována za nenačtenou. Avšak díky metodě v IsEntityPropertyLoaded v DbDataLoaderWithLoadedPropertiesMemory nebude docházet k opakovanému zpracování této vlastnosti.
			if (shouldFixup)
			{
				dbContext.ChangeTracker.DetectChanges();
			}
		}

		private IQueryable<TProperty> LoadReferencePropertyInternal_GetQuery<TProperty>(List<object> foreignKeysToLoad)
			where TProperty : class
		{
            // získáme název vlastnosti primárního klíče třídy načítané vlastnosti (obvykle "Id")
            // Performance: No big issue.
            string propertyPrimaryKey = dbContext.Model.FindEntityType(typeof(TProperty)).FindPrimaryKey().Properties.Single().Name;

			// získáme query pro načtení objektů

			// https://github.com/aspnet/EntityFrameworkCore/issues/14408
			// Jako workadound stačí místo v EF.Property<object> namísto object zvolit skutečný typ. Aktuálně používáme jen int, hardcoduji tedy int bez vynakládání většího úsilí na obecnější řešení.
			List<int> foreignKeysToQueryInt = foreignKeysToLoad.Cast<int>().ToList();
			return dbContext.Set<TProperty>().AsQueryable().Where(foreignKeysToQueryInt.ContainsEffective<TProperty>(item => EF.Property<int>(item, propertyPrimaryKey)));
		}

		private void LoadReferencePropertyInternal_StoreToCache<TProperty>(List<TProperty> loadedProperties)
			where TProperty : class
		{
			// uložíme do cache, pokud je cachovaná
			foreach (TProperty loadedEntity in loadedProperties)
			{
				entityCacheManager.StoreEntity(loadedEntity);
			}
		}

		private LoadPropertyInternalResult LoadReferencePropertyInternal_GetResult<TEntity, TProperty>(string propertyName, TEntity[] entities)
			where TEntity : class
			where TProperty : class
		{
			var propertyLambdaExpression = lambdaExpressionManager.GetPropertyLambdaExpression<TEntity, TProperty>(propertyName);
			
			// zde spoléháme na proběhnutí fixupu
			var loadedEntities = entities.Select(item => propertyLambdaExpression.LambdaCompiled(item)).Where(item => item != null).ToArray();
			return new LoadPropertyInternalResult
			{
				Entities = loadedEntities,
				FluentDataLoader = new DbFluentDataLoader<TProperty>(this, loadedEntities)
			};
		}
	}
}
