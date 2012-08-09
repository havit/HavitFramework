using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

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
		/// Název tabulky nebo view, do které se tvoøí dotaz (FROM xxx).
		/// </summary>
		public string TableName
		{
			get { return tableName; }
			set { tableName = value; }
		}
		private string tableName;

		/// <summary>
		/// Seznam sloupcù (sekce SQL dotazu SELECT), které se vytáhnou v pøípadì, že kolekce fields je prázdná.
		/// </summary>
		internal string FieldsWhenEmpty
		{
			get { return fieldsWhenEmpty; }
			set { fieldsWhenEmpty = value; }
		}
		private string fieldsWhenEmpty;

		/// <summary>
		/// Seznam sloupcù, které jsou výsledkem dotazu (SELECT sloupec1, sloupec2...).
		/// </summary>
		public PropertyCollection Properties
		{
		  get { return properties; }
		}
		private PropertyCollection properties = new PropertyCollection();

		/// <summary>
		/// Podmínky dotazu (WHERE ...).
		/// </summary>
		public CompositeCondition Conditions
		{
		  get { return conditions; }
		}
		private AndCondition conditions = new AndCondition();

		/// <summary>
		/// Poøadí záznamù (ORDER BY ...).
		/// </summary>
		public OrderItemCollection OrderBy
		{
			get { return orderBy; }
		}
		private OrderItemCollection orderBy = new OrderItemCollection();
		#endregion

		#region PrepareCommand
		/// <summary>
		/// Vytvoøí dotaz, nastaví jej do commandu.
		/// Pøidá parametry.
		/// </summary>
		/// <param name="command"></param>
		public void PrepareCommand(DbCommand command)
		{
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
				return "SELECT";
			else
				return "SELECT TOP " + topRecords.Value.ToString();
		}

		/// <summary>
		/// Vrátí seznam sloupcù, které se z databáze získávají.
		/// </summary>
		protected virtual string GetSelectFieldsStatement(DbCommand command)
		{
			if (properties.Count == 0)
				return fieldsWhenEmpty;

			StringBuilder fieldsBuilder = new StringBuilder();			
			for (int i = 0; i < properties.Count; i++)				
			{
				if (i > 0)
					fieldsBuilder.Append(", ");
				fieldsBuilder.Append(properties[i].GetSelectFieldStatement(command));
			}
			return fieldsBuilder.ToString();
		}

		/// <summary>
		/// Vrátí sekci SQL dotazu FROM.
		/// </summary>
		protected virtual string GetFromStatement(DbCommand command)
		{
			return String.Format("FROM {0}", tableName);
		}

		/// <summary>
		/// Vrátí sekci SQL dotazu WHERE.
		/// </summary>
		public virtual string GetWhereStatement(DbCommand command)
		{
			StringBuilder whereBuilder = new StringBuilder();
			
			Conditions.GetWhereStatement(command, whereBuilder);
			if (whereBuilder.Length > 0)
				whereBuilder.Insert(0, "WHERE ");
						
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
				orderByBuilder.Append(orderBy[i].GetSqlOrderBy());
			}
			return orderBy.ToString();
		}
	}
}
