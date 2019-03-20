using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using Havit.Data;
using Havit.Data.SqlServer;
using Havit.Diagnostics.Contracts;

namespace Havit.Business.Query
{
	/// <summary>
	/// Objektová struktura SQL dotazu.
	/// Obsahuje seznam properties, které se mají z databáze získat, seznam podmínek fitrující záznamy
	/// a řazení v jakém se mají záznamy (objekty) získat.
	/// </summary>
	public class QueryParams
	{
		/// <summary>
		/// Instance třídy ObjectInfo nesoucí informace o tom, z jaké tabulky se bude dotaz dotazovat.
		/// </summary>
		public ObjectInfo ObjectInfo
		{
			get { return objectInfo; }
			set { objectInfo = value; }
		}
		private ObjectInfo objectInfo;

		/// <summary>
		/// Maximální počet záznamů, který se vrací z databáze - (SELECT TOP n ...).
		/// </summary>
		public int? TopRecords
		{
			get { return topRecords; }
			set { topRecords = value; }
		}
		private int? topRecords;

		/// <summary>
		/// Udává, zda se mají vracet i záznamy označené za smazané.
		/// Výchozí hodnota je false, smazané záznamy se nevrací.
		/// </summary>
		public bool IncludeDeleted
		{
			get { return includeDeleted; }
			set { includeDeleted = value; }
		}
		private bool includeDeleted = false;

		/// <summary>
		/// Seznam sloupců, které jsou výsledkem dotazu (SELECT sloupec1, sloupec2...).
		/// </summary>
		public PropertyInfoCollection Properties
		{
			get
			{
				//Contract.Ensures(Contract.Result<PropertyInfoCollection>() != null);
				return properties;
			}
		}
		private readonly PropertyInfoCollection properties = new PropertyInfoCollection();

		/// <summary>
		/// Podmínky dotazu (WHERE ...).
		/// </summary>
		public ConditionList Conditions
		{
			get
			{
				//Contract.Ensures(Contract.Result<ConditionList>() != null);
				return conditions.Conditions;
			}
		}
		private readonly AndCondition conditions = new AndCondition();

		/// <summary>
		/// Pořadí záznamů (ORDER BY ...).
		/// </summary>
		public OrderByCollection OrderBy
		{
			get
			{
				//Contract.Ensures(Contract.Result<OrderByCollection>() != null);
				return orderBy;
			}
		}
		private readonly OrderByCollection orderBy = new OrderByCollection();

		/// <summary>
		/// Podle kolekce properties určí režim záznamů, které budou vráceny.
		/// Pro prázdnou kolekci vrací FullLoad, pro kolekci o jednom prvku, který je primárním klíčem, vrací Ghost. Jinak vrací PartialLoad.
		/// </summary>		
		public DataLoadPower GetDataLoadPower()
		{
			if (properties.Count == 0)
			{
				return DataLoadPower.FullLoad;
			}

			if (properties.Count == 1)
			{
				FieldPropertyInfo fieldPropertyInfo = properties[0] as FieldPropertyInfo;
				if ((fieldPropertyInfo != null) && (fieldPropertyInfo.IsPrimaryKey))
				{
					return DataLoadPower.Ghost;
				}
			}
			
			return DataLoadPower.PartialLoad;
		}

		/// <summary>
		/// Vytvoří dotaz, nastaví jej do commandu.
		/// Přidá parametry.
		/// </summary>
		public void PrepareCommand(DbCommand command, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
		{
			Contract.Requires<ArgumentNullException>(command != null, nameof(command));

			OnBeforePrepareCommand();

			StringBuilder commandBuilder = new StringBuilder();
			commandBuilder.Append(GetSelectStatement(command));
			commandBuilder.Append(" ");
			commandBuilder.Append(GetSelectFieldsStatement(command));
			commandBuilder.Append(" ");
			commandBuilder.Append(GetFromStatement(command));
			commandBuilder.Append(" ");
			//GetJoinStatement(commandBuilder, command);
			//commandBuilder.Append(" ");
			commandBuilder.Append(GetWhereStatement(command, sqlServerPlatform, commandBuilderOptions));
			commandBuilder.Append(" ");
			commandBuilder.Append(GetOrderByStatement(command));
			//commandBuilder.Append(" ");
			//commandBuilder.Append(GetOptionStatementStatement(command, sqlServerPlatform, commandBuilderOptions));
			while ((commandBuilder.Length > 0 /* nicméně zde se na nulu nikdy nemůžeme dostat */) && (commandBuilder[commandBuilder.Length - 1] == ' '))
			{
				commandBuilder.Remove(commandBuilder.Length - 1, 1);
			}
			commandBuilder.Append(";");

			OnAfterPrepareCommand(command, commandBuilder);

			command.CommandText = command.CommandText + commandBuilder.ToString();
		}

		/// <summary>
		/// Slouží k přípravě objektu před začátkem skládání databázového dotazu.
		/// </summary>
		public virtual void OnBeforePrepareCommand()
		{
		}

		/// <summary>
		/// Slouží k dokončení skládání databázového dotazu.
		/// Voláno po poskládání databázového dotazu, naskládání parametrů do commandu,
		/// ale PŘED nastavením property command.CommandText. Je tak možno databázový
		/// dotaz upravit na poslední chvíli.
		/// </summary>
		public virtual void OnAfterPrepareCommand(DbCommand command, StringBuilder commandBuilder)
		{
		}

		/// <summary>
		/// Vrátí sekci SQL dotazu SELECT.
		/// </summary>
		protected virtual string GetSelectStatement(DbCommand command)
		{
			Debug.Assert(command != null);

			if (topRecords == null)
			{
				return "SELECT";
			}
			else
			{
				return String.Format("SELECT TOP ({0})", topRecords.Value.ToString(CultureInfo.InvariantCulture));
			}
		}

		/// <summary>
		/// Vrátí seznam sloupců, které se z databáze získávají.
		/// </summary>
		protected virtual string GetSelectFieldsStatement(DbCommand command)
		{
			Debug.Assert(command != null);

			PropertyInfoCollection queryProperties = properties;

			if (queryProperties.Count == 0)
			{
				queryProperties = objectInfo.Properties;
			}

			StringBuilder fieldsBuilder = new StringBuilder();
			for (int i = 0; i < queryProperties.Count; i++)				
			{
				if (i > 0)
				{
					fieldsBuilder.Append(", ");
				}

				if (queryProperties[i] is IFieldsBuilder)
				{
					fieldsBuilder.Append(((IFieldsBuilder)queryProperties[i]).GetSelectFieldStatement(command));
				}
			}
			return fieldsBuilder.ToString();
		}

		/// <summary>
		/// Vrátí sekci SQL dotazu FROM.
		/// </summary>
		protected virtual string GetFromStatement(DbCommand command)
		{
			Debug.Assert(command != null);

			if (String.IsNullOrEmpty(objectInfo.DbSchema))
            {
                return String.Format(CultureInfo.InvariantCulture, "FROM [{0}]", objectInfo.DbTable);
            }
            else
            {
                return String.Format(CultureInfo.InvariantCulture, "FROM [{0}].[{1}]", objectInfo.DbSchema, objectInfo.DbTable);
            }
		}

		/// <summary>
		/// Vrátí sekci SQL dotazu WHERE.
		/// </summary>
		public virtual string GetWhereStatement(DbCommand command, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
		{
			Debug.Assert(command != null);

			StringBuilder whereBuilder = new StringBuilder();
			Condition deletedCondition = null;

			if (!includeDeleted && objectInfo.DeletedProperty != null)
			{
				if (objectInfo.DeletedProperty.FieldType == System.Data.SqlDbType.Bit)
				{
					deletedCondition = BoolCondition.CreateFalse(objectInfo.DeletedProperty);
					Conditions.Add(deletedCondition);
				}

				if ((objectInfo.DeletedProperty.FieldType == System.Data.SqlDbType.DateTime) || (objectInfo.DeletedProperty.FieldType == System.Data.SqlDbType.SmallDateTime))
				{
					deletedCondition = NullCondition.CreateIsNull(objectInfo.DeletedProperty);
					Conditions.Add(deletedCondition);
				}
			}

			conditions.GetWhereStatement(command, whereBuilder, sqlServerPlatform, commandBuilderOptions);
			if (whereBuilder.Length > 0)
			{
				whereBuilder.Insert(0, "WHERE ");
			}

			if (deletedCondition != null)
			{
				Conditions.Remove(deletedCondition);
			}
						
			return whereBuilder.ToString();
		}

		/// <summary>
		/// Vrátí sekci SQL dotazu ORDER BY.
		/// </summary>
		protected virtual string GetOrderByStatement(DbCommand command)
		{
			Debug.Assert(command != null);

			if (orderBy.Count == 0)
			{
				return String.Empty;
			}

			StringBuilder orderByBuilder = new StringBuilder();
			orderByBuilder.Append("ORDER BY ");
			for (int i = 0; i < orderBy.Count; i++)
			{
				if (i > 0)
				{
					orderByBuilder.Append(", ");
				}

				orderByBuilder.Append(orderBy[i].Expression);
				if (orderBy[i].Direction == Havit.Collections.SortDirection.Descending)
				{
					orderByBuilder.Append(" DESC");
				}
			}
			return orderByBuilder.ToString();
		}

		//#region GetOptionStatementStatement
		///// <summary>
		///// Vrátí sekci SQL dotazu OPTION - použito na OPTION (RECOMPILE).
		///// OPTION (RECOMPILE): workaround pro http://connect.microsoft.com/SQLServer/feedback/ViewFeedback.aspx?FeedbackID=256717
		///// </summary>
		//protected virtual string GetOptionStatementStatement(DbCommand command, SqlServerPlatform sqlServerPlatform, CommandBuilderOptions commandBuilderOptions)
		//{
		//	Debug.Assert(command != null);

		//	// jen pro SQL Server 2005 a to ještě nesmí být požadováno ReferenceInAsEnumeretaion.
		//	if (sqlServerPlatform == SqlServerPlatform.SqlServer2005)
		//	{
		//		// ideálně bychom ještě otestovali, zda obsahuje podmínku s IntArray...
		//		return "OPTION (RECOMPILE)";
		//	}

		//	return "";
		//}
		//#endregion

	}
}
