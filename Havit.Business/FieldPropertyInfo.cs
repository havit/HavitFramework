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
		/// <param name="fieldName">Název sloupce v databázy.</param>
		/// <param name="isPrimaryKey">Indikuje, zda je sloupec primárním klíèem</param>
		/// <param name="nullable">Indukuje, zda je povolena hodnota null.</param>
		/// <param name="fieldType">Typ databázového sloupce.</param>
		/// <param name="maximumLength">Maximální délka dat databázového sloupce.</param>		
		public FieldPropertyInfo(string fieldName, bool isPrimaryKey, SqlDbType fieldType, bool nullable, int maximumLength)
		{
			this.fieldName = fieldName;
			this.nullable = nullable;
			this.fieldType = fieldType;
			this.isPrimaryKey = isPrimaryKey;
			this.maximumLength = maximumLength;
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
		/// Maximální délka øetìzce (u typù varchar, nvarchar, apod.), pøípadnì velikost datového typu (u typù 
		/// </summary>
		public int MaximumLength
		{
			get { return maximumLength; }
		}
		private int maximumLength;

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
