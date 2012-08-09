using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Reprezentuje informace o objektu (tøídì).
	/// </summary>
	public class ObjectInfo
	{
		/// <summary>
		/// Vytvoøí instanci tøídy.
		/// </summary>
		/// <param name="dbSchema">Název schémata databázové tabulky.</param>
		/// <param name="dbTable">Název databázové tabulky.</param>
		/// <param name="readOnly">Urèuje, zda je tøída jen ke ètení.</param>
		/// <param name="properties">Properties tøídy.</param>
		/// <param name="deletedProperty">FieldPropertyInfo, která identifikuje pøíznakem smazané záznamy.</param>
		public ObjectInfo(string dbSchema, string dbTable, bool readOnly, PropertyInfoCollection properties, FieldPropertyInfo deletedProperty)
		{
			if (properties == null)
				throw new ArgumentNullException("properties");

			this.dbSchema = dbSchema;
			this.dbTable = dbTable;
			this.readOnly = readOnly;
			this.properties = properties;
			this.deletedProperty = deletedProperty;

			foreach (PropertyInfo propertyInfo in properties)
			{
				propertyInfo.Parent = this;
			}
		}

		/// <summary>
		/// Indikuje, zda je objekt urèen jen ke ètení.
		/// </summary>
		public bool ReadOnly
		{
			get { return readOnly; }
		}
		private bool readOnly;

		/// <summary>
		/// Název schématu databázové tabulky.
		/// </summary>
		public string DbSchema
		{
			get { return dbSchema; }
		}
		private string dbSchema;

		/// <summary>
		/// Název databázové tabulky.
		/// </summary>
		public string DbTable
		{
			get { return dbTable; }
		}
		private string dbTable;

		/// <summary>
		/// Property ve tøídì.
		/// </summary>
		public PropertyInfoCollection Properties
		{
			get { return properties; }
		}
		private PropertyInfoCollection properties;

		/// <summary>
		/// Property, která oznaèuje smazané záznamy.
		/// </summary>
		public FieldPropertyInfo DeletedProperty
		{
			get { return deletedProperty;  }
		}
		private FieldPropertyInfo deletedProperty;

	}
}