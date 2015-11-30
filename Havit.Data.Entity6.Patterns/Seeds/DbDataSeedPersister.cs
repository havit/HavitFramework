using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Data.Patterns.DataSeeds;
using Havit.Linq;
using Havit.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.Seeds
{
	/// <summary>
	/// Persistence předpisu pro seedování dat.
	/// </summary>
	public class DbDataSeedPersister : IDataSeedPersister
	{
		private readonly IDbContext dbContext;
		
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbDataSeedPersister(IDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		/// <summary>
		/// Dle předpisu seedování dat (konfigurace) provede persistenci seedovaných dat.
		/// </summary>
		public void Save<TEntity>(DataSeedConfiguration<TEntity> configuration)
			where TEntity : class
		{
			DbSet<TEntity> dbSet = dbContext.Set<TEntity>();
			List<SeedDataPair<TEntity>> seedDataPairs = PairWithDbData(dbSet, configuration);

			// TODO: Pokud je update Enabled false, nedojde k napárování a aftersave (k čemuž dojít má, ale nejde už o aftersave)
			if (!configuration.UpdateEnabled)
			{
				seedDataPairs.RemoveAll(item => item.DbEntity != null);
			}

			List<SeedDataPair<TEntity>> unpairedSeedDataPairs = seedDataPairs.Where(item => item.DbEntity == null).ToList();
			foreach (SeedDataPair<TEntity> unpairedSeedDataPair in unpairedSeedDataPairs)
			{
				unpairedSeedDataPair.DbEntity = (TEntity)Activator.CreateInstance(typeof(TEntity));
				unpairedSeedDataPair.IsNew = true;
			}
			dbSet.AddRange(unpairedSeedDataPairs.Select(item => item.DbEntity));

			Update(configuration, seedDataPairs);

			dbContext.SaveChanges();

			DoAfterSaveActions(configuration, seedDataPairs);

			if (configuration.ChildrenSeeds != null)
			{
				foreach (ChildDataSeedConfigurationEntry childSeed in configuration.ChildrenSeeds)
				{
					childSeed.SaveAction(this);
				}
			}
		}

		/// <summary>
		/// Provede párování předpisu seedovaných dat s existujícími objekty.
		/// </summary>
		private List<SeedDataPair<TEntity>> PairWithDbData<TEntity>(DbSet<TEntity> dbSet, DataSeedConfiguration<TEntity> configuration)
			where TEntity : class
		{
			return PairWithDbData_LoadChunksStrategy<TEntity>(dbSet, configuration);
		}

		/// <summary>
		/// Provede párování předpisu seedovaných dat tak, že jsou objekty načteny v dávkách, čímž dochází k optimalizaci množství prováděných databázových operací.
		/// </summary>
		private List<SeedDataPair<TEntity>> PairWithDbData_LoadChunksStrategy<TEntity>(IQueryable<TEntity> dataQueryable, DataSeedConfiguration<TEntity> configuration)
			where TEntity : class
		{
			IEnumerable<TEntity> seedData = configuration.SeedData;
			Func<TEntity, object>[] pairByLambdas = configuration.PairByExpressions.Select(item => item.Compile()).ToArray();
			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
			List<TEntity> dbEntities = new List<TEntity>();

			foreach (var chunk in seedData.Chunkify(1000))
			{
				Expression<Func<TEntity, bool>> chunkWhereExpression = null;
				foreach (var seedEntity in chunk)
				{
					Expression<Func<TEntity, bool>> seedEntityWhereExpression = null;

					for (int i = 0; i < configuration.PairByExpressions.Count; i++)
					{
						Expression<Func<TEntity, object>> expression = configuration.PairByExpressions[i];
						Func<TEntity, object> lambda = pairByLambdas[i];

						object value = lambda.Invoke(seedEntity);
						var pairByConditionExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(
							Expression.Equal(ExpressionExt.ReplaceParameter(expression.Body, expression.Parameters[0], parameter).RemoveConvert(), Expression.Constant(value)), // TODO: Expression.Constant nejde pro references
							parameter);

						if (seedEntityWhereExpression != null)
						{
							seedEntityWhereExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(Expression.AndAlso(seedEntityWhereExpression.Body, pairByConditionExpression.Body), parameter);
						}
						else
						{
							seedEntityWhereExpression = pairByConditionExpression;
						}
					}

					if (chunkWhereExpression != null)
					{
						chunkWhereExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(Expression.OrElse(chunkWhereExpression.Body, seedEntityWhereExpression.Body), parameter);
					}
					else
					{
						chunkWhereExpression = seedEntityWhereExpression;
					}
				}
				dbEntities.AddRange(dataQueryable.Where(chunkWhereExpression).ToList());
			}

			return PairWithDbData_PairOneByOneStrategy(dbEntities.AsQueryable(), configuration);
		}

		/// <summary>
		/// Provede párování předpisu seedovaných dat, párování se provádí "po jednom".
		/// </summary>
		private List<SeedDataPair<TEntity>> PairWithDbData_PairOneByOneStrategy<TEntity>(IQueryable<TEntity> dataQueryable, DataSeedConfiguration<TEntity> configuration)
			where TEntity : class
		{
			// V existujícím kódu je voláno jen z PairWithDbData_LoadChunksStrategy a dataQueryable je in-memory úložiště. Tato metoda tedy při současném použití neprovádí dotazy do databáze.

			IEnumerable<TEntity> seedData = configuration.SeedData;

			var result = new List<SeedDataPair<TEntity>>();

			Func<TEntity, object>[] pairByLambdas = configuration.PairByExpressions.Select(item => item.Compile()).ToArray();

			foreach (var seedEntity in seedData)
			{
				ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");

				Expression<Func<TEntity, bool>> whereExpression = null;

				for (int i = 0; i < configuration.PairByExpressions.Count; i++)
				{
					Expression<Func<TEntity, object>> expression = configuration.PairByExpressions[i];
					Func<TEntity, object> lambda = pairByLambdas[i];

					object value = lambda.Invoke(seedEntity);

					var pairByConditionExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(
						Expression.Equal(ExpressionExt.ReplaceParameter(expression.Body, expression.Parameters[0], parameter).RemoveConvert(), Expression.Constant(value)), // TODO: Expression.Constant nejde pro references
						parameter);

					if (whereExpression != null)
					{
						whereExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(Expression.AndAlso(whereExpression.Body, pairByConditionExpression.Body), parameter);
					}
					else
					{
						whereExpression = pairByConditionExpression;
					}
				}

				var dbEntity = dataQueryable.Where(whereExpression).SingleOrDefault();

				result.Add(new SeedDataPair<TEntity>
				{
					SeedEntity = seedEntity,
					DbEntity = dbEntity
				});
			}

			return result;
		}

		/// <summary>
		/// Provede vytvoření či aktualizaci dat dle předpisu seedování.
		/// </summary>
		private void Update<TEntity>(DataSeedConfiguration<TEntity> configuration, IEnumerable<SeedDataPair<TEntity>> pairs)
			where TEntity : class
		{
			var entityTypes = ((IObjectContextAdapter)dbContext).ObjectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList(); // TODO: Duplikace kódu
			EntityType entityType = entityTypes.Single(item => typeof(TEntity) == (Type)item.GetType().GetProperty("ClrType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(item)); // TODO: Duplikace kódu!

			List<string> properties = entityType.Properties.Except(entityType.KeyProperties.Where(item => item.StoreGeneratedPattern != StoreGeneratedPattern.None)).Select(item => item.Name).ToList();
			List<string> updateProperties;
			if (configuration.ExcludeUpdateExpressions != null)
			{
				// TODO: Hezčí kód
				// TODO: Duplikace vyhození KeyProperties
				List<string> exludedUpdateProperties = configuration.ExcludeUpdateExpressions.Select(item => GetPropertyName(item.Body.RemoveConvert())).ToList().Concat(entityType.KeyProperties.Select(item => item.Name)).ToList();
				updateProperties = properties.Except(exludedUpdateProperties).ToList();
			}
			else
			{
				updateProperties = properties.Except(entityType.KeyProperties.Select(item => item.Name)).ToList();
			}

			foreach (var pair in pairs)
			{
				foreach (var property in (pair.IsNew ? properties : updateProperties))
				{
					object value = DataBinderExt.GetValue(pair.SeedEntity, property);
					DataBinderExt.SetValue(pair.DbEntity, property, value);
				}
			}
		}

		/// <summary>
		/// Provede volání AfterSaveActions.
		/// </summary>
		private void DoAfterSaveActions<TEntity>(DataSeedConfiguration<TEntity> configuration, List<SeedDataPair<TEntity>> seedDataPairs)
			where TEntity : class
		{
			if (configuration.AfterSaveActions != null)
			{
				foreach (SeedDataPair<TEntity> pair in seedDataPairs)
				{
					AfterSaveDataArgs<TEntity> args = new AfterSaveDataArgs<TEntity>(pair.SeedEntity, pair.DbEntity, pair.IsNew);

					foreach (Action<AfterSaveDataArgs<TEntity>> afterSaveAction in configuration.AfterSaveActions)
					{
						afterSaveAction(args);
					}
				}
			}
		}

		/// <summary>
		/// Vrátí název vlastnosti, která je reprezentována daným výrazem.
		/// </summary>
		// TODO: Extrahovat!
		private string GetPropertyName(Expression item)
		{
			if (item is MemberExpression)
			{
				MemberExpression memberExpression = (MemberExpression)item;
				if (memberExpression.Expression is System.Linq.Expressions.ParameterExpression)
				{
					return memberExpression.Member.Name;
				}
			}
			throw new NotSupportedException(item.ToString());
		}

	}
}
