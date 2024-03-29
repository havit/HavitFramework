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
using System.Xml;
using Havit.Business;
using Havit.Business.Query;
using Havit.Collections;
using Havit.Data;
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest.Resources
{
	/// <summary>
	/// Lokalizace objektu Havit.BusinessLayerTest.Resources.ResourceItem. [cached]
	/// </summary>
	/// <remarks>
	/// <code>
	/// CREATE TABLE [dbo].[ResourceItemLocalization](
	/// 	[ResourceItemLocalizationID] [int] IDENTITY(1,1) NOT NULL,
	/// 	[ResourceItemID] [int] NOT NULL,
	/// 	[LanguageID] [int] NOT NULL,
	/// 	[Value] [nvarchar](max) COLLATE Czech_CI_AS NOT NULL,
	///  CONSTRAINT [PK_ResourceItemLocalization] PRIMARY KEY CLUSTERED 
	/// (
	/// 	[ResourceItemLocalizationID] ASC
	/// )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	/// ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	/// ALTER TABLE [dbo].[ResourceItemLocalization] ADD  CONSTRAINT [DF_ResourceItemLocalization_Value]  DEFAULT (N'') FOR [Value]
	/// ALTER TABLE [dbo].[ResourceItemLocalization]  WITH NOCHECK ADD  CONSTRAINT [FK_ResourceItemLocalization_Language] FOREIGN KEY([LanguageID])
	/// REFERENCES [dbo].[Language] ([LanguageID])
	/// ALTER TABLE [dbo].[ResourceItemLocalization] CHECK CONSTRAINT [FK_ResourceItemLocalization_Language]
	/// ALTER TABLE [dbo].[ResourceItemLocalization]  WITH NOCHECK ADD  CONSTRAINT [FK_ResourceItemLocalization_ResourceItem] FOREIGN KEY([ResourceItemID])
	/// REFERENCES [dbo].[ResourceItem] ([ResourceItemID])
	/// ALTER TABLE [dbo].[ResourceItemLocalization] CHECK CONSTRAINT [FK_ResourceItemLocalization_ResourceItem]
	/// </code>
	/// </remarks>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public abstract class ResourceItemLocalizationBase : ActiveRecordBusinessObjectBase
	{
		#region Static constructor
		static ResourceItemLocalizationBase()
		{
			objectInfo = new ObjectInfo();
			properties = new ResourceItemLocalizationProperties();
			objectInfo.Initialize("dbo", "ResourceItemLocalization", "ResourceItemLocalization", "Havit.BusinessLayerTest.Resources", false, null, ResourceItemLocalization.GetObject, ResourceItemLocalization.GetAll, null, properties.All);
			properties.Initialize(objectInfo);
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Vytvoří instanci objektu jako nový prvek.
		/// </summary>
		/// <param name="connectionMode">Režim business objektu.</param>
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected ResourceItemLocalizationBase(ConnectionMode connectionMode) : base(connectionMode)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci existujícího objektu.
		/// </summary>
		/// <param name="id">ResourceItemLocalizationID (PK).</param>
		/// <param name="connectionMode">Režim business objektu.</param>
		protected ResourceItemLocalizationBase(int id, ConnectionMode connectionMode) : base(id, connectionMode)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.
		/// </summary>
		/// <param name="id">ResourceItemLocalizationID (PK).</param>
		/// <param name="record">DataRecord s daty objektu (i částečnými).</param>
		protected ResourceItemLocalizationBase(int id, DataRecord record) : base(id, record)
		{
		}
		#endregion
		
		#region Properties dle sloupců databázové tabulky
		/// <summary>
		/// Lokalizovaný objekt. [int, not-null]
		/// </summary>
		public virtual Havit.BusinessLayerTest.Resources.ResourceItem ResourceItem
		{
			get
			{
				EnsureLoaded();
				return _ResourceItemPropertyHolder.Value;
			}
			set
			{
				EnsureLoaded();
				
				if (!Object.Equals(_ResourceItemPropertyHolder.Value, value))
				{
					Havit.BusinessLayerTest.Resources.ResourceItem oldValue = _ResourceItemPropertyHolder.Value;
					_ResourceItemPropertyHolder.Value = value;
					OnPropertyChanged(new PropertyChangedEventArgs(nameof(ResourceItem), oldValue, value));
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost ResourceItem.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<Havit.BusinessLayerTest.Resources.ResourceItem> _ResourceItemPropertyHolder;
		
		/// <summary>
		/// Jazyk lokalizovaných dat. [int, not-null]
		/// </summary>
		public virtual Havit.BusinessLayerTest.Language Language
		{
			get
			{
				EnsureLoaded();
				return _LanguagePropertyHolder.Value;
			}
			set
			{
				EnsureLoaded();
				
				if (!Object.Equals(_LanguagePropertyHolder.Value, value))
				{
					Havit.BusinessLayerTest.Language oldValue = _LanguagePropertyHolder.Value;
					_LanguagePropertyHolder.Value = value;
					OnPropertyChanged(new PropertyChangedEventArgs(nameof(Language), oldValue, value));
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Language.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<Havit.BusinessLayerTest.Language> _LanguagePropertyHolder;
		
		/// <summary>
		/// Hodnota. [nvarchar(MAX), not-null, default N'']
		/// </summary>
		public virtual string Value
		{
			get
			{
				EnsureLoaded();
				return _ValuePropertyHolder.Value;
			}
			set
			{
				EnsureLoaded();
				
				string newValue = value ?? String.Empty;
				if (!Object.Equals(_ValuePropertyHolder.Value, newValue))
				{
					string oldValue = _ValuePropertyHolder.Value;
					_ValuePropertyHolder.Value = newValue;
					OnPropertyChanged(new PropertyChangedEventArgs(nameof(Value), oldValue, newValue));
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Value.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<string> _ValuePropertyHolder;
		
		#endregion
		
		#region Init
		/// <summary>
		/// Inicializuje třídu (vytvoří instance PropertyHolderů).
		/// </summary>
		protected override void Init()
		{
			_ResourceItemPropertyHolder = new PropertyHolder<Havit.BusinessLayerTest.Resources.ResourceItem>(this);
			_LanguagePropertyHolder = new PropertyHolder<Havit.BusinessLayerTest.Language>(this);
			_ValuePropertyHolder = new PropertyHolder<string>(this);
			
			if (IsNew || IsDisconnected)
			{
				_ResourceItemPropertyHolder.Value = null;
				_LanguagePropertyHolder.Value = null;
				_ValuePropertyHolder.Value = String.Empty;
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
			
			_ResourceItemPropertyHolder.IsDirty = false;
			_LanguagePropertyHolder.IsDirty = false;
			_ValuePropertyHolder.IsDirty = false;
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
			
			if (_ResourceItemPropertyHolder.IsDirty && (_ResourceItemPropertyHolder.Value == null))
			{
				throw new ConstraintViolationException(this, "Vlastnost \"ResourceItem\" nesmí nabývat hodnoty null.");
			}
			
			if (_LanguagePropertyHolder.IsDirty && (_LanguagePropertyHolder.Value == null))
			{
				throw new ConstraintViolationException(this, "Vlastnost \"Language\" nesmí nabývat hodnoty null.");
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
			
			result = GetDataRecordFromCache(this.ID);
			if (result != null)
			{
				return result;
			}
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.CommandText = "SELECT [ResourceItemLocalizationID], [ResourceItemID], [LanguageID], [Value] FROM [dbo].[ResourceItemLocalization] WHERE [ResourceItemLocalizationID] = @ResourceItemLocalizationID";
			dbCommand.Transaction = transaction;
			
			DbParameter dbParameterResourceItemLocalizationID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceItemLocalizationID.DbType = DbType.Int32;
			dbParameterResourceItemLocalizationID.Direction = ParameterDirection.Input;
			dbParameterResourceItemLocalizationID.ParameterName = "ResourceItemLocalizationID";
			dbParameterResourceItemLocalizationID.Value = this.ID;
			dbCommand.Parameters.Add(dbParameterResourceItemLocalizationID);
			
			result = DbConnector.Default.ExecuteDataRecord(dbCommand);
			if (result != null)
			{
				AddDataRecordToCache(result);
			}
			
			return result;
		}
		
		/// <summary>
		/// Vytahá data objektu z DataRecordu.
		/// </summary>
		/// <param name="record">DataRecord s daty objektu</param>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Load_ParseDataRecord(DataRecord record)
		{
			this.ID = record.Get<int>("ResourceItemLocalizationID");
			
			int _tempResourceItem;
			if (record.TryGet<int>("ResourceItemID", out _tempResourceItem))
			{
				_ResourceItemPropertyHolder.Value = Havit.BusinessLayerTest.Resources.ResourceItem.GetObject(_tempResourceItem);
			}
			
			int _tempLanguage;
			if (record.TryGet<int>("LanguageID", out _tempLanguage))
			{
				_LanguagePropertyHolder.Value = Havit.BusinessLayerTest.Language.GetObject(_tempLanguage);
			}
			
			string _tempValue;
			if (record.TryGet<string>("Value", out _tempValue))
			{
				_ValuePropertyHolder.Value = _tempValue ?? String.Empty;
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
			
			if (_ResourceItemPropertyHolder.IsInitialized && (_ResourceItemPropertyHolder.Value != null))
			{
				_ResourceItemPropertyHolder.Value.Save(transaction);
			}
			
			if (_LanguagePropertyHolder.IsInitialized && (_LanguagePropertyHolder.Value != null))
			{
				_LanguagePropertyHolder.Value.Save(transaction);
			}
			
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
			
			DbParameter dbParameterResourceItemID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceItemID.DbType = DbType.Int32;
			dbParameterResourceItemID.Direction = ParameterDirection.Input;
			dbParameterResourceItemID.ParameterName = "ResourceItemID";
			dbParameterResourceItemID.Value = (_ResourceItemPropertyHolder.Value == null) ? DBNull.Value : (object)_ResourceItemPropertyHolder.Value.ID;
			dbCommand.Parameters.Add(dbParameterResourceItemID);
			_ResourceItemPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterLanguageID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterLanguageID.DbType = DbType.Int32;
			dbParameterLanguageID.Direction = ParameterDirection.Input;
			dbParameterLanguageID.ParameterName = "LanguageID";
			dbParameterLanguageID.Value = (_LanguagePropertyHolder.Value == null) ? DBNull.Value : (object)_LanguagePropertyHolder.Value.ID;
			dbCommand.Parameters.Add(dbParameterLanguageID);
			_LanguagePropertyHolder.IsDirty = false;
			
			DbParameter dbParameterValue = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterValue.DbType = DbType.String;
			dbParameterValue.Size = -1;
			dbParameterValue.Direction = ParameterDirection.Input;
			dbParameterValue.ParameterName = "Value";
			dbParameterValue.Value = _ValuePropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterValue);
			_ValuePropertyHolder.IsDirty = false;
			
			dbCommand.CommandText = "DECLARE @ResourceItemLocalizationID INT; INSERT INTO [dbo].[ResourceItemLocalization] ([ResourceItemID], [LanguageID], [Value]) VALUES (@ResourceItemID, @LanguageID, @Value); SELECT @ResourceItemLocalizationID = SCOPE_IDENTITY(); SELECT @ResourceItemLocalizationID; ";
			this.ID = (int)DbConnector.Default.ExecuteScalar(dbCommand);
			this.IsNew = false; // uložený objekt není už nový, dostal i přidělené ID
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			global::Havit.Diagnostics.Contracts.Contract.Assert(currentIdentityMap != null, "currentIdentityMap != null");
			currentIdentityMap.Store(this);
			
			RemoveAllIDsFromCache();
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
			
			DbParameter dbParameterResourceItemID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceItemID.DbType = DbType.Int32;
			dbParameterResourceItemID.Direction = ParameterDirection.Input;
			dbParameterResourceItemID.ParameterName = "ResourceItemID";
			dbParameterResourceItemID.Value = (_ResourceItemPropertyHolder.Value == null) ? DBNull.Value : (object)_ResourceItemPropertyHolder.Value.ID;
			dbCommand.Parameters.Add(dbParameterResourceItemID);
			_ResourceItemPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterLanguageID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterLanguageID.DbType = DbType.Int32;
			dbParameterLanguageID.Direction = ParameterDirection.Input;
			dbParameterLanguageID.ParameterName = "LanguageID";
			dbParameterLanguageID.Value = (_LanguagePropertyHolder.Value == null) ? DBNull.Value : (object)_LanguagePropertyHolder.Value.ID;
			dbCommand.Parameters.Add(dbParameterLanguageID);
			_LanguagePropertyHolder.IsDirty = false;
			
			DbParameter dbParameterValue = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterValue.DbType = DbType.String;
			dbParameterValue.Size = -1;
			dbParameterValue.Direction = ParameterDirection.Input;
			dbParameterValue.ParameterName = "Value";
			dbParameterValue.Value = _ValuePropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterValue);
			_ValuePropertyHolder.IsDirty = false;
			
			dbCommand.CommandText = "DECLARE @ResourceItemLocalizationID INT; INSERT INTO [dbo].[ResourceItemLocalization] ([ResourceItemID], [LanguageID], [Value]) VALUES (@ResourceItemID, @LanguageID, @Value); SELECT @ResourceItemLocalizationID = SCOPE_IDENTITY(); SELECT @ResourceItemLocalizationID; ";
			this.ID = (int)DbConnector.Default.ExecuteScalar(dbCommand);
			this.IsNew = false; // uložený objekt není už nový, dostal i přidělené ID
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			global::Havit.Diagnostics.Contracts.Contract.Assert(currentIdentityMap != null, "currentIdentityMap != null");
			currentIdentityMap.Store(this);
			
			RemoveAllIDsFromCache();
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
			commandBuilder.Append("UPDATE [dbo].[ResourceItemLocalization] SET ");
			
			bool dirtyFieldExists = false;
			if (_ResourceItemPropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[ResourceItemID] = @ResourceItemID");
				
				DbParameter dbParameterResourceItemID = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterResourceItemID.DbType = DbType.Int32;
				dbParameterResourceItemID.Direction = ParameterDirection.Input;
				dbParameterResourceItemID.ParameterName = "ResourceItemID";
				dbParameterResourceItemID.Value = (_ResourceItemPropertyHolder.Value == null) ? DBNull.Value : (object)_ResourceItemPropertyHolder.Value.ID;
				dbCommand.Parameters.Add(dbParameterResourceItemID);
				
				dirtyFieldExists = true;
			}
			
			if (_LanguagePropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[LanguageID] = @LanguageID");
				
				DbParameter dbParameterLanguageID = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterLanguageID.DbType = DbType.Int32;
				dbParameterLanguageID.Direction = ParameterDirection.Input;
				dbParameterLanguageID.ParameterName = "LanguageID";
				dbParameterLanguageID.Value = (_LanguagePropertyHolder.Value == null) ? DBNull.Value : (object)_LanguagePropertyHolder.Value.ID;
				dbCommand.Parameters.Add(dbParameterLanguageID);
				
				dirtyFieldExists = true;
			}
			
			if (_ValuePropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[Value] = @Value");
				
				DbParameter dbParameterValue = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterValue.DbType = DbType.String;
				dbParameterValue.Size = -1;
				dbParameterValue.Direction = ParameterDirection.Input;
				dbParameterValue.ParameterName = "Value";
				dbParameterValue.Value = _ValuePropertyHolder.Value ?? String.Empty;
				dbCommand.Parameters.Add(dbParameterValue);
				
				dirtyFieldExists = true;
			}
			
			if (dirtyFieldExists)
			{
				// objekt je sice IsDirty (volá se tato metoda), ale může být změněná jen kolekce
				commandBuilder.Append(" WHERE [ResourceItemLocalizationID] = @ResourceItemLocalizationID; ");
			}
			else
			{
				commandBuilder = new StringBuilder();
			}
			
			bool dirtyCollectionExists = false;
			// pokud je objekt dirty, ale žádná property není dirty (Save_MinimalInsert poukládal všechno), neukládáme
			if (dirtyFieldExists || dirtyCollectionExists)
			{
				DbParameter dbParameterResourceItemLocalizationID = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterResourceItemLocalizationID.DbType = DbType.Int32;
				dbParameterResourceItemLocalizationID.Direction = ParameterDirection.Input;
				dbParameterResourceItemLocalizationID.ParameterName = "ResourceItemLocalizationID";
				dbParameterResourceItemLocalizationID.Value = this.ID;
				dbCommand.Parameters.Add(dbParameterResourceItemLocalizationID);
				dbCommand.CommandText = commandBuilder.ToString();
				DbConnector.Default.ExecuteNonQuery(dbCommand);
			}
			
			RemoveDataRecordFromCache();
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
			
			if (_ResourceItemPropertyHolder.Value.IsNew)
			{
				_ResourceItemPropertyHolder.Value.Save_MinimalInsert(transaction);
			}
			
			if (_LanguagePropertyHolder.Value.IsNew)
			{
				_LanguagePropertyHolder.Value.Save_MinimalInsert(transaction);
			}
			
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
			ResourceItem.Localizations.Remove((ResourceItemLocalization)this);
			base.Delete(transaction);
		}
		
		/// <summary>
		/// Metoda vymaže objekt z perzistentního uložiště.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Delete_Perform(DbTransaction transaction)
		{
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			StringBuilder commandBuilder = new StringBuilder();
			commandBuilder.AppendFormat("DELETE FROM [dbo].[ResourceItemLocalization] WHERE [ResourceItemLocalizationID] = @ResourceItemLocalizationID");
			
			DbParameter dbParameterResourceItemLocalizationID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceItemLocalizationID.DbType = DbType.Int32;
			dbParameterResourceItemLocalizationID.Direction = ParameterDirection.Input;
			dbParameterResourceItemLocalizationID.ParameterName = "ResourceItemLocalizationID";
			dbParameterResourceItemLocalizationID.Value = this.ID;
			dbCommand.Parameters.Add(dbParameterResourceItemLocalizationID);
			
			dbCommand.CommandText = commandBuilder.ToString();
			DbConnector.Default.ExecuteNonQuery(dbCommand);
			
			RemoveDataRecordFromCache();
			RemoveAllIDsFromCache();
			InvalidateSaveCacheDependencyKey();
			InvalidateAnySaveCacheDependencyKey();
		}
		
		#endregion
		
		#region DataRecord cache access methods
		/// <summary>
		/// Vrátí název klíče pro data record objektu s daným ID.
		/// </summary>
		private static string GetDataRecordCacheKey(int id)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(id != BusinessObjectBase.NoID, "id != BusinessObjectBase.NoID");
			
			return "BL|ResourceItemLocalization|" + id;
		}
		
		/// <summary>
		/// Přidá DataRecord do cache.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal void AddDataRecordToCache(DataRecord dataRecord)
		{
			Havit.Business.BusinessLayerContext.BusinessLayerCacheService.AddDataRecordToCache(typeof(ResourceItemLocalization), GetDataRecordCacheKey(this.ID), dataRecord);
		}
		
		/// <summary>
		/// Vyhledá v cache DataRecord pro objekt daného ID a vrátí jej. Není-li v cache nalezen, vrací null.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static DataRecord GetDataRecordFromCache(int id)
		{
			return Havit.Business.BusinessLayerContext.BusinessLayerCacheService.GetDataRecordFromCache(typeof(ResourceItemLocalization), GetDataRecordCacheKey(id));
		}
		
		/// <summary>
		/// Odstraní DataRecord z cache.
		/// </summary>
		private void RemoveDataRecordFromCache()
		{
			Havit.Business.BusinessLayerContext.BusinessLayerCacheService.RemoveDataRecordFromCache(typeof(ResourceItemLocalization), GetDataRecordCacheKey(this.ID));
		}
		#endregion
		
		#region GetAll IDs cache access methods
		/// <summary>
		/// Vrátí název klíče pro kolekci IDs metody GetAll.
		/// </summary>
		private static string GetAllIDsCacheKey()
		{
			return "BL|ResourceItemLocalization|GetAll";
		}
		
		/// <summary>
		/// Vyhledá v cache pole IDs metody GetAll a vrátí jej. Není-li v cache nalezena, vrací null.
		/// </summary>
		private static int[] GetAllIDsFromCache()
		{
			return Havit.Business.BusinessLayerContext.BusinessLayerCacheService.GetAllIDsFromCache(typeof(ResourceItemLocalization), GetAllIDsCacheKey());
		}
		
		/// <summary>
		/// Přidá pole IDs metody GetAll do cache.
		/// </summary>
		private static void AddAllIDsToCache(int[] ids)
		{
			Havit.Business.BusinessLayerContext.BusinessLayerCacheService.AddAllIDsToCache(typeof(ResourceItemLocalization), GetAllIDsCacheKey(), ids);
		}
		
		/// <summary>
		/// Odstraní pole IDs metody GetAll z cache.
		/// </summary>
		private static void RemoveAllIDsFromCache()
		{
			Havit.Business.BusinessLayerContext.BusinessLayerCacheService.RemoveAllIDsFromCache(typeof(ResourceItemLocalization), GetAllIDsCacheKey());
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
			
			string key = "BL|ResourceItemLocalization|SaveDK|" + this.ID;
			
			if (ensureInCache)
			{
				Havit.Business.BusinessLayerContext.BusinessLayerCacheService.EnsureCacheDependencyKey(typeof(ResourceItemLocalization), key);
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
				Havit.Business.BusinessLayerContext.BusinessLayerCacheService.InvalidateCacheDependencies(typeof(ResourceItemLocalization), GetSaveCacheDependencyKey(false));
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
			
			string key = "BL|ResourceItemLocalization|AnySaveDK";
			
			if (ensureInCache)
			{
				Havit.Business.BusinessLayerContext.BusinessLayerCacheService.EnsureCacheDependencyKey(typeof(ResourceItemLocalization), key);
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
				Havit.Business.BusinessLayerContext.BusinessLayerCacheService.InvalidateCacheDependencies(typeof(ResourceItemLocalization), GetAnySaveCacheDependencyKey(false));
			}
		}
		#endregion
		
		#region GetFirst, GetList, GetAll
		/// <summary>
		/// Vrátí první nalezený objekt typu ResourceItemLocalization dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null.
		/// </summary>
		public static ResourceItemLocalization GetFirst(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return ResourceItemLocalization.GetFirst(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí první nalezený objekt typu ResourceItemLocalization dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null. Data jsou načítána v předané transakci.
		/// </summary>
		public static ResourceItemLocalization GetFirst(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			int? originalTopRecords = queryParams.TopRecords;
			queryParams.TopRecords = 1;
			ResourceItemLocalizationCollection getListResult = ResourceItemLocalization.GetList(queryParams, transaction);
			queryParams.TopRecords = originalTopRecords;
			return (getListResult.Count == 0) ? null : getListResult[0];
		}
		
		/// <summary>
		/// Vrátí objekty typu ResourceItemLocalization dle parametrů v queryParams.
		/// </summary>
		internal static ResourceItemLocalizationCollection GetList(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return ResourceItemLocalization.GetList(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí objekty typu ResourceItemLocalization dle parametrů v queryParams. Data jsou načítána v předané transakci.
		/// </summary>
		internal static ResourceItemLocalizationCollection GetList(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			queryParams.ObjectInfo = ResourceItemLocalization.ObjectInfo;
			if (queryParams.Properties.Count > 0)
			{
				queryParams.Properties.Add(ResourceItemLocalization.Properties.ID);
			}
			
			queryParams.PrepareCommand(dbCommand, SqlServerPlatform.SqlServer2008, CommandBuilderOptions.None);
			return ResourceItemLocalization.GetList(dbCommand, queryParams.GetDataLoadPower());
		}
		
		private static ResourceItemLocalizationCollection GetList(DbCommand dbCommand, DataLoadPower dataLoadPower)
		{
			if (dbCommand == null)
			{
				throw new ArgumentNullException("dbCommand");
			}
			
			ResourceItemLocalizationCollection result = new ResourceItemLocalizationCollection();
			
			using (DbDataReader reader = DbConnector.Default.ExecuteReader(dbCommand))
			{
				while (reader.Read())
				{
					DataRecord dataRecord = new DataRecord(reader, dataLoadPower);
					ResourceItemLocalization resourceItemLocalization = ResourceItemLocalization.GetObject(dataRecord);
					if (dataLoadPower == DataLoadPower.FullLoad)
					{
						resourceItemLocalization.AddDataRecordToCache(dataRecord);
					}
					result.Add(resourceItemLocalization);
				}
			}
			
			return result;
		}
		
		private static object lockGetAllCacheAccess = new object();
		
		/// <summary>
		/// Vrátí všechny objekty typu ResourceItemLocalization.
		/// </summary>
		public static ResourceItemLocalizationCollection GetAll()
		{
			ResourceItemLocalizationCollection collection = null;
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
						collection = ResourceItemLocalization.GetList(queryParams);
						ids = collection.GetIDs();
						
						AddAllIDsToCache(ids);
					}
				}
			}
			if (collection == null)
			{
				collection = new ResourceItemLocalizationCollection();
				collection.AddRange(ResourceItemLocalization.GetObjects(ids));
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
			if (IsNew)
			{
				return "ResourceItemLocalization(New)";
			}
			
			return String.Format("ResourceItemLocalization(ID={0})", this.ID);
		}
		#endregion
		
		#region ObjectInfo
		/// <summary>
		/// Objektová reprezentace metadat typu ResourceItemLocalization.
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
		/// Objektová reprezentace metadat vlastností typu ResourceItemLocalization.
		/// </summary>
		public static ResourceItemLocalizationProperties Properties
		{
			get
			{
				return properties;
			}
		}
		private static ResourceItemLocalizationProperties properties;
		#endregion
		
	}
}
