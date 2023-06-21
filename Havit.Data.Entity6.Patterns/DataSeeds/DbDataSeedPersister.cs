using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Data.Patterns.DataSeeds;
using Havit.Diagnostics.Contracts;
using Havit.Linq;
using Havit.Linq.Expressions;

namespace Havit.Data.Entity.Patterns.DataSeeds;

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
		CheckConditions(configuration);

		DbSet<TEntity> dbSet = dbContext.Set<TEntity>();
		List<SeedDataPair<TEntity>> seedDataPairs = PairWithDbData(dbSet, configuration);
		List<SeedDataPair<TEntity>> seedDataPairsToUpdate = new List<SeedDataPair<TEntity>>(seedDataPairs);

		if (!configuration.UpdateEnabled)
		{
			seedDataPairsToUpdate.RemoveAll(item => item.DbEntity != null);
		}

		List<SeedDataPair<TEntity>> unpairedSeedDataPairs = seedDataPairsToUpdate.Where(item => item.DbEntity == null).ToList();
		foreach (SeedDataPair<TEntity> unpairedSeedDataPair in unpairedSeedDataPairs)
		{
			unpairedSeedDataPair.DbEntity = (TEntity)Activator.CreateInstance(typeof(TEntity));
			unpairedSeedDataPair.IsNew = true;
		}
		dbSet.AddRange(unpairedSeedDataPairs.Select(item => item.DbEntity));

		Update(configuration, seedDataPairsToUpdate);
		
		DoBeforeSaveActions(configuration, seedDataPairs);
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

	private void CheckConditions<TEntity>(DataSeedConfiguration<TEntity> configuration)
	{
		Contract.Requires<ArgumentNullException>(configuration != null, nameof(configuration));
		Contract.Requires<InvalidOperationException>((configuration.PairByExpressions != null) && (configuration.PairByExpressions.Count > 0), "Expression to pair object missing (missing PairBy method call).");
	}

	/// <summary>
	/// Provede párování předpisu seedovaných dat s existujícími objekty.
	/// </summary>
	internal List<SeedDataPair<TEntity>> PairWithDbData<TEntity>(IQueryable<TEntity> dataQueryable, DataSeedConfiguration<TEntity> configuration)
		where TEntity : class
	{
		return PairWithDbData_LoadChunksStrategy<TEntity>(dataQueryable, configuration);
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

		// Chunkify(1000) --> SQL Server 2008: Some part of your SQL statement is nested too deeply. Rewrite the query or break it up into smaller queries.
		// Proto došlo ke změně na .Chunkify(100), správné číslo hledáme.
		foreach (TEntity[] chunk in seedData.Chunkify(100))
		{
			Expression<Func<TEntity, bool>> chunkWhereExpression = null;
			foreach (TEntity seedEntity in chunk)
			{
				Expression<Func<TEntity, bool>> seedEntityWhereExpression = null;

				for (int i = 0; i < configuration.PairByExpressions.Count; i++)
				{
					Expression<Func<TEntity, object>> expression = configuration.PairByExpressions[i];
					Func<TEntity, object> lambda = pairByLambdas[i];

					object value = lambda.Invoke(seedEntity);

			        Type expressionBodyType = expression.Body.RemoveConvert().Type;

				    Expression valueExpression = ((value != null) && (value.GetType() != expressionBodyType))
				        ? (Expression)Expression.Convert(Expression.Constant(value), expressionBodyType)
				        : (Expression)Expression.Constant(value);

                        Expression<Func<TEntity, bool>> pairByConditionExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(
						Expression.Equal(ExpressionExt.ReplaceParameter(expression.Body, expression.Parameters[0], parameter).RemoveConvert(), valueExpression), // Expression.Constant nejde pro references
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

		List<SeedDataPair<TEntity>> result = new List<SeedDataPair<TEntity>>();

		Func<TEntity, object>[] pairByLambdas = configuration.PairByExpressions.Select(item => item.Compile()).ToArray();

		foreach (TEntity seedEntity in seedData)
		{
			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
		    
			Expression<Func<TEntity, bool>> whereExpression = null;

			for (int i = 0; i < configuration.PairByExpressions.Count; i++)
			{
				Expression<Func<TEntity, object>> expression = configuration.PairByExpressions[i];
				Func<TEntity, object> lambda = pairByLambdas[i];

				object value = lambda.Invoke(seedEntity);

			    Type expressionBodyType = expression.Body.RemoveConvert().Type;

			    Expression valueExpression = ((value != null) && (value.GetType() != expressionBodyType))
			        ? (Expression)Expression.Convert(Expression.Constant(value), expressionBodyType)
			        : (Expression)Expression.Constant(value);

                    Expression<Func<TEntity, bool>> pairByConditionExpression = (Expression<Func<TEntity, bool>>)Expression.Lambda(
					Expression.Equal(ExpressionExt.ReplaceParameter(expression.Body, expression.Parameters[0], parameter).RemoveConvert(), valueExpression), // Expression.Constant nejde pro references
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

			TEntity dbEntity;
			IQueryable<TEntity> pairExpression = dataQueryable.Where(whereExpression);

			try
			{
				dbEntity = pairExpression.SingleOrDefault();
			}
			catch (InvalidOperationException) // The input sequence contains more than one element.
			{
				throw new InvalidOperationException($"More than one object found for \"{whereExpression}\".");
			}

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
		List<EntityType> entityTypes = ((IObjectContextAdapter)dbContext).ObjectContext.MetadataWorkspace.GetItems<EntityType>(DataSpace.OSpace).ToList(); // TODO: Duplikace kódu
		EntityType entityType = entityTypes.Single(item => typeof(TEntity) == (Type)item.GetType().GetProperty("ClrType", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(item)); // TODO: Duplikace kódu!

		List<string> properties = entityType.Properties.Except(entityType.KeyProperties.Where(item => item.StoreGeneratedPattern != StoreGeneratedPattern.None)).Select(item => item.Name).ToList();
		List<string> updateProperties;
		if (configuration.ExcludeUpdateExpressions != null)
		{
			// TODO: Hezčí kód
			// TODO: Duplikace vyhození KeyProperties
			List<string> exludedUpdateProperties = configuration.ExcludeUpdateExpressions.Select(item => ExpressionExt.GetMemberAccessMemberName(item)).ToList().Concat(entityType.KeyProperties.Select(item => item.Name)).ToList();
			updateProperties = properties.Except(exludedUpdateProperties).ToList();
		}
		else
		{
			updateProperties = properties.Except(entityType.KeyProperties.Select(item => item.Name)).ToList();
		}

		foreach (SeedDataPair<TEntity> pair in pairs)
		{
			Type dbEntityType = pair.DbEntity.GetType(); // očekáváme TEntity, snad jen v případě dědičnosti by mohl být potomek
			foreach (string property in (pair.IsNew ? properties : updateProperties))
			{
				object value = DataBinderExt.GetValue(pair.SeedEntity, property);
				dbEntityType.GetProperty(property).SetValue(pair.DbEntity, value); // tímto umožníme nastavit i membery s protected settery, což DataBinderExt neumí
			}
		}
	}

	/// <summary>
	/// Provede volání BeforeSaveActions.
	/// </summary>
	private void DoBeforeSaveActions<TEntity>(DataSeedConfiguration<TEntity> configuration, List<SeedDataPair<TEntity>> seedDataPairs)
		where TEntity : class
	{
		DoBeforeAfterSaveAction(configuration.BeforeSaveActions, seedDataPairs, seedDataPair => new BeforeSaveDataArgs<TEntity>(seedDataPair.SeedEntity, seedDataPair.DbEntity, seedDataPair.IsNew));
	}

	/// <summary>
	/// Provede volání AfterSaveActions.
	/// </summary>
	private void DoAfterSaveActions<TEntity>(DataSeedConfiguration<TEntity> configuration, List<SeedDataPair<TEntity>> seedDataPairs)
		where TEntity : class
	{
		DoBeforeAfterSaveAction(configuration.AfterSaveActions, seedDataPairs, pair => new AfterSaveDataArgs<TEntity>(pair.SeedEntity, pair.DbEntity, pair.IsNew));
	}

	private void DoBeforeAfterSaveAction<TEntity, TEventArgs>(List<Action<TEventArgs>> actions, List<SeedDataPair<TEntity>> seedDataPairs, Func<SeedDataPair<TEntity>, TEventArgs> eventArgsCreator)
		where TEntity : class
	{
		if (actions != null)
		{
			foreach (SeedDataPair<TEntity> pair in seedDataPairs)
			{
				TEventArgs args = eventArgsCreator(pair);

				foreach (Action<TEventArgs> action in actions)
				{
					action(args);
				}
			}
		}
	}


}
