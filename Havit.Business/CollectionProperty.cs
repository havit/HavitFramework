using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Kolekce objektù tøídy IProperty.
	/// </summary>
	public class CollectionProperty : IProperty
	{
		/// <summary>
		/// Vytvoøí instanci CollectionProperty.
		/// </summary>
		/// <param name="itemType">Typ prvkù kolekce.</param>
		/// <param name="collectionSelectFieldStatement">Èást SQL dotazu pro vytažení hodnoty daného sloupce.</param>
		public CollectionProperty(Type itemType, string collectionSelectFieldStatement)
		{
			this.itemType = itemType;
			this.collectionSelectFieldStatement = collectionSelectFieldStatement;
		}

		/// <summary>
		/// Typ prvkù kolekce.
		/// </summary>
		public Type ItemType
		{
			get { return itemType; }
		}
		private Type itemType;

		/// <summary>
		/// Èást SQL dotazu pro vytažení hodnoty daného sloupce.
		/// </summary>
		public string CollectionSelectFieldStatement
		{
			get { return collectionSelectFieldStatement; }
		}
		private string collectionSelectFieldStatement;

		/// <summary>
		/// Vrátí øetìzec pro vytažení daného sloupce z databáze.
		/// </summary>
		public string GetSelectFieldStatement(System.Data.Common.DbCommand command)
		{
			return collectionSelectFieldStatement;
		}
	}
}
