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
		#region Initialize
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
		public void Initialize(
			string dbSchema,
			string dbTable,
			string className,
			string _namespace,
			bool readOnly,
			CreateObjectDelegate createObjectMethod,
			GetObjectDelegate getObjectMethod,
			GetAllDelegate getAllMethod,
			FieldPropertyInfo deletedProperty,
			PropertyInfoCollection properties)
		{
			this.dbSchema = dbSchema;
			this.dbTable = dbTable;
			this.className = className;
			this._namespace = _namespace;
			this.readOnly = readOnly;
			this.createObjectMethod = createObjectMethod;
			this.getObjectMethod = getObjectMethod;
			this.getAllMethod = getAllMethod;
			this.deletedProperty = deletedProperty;
			this.properties = properties;

			this.isInitialized = true;
		}
		private bool isInitialized = false; 
	#endregion

		#region ReadOnly
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
		#endregion

		#region DbSchema
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
		#endregion

		#region DbTable
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
		#endregion

		#region ClassName
		/// <summary>
		/// Název tøídy dle databázové tabulky. Bez namespace.
		/// </summary>
		public string ClassName
		{
			get
			{
				CheckInitialization();
				return className;
			}
		}
		private string className;
		#endregion

		#region Namespace
		/// <summary>
		/// Namespace tøídy dle databázové tabulky. Bez názvu samotné tøídy.
		/// </summary>
		public string Namespace
		{
			get
			{
				CheckInitialization();
				return _namespace;
			}
		}
		private string _namespace;
		#endregion

		#region Properties
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
		#endregion

		#region DeletedProperty
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
		#endregion

		#region CreateObjectMethod
		/// <summary>
		/// Delegát metody (bez parametrù) vracující nový objekt.
		/// </summary>
		public CreateObjectDelegate CreateObjectMethod
		{
			get
			{
				CheckInitialization();
				return createObjectMethod;
			}
		}
		private CreateObjectDelegate createObjectMethod; 
		#endregion

		#region GetObjectMethod
		/// <summary>
		/// Delegát metody vracující instanci objektu.
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
		#endregion

		#region GetAllMethod
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
		#endregion

		#region CheckInitialization
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
		#endregion

	}
}