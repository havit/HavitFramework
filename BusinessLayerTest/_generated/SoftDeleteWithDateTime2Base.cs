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
	/// Tabulka pro ověření generovaného kódu pro soft-delete dle DateTime2.
	/// </summary>
	/// <remarks>
	/// <code>
	/// CREATE TABLE [dbo].[SoftDeleteWithDateTime2](
	/// 	[SoftDeleteWithDateTime2ID] [int] IDENTITY(1,1) NOT NULL,
	/// 	[Deleted] [datetime2](7) NULL,
	///  CONSTRAINT [PK_SoftDeleteWithDateTime2] PRIMARY KEY CLUSTERED 
	/// (
	/// 	[SoftDeleteWithDateTime2ID] ASC
	/// )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	/// ) ON [PRIMARY]
	/// </code>
	/// </remarks>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public abstract class SoftDeleteWithDateTime2Base : ActiveRecordBusinessObjectBase
	{
		#region Static constructor
		static SoftDeleteWithDateTime2Base()
		{
			objectInfo = new ObjectInfo();
			properties = new SoftDeleteWithDateTime2Properties();
			objectInfo.Initialize("dbo", "SoftDeleteWithDateTime2", "SoftDeleteWithDateTime2", "Havit.BusinessLayerTest", false, SoftDeleteWithDateTime2.CreateObject, SoftDeleteWithDateTime2.GetObject, SoftDeleteWithDateTime2.GetAll, properties.Deleted, properties.All);
			properties.Initialize(objectInfo);
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Vytvoří instanci objektu jako nový prvek.
		/// </summary>
		/// <param name="connectionMode">Režim business objektu.</param>
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected SoftDeleteWithDateTime2Base(ConnectionMode connectionMode) : base(connectionMode)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci existujícího objektu.
		/// </summary>
		/// <param name="id">SoftDeleteWithDateTime2ID (PK).</param>
		/// <param name="connectionMode">Režim business objektu.</param>
		protected SoftDeleteWithDateTime2Base(int id, ConnectionMode connectionMode) : base(id, connectionMode)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.
		/// </summary>
		/// <param name="id">SoftDeleteWithDateTime2ID (PK).</param>
		/// <param name="record">DataRecord s daty objektu (i částečnými).</param>
		protected SoftDeleteWithDateTime2Base(int id, DataRecord record) : base(id, record)
		{
		}
		#endregion
		
		#region Properties dle sloupců databázové tabulky
		/// <summary>
		/// Čas smazání objektu. [datetime2, nullable]
		/// </summary>
		public virtual DateTime? Deleted
		{
			get
			{
				EnsureLoaded();
				return _DeletedPropertyHolder.Value;
			}
			protected set
			{
				EnsureLoaded();
				
				if (!Object.Equals(_DeletedPropertyHolder.Value, value))
				{
					DateTime? oldValue = _DeletedPropertyHolder.Value;
					_DeletedPropertyHolder.Value = value;
					OnPropertyChanged(new PropertyChangedEventArgs(nameof(Deleted), oldValue, value));
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Deleted.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<DateTime?> _DeletedPropertyHolder;
		
		#endregion
		
		#region IsDeleted
		/// <summary>
		/// Indikuje, zda je nastaven příznak smazaného záznamu.
		/// </summary>
		public override bool IsDeleted
		{
			get
			{
				return Deleted != null;
			}
		}
		#endregion
		
		#region Init
		/// <summary>
		/// Inicializuje třídu (vytvoří instance PropertyHolderů).
		/// </summary>
		protected override void Init()
		{
			_DeletedPropertyHolder = new PropertyHolder<DateTime?>(this);
			
			if (IsNew || IsDisconnected)
			{
				_DeletedPropertyHolder.Value = null;
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
			
			_DeletedPropertyHolder.IsDirty = false;
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
			
			if (_DeletedPropertyHolder.IsDirty)
			{
				if (_DeletedPropertyHolder.Value != null)
				{
					if ((_DeletedPropertyHolder.Value.Value < SqlDateTime.MinValue.Value) || (_DeletedPropertyHolder.Value.Value > SqlDateTime.MaxValue.Value))
					{
						throw new ConstraintViolationException(this, "Vlastnost \"Deleted\" nesmí nabývat hodnoty mimo rozsah SqlDateTime.MinValue-SqlDateTime.MaxValue.");
					}
				}
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
			dbCommand.CommandText = "SELECT [SoftDeleteWithDateTime2ID], [Deleted] FROM [dbo].[SoftDeleteWithDateTime2] WHERE [SoftDeleteWithDateTime2ID] = @SoftDeleteWithDateTime2ID";
			dbCommand.Transaction = transaction;
			
			DbParameter dbParameterSoftDeleteWithDateTime2ID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterSoftDeleteWithDateTime2ID.DbType = DbType.Int32;
			dbParameterSoftDeleteWithDateTime2ID.Direction = ParameterDirection.Input;
			dbParameterSoftDeleteWithDateTime2ID.ParameterName = "SoftDeleteWithDateTime2ID";
			dbParameterSoftDeleteWithDateTime2ID.Value = this.ID;
			dbCommand.Parameters.Add(dbParameterSoftDeleteWithDateTime2ID);
			
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
			this.ID = record.Get<int>("SoftDeleteWithDateTime2ID");
			
			DateTime? _tempDeleted;
			if (record.TryGet<DateTime?>("Deleted", out _tempDeleted))
			{
				_DeletedPropertyHolder.Value = _tempDeleted;
			}
			
		}
		#endregion
		
		#region Save & Delete: Save_SaveMembers, Save_SaveCollections, Save_MinimalInsert, Save_FullInsert, Save_Update, Save_Insert_InsertRequiredForMinimalInsert, Save_Insert_InsertRequiredForFullInsert, Delete, Delete_Perform
		
		/// <summary>
		/// Ukládá member-objekty.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_SaveMembers(DbTransaction transaction)
		{
			base.Save_SaveMembers(transaction);
			
			// Není co ukládat.
		}
		
		/// <summary>
		/// Ukládá member-kolekce objektu.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_SaveCollections(DbTransaction transaction)
		{
			base.Save_SaveCollections(transaction);
			
			// Není co ukládat.
		}
		
		/// <summary>
		/// Implementace metody vloží jen not-null vlastnosti objektu do databáze a nastaví nově přidělené ID (primární klíč).
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		public override sealed void Save_MinimalInsert(DbTransaction transaction)
		{
			base.Save_MinimalInsert(transaction);
			Save_Insert_InsertRequiredForMinimalInsert(transaction);
			
			DbCommand dbCommand;
			dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			DbParameter dbParameterDeleted = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterDeleted.DbType = DbType.DateTime2;
			dbParameterDeleted.Direction = ParameterDirection.Input;
			dbParameterDeleted.ParameterName = "Deleted";
			dbParameterDeleted.Value = (_DeletedPropertyHolder.Value == null) ? DBNull.Value : (object)_DeletedPropertyHolder.Value;
			dbCommand.Parameters.Add(dbParameterDeleted);
			_DeletedPropertyHolder.IsDirty = false;
			
			dbCommand.CommandText = "DECLARE @SoftDeleteWithDateTime2ID INT; INSERT INTO [dbo].[SoftDeleteWithDateTime2] ([Deleted]) VALUES (@Deleted); SELECT @SoftDeleteWithDateTime2ID = SCOPE_IDENTITY(); SELECT @SoftDeleteWithDateTime2ID; ";
			this.ID = (int)DbConnector.Default.ExecuteScalar(dbCommand);
			this.IsNew = false; // uložený objekt není už nový, dostal i přidělené ID
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			global::Havit.Diagnostics.Contracts.Contract.Assert(currentIdentityMap != null, "currentIdentityMap != null");
			currentIdentityMap.Store(this);
		}
		
		/// <summary>
		/// Implementace metody vloží nový objekt do databáze a nastaví nově přidělené ID (primární klíč).
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_FullInsert(DbTransaction transaction)
		{
			DbCommand dbCommand;
			dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			DbParameter dbParameterDeleted = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterDeleted.DbType = DbType.DateTime2;
			dbParameterDeleted.Direction = ParameterDirection.Input;
			dbParameterDeleted.ParameterName = "Deleted";
			dbParameterDeleted.Value = (_DeletedPropertyHolder.Value == null) ? DBNull.Value : (object)_DeletedPropertyHolder.Value;
			dbCommand.Parameters.Add(dbParameterDeleted);
			_DeletedPropertyHolder.IsDirty = false;
			
			dbCommand.CommandText = "DECLARE @SoftDeleteWithDateTime2ID INT; INSERT INTO [dbo].[SoftDeleteWithDateTime2] ([Deleted]) VALUES (@Deleted); SELECT @SoftDeleteWithDateTime2ID = SCOPE_IDENTITY(); SELECT @SoftDeleteWithDateTime2ID; ";
			this.ID = (int)DbConnector.Default.ExecuteScalar(dbCommand);
			this.IsNew = false; // uložený objekt není už nový, dostal i přidělené ID
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			global::Havit.Diagnostics.Contracts.Contract.Assert(currentIdentityMap != null, "currentIdentityMap != null");
			currentIdentityMap.Store(this);
			
			InvalidateAnySaveCacheDependencyKey();
		}
		
		/// <summary>
		/// Implementace metody aktualizuje data objektu v databázi.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_Update(DbTransaction transaction)
		{
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			StringBuilder commandBuilder = new StringBuilder();
			commandBuilder.Append("UPDATE [dbo].[SoftDeleteWithDateTime2] SET ");
			
			bool dirtyFieldExists = false;
			if (_DeletedPropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[Deleted] = @Deleted");
				
				DbParameter dbParameterDeleted = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterDeleted.DbType = DbType.DateTime2;
				dbParameterDeleted.Direction = ParameterDirection.Input;
				dbParameterDeleted.ParameterName = "Deleted";
				dbParameterDeleted.Value = (_DeletedPropertyHolder.Value == null) ? DBNull.Value : (object)_DeletedPropertyHolder.Value;
				dbCommand.Parameters.Add(dbParameterDeleted);
				
				dirtyFieldExists = true;
			}
			
			if (dirtyFieldExists)
			{
				// objekt je sice IsDirty (volá se tato metoda), ale může být změněná jen kolekce
				commandBuilder.Append(" WHERE [SoftDeleteWithDateTime2ID] = @SoftDeleteWithDateTime2ID; ");
			}
			else
			{
				commandBuilder = new StringBuilder();
			}
			
			bool dirtyCollectionExists = false;
			// pokud je objekt dirty, ale žádná property není dirty (Save_MinimalInsert poukládal všechno), neukládáme
			if (dirtyFieldExists || dirtyCollectionExists)
			{
				DbParameter dbParameterSoftDeleteWithDateTime2ID = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterSoftDeleteWithDateTime2ID.DbType = DbType.Int32;
				dbParameterSoftDeleteWithDateTime2ID.Direction = ParameterDirection.Input;
				dbParameterSoftDeleteWithDateTime2ID.ParameterName = "SoftDeleteWithDateTime2ID";
				dbParameterSoftDeleteWithDateTime2ID.Value = this.ID;
				dbCommand.Parameters.Add(dbParameterSoftDeleteWithDateTime2ID);
				dbCommand.CommandText = commandBuilder.ToString();
				DbConnector.Default.ExecuteNonQuery(dbCommand);
			}
			
			InvalidateSaveCacheDependencyKey();
			InvalidateAnySaveCacheDependencyKey();
		}
		
		/// <summary>
		/// Ukládá hodnoty potřebné pro provedení minimálního insertu. Volá Save_Insert_SaveRequiredForMinimalInsert.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_Insert_InsertRequiredForMinimalInsert(DbTransaction transaction)
		{
			base.Save_Insert_InsertRequiredForMinimalInsert(transaction);
			
		}
		
		/// <summary>
		/// Ukládá hodnoty potřebné pro provedení plného insertu.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_Insert_InsertRequiredForFullInsert(DbTransaction transaction)
		{
			base.Save_Insert_InsertRequiredForFullInsert(transaction);
			
		}
		
		/// <summary>
		/// Smaže objekt, nebo ho označí jako smazaný, podle zvolené logiky. Změnu uloží do databáze, v transakci.
		/// </summary>
		/// <remarks>
		/// Neprovede se, pokud je již objekt smazán.
		/// </remarks>
		/// <param name="transaction">Transakce DbTransaction, v rámci které se smazání provede; null, pokud bez transakce.</param>
		public override void Delete(DbTransaction transaction)
		{
			EnsureLoaded(transaction);
			if (Deleted == null)
			{
				Deleted = System.DateTime.Now;
			}
			base.Delete(transaction);
		}
		
		/// <summary>
		/// Metoda označí objekt jako smazaný a uloží jej.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Delete_Perform(DbTransaction transaction)
		{
			Save_Update(transaction);
		}
		
		#endregion
		
		#region Cache dependencies methods
		/// <summary>
		/// Vrátí klíč pro tvorbu závislostí cache na objektu. Při uložení objektu jsou závislosti invalidovány.
		/// </summary>
		public string GetSaveCacheDependencyKey(bool ensureInCache = true)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(!this.IsNew, "!this.IsNew");
			
			if (!Havit.Business.BusinessLayerContext.BusinessLayerCacheService.SupportsCacheDependencies)
			{
				throw new InvalidOperationException("Použitá BusinessLayerCacheService nepodporuje cache dependencies.");
			}
			
			string key = "BL|SoftDeleteWithDateTime2|SaveDK|" + this.ID;
			
			if (ensureInCache)
			{
				Havit.Business.BusinessLayerContext.BusinessLayerCacheService.EnsureCacheDependencyKey(typeof(SoftDeleteWithDateTime2), key);
			}
			
			return key;
		}
		
		/// <summary>
		/// Odstraní z cache závislosti na klíči CacheDependencyKey.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected void InvalidateSaveCacheDependencyKey()
		{
			if (Havit.Business.BusinessLayerContext.BusinessLayerCacheService.SupportsCacheDependencies)
			{
				Havit.Business.BusinessLayerContext.BusinessLayerCacheService.InvalidateCacheDependencies(typeof(SoftDeleteWithDateTime2), GetSaveCacheDependencyKey(false));
			}
		}
		
		/// <summary>
		/// Vrátí klíč pro tvorbu závislostí cache. Po uložení jakéhokoliv objektu této třídy jsou závislosti invalidovány.
		/// </summary>
		public static string GetAnySaveCacheDependencyKey(bool ensureInCache = true)
		{
			if (!Havit.Business.BusinessLayerContext.BusinessLayerCacheService.SupportsCacheDependencies)
			{
				throw new InvalidOperationException("Použitá BusinessLayerCacheService nepodporuje cache dependencies.");
			}
			
			string key = "BL|SoftDeleteWithDateTime2|AnySaveDK";
			
			if (ensureInCache)
			{
				Havit.Business.BusinessLayerContext.BusinessLayerCacheService.EnsureCacheDependencyKey(typeof(SoftDeleteWithDateTime2), key);
			}
			
			return key;
		}
		
		/// <summary>
		/// Odstraní z cache závislosti na klíči AnySaveCacheDependencyKey.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected static void InvalidateAnySaveCacheDependencyKey()
		{
			if (Havit.Business.BusinessLayerContext.BusinessLayerCacheService.SupportsCacheDependencies)
			{
				Havit.Business.BusinessLayerContext.BusinessLayerCacheService.InvalidateCacheDependencies(typeof(SoftDeleteWithDateTime2), GetAnySaveCacheDependencyKey(false));
			}
		}
		#endregion
		
		#region GetFirst, GetList, GetAll
		/// <summary>
		/// Vrátí první nalezený objekt typu SoftDeleteWithDateTime2 dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null.
		/// </summary>
		public static SoftDeleteWithDateTime2 GetFirst(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return SoftDeleteWithDateTime2.GetFirst(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí první nalezený objekt typu SoftDeleteWithDateTime2 dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null. Data jsou načítána v předané transakci.
		/// </summary>
		public static SoftDeleteWithDateTime2 GetFirst(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			int? originalTopRecords = queryParams.TopRecords;
			queryParams.TopRecords = 1;
			SoftDeleteWithDateTime2Collection getListResult = SoftDeleteWithDateTime2.GetList(queryParams, transaction);
			queryParams.TopRecords = originalTopRecords;
			return (getListResult.Count == 0) ? null : getListResult[0];
		}
		
		/// <summary>
		/// Vrátí objekty typu SoftDeleteWithDateTime2 dle parametrů v queryParams.
		/// </summary>
		internal static SoftDeleteWithDateTime2Collection GetList(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return SoftDeleteWithDateTime2.GetList(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí objekty typu SoftDeleteWithDateTime2 dle parametrů v queryParams. Data jsou načítána v předané transakci.
		/// </summary>
		internal static SoftDeleteWithDateTime2Collection GetList(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			queryParams.ObjectInfo = SoftDeleteWithDateTime2.ObjectInfo;
			if (queryParams.Properties.Count > 0)
			{
				queryParams.Properties.Add(SoftDeleteWithDateTime2.Properties.ID);
			}
			
			queryParams.PrepareCommand(dbCommand, SqlServerPlatform.SqlServer2008, CommandBuilderOptions.None);
			return SoftDeleteWithDateTime2.GetList(dbCommand, queryParams.GetDataLoadPower());
		}
		
		private static SoftDeleteWithDateTime2Collection GetList(DbCommand dbCommand, DataLoadPower dataLoadPower)
		{
			if (dbCommand == null)
			{
				throw new ArgumentNullException("dbCommand");
			}
			
			SoftDeleteWithDateTime2Collection result = new SoftDeleteWithDateTime2Collection();
			
			using (DbDataReader reader = DbConnector.Default.ExecuteReader(dbCommand))
			{
				while (reader.Read())
				{
					DataRecord dataRecord = new DataRecord(reader, dataLoadPower);
					SoftDeleteWithDateTime2 softDeleteWithDateTime2 = SoftDeleteWithDateTime2.GetObject(dataRecord);
					result.Add(softDeleteWithDateTime2);
				}
			}
			
			return result;
		}
		
		/// <summary>
		/// Vrátí všechny (příznakem) nesmazané objekty typu SoftDeleteWithDateTime2.
		/// </summary>
		public static SoftDeleteWithDateTime2Collection GetAll()
		{
			return SoftDeleteWithDateTime2.GetAll(false);
		}
		
		/// <summary>
		/// Vrátí všechny objekty typu SoftDeleteWithDateTime2. Parametr udává, zda se mají vrátit i (příznakem) smazané záznamy.
		/// </summary>
		public static SoftDeleteWithDateTime2Collection GetAll(bool includeDeleted)
		{
			SoftDeleteWithDateTime2Collection collection = null;
			QueryParams queryParams = new QueryParams();
			queryParams.IncludeDeleted = includeDeleted;
			collection = SoftDeleteWithDateTime2.GetList(queryParams);
			return collection;
		}
		
		#endregion
		
		#region ToString
		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			if (IsNew)
			{
				return "SoftDeleteWithDateTime2(New)";
			}
			
			return String.Format("SoftDeleteWithDateTime2(ID={0})", this.ID);
		}
		#endregion
		
		#region ObjectInfo
		/// <summary>
		/// Objektová reprezentace metadat typu SoftDeleteWithDateTime2.
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
		/// Objektová reprezentace metadat vlastností typu SoftDeleteWithDateTime2.
		/// </summary>
		public static SoftDeleteWithDateTime2Properties Properties
		{
			get
			{
				return properties;
			}
		}
		private static SoftDeleteWithDateTime2Properties properties;
		#endregion
		
	}
}