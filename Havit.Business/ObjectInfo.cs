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
		/// Nastaví instanci tøídy.
		/// </summary>
		/// <param name="dbSchema">Název schémata databázové tabulky.</param>
		/// <param name="dbTable">Název databázové tabulky.</param>
		/// <param name="readOnly">Urèuje, zda je tøída jen ke ètení.</param>
		/// <param name="getObjectMethod">Delegát na metodu vracející objekt tøídy na základì ID.</param>
		/// <param name="getAllMethod">Delegát na metodu vracející všechny (nesmazané) objekty tøídy.</param>
		/// <param name="deletedProperty">FieldPropertyInfo, která identifikuje pøíznakem smazané záznamy.</param>
		/// <param name="properties">Kolekce všech vlastností objektu.</param>
		public void Initialize(string dbSchema, string dbTable, bool readOnly, 
			GetObjectDelegate getObjectMethod, GetAllDelegate getAllMethod, FieldPropertyInfo deletedProperty, PropertyInfoCollection properties)
		{
			this.dbSchema = dbSchema;
			this.dbTable = dbTable;
			this.readOnly = readOnly;
			this.getObjectMethod = getObjectMethod;
			this.getAllMethod = getAllMethod;
			this.deletedProperty = deletedProperty;
			this.properties = properties;

			this.isInitialized = true;
		}
		private bool isInitialized = false;

		/// <summary>
		/// Indikuje, zda je objekt urèen jen ke ètení.
		/// </summary>
		public bool ReadOnly
		{
			get
			{
				CheckInitialization();
				return readOnly;
			}
		}
		private bool readOnly;

		/// <summary>
		/// Název schématu databázové tabulky.
		/// </summary>
		public string DbSchema
		{
			get
			{
				CheckInitialization();
				return dbSchema;
			}
		}
		private string dbSchema;

		/// <summary>
		/// Název databázové tabulky.
		/// </summary>
		public string DbTable
		{
			get
			{
				CheckInitialization();
				return dbTable;
			}
		}
		private string dbTable;

		/// <summary>
		/// Property ve tøídì.
		/// </summary>
		public PropertyInfoCollection Properties
		{
			get
			{
				CheckInitialization();
				return properties;
			}
		}
		private PropertyInfoCollection properties;

		/// <summary>
		/// Property, která oznaèuje smazané záznamy.
		/// </summary>
		public FieldPropertyInfo DeletedProperty
		{
			get
			{
				CheckInitialization();
				return deletedProperty;
			}
		}
		private FieldPropertyInfo deletedProperty;

		/// <summary>
		/// Metoda vracující instanci objektu.
		/// </summary>
		public GetObjectDelegate GetObjectMethod
		{
			get
			{
				CheckInitialization();
				return getObjectMethod;
			}
		}
		private GetObjectDelegate getObjectMethod;

		/// <summary>
		/// Metoda vracející seznam všech instancí.
		/// </summary>
		public GetAllDelegate GetAllMethod
		{
			get
			{
				CheckInitialization();
				return getAllMethod; 
			}
		}
		private GetAllDelegate getAllMethod;

		/// <summary>
		/// Ovìøí, že byla instance inicializována. Pokud ne, vyhodí výjimku.
		/// </summary>
		protected void CheckInitialization()
		{
			if (!isInitialized)
			{
				throw new InvalidOperationException("Instance nebyla inicializována.");
			}
		}

	}
}