using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Havit.Linq.Expressions;
using Havit.Model.Localizations;

namespace Havit.Data.Patterns.DataSeeds
{
	internal class DataSeedFor<TEntity> : IDataSeedFor<TEntity>, IDataSeedForPaired<TEntity>
		where TEntity : class
	{
		/// <summary>
		/// Konfigurace seedování dat.
		/// </summary>
		public DataSeedConfiguration<TEntity> Configuration { get; private set; }

		#pragma warning disable SA1300 // Element must begin with upper-case letter
		internal Dictionary<string, object> _childDataForsRegistry { get; private set; } = new Dictionary<string, object>();
		#pragma warning restore SA1300 // Element must begin with upper-case letter

		/// <summary>
		/// Konstuktor.
		/// </summary>
		/// <param name="data">Objekty, které mají být seedovány.</param>
		public DataSeedFor(TEntity[] data)
		{
			this.Configuration = new DataSeedConfiguration<TEntity>(data);

			InitializeDefaults();
		}

		/// <summary>
		/// Inicializuje výchozí hodnoty pro seedování (na základě datového typu).
		/// <list type="bullet">
		///		<item>
		///			<description>Symbol - pokud existuje vlastnost pojmenovaná Symbol, je automaticky použita pro párování.</description>
		///		</item>
		///		<item>
		///			<description>ILocalization - pokud třída implementuje ILocalization&lt;,&gt;, je použito pro párování vlastností ParentId a LanguageId. Dále je zajišťěno seedování kolekce Localizations.</description>
		///		</item>
		/// </list>
		/// </summary>
		private void InitializeDefaults()
		{
			// pokud je definována vlastnost Symbol, použijeme ji jako výchozí hodnotu pro párování
			if (typeof(TEntity).GetProperty("Symbol", BindingFlags.Public | BindingFlags.Instance) != null)
			{
				ParameterExpression parameter = Expression.Parameter(typeof(TEntity));
				Expression<Func<TEntity, object>> symbolExpression = (Expression<Func<TEntity, object>>)Expression.Lambda(Expression.Convert(Expression.Property(parameter, typeof(TEntity), "Symbol"), typeof(object)), parameter);
				PairBy(symbolExpression);
			}
			// jinak pokud jde o ILocalization, použijeme jako výchozí párování LanguageId a ParentId
			else if ((typeof(TEntity).GetInterfaces().Any(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ILocalization<,>))))
			{
				ParameterExpression parameter = Expression.Parameter(typeof(TEntity));
				// TODO: ParentId, LanguageId je EF specialita, interface nic takového nepředepisuje!
				Expression<Func<TEntity, object>> parentExpression = (Expression<Func<TEntity, object>>)Expression.Lambda(Expression.Convert(Expression.Property(parameter, typeof(TEntity), "ParentId"), typeof(object)), parameter);
				Expression<Func<TEntity, object>> languageExpression = (Expression<Func<TEntity, object>>)Expression.Lambda(Expression.Convert(Expression.Property(parameter, typeof(TEntity), "LanguageId"), typeof(object)), parameter);
				PairBy(parentExpression, languageExpression);
			}
			
			// (zde již nemá být "else if" - předchozí body definují výchozí párování - buď/anebo, následující definuje závislost)
			// pokud jde o lokalizovaný záznam
			Type localizationType = typeof(TEntity).GetInterfaces().FirstOrDefault(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(ILocalized<,>));
			if (localizationType != null)
			{
				Type localizedByType = localizationType.GenericTypeArguments[0]; // typ, kterým je tento lokalizován (tj. typ prvků v kolekci Localizations).
				
				ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "item");
				// item => item.Localizations
				LambdaExpression localizationsExpression = Expression.Lambda(Expression.Convert(Expression.Property(parameter, typeof(TEntity), "Localizations"), typeof(IEnumerable<>).MakeGenericType(localizedByType)), parameter);

				// metoda AndForAll je generická, avšak v kódu nejsme schopni generický typ získat (a obecnější typ pro přetypování jednak nelze použít a jednak by byl k ničemu)
				// proto metodu AndForAll zavoláme reflexí!
				// AndForInternal(item => item.Localizations, null)
				MethodInfo andForInternalMethod = this.GetType().GetMethod("AndForAll", BindingFlags.Public | BindingFlags.Instance);
				andForInternalMethod.MakeGenericMethod(localizedByType).Invoke(this, new object[]
				{
					localizationsExpression,
					null
				});
				
				AfterSave(data =>
				{
					dynamic seedEntity = data.SeedEntity;
					if (seedEntity.Localizations != null)
					{
						object dbEntityId = DataBinderExt.GetValue(data.PersistedEntity, "Id");
						foreach (object seedLocalization in seedEntity.Localizations)
						{
							DataBinderExt.SetValue(seedLocalization, "ParentId", dbEntityId);
						}						
					}
				});
			}
		}

		/// <summary>
		/// Nastaví způsob párování dat.
		/// </summary>
		public IDataSeedForPaired<TEntity> PairBy(params Expression<Func<TEntity, object>>[] pairByExpresssions)
		{
			Configuration.PairByExpressions = new List<Expression<Func<TEntity, object>>>(pairByExpresssions);
			return this;
		}

		/// <summary>
		/// Nastaví způsob párování dat.
		/// </summary>
		public IDataSeedForPaired<TEntity> AndBy(params Expression<Func<TEntity, object>>[] andByExpressions)
		{
			Configuration.PairByExpressions.AddRange(andByExpressions);
			return this;
		}

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		public IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, TReferencedEntity>> selector, Action<IDataSeedFor<TReferencedEntity>> configure)
			where TReferencedEntity : class
		{
			AndForInternal(
				selector,
				() => Configuration.SeedData.Select<TEntity, TReferencedEntity>(selector.Compile()),
				configure);

			return this;
		}

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		public IDataSeedFor<TEntity> AndForAll<TReferencedEntity>(Expression<Func<TEntity, IEnumerable<TReferencedEntity>>> selector, Action<IDataSeedFor<TReferencedEntity>> configure)
			where TReferencedEntity : class
		{
			AndForInternal(
				selector,
				() => Configuration.SeedData.SelectMany<TEntity, TReferencedEntity>(selector.Compile()),
				configure);

			return this;
		}

		private void AndForInternal<TReferencedEntity>(LambdaExpression selector, Func<IEnumerable<TReferencedEntity>> dataSelector, Action<IDataSeedFor<TReferencedEntity>> configure)
			where TReferencedEntity : class
		{
			DataSeedFor<TReferencedEntity> dataSeedFor;
			string key = ExpressionExt.ReplaceParameter(selector.Body, selector.Parameters[0], Expression.Parameter(typeof(TEntity), "item")).RemoveConvert().ToString();

			object tmp;
			if (_childDataForsRegistry.TryGetValue(key, out tmp))
			{
				dataSeedFor = (DataSeedFor<TReferencedEntity>)tmp;
			}
			else
			{
				TReferencedEntity[] newData = dataSelector().Where(item => item != null).Distinct().ToArray();
				dataSeedFor = new DataSeedFor<TReferencedEntity>(newData);
				_childDataForsRegistry.Add(key, dataSeedFor);

				ChildDataSeedConfigurationEntry childEntry = new ChildDataSeedConfigurationEntry((IDataSeedPersister persister) => persister.Save(dataSeedFor.Configuration));
				if (Configuration.ChildrenSeeds == null)
				{
					Configuration.ChildrenSeeds = new List<ChildDataSeedConfigurationEntry>();
				}
				Configuration.ChildrenSeeds.Add(childEntry);
			}

			configure?.Invoke(dataSeedFor);

		}

		/// <summary>
		/// Přidá do konfigurace vlastnosti, které nebudou aktualizovány. Ty tak slouží jen při zakládání nových objektů.
		/// </summary>
		public IDataSeedFor<TEntity> ExcludeUpdate(params Expression<Func<TEntity, object>>[] excludeUpdateExpressions)
		{
			if (Configuration.ExcludeUpdateExpressions == null)
			{
				Configuration.ExcludeUpdateExpressions = new List<Expression<Func<TEntity, object>>>();
			}
			Configuration.ExcludeUpdateExpressions.AddRange(excludeUpdateExpressions);
			return this;
		}

		/// <summary>
		/// Konfiguruje seedování tak, aby nedošlo k aktualizi existujících objektů, jsou pouze zakládány nové objekty.
		/// </summary>
		public IDataSeedFor<TEntity> WithoutUpdate()
		{
			Configuration.UpdateEnabled = false;
			return this;
		}

		/// <summary>
		/// Konfiguruje seedování tak, aby před fázi uložení seedovaných dat došlo k provedení callbacku (zavolání metody), která je parametrem metody.
		/// Before je volán nad všemi seedovanými objekty, nové objekty jsou již trackované.
		/// </summary>
		public IDataSeedFor<TEntity> BeforeSave(Action<BeforeSaveDataArgs<TEntity>> callback)
		{
			if (Configuration.BeforeSaveActions == null)
			{
				Configuration.BeforeSaveActions = new List<Action<BeforeSaveDataArgs<TEntity>>>();
			}
			Configuration.BeforeSaveActions.Add(callback);
			return this;
		}

		/// <summary>
		/// Konfiguruje seedování tak, aby po fázi uložení seedovaných dat došlo k provedení callbacku (zavolání metody), která je parametrem metody.
		/// AfterSave je volán nad všemi seedovanými objekty, i nad těmi, které nebyly uloženy (AfterSave je označení fáze seedování, nikoliv označení události nad objektem).
		/// </summary>
		public IDataSeedFor<TEntity> AfterSave(Action<AfterSaveDataArgs<TEntity>> callback)
		{
			if (Configuration.AfterSaveActions == null)
			{
				Configuration.AfterSaveActions = new List<Action<AfterSaveDataArgs<TEntity>>>();
			}
			Configuration.AfterSaveActions.Add(callback);
			return this;
		}

		/// <summary>
		/// Konfiguruje seedování tak, že se při načítání z databáze nepoužije WHERE podmínka, tj. načte se celá databázová tabulka.
		/// Použije se tam, kde seedujeme veškerá data v databázové tabulce (veškeré systémové číselníky).
		/// </summary>
		public IDataSeedFor<TEntity> NoDatabaseCondition()
		{
			return CustomDatabaseCondition((TEntity item) => true);
		}

		/// <summary>
		/// Konfiguruje seedování tak, že se při načítání z databáze použije konkrétní WHERE podmínka.
		/// Použije se tam, kde se vyplatí použít jednodušší podmínku popisující seedovaná data, než sestavovat dotaz dle seedovaných dat (např. item => item.Id &lt; 0).
		/// </summary>
		public IDataSeedFor<TEntity> CustomDatabaseCondition(Expression<Func<TEntity, bool>> predicate)
		{
			Configuration.CustomQueryCondition = predicate;
			return this;
		}
	}
}
