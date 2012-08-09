using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Havit.Business.Query;

namespace Havit.Business
{
	/// <summary>
	/// Reprezentuje sloupec v databázi,
	/// nese informace o daném sloupci a jeho vazbu na objektovou strukturu.
	/// </summary>
	[Serializable]
	public class FieldPropertyInfo : PropertyInfo, IFieldsBuilder, IOperand
	{
		/// <summary>
		/// Vytvoøí instanci sloupce.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="isPrimaryKey"></param>
		/// <param name="nullable"></param>
		/// <param name="fieldType"></param>
		public FieldPropertyInfo(string fieldName, bool isPrimaryKey, SqlDbType fieldType, bool nullable)
		{
			this.fieldName = fieldName;
			this.nullable = nullable;
			this.fieldType = fieldType;
			this.isPrimaryKey = isPrimaryKey;
		}

		/// <summary>
		/// Název sloupce v databázi.
		/// </summary>
		public string FieldName
		{
			get { return fieldName; }
		}
		private string fieldName;

		/// <summary>
		/// Udává, zda je možné uložit null hodnotu.
		/// </summary>
		public bool Nullable
		{
			get { return nullable; }
		}
		private bool nullable;

		/// <summary>
		/// Typ sloupce v databázi.
		/// </summary>
		public SqlDbType FieldType
		{
			get { return fieldType; }
		}
		private SqlDbType fieldType;

		/// <summary>
		/// Udává, zda je sloupec primárním klíèem.
		/// </summary>
		public bool IsPrimaryKey
		{
			get { return isPrimaryKey; }
		}
		private bool isPrimaryKey;

		/// <summary>
		/// Vrátí øetìzec pro vytažení daného sloupce z databáze.
		/// </summary>
		public virtual string GetSelectFieldStatement(DbCommand command)
		{
			return fieldName;
		}

		string IOperand.GetCommandValue(DbCommand command)
		{
			return fieldName;
		}
	}
}
