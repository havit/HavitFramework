using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Reprezentuje informace o objektu (třídě).
	/// </summary>
	public class ObjectInfo
	{
		#region ReadOnly
		/// <summary>
		/// Indikuje, zda je objekt určen jen ke čtení.
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
		/// Název třídy dle databázové tabulky. Bez namespace.
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
		/// Namespace třídy dle databázové tabulky. Bez názvu samotné třídy.
		/// </summary>
		public string Namespace
		{
			get
			{
				CheckInitialization();
				return namespaceName;
			}
		}
		private string namespaceName;
		#endregion

		#region Properties
		/// <summary>
		/// Property ve třídě.
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
		/// Property, která označuje smazané záznamy.
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
		/// Delegát metody (bez parametrů) vracující nový objekt.
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

		#region Initialize
		/// <summary>
		/// Nastaví instanci třídy.
		/// </summary>
		/// <param name="dbSchema">Název schémata databázové tabulky.</param>
		/// <param name="dbTable">Název databázové tabulky.</param>
		/// <param name="className">Název třídy.</param>
		/// <param name="namespaceName">Název namespace třídy.</param>
		/// <param name="readOnly">Určuje, zda je třída jen ke čtení.</param>
		/// <param name="createObjectMethod">Delegát na metodu (bez parametrů) vytvářející instanci nového objektu. Null, pokud taková metoda neexistuje (třída je readonly nebo máowner field).</param>
		/// <param name="getObjectMethod">Delegát na metodu vracející objekt třídy na základě ID.</param>
		/// <param name="getAllMethod">Delegát na metodu vracející všechny (nesmazané) objekty třídy.</param>
		/// <param name="deletedProperty">FieldPropertyInfo, která identifikuje příznakem smazané záznamy.</param>
		/// <param name="properties">Kolekce všech vlastností objektu.</param>
		[SuppressMessage("SonarLint", "S1117", Justification = "Není chybou mít parametr metody stejného jména ve třídě.")]
		public void Initialize(
			string dbSchema,
			string dbTable,
			string className,
			string namespaceName,
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
			this.namespaceName = namespaceName;
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

		#region CheckInitialization
		/// <summary>
		/// Ověří, že byla instance inicializována. Pokud ne, vyhodí výjimku.
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