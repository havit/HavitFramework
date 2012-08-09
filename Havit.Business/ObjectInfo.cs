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
		/// <param name="deletedProperty">FieldPropertyInfo, která identifikuje pøíznakem smazané záznamy.</param>
		/// <param name="getObjectMethod">Delegát na metodu vracející objekt tøídy na základì ID.</param>
		/// <param name="getAllMethod">Delegát na metodu vracející všechny (nesmazané) objekty tøídy.</param>
		public ObjectInfo(string dbSchema, string dbTable, bool readOnly, FieldPropertyInfo deletedProperty, 
			GetObjectDelegate getObjectMethod, GetAllDelegate getAllMethod)
		{
			this.dbSchema = dbSchema;
			this.dbTable = dbTable;
			this.readOnly = readOnly;
			this.deletedProperty = deletedProperty;
			this.getObjectMethod = getObjectMethod;
			this.getAllMethod = getAllMethod;
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

		/// <summary>
		/// Metoda vracující instanci objektu.
		/// </summary>
		public GetObjectDelegate GetObjectMethod
		{
			get { return getObjectMethod; }
		}
		private GetObjectDelegate getObjectMethod;

		/// <summary>
		/// Metoda vracející seznam všech instancí.
		/// </summary>
		public GetAllDelegate GetAllMethod
		{
			get { return getAllMethod; }
		}
		private GetAllDelegate getAllMethod;

		/// <summary>
		/// Registruje kolekci properties.
		/// Každé registrované property nastaví parenta na tuto instanci tøídy ObjectInfo.
		/// </summary>
		public void RegisterProperties(PropertyInfoCollection properties)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}

			this.properties = properties;

			foreach (PropertyInfo propertyInfo in properties)
			{
				propertyInfo.Parent = this;
			}
		}
	}
}