﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Havit.Business;
using Havit.Business.Query;
using Havit.Collections;
using Havit.Data;
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest
{
	/// <summary>
	/// Uživatelská role. Určuje oprávnění v systému. [cached, read-only]
	/// </summary>
	/// <remarks>
	/// <code>
	/// CREATE TABLE [dbo].[Role](
	/// 	[RoleID] [int] NOT NULL,
	/// 	[Symbol] [varchar](50) COLLATE Czech_CI_AS NULL CONSTRAINT [DF_Role_Symbol]  DEFAULT (''),
	///  CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
	/// (
	/// 	[RoleID] ASC
	/// )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	/// ) ON [PRIMARY]
	/// </code>
	/// </remarks>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public abstract class RoleBase : ActiveRecordBusinessObjectBase
	{
		#region Static constructor
		static RoleBase()
		{
			objectInfo = new ObjectInfo();
			properties = new RoleProperties();
			objectInfo.Initialize("dbo", "Role", "Role", "Havit.BusinessLayerTest", true, null, Role.GetObject, Role.GetAll, null, properties.All);
			properties.Initialize(objectInfo);
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Vytvoří instanci objektu jako nový prvek.
		/// </summary>
		/// <param name="connectionMode">Režim business objektu.</param>
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected RoleBase(ConnectionMode connectionMode) : base(connectionMode)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci existujícího objektu.
		/// </summary>
		/// <param name="id">RoleID (PK).</param>
		/// <param name="connectionMode">Režim business objektu.</param>
		protected RoleBase(int id, ConnectionMode connectionMode) : base(id, connectionMode)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.
		/// </summary>
		/// <param name="id">RoleID (PK).</param>
		/// <param name="record">DataRecord s daty objektu (i částečnými).</param>
		protected RoleBase(int id, DataRecord record) : base(id, record)
		{
		}
		#endregion
		
		#region Properties dle sloupců databázové tabulky
		/// <summary>
		/// Symbol role (název pro ASP.NET autrhorization) [varchar(50), nullable, default '']
		/// </summary>
		public virtual string Symbol
		{
			get
			{
				EnsureLoaded();
				return _SymbolPropertyHolder.Value;
			}
			private set
			{
				EnsureLoaded();
				if (value == null)
				{
					_SymbolPropertyHolder.Value = String.Empty;
				}
				else
				{
					_SymbolPropertyHolder.Value = value;
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Symbol.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<string> _SymbolPropertyHolder;
		
		#endregion
		
		#region Init
		/// <summary>
		/// Inicializuje třídu (vytvoří instance PropertyHolderů).
		/// </summary>
		protected override void Init()
		{
			_SymbolPropertyHolder = new PropertyHolder<string>(this);
			
			if (IsNew || IsDisconnected)
			{
				_SymbolPropertyHolder.Value = String.Empty;
			}
			
			base.Init();
		}
		#endregion
		
		#region CleanDirty
		/// <summary>
		/// Nastaví property holderům IsDirty na false.
		/// </summary>
		protected override void CleanDirty()
		{
			base.CleanDirty();
			
			_SymbolPropertyHolder.IsDirty = false;
		}
		#endregion
		
		#region CheckConstraints
		/// <summary>
		/// Kontroluje konzistenci objektu jako celku.
		/// </summary>
		/// <remarks>
		/// Automaticky je voláno před ukládáním objektu Save(), pokud je objekt opravdu ukládán.
		/// </remarks>
		protected override void CheckConstraints()
		{
			base.CheckConstraints();
			
			if (_SymbolPropertyHolder.IsDirty && (_SymbolPropertyHolder.Value != null) && (_SymbolPropertyHolder.Value.Length > 50))
			{
				throw new ConstraintViolationException(this, "Vlastnost \"Symbol\" - řetězec přesáhl maximální délku 50 znaků.");
			}
			
		}
		#endregion
		
		#region Load: Load_GetDataRecord, Load_ParseDataRecord
		/// <summary>
		/// Načte data objektu z DB a vrátí je ve formě DataRecordu.
		/// </summary>
		/// <param name="transaction">Transakce.</param>
		/// <returns>Úplná data objektu.</returns>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed DataRecord Load_GetDataRecord(DbTransaction transaction)
		{
			DataRecord result;
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.CommandText = "SELECT [RoleID], [Symbol] FROM [dbo].[Role] WHERE [RoleID] = @RoleID";
			dbCommand.Transaction = transaction;
			
			DbParameter dbParameterRoleID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterRoleID.DbType = DbType.Int32;
			dbParameterRoleID.Direction = ParameterDirection.Input;
			dbParameterRoleID.ParameterName = "RoleID";
			dbParameterRoleID.Value = this.ID;
			dbCommand.Parameters.Add(dbParameterRoleID);
			
			result = DbConnector.Default.ExecuteDataRecord(dbCommand);
			
			return result;
		}
		
		/// <summary>
		/// Vytahá data objektu z DataRecordu.
		/// </summary>
		/// <param name="record">DataRecord s daty objektu</param>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Load_ParseDataRecord(DataRecord record)
		{
			if (!this.IsLoaded)
			{
				lock (_loadParseDataRecordLock)
				{
					if (!this.IsLoaded)
					{
						this.ID = record.Get<int>("RoleID");
						
						string _tempSymbol;
						if (record.TryGet<string>("Symbol", out _tempSymbol))
						{
							_SymbolPropertyHolder.Value = _tempSymbol ?? String.Empty;
						}
						
					}
				}
			}
		}
		private object _loadParseDataRecordLock = new object();
		#endregion
		
		#region Save & Delete: Save_SaveMembers, Save_SaveCollections, Save_MinimalInsert, Save_FullInsert, Save_Update, Save_Insert_InsertRequiredForMinimalInsert, Save_Insert_InsertRequiredForFullInsert, Delete, Delete_Perform
		
		/// <summary>
		/// Ukládá member-objekty.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_SaveMembers(DbTransaction transaction)
		{
			base.Save_SaveMembers(transaction);
			
			// Neukládáme, jsme read-only třídou.
		}
		
		/// <summary>
		/// Ukládá member-kolekce objektu.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_SaveCollections(DbTransaction transaction)
		{
			base.Save_SaveCollections(transaction);
			
			// Neukládáme, jsme read-only třídou.
		}
		
		/// <summary>
		/// Implementace metody vloží jen not-null vlastnosti objektu do databáze a nastaví nově přidělené ID (primární klíč).
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		public override sealed void Save_MinimalInsert(DbTransaction transaction)
		{
			throw new InvalidOperationException("Objekty třídy Havit.BusinessLayerTest.Role jsou určeny jen pro čtení.");
		}
		
		/// <summary>
		/// Implementace metody vloží nový objekt do databáze a nastaví nově přidělené ID (primární klíč).
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_FullInsert(DbTransaction transaction)
		{
			throw new InvalidOperationException("Objekty třídy Havit.BusinessLayerTest.Role jsou určeny jen pro čtení.");
		}
		
		/// <summary>
		/// Implementace metody aktualizuje data objektu v databázi.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_Update(DbTransaction transaction)
		{
			throw new InvalidOperationException("Objekty třídy Havit.BusinessLayerTest.Role jsou určeny jen pro čtení.");
		}
		
		/// <summary>
		/// Ukládá hodnoty potřebné pro provedení minimálního insertu. Volá Save_Insert_SaveRequiredForMinimalInsert.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_Insert_InsertRequiredForMinimalInsert(DbTransaction transaction)
		{
			throw new InvalidOperationException("Objekty třídy Havit.BusinessLayerTest.Role jsou určeny jen pro čtení.");
		}
		
		/// <summary>
		/// Ukládá hodnoty potřebné pro provedení plného insertu.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_Insert_InsertRequiredForFullInsert(DbTransaction transaction)
		{
			throw new InvalidOperationException("Objekty třídy Havit.BusinessLayerTest.Role jsou určeny jen pro čtení.");
		}
		
		/// <summary>
		/// Objekt je typu readonly. Metoda vyhazuje výjimku InvalidOperationException.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Delete_Perform(DbTransaction transaction)
		{
			throw new InvalidOperationException("Objekty třídy Havit.BusinessLayerTest.Role jsou určeny jen pro čtení.");
		}
		
		#endregion
		
		#region BusinessObject cache access methods
		/// <summary>
		/// Vrátí název klíče pro business object.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected static string GetBusinessObjectCacheKey(int id)
		{
			return "Role.GetObject|ID=" + id;
		}
		
		/// <summary>
		/// Přidá business object do cache.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected static void AddBusinessObjectToCache(BusinessObjectBase businessObject)
		{
			Havit.Services.Caching.CacheOptions options = new Havit.Services.Caching.CacheOptions
			{
				Priority = Havit.Services.Caching.CacheItemPriority.NotRemovable
			};
			Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.AddBusinessObjectToCache(typeof(Role), GetBusinessObjectCacheKey(businessObject.ID), businessObject, options);
		}
		
		/// <summary>
		/// Vyhledá v cache business object pro objekt daného ID a vrátí jej. Není-li v cache nalezen, vrací null.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static BusinessObjectBase GetBusinessObjectFromCache(int id)
		{
			return Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.GetBusinessObjectFromCache(typeof(Role), GetBusinessObjectCacheKey(id));
		}
		
		#endregion
		
		#region GetAll IDs cache access methods
		/// <summary>
		/// Vrátí název klíče pro kolekci IDs metody GetAll.
		/// </summary>
		private static string GetAllIDsCacheKey()
		{
			return "Role.GetAll";
		}
		
		/// <summary>
		/// Vyhledá v cache pole IDs metody GetAll a vrátí jej. Není-li v cache nalezena, vrací null.
		/// </summary>
		private static int[] GetAllIDsFromCache()
		{
			return Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.GetAllIDsFromCache(typeof(Role), GetAllIDsCacheKey());
		}
		
		/// <summary>
		/// Přidá pole IDs metody GetAll do cache.
		/// </summary>
		private static void AddAllIDsToCache(int[] ids)
		{
			Havit.Services.Caching.CacheOptions options = new Havit.Services.Caching.CacheOptions
			{
				Priority = Havit.Services.Caching.CacheItemPriority.NotRemovable
			};
			Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.AddAllIDsToCache(typeof(Role), GetAllIDsCacheKey(), ids, options);
		}
		
		/// <summary>
		/// Odstraní pole IDs metody GetAll z cache.
		/// </summary>
		private static void RemoveAllIDsFromCache()
		{
			Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.RemoveAllIDsFromCache(typeof(Role), GetAllIDsCacheKey());
		}
		#endregion
		
		#region Enum members
		/// <summary>
		/// ZaporneID [-1]
		/// </summary>
		public static Role ZaporneID
		{
			get
			{
				return Role.GetObject(EnumIDs.ZaporneID);
			}
		}
		
		/// <summary>
		/// NuloveID [0]
		/// </summary>
		public static Role NuloveID
		{
			get
			{
				return Role.GetObject(EnumIDs.NuloveID);
			}
		}
		
		/// <summary>
		/// Administrator [1]
		/// </summary>
		public static Role Administrator
		{
			get
			{
				return Role.GetObject(EnumIDs.Administrator);
			}
		}
		
		/// <summary>
		/// Editor [2]
		/// </summary>
		public static Role Editor
		{
			get
			{
				return Role.GetObject(EnumIDs.Editor);
			}
		}
		
		/// <summary>
		/// Publisher [3]
		/// </summary>
		public static Role Publisher
		{
			get
			{
				return Role.GetObject(EnumIDs.Publisher);
			}
		}
		
		/// <summary>
		/// Operator [4]
		/// </summary>
		public static Role Operator
		{
			get
			{
				return Role.GetObject(EnumIDs.Operator);
			}
		}
		
		#endregion
		
		#region EnumIDs (class)
		/// <summary>
		/// Konstanty ID objektů EnumClass.
		/// </summary>
		public static class EnumIDs
		{
			/// <summary>
			/// ZaporneID [-1]
			/// </summary>
			public const int ZaporneID = -1;
			
			/// <summary>
			/// NuloveID [0]
			/// </summary>
			public const int NuloveID = 0;
			
			/// <summary>
			/// Administrator [1]
			/// </summary>
			public const int Administrator = 1;
			
			/// <summary>
			/// Editor [2]
			/// </summary>
			public const int Editor = 2;
			
			/// <summary>
			/// Publisher [3]
			/// </summary>
			public const int Publisher = 3;
			
			/// <summary>
			/// Operator [4]
			/// </summary>
			public const int Operator = 4;
			
		}
		#endregion
		
		#region GetFirst, GetList, GetAll
		/// <summary>
		/// Vrátí první nalezený objekt typu Role dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null.
		/// </summary>
		public static Role GetFirst(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return Role.GetFirst(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí první nalezený objekt typu Role dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null. Data jsou načítána v předané transakci.
		/// </summary>
		public static Role GetFirst(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			int? originalTopRecords = queryParams.TopRecords;
			queryParams.TopRecords = 1;
			RoleCollection getListResult = Role.GetList(queryParams, transaction);
			queryParams.TopRecords = originalTopRecords;
			return (getListResult.Count == 0) ? null : getListResult[0];
		}
		
		/// <summary>
		/// Vrátí objekty typu Role dle parametrů v queryParams.
		/// </summary>
		internal static RoleCollection GetList(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return Role.GetList(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí objekty typu Role dle parametrů v queryParams. Data jsou načítána v předané transakci.
		/// </summary>
		internal static RoleCollection GetList(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			queryParams.ObjectInfo = Role.ObjectInfo;
			if (queryParams.Properties.Count > 0)
			{
				queryParams.Properties.Add(Role.Properties.ID);
			}
			
			queryParams.PrepareCommand(dbCommand, SqlServerPlatform.SqlServer2008, CommandBuilderOptions.None);
			return Role.GetList(dbCommand, queryParams.GetDataLoadPower());
		}
		
		private static RoleCollection GetList(DbCommand dbCommand, DataLoadPower dataLoadPower)
		{
			if (dbCommand == null)
			{
				throw new ArgumentNullException("dbCommand");
			}
			
			RoleCollection result = new RoleCollection();
			
			using (DbDataReader reader = DbConnector.Default.ExecuteReader(dbCommand))
			{
				while (reader.Read())
				{
					DataRecord dataRecord = new DataRecord(reader, dataLoadPower);
					Role role = Role.GetObject(dataRecord);
					result.Add(role);
				}
			}
			
			return result;
		}
		
		private static object lockGetAllCacheAccess = new object();
		
		/// <summary>
		/// Vrátí všechny objekty typu Role.
		/// </summary>
		public static RoleCollection GetAll()
		{
			RoleCollection collection = null;
			int[] ids = null;
			
			ids = GetAllIDsFromCache();
			if (ids == null)
			{
				lock (lockGetAllCacheAccess)
				{
					ids = GetAllIDsFromCache();
					if (ids == null)
					{
						QueryParams queryParams = new QueryParams();
						collection = Role.GetList(queryParams);
						ids = collection.GetIDs();
						
						AddAllIDsToCache(ids);
					}
				}
			}
			if (collection == null)
			{
				collection = new RoleCollection();
				collection.AddRange(Role.GetObjects(ids));
				collection.LoadAll();
			}
			
			return collection;
		}
		
		#endregion
		
		#region ToString
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			switch (this.ID)
			{
				case EnumIDs.ZaporneID: return "Role.ZaporneID";
				case EnumIDs.NuloveID: return "Role.NuloveID";
				case EnumIDs.Administrator: return "Role.Administrator";
				case EnumIDs.Editor: return "Role.Editor";
				case EnumIDs.Publisher: return "Role.Publisher";
				case EnumIDs.Operator: return "Role.Operator";
			}
			
			return String.Format("Role(ID={0})", this.ID);
		}
		#endregion
		
		#region ObjectInfo
		/// <summary>
		/// Objektová reprezentace metadat typu Role.
		/// </summary>
		public static ObjectInfo ObjectInfo
		{
			get
			{
				return objectInfo;
			}
		}
		private static ObjectInfo objectInfo;
		#endregion
		
		#region Properties
		/// <summary>
		/// Objektová reprezentace metadat vlastností typu Role.
		/// </summary>
		public static RoleProperties Properties
		{
			get
			{
				return properties;
			}
		}
		private static RoleProperties properties;
		#endregion
		
	}
}
