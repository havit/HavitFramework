using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Havit.Data.Patterns.DataLoaders;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.Entity.Patterns.DataLoaders
{
	/// <summary>
	/// Explicity data loader.
	/// Načte hodnoty vlastnosti třídy, pokud ještě nejsou načteny. Podporuje synchronní i asynchronní načítání.
	/// Načítání lze zřetězit:
	/// <code>dbDataLoader.For(...).Load(item => item.Property1).Load(property1 => property1.Property2)</code>
	/// </summary>
	internal class DbDataLoaderFor<TEntity> : IDbDataLoaderFor<TEntity>, IDbDataLoaderForAsync<TEntity>
		where TEntity : class
	{
		private readonly IDbContext dbContext;
		private readonly List<TEntity> entities;

		/// <summary>
		/// Konstructor.
		/// </summary>
		/// <param name="dbContext">DbContext, pomocí něhož budou objekty načítány.</param>
		/// <param name="entities">Entity, jejichž vlastnosti budou načítány.</param>
		public DbDataLoaderFor(IDbContext dbContext, IEnumerable<TEntity> entities) : this(dbContext, entities, true)
		{
		}

		/// <summary>
		/// Konstructor.
		/// </summary>
		private DbDataLoaderFor(IDbContext dbContext, IEnumerable<TEntity> entities, bool checkEntitiesState)
		{
			Contract.Requires(dbContext != null);
			Contract.Requires(entities != null);

			this.dbContext = dbContext;
			this.entities = entities.Where(entity => entity != null).ToList();

			if (checkEntitiesState)
			{
				Contract.Assert<InvalidOperationException>(this.entities.All(item => dbContext.GetEntityState(item) != EntityState.Detached), "DbDataLoader can be used only for objects tracked by a change tracker.");
			}
		}

		/// <summary>
		/// Omezuje načtení jen některých záznamů při zřetězení načítání.
		/// </summary>
		/// <param name="predicate">Podmínka, kterou musí splnit záznamy, aby byla načteny následujícím loadem.</param>
		/// <example>
		/// <code>dbDataLoader.For(subjekt).Load(item =&gt; item.Faktury).Where(faktura =&gt; faktura.Castka &gt; 0).Load(faktura =&gt; faktura.RadkyFaktury);</code>
		/// </example>
		public IDbDataLoaderFor<TEntity> Where(Func<TEntity, bool> predicate)
		{
			Contract.Requires(predicate != null);

			List<TEntity> newEntities = entities.Where(predicate).ToList();
			return new DbDataLoaderFor<TEntity>(dbContext, newEntities, false);
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			if (entities.Count > 0)
			{
				Func<TEntity, TProperty> propertyPathLambda = propertyPath.Compile();
				List<int> ids = GetIds(propertyPathLambda);
				if (ids.Count > 0)
				{
					IQueryable loadQuery = GetLoadQuery(propertyPath, ids);
					loadQuery.Load();
				}

				List<TProperty> references = entities.Select(item => propertyPathLambda(item)).ToList();
				return new DbDataLoaderFor<TProperty>(dbContext, references, false);
			}
			return new DbDataLoaderFor<TProperty>(this.dbContext, Enumerable.Empty<TProperty>(), false);
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> propertyPath)
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			return LoadCollectionInternal<IEnumerable<TProperty>, TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, ICollection<TProperty>>> propertyPath)
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			return LoadCollectionInternal<ICollection<TProperty>, TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IDbDataLoaderFor<TProperty> Load<TProperty>(Expression<Func<TEntity, List<TProperty>>> propertyPath)
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			return LoadCollectionInternal<List<TProperty>, TProperty>(propertyPath);
		}

		public async Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath)
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			if (entities.Count > 0)
			{
				Func<TEntity, TProperty> propertyPathLambda = propertyPath.Compile();
				List<int> ids = GetIds(propertyPathLambda);

				if (ids.Count > 0)
				{
					IQueryable loadQuery = GetLoadQuery(propertyPath, ids);
					await loadQuery.LoadAsync();
				}

				List<TProperty> references = entities.Select(item => propertyPathLambda(item)).ToList();
				return new DbDataLoaderFor<TProperty>(dbContext, references, false);
			}
			return new DbDataLoaderFor<TProperty>(this.dbContext, Enumerable.Empty<TProperty>(), false);
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> propertyPath)
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			return LoadCollectionInternalAsync<IEnumerable<TProperty>, TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, ICollection<TProperty>>> propertyPath)
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			return LoadCollectionInternalAsync<ICollection<TProperty>, TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public Task<IDbDataLoaderForAsync<TProperty>> LoadAsync<TProperty>(Expression<Func<TEntity, List<TProperty>>> propertyPath)
			where TProperty : class
		{
			Contract.Requires(propertyPath != null);

			return LoadCollectionInternalAsync<List<TProperty>, TProperty>(propertyPath);
		}

		/// <summary>
		/// Načte vlastnosti třídy, pokud již nejsou načteny.
		/// </summary>
		/// <param name="propertyPath">Vlastnost, která má být načtena.</param>
		public IDbDataLoaderFor<TProperty> LoadCollectionInternal<TCollectionProperty, TProperty>(Expression<Func<TEntity, TCollectionProperty>> propertyPath)
			where TCollectionProperty : class, IEnumerable<TProperty>
			where TProperty : class
		{
			if (entities.Count > 0)
			{
				Func<TEntity, TCollectionProperty> propertyPathLambda = propertyPath.Compile();
				List<int> ids = GetIds(propertyPathLambda);
				if (ids.Count > 0)
				{
					IQueryable loadQuery = GetLoadQuery(propertyPath, ids);
					loadQuery.Load();
				}
				List<TProperty> references = entities.SelectMany(item => propertyPathLambda(item)).ToList();
				return new DbDataLoaderFor<TProperty>(dbContext, references, false);
			}

			return new DbDataLoaderFor<TProperty>(this.dbContext, Enumerable.Empty<TProperty>(), false);
		}

		private async Task<IDbDataLoaderForAsync<TProperty>> LoadCollectionInternalAsync<TCollectionProperty, TProperty>(Expression<Func<TEntity, TCollectionProperty>> propertyPath)
			where TCollectionProperty : class, IEnumerable<TProperty>
			where TProperty : class
		{
			if (entities.Count > 0)
			{
				Func<TEntity, TCollectionProperty> propertyPathLambda = propertyPath.Compile();
				List<int> ids = GetIds(propertyPathLambda);
				if (ids.Count > 0)
				{
					IQueryable loadQuery = GetLoadQuery(propertyPath, ids);
					await loadQuery.LoadAsync();
				}
				List<TProperty> references = entities.SelectMany(item => propertyPathLambda(item)).ToList();
				return new DbDataLoaderFor<TProperty>(dbContext, references, false);
			}

			return new DbDataLoaderFor<TProperty>(this.dbContext, Enumerable.Empty<TProperty>(), false);
		}

		/// <summary>
		/// Vrátí seznam Id objektů, jejichž vlastnost má být načtena.
		/// </summary>
		private List<int> GetIds<TProperty>(Func<TEntity, TProperty> propertyPathLambda)
			where TProperty : class
		{
			return entities.Where(item => propertyPathLambda(item) == null).Select(entity => (int)(((dynamic)entity).Id)).Distinct().ToList();
		}

		/// <summary>
		/// Vrátí WHERE podmínku omezující množinu záznamů dle Id.
		/// </summary>
		/// <param name="ids">Identifikátory objektů, které mají být ve where klauzuli.</param>
		private Expression<Func<TEntity, bool>> GetWhereExpression(List<int> ids)
		{
			Contract.Requires(ids != null);
			Contract.Requires(ids.Count > 0);

			var parameter = Expression.Parameter(typeof(TEntity), "item");

			// jediný záznam - testujeme na rovnost
			if (ids.Count == 1)
			{
				return (Expression<Func<TEntity, bool>>)Expression.Lambda(
					Expression.Equal(
						Expression.Property(parameter, typeof(TEntity), "Id"),
						Expression.Constant(ids[0])),
					parameter);
			}
			
			// více záznamů
			// pokud jde o řadu IDček (1, 2, 3, 4) bez přeskakování, pak použijeme porovnání >= a  <=.
			int[] sortedIds = ids.OrderBy(item => item).Distinct().ToArray();

				//pro pole: 1, 2, 3, 4
				// if 1 + 4 - 1 (4) == 4
			if ((sortedIds[0] + sortedIds.Length - 1) == sortedIds[sortedIds.Length - 1]) // testujeme, zda jde o posloupnost IDček
			{
				return (Expression<Func<TEntity, bool>>)Expression.Lambda(
					Expression.AndAlso(
						Expression.GreaterThanOrEqual(Expression.Property(parameter, typeof(TEntity), "Id"), Expression.Constant(sortedIds[0])),
						Expression.LessThanOrEqual(Expression.Property(parameter, typeof(TEntity), "Id"), Expression.Constant(sortedIds[sortedIds.Length - 1]))),
					parameter);
			}

			// v obecném případě hledáme přes IN (...)
			return (Expression<Func<TEntity, bool>>)Expression.Lambda(
				Expression.Call(
					Expression.Constant(ids),
					typeof(List<int>).GetMethod("Contains"),
					new List<Expression> { Expression.Property(parameter, typeof(TEntity), "Id") }),
				parameter);
		}

		/// <summary>
		/// Vrátí dotaz načítající vlastnosti objektů s daným identifikátorem.
		/// </summary>
		/// <param name="propertyPath">Načítaná vlastnost.</param>
		/// <param name="ids">Identifikátory objektů, jejichž vlastnost má být načtena.</param>
		private IQueryable GetLoadQuery<TProperty>(Expression<Func<TEntity, TProperty>> propertyPath, List<int> ids)
		{
			IQueryable loadQuery;

			// jde o kolekci?
			Type propertyType = propertyPath.Body.Type;
			if (propertyType.IsGenericType && typeof(IEnumerable<>).MakeGenericType(propertyType.GenericTypeArguments).IsAssignableFrom(propertyType))
			{
				loadQuery = dbContext.Set<TEntity>()
					.Where(GetWhereExpression(ids))
					.Include(propertyPath);
			}
			else
			{
				loadQuery = dbContext.Set<TEntity>()
					.Where(GetWhereExpression(ids))
					.Select(propertyPath);
			}
			return loadQuery;
		}

		/// <summary>
		/// Omezuje načtení jen některých záznamů při zřetězení načítání.
		/// </summary>
		/// <param name="predicate">Podmínka, kterou musí splnit záznamy, aby byla načteny následujícím loadem.</param>
		/// <example>
		/// <code>dbDataLoader.For(subjekt).Load(item =&gt; item.Faktury).Where(faktura =&gt; faktura.Castka &gt; 0).Load(faktura =&gt; faktura.RadkyFaktury);</code>
		/// </example>
		IDbDataLoaderForAsync<TEntity> IDbDataLoaderForAsync<TEntity>.Where(Func<TEntity, bool> predicate)
		{
			List<TEntity> newEntities = entities.Where(predicate).ToList();
			return new DbDataLoaderFor<TEntity>(dbContext, newEntities, false);
		}
	}
}