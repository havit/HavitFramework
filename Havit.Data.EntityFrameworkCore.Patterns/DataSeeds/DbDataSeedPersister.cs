using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Havit.Data.Patterns.DataSeeds;
using Havit.Diagnostics.Contracts;
using Havit.Linq;
using Havit.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds
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
            CheckConditions(configuration);

            IDbSet<TEntity> dbSet = dbContext.Set<TEntity>();
            List<SeedDataPair<TEntity>> seedDataPairs = PairWithDbData(dbSet.AsQueryable(), configuration);
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
            dbSet.AddRange(unpairedSeedDataPairs.Select(item => item.DbEntity).ToArray());

            Update(configuration, seedDataPairsToUpdate);

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

            foreach (TEntity[] chunk in seedData.Chunkify(1000))
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
            // current entity type from model
            IEntityType entityType = dbContext.Model.FindEntityType(typeof(TEntity));

            IEnumerable<IProperty> allPropertiesOfEntity = entityType.GetProperties().Where(item => !item.IsShadowProperty);

            // properties that are NOT keys and are NOT generated in database in insert

			// TODO JK: Ověřit, zda je kompatibilní s použitím sekvence (při použití sekvence chceme nastavit hodnotu primárního klíče při zakládání záznamu)
			
			// novým záznamům můžeme nastavit hodnotu primárního klíče - pokud jej seedovaná data obsahují, chceme ji rovněž nastavit
			// nenastavujeme pouze hodnoty, které vznikají při uložení na serveru
            IEnumerable<IProperty> propertiesToAdd = allPropertiesOfEntity.Where(p => !p.ValueGenerated.HasFlag(ValueGenerated.OnAdd));

			// aktualizovaným záznamům hodnotu primárního klíče nikdy nenastavujeme, vznikla při založení záznamu
            IEnumerable<IProperty> propertiesToUpdate = allPropertiesOfEntity.Where(p => !p.IsPrimaryKey() && !p.ValueGenerated.HasFlag(ValueGenerated.OnUpdate));
            
            // if there are any properties, that should NOT update, we must exclude them from update list. But they remain in insert
            if (configuration.ExcludeUpdateExpressions != null)
            {
                propertiesToUpdate = propertiesToUpdate.Where(p => !configuration.ExcludeUpdateExpressions.Select(exclude => GetPropertyName(exclude.Body.RemoveConvert())).Contains(p.Name));
            }

            // we will set 
            foreach (SeedDataPair<TEntity> pair in pairs)
            {
                foreach (IProperty property in (pair.IsNew ? propertiesToAdd : propertiesToUpdate))
                {
                    object value = DataBinderExt.GetValue(pair.SeedEntity, property.Name);
                    DataBinderExt.SetValue(pair.DbEntity, property.Name, value);
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
