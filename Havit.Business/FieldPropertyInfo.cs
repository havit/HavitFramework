using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Havit.Business.Query;
using Havit.Data.SqlServer;

namespace Havit.Business
{
	/// <summary>
	/// Reprezentuje sloupec v databázi,
	/// nese informace o daném sloupci a jeho vazbu na objektovou strukturu.
	/// </summary>
	public class FieldPropertyInfo : PropertyInfo, IFieldsBuilder, IOperand
	{
		/// <summary>
		/// Název sloupce v databázi.
		/// </summary>
		public string FieldName
		{
			get
			{
				CheckInitialization();
				return fieldName;
			}
		}
		private string fieldName;

		/// <summary>
		/// Udává, zda je možné uložit null hodnotu.
		/// </summary>
		public bool Nullable
		{
			get
			{
				CheckInitialization();
				return nullable;
			}
		}
		private bool nullable;

		/// <summary>
		/// Typ sloupce v databázi.
		/// </summary>
		public SqlDbType FieldType
		{
			get
			{
				CheckInitialization();
				return fieldType;
			}
		}
		private SqlDbType fieldType;

		/// <summary>
		/// Udává, zda je sloupec primárním klíčem.
		/// </summary>
		public bool IsPrimaryKey
		{
			get
			{
				CheckInitialization();
				return isPrimaryKey;
			}
		}
		private bool isPrimaryKey;

		/// <summary>
		/// Maximální délka řetězce (u typů varchar, nvarchar, apod.), případně velikost datového typu (u číselných typů).
		/// </summary>
		public int MaximumLength
		{
			get
			{
				CheckInitialization();
				return maximumLength;
			}

		}
		private int maximumLength;

		/// <summary>
		/// Inicializuje instanci sloupce.
		/// </summary>
		/// <param name="owner">Nadřazený objectInfo.</param>
		/// <param name="propertyName">Název property.</param>
		/// <param name="fieldName">Název sloupce v databázy.</param>
		/// <param name="isPrimaryKey">Indikuje, zda je sloupec primárním klíčem</param>
		/// <param name="fieldType">Typ databázového sloupce.</param>
		/// <param name="nullable">Indukuje, zda je povolena hodnota null.</param>
		/// <param name="maximumLength">Maximální délka dat databázového sloupce.</param>		
		[SuppressMessage("SonarLint", "S1117", Justification = "Není chybou mít parametr metody stejného jména ve třídě.")]
		public void Initialize(ObjectInfo owner, string propertyName, string fieldName, bool isPrimaryKey, SqlDbType fieldType, bool nullable, int maximumLength)
		{
			Initialize(owner, propertyName);
			this.fieldName = fieldName;
			this.nullable = nullable;
			this.fieldType = fieldType;
			this.isPrimaryKey = isPrimaryKey;
			this.maximumLength = maximumLength;
		}

		/// <summary>
		/// Vrátí řetězec pro vytažení daného sloupce z databáze.
		/// </summary>
		public virtual string GetSelectFieldStatement(DbCommand command)
		{
			CheckInitialization();
			return "[" + fieldName + "]";
		}

		string IOperand.GetCommandValue(DbCommand command, SqlServerPlatform sqlServerPlatform)
		{
			CheckInitialization();
			return "[" + fieldName + "]";
		}
	}
}
