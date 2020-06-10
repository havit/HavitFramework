using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;
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
		internal IDbContext DbContext { get; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <remarks>
		/// Chceme transientní DbContext, abychom od sebe odstínili jednotlivé seedy.
		/// Ale dále se k němu chováme jako k IDbContextu.
		/// </remarks>
		public DbDataSeedPersister(IDbContextTransient dbContext)
		{
			this.DbContext = dbContext;
		}

		/// <summary>
		/// Dle předpisu seedování dat (konfigurace) provede persistenci seedovaných dat.
		/// </summary>
		public void Save<TEntity>(DataSeedConfiguration<TEntity> configuration)
			where TEntity : class
		{
			CheckConditions(configuration);

			IDbSet<TEntity> dbSet = DbContext.Set<TEntity>();
			List<PairExpressionWithCompilation<TEntity>> pairByExpressionsWithCompilations = configuration.PairByExpressions.ToPairByExpressionsWithCompilations();
			List<SeedDataPair<TEntity>> seedDataPairs = PairWithDbData(configuration.SeedData, dbSet.AsQueryable(), pairByExpressionsWithCompilations, configuration.CustomQueryCondition);
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

			Update(configuration, seedDataPairsToUpdate);
			dbSet.AddRange(unpairedSeedDataPairs.Select(item => item.DbEntity).ToArray());

			DoBeforeSaveActions(configuration, seedDataPairs);
			DbContext.SaveChanges();
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
		/// Zkontrolujeme, že se nepokoušíme párovat podle klíče, pokud jej nemůžeme vložit (primární klíč, pokud je nastaven na autoincrement).
		/// </summary>
		internal void CheckConditions<TEntity>(DataSeedConfiguration<TEntity> configuration)
		{
			Contract.Requires<ArgumentNullException>(configuration != null, nameof(configuration));
			Contract.Requires<InvalidOperationException>((configuration.PairByExpressions != null) && (configuration.PairByExpressions.Count > 0), "Expression to pair object missing (missing PairBy method call).");

			var entityType = DbContext.Model.FindEntityType(typeof(TEntity));
			var propertiesForInserting = GetPropertiesForInserting(entityType).Select(item => item.PropertyInfo.Name).ToList();

			Contract.Assert<InvalidOperationException>(configuration.PairByExpressions.TrueForAll(expression => propertiesForInserting.Contains(GetPropertyName(expression.Body.RemoveConvert()))), "Expression to pair object contains not supported property (only properties which can be inserted are allowed).");
		}

		/// <summary>
		/// Provede párování předpisu seedovaných dat s existujícími objekty.
		/// </summary>
		internal List<SeedDataPair<TEntity>> PairWithDbData<TEntity>(IEnumerable<TEntity> seedData, IQueryable<TEntity> databaseDataQueryable, List<PairExpressionWithCompilation<TEntity>> pairByExpressions, Expression<Func<TEntity, bool>> customQueryCondition)
			where TEntity : class
		{
			// vezmeme data k seedování, přidáme k nim klíč pro párování (pro pohodlný left join)
			List<DataWithPairByValues<TEntity>> seedDataWithPairByValues = ToDataWithPairByValues(seedData, pairByExpressions);
			// zkontrolujeme, zda data k seedování neobsahují duplicity
			seedDataWithPairByValues.ThrowIfContainsDuplicates("Source data contains duplicates in pair by expression.");

			List<DataWithPairByValues<TEntity>> databaseDataWithPairByValues;

			// načteme databázová data k seedovaným datům
			List<TEntity> databaseData = PairWithDbData_LoadDatabaseData(seedData, seedDataWithPairByValues, databaseDataQueryable, pairByExpressions, customQueryCondition, out databaseDataWithPairByValues);

			// vezmeme databázová data, přidáme k nim klíč pro párování (pro pohodlný left join)
			// Protože v jiné části kódu již databaseDataWithPairByValues sestavujeme, může zde být výkonová zkratka, ať je nesestavujeme znovu (pak není databaseDataWithPairByValues null).
			// Pokud jsme je nesestavili jinde, sestavíme je zde.
			if (databaseDataWithPairByValues == null)
			{
				databaseDataWithPairByValues = ToDataWithPairByValues(databaseData, pairByExpressions);
			}

			// zkontrolujeme, zda databázová data neobsahují duplicity
			databaseDataWithPairByValues.ThrowIfContainsDuplicates("Database data contains duplicates in pair by expression.");

			// ke zdrojovám datům připojíme databázová, porovnání proběhne podle PairBy
			return seedDataWithPairByValues.LeftJoin(
				databaseDataWithPairByValues,
				seedDataItem => seedDataItem.PairByValues, // overrides Equals
				databaseDataItem => databaseDataItem.PairByValues, // overrides Equals
				(seedDataItem, databaseDataItem) => new SeedDataPair<TEntity>
				{
					SeedEntity = seedDataItem.OriginalItem,
					DbEntity = databaseDataItem?.OriginalItem // ? ... null, pokud ke zdrojovám datům neexistuje záznam v databázi
				}).ToList();
		}

		/// <summary>
		/// Ke každému objektu přidáme hodnotu (resp. hodnoty) dle PairBy.
		/// </summary>
		private List<DataWithPairByValues<TEntity>> ToDataWithPairByValues<TEntity>(IEnumerable<TEntity> seedData, List<PairExpressionWithCompilation<TEntity>> pairByExpressions)
			where TEntity : class
		{
			return seedData.Select(item => new DataWithPairByValues<TEntity>
			{
				OriginalItem = item,
				PairByValues = new PairByValues(pairByExpressions.Select(pairByExpression => pairByExpression.CompiledLambda.Invoke(item)).ToArray())
			}).ToList();
		}

		/// <summary>
		/// Provede načtení dat pro párování seedovaných dat s databázovými entitami.
		/// </summary>
		private List<TEntity> PairWithDbData_LoadDatabaseData<TEntity>(IEnumerable<TEntity> seedData, List<DataWithPairByValues<TEntity>> seedDataWithPairByValues, IQueryable<TEntity> databaseDataQueryable, List<PairExpressionWithCompilation<TEntity>> pairByExpressionsWithCompilations, Expression<Func<TEntity, bool>> customQueryCondition, out List<DataWithPairByValues<TEntity>> databaseDataWithPairByValues)
			where TEntity : class
		{
			databaseDataWithPairByValues = null;

			return (customQueryCondition == null)
				? PairWithDbData_LoadDatabaseData_AutoQueryCondition(seedData, databaseDataQueryable, pairByExpressionsWithCompilations)
				: PairWithDbData_LoadDatabaseData_CustomQueryCondition(seedDataWithPairByValues, databaseDataQueryable, pairByExpressionsWithCompilations, customQueryCondition, out databaseDataWithPairByValues);
		}

		/// <summary>
		/// Provede načtení dat pro párování seedovaných dat s databázovými entitami.
		/// Objekty načteny v dávkách, čímž dochází k optimalizaci množství prováděných databázových operací.
		/// </summary>
		private List<TEntity> PairWithDbData_LoadDatabaseData_AutoQueryCondition<TEntity>(IEnumerable<TEntity> seedData, IQueryable<TEntity> databaseDataQueryable, List<PairExpressionWithCompilation<TEntity>> pairByExpressionsWithCompilations)
			where TEntity : class
		{
			List<TEntity> dbEntities = new List<TEntity>();

			// Chunkify(1000) --> SQL Server 2008: Some part of your SQL statement is nested too deeply. Rewrite the query or break it up into smaller queries.
			// Proto došlo ke změně na .Chunkify(100), správné číslo hledáme.
			foreach (TEntity[] chunk in seedData.Chunkify(100))
			{
				Expression<Func<TEntity, bool>> chunkWhereExpression = PairWithDbData_LoadDatabaseData_BuildWhereCondition(chunk, pairByExpressionsWithCompilations);
				dbEntities.AddRange(databaseDataQueryable.Where(chunkWhereExpression).ToList());
			}

			return dbEntities;
		}

		/// <summary>
		/// Provede načtení dat pro párování seedovaných dat s databázovými entitami.
		/// Použije uživatelsky definovaný filtr.
		/// </summary>
		private List<TEntity> PairWithDbData_LoadDatabaseData_CustomQueryCondition<TEntity>(List<DataWithPairByValues<TEntity>> seedDataWithPairByValues, IQueryable<TEntity> databaseDataQueryable, List<PairExpressionWithCompilation<TEntity>> pairByExpressionsWithCompilations, Expression<Func<TEntity, bool>> customDatabaseCondition, out List<DataWithPairByValues<TEntity>> databaseDataWithPairByValues)
			where TEntity : class
		{
			// načteme entity dle podmínky, můžeme však načíst více dat, než kolik budeme dále párovat
			List<TEntity> loadedDbEntities = databaseDataQueryable.Where(customDatabaseCondition).ToList();

			// potřebujeme zajistit, aby se nám vrátila jen data, která budeme párovat s seedovaným hodnotam, ale žádná jiná
			// (dále kontrolujeme duplicitu párovacích klíčů, což může vadit, pokud načteme celou tabulku kde jsou nepárované záznamy bez párovacího klíče)

			// sestavíme hashset ze PairByValues seedovaných dat (nikoliv načtených dat)
			HashSet<PairByValues> seedDataWithPairByValueHashSet = new HashSet<PairByValues>(seedDataWithPairByValues.Select(item => item.PairByValues));

			// získáme z načtených dat PairByValues
			List<DataWithPairByValues<TEntity>> loadedDbEntitiesWithPairByValues = ToDataWithPairByValues(loadedDbEntities, pairByExpressionsWithCompilations);
			// a zafiltrujeme je jen na takové entity, které mají PairByValues mezi seedovanými daty (tím dostaneme pryč objekty, které nemají být seedovány)
			List<TEntity> filteredLoadedDbEntities = loadedDbEntitiesWithPairByValues.Where(item => seedDataWithPairByValueHashSet.Contains(item.PairByValues /* overrides Equals */)).Select(item => item.OriginalItem).ToList();

			// performance zkratka - pokud načtená a filtrovaná data odpovídají, vrátíme loadedDbEntitiesWithPairByValues, ať se nemusí sestavovat znovu
			// out parameter
			databaseDataWithPairByValues = (loadedDbEntities.Count == filteredLoadedDbEntities.Count) // všechny načtené objekty byly vyfiltrovány, pak můžeme sestavené PairByValues vratít
				? loadedDbEntitiesWithPairByValues
				: null; // objekt můžeme vrátit

			return filteredLoadedDbEntities;
		}

		/// <summary>
		/// Sestaví podmínku pro vyfiltrování dat (seedData) při zadaných párovacích výrazech.
		/// </summary>
		private Expression<Func<TEntity, bool>> PairWithDbData_LoadDatabaseData_BuildWhereCondition<TEntity>(IEnumerable<TEntity> seedData, List<PairExpressionWithCompilation<TEntity>> pairByExpressionsWithCompilations)
			where TEntity : class
		{
			ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");

			// Dříve jsme stavěli podmínku jako A OR B OR C OR D OR E, což při uvážení OR jako binárního operátoru vypadalo takto:
			// ((((A OR B) OR C) OR D) OR E)
			// Při tomto způsobu roste hloubka výrazu lineárně, tj. zdvojnásobíme počet seedovaných entit, zdvojnásobí se hloubka stromu.
			// Jenže hloubka stromu může být maximálně okolo 1000 položek, jinak dostáváme při kompilaci dotazu StackOverflowException.
			// Proto nyní stavíme z podmínek strom, jehož hloubka roste logaritmicky vůči počtu seedovaných položek.

			TEntity[] seedDataArray = seedData.ToArray();
			return PairWithDbData_LoadDatabaseData_BuildWhereCondition_Recursive(seedDataArray, pairByExpressionsWithCompilations, 0, seedDataArray.Length - 1, parameter);
		}

		private Expression<Func<TEntity, bool>> PairWithDbData_LoadDatabaseData_BuildWhereCondition_Recursive<TEntity>(TEntity[] seedDataArray, List<PairExpressionWithCompilation<TEntity>> pairByExpressionsWithCompilations, int indexFrom, int indexTo, ParameterExpression parameter)
			where TEntity : class
		{
			if (indexFrom == indexTo)
			{
				return PairWithDbData_LoadDatabaseData_BuildWhereCondition_Recursive_BuildItem(seedDataArray[indexFrom], pairByExpressionsWithCompilations, parameter);
			}
			else
			{
				return (Expression<Func<TEntity, bool>>)Expression.Lambda(
					Expression.OrElse(
						PairWithDbData_LoadDatabaseData_BuildWhereCondition_Recursive(seedDataArray, pairByExpressionsWithCompilations, indexFrom, (indexFrom + indexTo) / 2, parameter).Body,
						PairWithDbData_LoadDatabaseData_BuildWhereCondition_Recursive(seedDataArray, pairByExpressionsWithCompilations, ((indexFrom + indexTo) / 2) + 1, indexTo, parameter).Body
					), parameter);
			}
		}

		private Expression<Func<TEntity, bool>> PairWithDbData_LoadDatabaseData_BuildWhereCondition_Recursive_BuildItem<TEntity>(TEntity seedEntity, List<PairExpressionWithCompilation<TEntity>> pairByExpressionsWithCompilations, ParameterExpression parameter)
			where TEntity : class
		{
			Expression<Func<TEntity, bool>> seedEntityWhereExpression = null;

			for (int i = 0; i < pairByExpressionsWithCompilations.Count; i++)
			{
				Expression<Func<TEntity, object>> expression = pairByExpressionsWithCompilations[i].Expression;
				Func<TEntity, object> lambda = pairByExpressionsWithCompilations[i].CompiledLambda;

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

			return seedEntityWhereExpression;
		}


		/// <summary>
		/// Provede vytvoření či aktualizaci dat dle předpisu seedování.
		/// </summary>
		private void Update<TEntity>(DataSeedConfiguration<TEntity> configuration, IEnumerable<SeedDataPair<TEntity>> pairs)
			where TEntity : class
		{
			// current entity type from model
			IEntityType entityType = DbContext.Model.FindEntityType(typeof(TEntity));

			List<IProperty> propertiesForInserting = GetPropertiesForInserting(entityType);
			List<IProperty> propertiesForUpdating = GetPropertiesForUpdating<TEntity>(entityType,
				(configuration.ExcludeUpdateExpressions ?? Enumerable.Empty<Expression<Func<TEntity, object>>>())
				.Concat((configuration.PairByExpressions ?? Enumerable.Empty<Expression<Func<TEntity, object>>>()))
				.ToList());

			// we will set 
			foreach (SeedDataPair<TEntity> pair in pairs)
			{
				Type dbEntityType = pair.DbEntity.GetType(); // očekáváme TEntity, snad jen v případě dědičnosti by mohl být potomek
				foreach (IProperty property in (pair.IsNew ? propertiesForInserting : propertiesForUpdating))
				{
					object value = DataBinderExt.GetValue(pair.SeedEntity, property.Name);
					dbEntityType.GetProperty(property.Name).SetValue(pair.DbEntity, value); // tímto umožníme nastavit i membery s protected settery, což DataBinderExt neumí
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
			DoBeforeAfterSaveAction(configuration.AfterSaveActions, seedDataPairs, seedDataPair => new AfterSaveDataArgs<TEntity>(seedDataPair.SeedEntity, seedDataPair.DbEntity, seedDataPair.IsNew));
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

		/// <summary>
		/// Vrátí název vlastnosti, která je reprezentována daným výrazem.
		/// </summary>
		internal string GetPropertyName(Expression item)
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

		/// <summary>
		/// Vrátí seznam vlasností, které můžeme vložit.
		/// </summary>		
		/// <remarks>
		/// Nelze se spolehnout na ValueGenerated.OnAdd, protože podle https://github.com/aspnet/EntityFrameworkCore/issues/5366 platí:
		/// ValueGenerated.OnAdd does mean that a value will be generated if the value set is the CLR default for the property type.
		/// Proto nepoužijeme jen ty vlastnosti, o kterých víme, že je určitě nelze vložit - identitu.
		/// </remarks>
		internal List<IProperty> GetPropertiesForInserting(IEntityType entityType)
		{
			Contract.Requires<ArgumentNullException>(entityType != null, nameof(entityType));

			return entityType
				.GetProperties()
				.Where(item => !item.IsShadowProperty())
				.Where(p => !PropertyIsIdentity(p))				
				.ToList();
		}

		/// <summary>
		/// Vrátí seznam vlasností, které můžeme aktualizovat.
		/// Nelze aktualizovat vlastnosti/sloupce, které jsou primárním klíčem.
		/// </summary>		
		internal List<IProperty> GetPropertiesForUpdating<TEntity>(IEntityType entityType, List<Expression<Func<TEntity, object>>> excludedProperties)
		{
			Contract.Requires<ArgumentNullException>(entityType != null, nameof(entityType));

			List<IProperty> result = entityType
				.GetProperties()
				.Where(item => !item.IsShadowProperty())
				.Where(p => !p.IsKey()) // hodnotu primárního klíče nelze aktualizovat
										// .Where(p => !PropertyIsIdentity(p)) - není potřeba, neaktualizujeme žádný PK, natož identitu
				.ToList();

			if (excludedProperties != null)
			{
				result = result
					.Where(p => !excludedProperties.Select(exclude => GetPropertyName(exclude.Body.RemoveConvert())).Contains(p.Name))
					.ToList();
			}

			return result;
		}

		internal bool PropertyIsIdentity(IProperty property)
		{
			return property.ClrType == typeof(Int32) // identitu uvažujeme jen pro Int32
				&& property.IsPrimaryKey() // byť to není správné, uvažujeme identitu jen na primárním klíči
				&& property.ValueGenerated.HasFlag(ValueGenerated.OnAdd) // pokud má nastaveno OnAdd, může mít použito autoincrement
				&& (property.GetDefaultValue() == null) // ovšem to jen tehdy, pokud nemá použitu nějakou jinou defaultní hodnotu výrazem (třeba prázdný řetězec)
				&& String.IsNullOrEmpty(property.GetDefaultValueSql()); // a ovšem to jen tehdy, pokud nemá použitu nějakou jinou defaultní hodnotu (třeba sekvenci)
		}
	}
}