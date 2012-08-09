using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Kolekce objektù tøídy IProperty.
	/// </summary>
	public class CollectionPropertyInfo : PropertyInfo, IFieldsBuilder
	{
		/// <summary>
		/// Vytvoøí instanci CollectionProperty.
		/// </summary>
		/// <param name="owner">Nadøazený objectInfo.</param>
		/// <param name="itemType">Typ prvkù kolekce.</param>
		/// <param name="collectionSelectFieldStatement">Èást SQL dotazu pro vytažení hodnoty daného sloupce.</param>
		public void Initialize(ObjectInfo owner, Type itemType, string collectionSelectFieldStatement)
		{
			Initialize(owner);
			this.itemType = itemType;
			this.collectionSelectFieldStatement = collectionSelectFieldStatement;
		}

		/// <summary>
		/// Typ prvkù kolekce.
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
		/// Èást SQL dotazu pro vytažení hodnoty daného sloupce.
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
		/// Vrátí øetìzec pro vytažení daného sloupce z databáze.
		/// </summary>
		public string GetSelectFieldStatement(System.Data.Common.DbCommand command)
		{
			CheckInitialization();
			return collectionSelectFieldStatement;
		}
	}
}
