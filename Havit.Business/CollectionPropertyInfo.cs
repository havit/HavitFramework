using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Kolekce objektů třídy IProperty.
	/// </summary>
	public class CollectionPropertyInfo : PropertyInfo, IFieldsBuilder
	{
		/// <summary>
		/// Typ prvků kolekce.
		/// </summary>
		public Type ItemType
		{
			get
			{
				CheckInitialization();
				return itemType;
			}
		}
		private Type itemType;

		/// <summary>
		/// Část SQL dotazu pro vytažení hodnoty daného sloupce.
		/// </summary>
		public string CollectionSelectFieldStatement
		{
			get
			{
				CheckInitialization();
				return collectionSelectFieldStatement;
			}
		}
		private string collectionSelectFieldStatement;

		/// <summary>
		/// Vytvoří instanci CollectionProperty.
		/// </summary>
		/// <param name="owner">Nadřazený objectInfo.</param>
		/// <param name="propertyName">Název property.</param>
		/// <param name="itemType">Typ prvků kolekce.</param>
		/// <param name="collectionSelectFieldStatement">Část SQL dotazu pro vytažení hodnoty daného sloupce.</param>
		[SuppressMessage("SonarLint", "S1117", Justification = "Není chybou mít parametr metody stejného jména ve třídě.")]
		public void Initialize(ObjectInfo owner, string propertyName, Type itemType, string collectionSelectFieldStatement)
		{
			Initialize(owner, propertyName);
			this.itemType = itemType;
			this.collectionSelectFieldStatement = collectionSelectFieldStatement;
		}

		/// <summary>
		/// Vrátí řetězec pro vytažení daného sloupce z databáze.
		/// </summary>
		public string GetSelectFieldStatement(System.Data.Common.DbCommand command)
		{
			CheckInitialization();
			return collectionSelectFieldStatement;
		}
	}
}
