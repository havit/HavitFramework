using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Globalization;

namespace Havit.Business.Query
{
	/// <summary>
	/// Objektová struktura SQL dotazu.
	/// Obsahuje seznam properties, které se mají z databáze získat, seznam podmínek fitrující záznamy
	/// a øazení v jakém se mají záznamy (objekty) získat.
	/// </summary>
	[Serializable]	
	public class QueryParams
	{
		/// <summary>
		/// Instance tøídy ObjectInfo nesoucí informace o tom, z jaké tabulky se bude dotaz dotazovat.
		/// </summary>
		public ObjectInfo ObjectInfo
		{
			get { return objectInfo; }
			set { objectInfo = value; }
		}
		private ObjectInfo objectInfo;

		#region Parametry dotazu
		/// <summary>
		/// Maximální poèet záznamù, který se vrací z databáze - (SELECT TOP n ...).
		/// </summary>
		public int? TopRecords
		{
			get { return topRecords;}
			set { topRecords = value;}
		}
		private int? topRecords;

		/// <summary>
		/// Udává, zda se mají vracet i záznamy oznaèené za smazané.
		/// Výchozí hodnota je false, smazané záznamy se nevrací.
		/// </summary>
		public bool IncludeDeleted
		{
			get { return includeDeleted; }
			set { includeDeleted = value; }
		}
		private bool includeDeleted = false;

		/// <summary>
		/// Seznam sloupcù, které jsou výsledkem dotazu (SELECT sloupec1, sloupec2...).
		/// </summary>
		public PropertyInfoCollection Properties
		{
		  get { return properties; }
		}
		private PropertyInfoCollection properties = new PropertyInfoCollection();

		/// <summary>
		/// Podmínky dotazu (WHERE ...).
		/// </summary>
		public List<Condition> Conditions
		{
			get { return conditions.Conditions; }
		}
		private AndCondition conditions = new AndCondition();

		/// <summary>
		/// Poøadí záznamù (ORDER BY ...).
		/// </summary>
		public OrderByCollection OrderBy
		{
			get { return orderBy; }
		}
		private OrderByCollection orderBy = new OrderByCollection();
		#endregion

		#region PrepareCommand
		/// <summary>
		/// Vytvoøí dotaz, nastaví jej do commandu.
		/// Pøidá parametry.
		/// </summary>
		/// <param name="command"></param>
		public void PrepareCommand(DbCommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

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
			commandBuilder.Append(GetWhereStatement(command));
			commandBuilder.Append(" ");
			commandBuilder.Append(GetOrderByStatement(command));
			commandBuilder.Append(";");

			OnAfterPrepareCommand(command, commandBuilder);

			command.CommandText = command.CommandText + commandBuilder.ToString();
		}

		#endregion

		/// <summary>
		/// Slouží k pøípravì objektu pøed zaèátkem skládání databázového dotazu.
		/// </summary>
		public virtual void OnBeforePrepareCommand()
		{
		}

		/// <summary>
		/// Slouží k dokonèení skládání databázového dotazu.
		/// Voláno po poskládání databázového dotazu, naskládání parametrù do commandu,
		/// ale PØED nastavením property command.CommandText. Je tak možno databázový
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
			if (topRecords == null)
			{
				return "SELECT";
			}
			else
			{
				return "SELECT TOP " + topRecords.Value.ToString(CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Vrátí seznam sloupcù, které se z databáze získávají.
		/// </summary>
		protected virtual string GetSelectFieldsStatement(DbCommand command)
		{
			PropertyInfoCollection queryProperties = properties;

			if (queryProperties.Count == 0)
			{
				queryProperties = objectInfo.Properties;
			}

			StringBuilder fieldsBuilder = new StringBuilder();
			for (int i = 0; i < queryProperties.Count; i++)				
			{
				if (i > 0)
					fieldsBuilder.Append(", ");

				if (queryProperties[i] is IFieldsBuilder)
				{
#warning Pøepracovat tak, aby každá property obecnì mohl emitovat fieldy, které potøebuje ke své inicializaci.
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
			return String.Format(CultureInfo.InvariantCulture, "FROM {0}.{1}", objectInfo.DbSchema, objectInfo.DbTable);
		}

		/// <summary>
		/// Vrátí sekci SQL dotazu WHERE.
		/// </summary>
		public virtual string GetWhereStatement(DbCommand command)
		{
			StringBuilder whereBuilder = new StringBuilder();

			if (!includeDeleted && objectInfo.DeletedProperty != null)
			{
				if (objectInfo.DeletedProperty.FieldType == System.Data.SqlDbType.Bit)
				{
					Conditions.Add(BoolCondition.CreateFalse(objectInfo.DeletedProperty));
				}

				if ((objectInfo.DeletedProperty.FieldType == System.Data.SqlDbType.DateTime) || (objectInfo.DeletedProperty.FieldType == System.Data.SqlDbType.SmallDateTime))
				{
					Conditions.Add(NullCondition.CreateIsNull(objectInfo.DeletedProperty));
				}
			}

			conditions.GetWhereStatement(command, whereBuilder);
			if (whereBuilder.Length > 0)
			{
				whereBuilder.Insert(0, "WHERE ");
			}
						
			return whereBuilder.ToString();
		}

		/// <summary>
		/// Vrátí sekci SQL dotazu ORDER BY.
		/// </summary>
		protected virtual string GetOrderByStatement(DbCommand command)
		{
			if (orderBy.Count == 0)
				return String.Empty;
			
			StringBuilder orderByBuilder = new StringBuilder();
			orderByBuilder.Append("ORDER BY ");
			for (int i = 0; i < orderBy.Count; i++)
			{
				if (i > 0)
					orderByBuilder.Append(", ");

#warning není moc OOP
				orderByBuilder.Append(orderBy[i].Expression);
				if (orderBy[i].Direction == Havit.Collections.SortDirection.Descending)
					orderByBuilder.Append(" DESC");
			}
			return orderByBuilder.ToString();
		}
	}
}
