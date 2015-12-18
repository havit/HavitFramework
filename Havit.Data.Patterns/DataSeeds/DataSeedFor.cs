using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
				Delegate localizationsLambda = localizationsExpression.Compile();

				// metoda AndForInternal je generická, avšak v kódu nejsme schopni generický typ získat (a obecnější typ pro přetypování jednak nelze použít a jednak by byl k ničemu)
				// proto metodu AndForInternal zavoláme reflexí!
				// AndForInternal(item => item.Localizations, null)
				MethodInfo andForInternalMethod = this.GetType().GetMethod("AndForInternal", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
				andForInternalMethod.MakeGenericMethod(localizedByType).Invoke(this, new object[]
				{
					localizationsLambda,
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
		public IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, TReferencedEntity>> selector, Action<IDataSeedFor<TReferencedEntity>> data)
			where TReferencedEntity : class
		{
			// TODO: Umožnit reuse, pokud již záznam existuje
			// Pamatovat si množinu přidaných children podle expression a umožnit vytáhnout z "paměti". -> nebo si držet i selector a hledat dle selectoru
			Func<TEntity, TReferencedEntity> selectorLambda = selector.Compile();

			TReferencedEntity[] newData = Configuration.SeedData.Select<TEntity, TReferencedEntity>(selectorLambda).Where(item => item != null).Distinct().ToArray();
			DataSeedFor<TReferencedEntity> dataSeedFor = new DataSeedFor<TReferencedEntity>(newData);
			if (data != null)
			{
				data(dataSeedFor);
			}

			Action<IDataSeedPersister> saveAction = (IDataSeedPersister persister) => persister.Save(dataSeedFor.Configuration);
			ChildDataSeedConfigurationEntry childEntry = new ChildDataSeedConfigurationEntry(saveAction);

			if (Configuration.ChildrenSeeds == null)
			{
				Configuration.ChildrenSeeds = new List<ChildDataSeedConfigurationEntry>();
			}
			Configuration.ChildrenSeeds.Add(childEntry);

			return this;
		}
		
		private IDataSeedFor<TEntity> AndForInternal<TReferencedEntity>(Func<TEntity, IEnumerable<TReferencedEntity>> selectorLambda, Action<IDataSeedFor<TReferencedEntity>> configure)
			where TReferencedEntity : class
		{
			// TODO: Umožnit reuse, pokud již záznam existuje

			// Pamatovat si množinu přidaných children podle expression a umožnit vytáhnout z "paměti". -> nebo si držet i selector a hledat dle selectoru
			// Umožní mít default na language a zároveň mu něco doplnit

			TReferencedEntity[] newData = Configuration.SeedData.SelectMany<TEntity, TReferencedEntity>(selectorLambda).Where(item => item != null).Distinct().ToArray();
			DataSeedFor<TReferencedEntity> dataSeedFor = new DataSeedFor<TReferencedEntity>(newData);
			if (configure != null)
			{
				configure(dataSeedFor);
			}

			// TODO: Duplikovaný kód
			Action<IDataSeedPersister> saveAction = (IDataSeedPersister persister) => persister.Save(dataSeedFor.Configuration);
			ChildDataSeedConfigurationEntry childEntry = new ChildDataSeedConfigurationEntry(saveAction);

			if (Configuration.ChildrenSeeds == null)
			{
				Configuration.ChildrenSeeds = new List<ChildDataSeedConfigurationEntry>();
			}
			Configuration.ChildrenSeeds.Add(childEntry);
			
			return this;
		}

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		public IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, ICollection<TReferencedEntity>>> selector, Action<IDataSeedFor<TReferencedEntity>> data)
			where TReferencedEntity : class
		{
			return AndForInternal(selector.Compile(), data);
		}

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		public IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, IEnumerable<TReferencedEntity>>> selector, Action<IDataSeedFor<TReferencedEntity>> data)
			where TReferencedEntity : class
		{
			return AndForInternal(selector.Compile(), data);
		}

		/// <summary>
		/// Konfiguruje seedování tak, že doplňuje "child" seedování.
		/// </summary>
		public IDataSeedFor<TEntity> AndFor<TReferencedEntity>(Expression<Func<TEntity, List<TReferencedEntity>>> selector, Action<IDataSeedFor<TReferencedEntity>> data)
			where TReferencedEntity : class
		{
			return AndForInternal(selector.Compile(), data);
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
		/// Konfiguruje seedování tak, aby po uložení seedovaných dat došlo k provedení callbacku (zavolání metody), která je parametrem metody.
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
	}
}
