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
	/// Měna. [cached]
	/// </summary>
	/// <remarks>
	/// <code>
	/// CREATE TABLE [dbo].[Currency](
	/// 	[CurrencyID] [int] IDENTITY(1,1) NOT NULL,
	/// 	[Nazev] [nvarchar](50) COLLATE Czech_CI_AS NOT NULL CONSTRAINT [DF_Currency_Nazev]  DEFAULT (''),
	/// 	[Zkratka] [nvarchar](5) COLLATE Czech_CI_AS NOT NULL CONSTRAINT [DF_Currency_Zkratka]  DEFAULT (''),
	/// 	[Created] [smalldatetime] NOT NULL CONSTRAINT [DF_Currency_Created]  DEFAULT (getdate()),
	///  CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
	/// (
	/// 	[CurrencyID] ASC
	/// )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	/// ) ON [PRIMARY]
	/// </code>
	/// </remarks>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public abstract class CurrencyBase : ActiveRecordBusinessObjectBase
	{
		#region Static constructor
		static CurrencyBase()
		{
			objectInfo = new ObjectInfo();
			properties = new CurrencyProperties();
			objectInfo.Initialize("dbo", "Currency", "Currency", "Havit.BusinessLayerTest", false, Currency.CreateObject, Currency.GetObject, Currency.GetAll, null, properties.All);
			properties.Initialize(objectInfo);
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Vytvoří instanci objektu jako nový prvek.
		/// </summary>
		/// <param name="connectionMode">Režim business objektu.</param>
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected CurrencyBase(ConnectionMode connectionMode) : base(connectionMode)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci existujícího objektu.
		/// </summary>
		/// <param name="id">CurrencyID (PK).</param>
		/// <param name="connectionMode">Režim business objektu.</param>
		protected CurrencyBase(int id, ConnectionMode connectionMode) : base(id, connectionMode)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.
		/// </summary>
		/// <param name="id">CurrencyID (PK).</param>
		/// <param name="record">DataRecord s daty objektu (i částečnými).</param>
		protected CurrencyBase(int id, DataRecord record) : base(id, record)
		{
		}
		#endregion
		
		#region Properties dle sloupců databázové tabulky
		/// <summary>
		/// Název měny. [nvarchar(50), not-null, default '']
		/// </summary>
		public virtual string Nazev
		{
			get
			{
				EnsureLoaded();
				return _NazevPropertyHolder.Value;
			}
			set
			{
				EnsureLoaded();
				if (value == null)
				{
					_NazevPropertyHolder.Value = String.Empty;
				}
				else
				{
					_NazevPropertyHolder.Value = value;
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Nazev.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<string> _NazevPropertyHolder;
		
		/// <summary>
		/// Zkratka měny, běžná, např. Kč. [nvarchar(5), not-null, default '']
		/// </summary>
		public virtual string Zkratka
		{
			get
			{
				EnsureLoaded();
				return _ZkratkaPropertyHolder.Value;
			}
			set
			{
				EnsureLoaded();
				if (value == null)
				{
					_ZkratkaPropertyHolder.Value = String.Empty;
				}
				else
				{
					_ZkratkaPropertyHolder.Value = value;
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Zkratka.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<string> _ZkratkaPropertyHolder;
		
		/// <summary>
		/// Čas vytvoření objektu. [smalldatetime, not-null, read-only, default getdate()]
		/// </summary>
		public virtual DateTime Created
		{
			get
			{
				EnsureLoaded();
				return _CreatedPropertyHolder.Value;
			}
			private set
			{
				EnsureLoaded();
				_CreatedPropertyHolder.Value = value;
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Created.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<DateTime> _CreatedPropertyHolder;
		
		#endregion
		
		#region Init
		/// <summary>
		/// Inicializuje třídu (vytvoří instance PropertyHolderů).
		/// </summary>
		protected override void Init()
		{
			_NazevPropertyHolder = new PropertyHolder<string>(this);
			_ZkratkaPropertyHolder = new PropertyHolder<string>(this);
			_CreatedPropertyHolder = new PropertyHolder<DateTime>(this);
			
			if (IsNew || IsDisconnected)
			{
				_NazevPropertyHolder.Value = String.Empty;
				_ZkratkaPropertyHolder.Value = String.Empty;
				_CreatedPropertyHolder.Value = System.DateTime.Now;
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
			
			_NazevPropertyHolder.IsDirty = false;
			_ZkratkaPropertyHolder.IsDirty = false;
			_CreatedPropertyHolder.IsDirty = false;
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
			
			if (_NazevPropertyHolder.IsDirty && (_NazevPropertyHolder.Value != null) && (_NazevPropertyHolder.Value.Length > 50))
			{
				throw new ConstraintViolationException(this, "Vlastnost \"Nazev\" - řetězec přesáhl maximální délku 50 znaků.");
			}
			
			if (_ZkratkaPropertyHolder.IsDirty && (_ZkratkaPropertyHolder.Value != null) && (_ZkratkaPropertyHolder.Value.Length > 5))
			{
				throw new ConstraintViolationException(this, "Vlastnost \"Zkratka\" - řetězec přesáhl maximální délku 5 znaků.");
			}
			
			if (_CreatedPropertyHolder.IsDirty)
			{
				if ((_CreatedPropertyHolder.Value < Havit.Data.SqlTypes.SqlSmallDateTime.MinValue.Value) || (_CreatedPropertyHolder.Value > Havit.Data.SqlTypes.SqlSmallDateTime.MaxValue.Value))
				{
					throw new ConstraintViolationException(this, "PropertyHolder \"_CreatedPropertyHolder\" nesmí nabývat hodnoty mimo rozsah SqlSmallDateTime.MinValue-SqlSmallDateTime.MaxValue.");
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
			
			result = GetDataRecordFromCache(this.ID);
			if (result != null)
			{
				return result;
			}
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.CommandText = "SELECT [CurrencyID], [Nazev], [Zkratka], [Created] FROM [dbo].[Currency] WHERE [CurrencyID] = @CurrencyID";
			dbCommand.Transaction = transaction;
			
			DbParameter dbParameterCurrencyID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterCurrencyID.DbType = DbType.Int32;
			dbParameterCurrencyID.Direction = ParameterDirection.Input;
			dbParameterCurrencyID.ParameterName = "CurrencyID";
			dbParameterCurrencyID.Value = this.ID;
			dbCommand.Parameters.Add(dbParameterCurrencyID);
			
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
			this.ID = record.Get<int>("CurrencyID");
			
			string _tempNazev;
			if (record.TryGet<string>("Nazev", out _tempNazev))
			{
				_NazevPropertyHolder.Value = _tempNazev ?? String.Empty;
			}
			
			string _tempZkratka;
			if (record.TryGet<string>("Zkratka", out _tempZkratka))
			{
				_ZkratkaPropertyHolder.Value = _tempZkratka ?? String.Empty;
			}
			
			DateTime _tempCreated;
			if (record.TryGet<DateTime>("Created", out _tempCreated))
			{
				_CreatedPropertyHolder.Value = _tempCreated;
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
			
			DbParameter dbParameterNazev = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterNazev.DbType = DbType.String;
			dbParameterNazev.Size = 50;
			dbParameterNazev.Direction = ParameterDirection.Input;
			dbParameterNazev.ParameterName = "Nazev";
			dbParameterNazev.Value = _NazevPropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterNazev);
			_NazevPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterZkratka = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterZkratka.DbType = DbType.String;
			dbParameterZkratka.Size = 5;
			dbParameterZkratka.Direction = ParameterDirection.Input;
			dbParameterZkratka.ParameterName = "Zkratka";
			dbParameterZkratka.Value = _ZkratkaPropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterZkratka);
			_ZkratkaPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterCreated = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterCreated.DbType = DbType.DateTime;
			dbParameterCreated.Direction = ParameterDirection.Input;
			dbParameterCreated.ParameterName = "Created";
			dbParameterCreated.Value = _CreatedPropertyHolder.Value;
			dbCommand.Parameters.Add(dbParameterCreated);
			_CreatedPropertyHolder.IsDirty = false;
			
			dbCommand.CommandText = "DECLARE @CurrencyID INT; INSERT INTO [dbo].[Currency] ([Nazev], [Zkratka], [Created]) VALUES (@Nazev, @Zkratka, @Created); SELECT @CurrencyID = SCOPE_IDENTITY(); SELECT @CurrencyID; ";
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
			
			DbParameter dbParameterNazev = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterNazev.DbType = DbType.String;
			dbParameterNazev.Size = 50;
			dbParameterNazev.Direction = ParameterDirection.Input;
			dbParameterNazev.ParameterName = "Nazev";
			dbParameterNazev.Value = _NazevPropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterNazev);
			_NazevPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterZkratka = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterZkratka.DbType = DbType.String;
			dbParameterZkratka.Size = 5;
			dbParameterZkratka.Direction = ParameterDirection.Input;
			dbParameterZkratka.ParameterName = "Zkratka";
			dbParameterZkratka.Value = _ZkratkaPropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterZkratka);
			_ZkratkaPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterCreated = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterCreated.DbType = DbType.DateTime;
			dbParameterCreated.Direction = ParameterDirection.Input;
			dbParameterCreated.ParameterName = "Created";
			dbParameterCreated.Value = _CreatedPropertyHolder.Value;
			dbCommand.Parameters.Add(dbParameterCreated);
			_CreatedPropertyHolder.IsDirty = false;
			
			dbCommand.CommandText = "DECLARE @CurrencyID INT; INSERT INTO [dbo].[Currency] ([Nazev], [Zkratka], [Created]) VALUES (@Nazev, @Zkratka, @Created); SELECT @CurrencyID = SCOPE_IDENTITY(); SELECT @CurrencyID; ";
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
			commandBuilder.Append("UPDATE [dbo].[Currency] SET ");
			
			bool dirtyFieldExists = false;
			if (_NazevPropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[Nazev] = @Nazev");
				
				DbParameter dbParameterNazev = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterNazev.DbType = DbType.String;
				dbParameterNazev.Size = 50;
				dbParameterNazev.Direction = ParameterDirection.Input;
				dbParameterNazev.ParameterName = "Nazev";
				dbParameterNazev.Value = _NazevPropertyHolder.Value ?? String.Empty;
				dbCommand.Parameters.Add(dbParameterNazev);
				
				dirtyFieldExists = true;
			}
			
			if (_ZkratkaPropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[Zkratka] = @Zkratka");
				
				DbParameter dbParameterZkratka = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterZkratka.DbType = DbType.String;
				dbParameterZkratka.Size = 5;
				dbParameterZkratka.Direction = ParameterDirection.Input;
				dbParameterZkratka.ParameterName = "Zkratka";
				dbParameterZkratka.Value = _ZkratkaPropertyHolder.Value ?? String.Empty;
				dbCommand.Parameters.Add(dbParameterZkratka);
				
				dirtyFieldExists = true;
			}
			
			if (_CreatedPropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[Created] = @Created");
				
				DbParameter dbParameterCreated = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterCreated.DbType = DbType.DateTime;
				dbParameterCreated.Direction = ParameterDirection.Input;
				dbParameterCreated.ParameterName = "Created";
				dbParameterCreated.Value = _CreatedPropertyHolder.Value;
				dbCommand.Parameters.Add(dbParameterCreated);
				
				dirtyFieldExists = true;
			}
			
			if (dirtyFieldExists)
			{
				// objekt je sice IsDirty (volá se tato metoda), ale může být změněná jen kolekce
				commandBuilder.Append(" WHERE [CurrencyID] = @CurrencyID; ");
			}
			else
			{
				commandBuilder = new StringBuilder();
			}
			
			bool dirtyCollectionExists = false;
			// pokud je objekt dirty, ale žádná property není dirty (Save_MinimalInsert poukládal všechno), neukládáme
			if (dirtyFieldExists || dirtyCollectionExists)
			{
				DbParameter dbParameterCurrencyID = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterCurrencyID.DbType = DbType.Int32;
				dbParameterCurrencyID.Direction = ParameterDirection.Input;
				dbParameterCurrencyID.ParameterName = "CurrencyID";
				dbParameterCurrencyID.Value = this.ID;
				dbCommand.Parameters.Add(dbParameterCurrencyID);
				dbCommand.CommandText = commandBuilder.ToString();
				DbConnector.Default.ExecuteNonQuery(dbCommand);
			}
			
			RemoveDataRecordFromCache();
			RemoveAllIDsFromCache();
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
		/// Metoda vymaže objekt z perzistentního uložiště.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Delete_Perform(DbTransaction transaction)
		{
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			StringBuilder commandBuilder = new StringBuilder();
			commandBuilder.AppendFormat("DELETE FROM [dbo].[Currency] WHERE [CurrencyID] = @CurrencyID");
			
			DbParameter dbParameterCurrencyID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterCurrencyID.DbType = DbType.Int32;
			dbParameterCurrencyID.Direction = ParameterDirection.Input;
			dbParameterCurrencyID.ParameterName = "CurrencyID";
			dbParameterCurrencyID.Value = this.ID;
			dbCommand.Parameters.Add(dbParameterCurrencyID);
			
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
			
			return "Currency.DataRecords|ID=" + id.ToString();
		}
		
		/// <summary>
		/// Přidá DataRecord do cache.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal void AddDataRecordToCache(DataRecord dataRecord)
		{
			Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.AddDataRecordToCache(typeof(Currency), GetDataRecordCacheKey(this.ID), dataRecord);
		}
		
		/// <summary>
		/// Vyhledá v cache DataRecord pro objekt daného ID a vrátí jej. Není-li v cache nalezen, vrací null.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static DataRecord GetDataRecordFromCache(int id)
		{
			return Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.GetDataRecordFromCache(typeof(Currency), GetDataRecordCacheKey(id));
		}
		
		/// <summary>
		/// Odstraní DataRecord z cache.
		/// </summary>
		private void RemoveDataRecordFromCache()
		{
			Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.RemoveDataRecordFromCache(typeof(Currency), GetDataRecordCacheKey(this.ID));
		}
		#endregion
		
		#region GetAll IDs cache access methods
		/// <summary>
		/// Vrátí název klíče pro kolekci IDs metody GetAll.
		/// </summary>
		private static string GetAllIDsCacheKey()
		{
			return "Currency.GetAll";
		}
		
		/// <summary>
		/// Vyhledá v cache pole IDs metody GetAll a vrátí jej. Není-li v cache nalezena, vrací null.
		/// </summary>
		private static int[] GetAllIDsFromCache()
		{
			return Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.GetAllIDsFromCache(typeof(Currency), GetAllIDsCacheKey());
		}
		
		/// <summary>
		/// Přidá pole IDs metody GetAll do cache.
		/// </summary>
		private static void AddAllIDsToCache(int[] ids)
		{
			Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.AddAllIDsToCache(typeof(Currency), GetAllIDsCacheKey(), ids);
		}
		
		/// <summary>
		/// Odstraní pole IDs metody GetAll z cache.
		/// </summary>
		private static void RemoveAllIDsFromCache()
		{
			Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.RemoveAllIDsFromCache(typeof(Currency), GetAllIDsCacheKey());
		}
		#endregion
		
		#region Cache dependencies methods
		/// <summary>
		/// Vrátí klíč pro tvorbu závislostí cache na objektu. Při uložení objektu jsou závislosti invalidovány.
		/// </summary>
		public string GetSaveCacheDependencyKey(bool ensureInCache = true)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(!this.IsNew, "!this.IsNew");
			
			if (!Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.SupportsCacheDependencies)
			{
				throw new InvalidOperationException("Použitá BusinessLayerCacheService nepodporuje cache dependencies.");
			}
			
			string key = "Currency.SaveCacheDependencyKey|ID=" + this.ID.ToString();
			
			if (ensureInCache)
			{
				Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.EnsureCacheDependencyKey(typeof(Currency), key);
			}
			
			return key;
		}
		
		/// <summary>
		/// Odstraní z cache závislosti na klíči CacheDependencyKey.
		/// </summary>
		private void InvalidateSaveCacheDependencyKey()
		{
			if (Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.SupportsCacheDependencies)
			{
				Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.InvalidateCacheDependencies(typeof(Currency), GetSaveCacheDependencyKey(false));
			}
		}
		
		/// <summary>
		/// Vrátí klíč pro tvorbu závislostí cache. Po uložení jakéhokoliv objektu této třídy jsou závislosti invalidovány.
		/// </summary>
		public static string GetAnySaveCacheDependencyKey(bool ensureInCache = true)
		{
			if (!Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.SupportsCacheDependencies)
			{
				throw new InvalidOperationException("Použitá BusinessLayerCacheService nepodporuje cache dependencies.");
			}
			
			string key = "Currency.AnySaveCacheDependencyKey";
			
			if (ensureInCache)
			{
				Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.EnsureCacheDependencyKey(typeof(Currency), key);
			}
			
			return key;
		}
		
		/// <summary>
		/// Odstraní z cache závislosti na klíči AnySaveCacheDependencyKey.
		/// </summary>
		private static void InvalidateAnySaveCacheDependencyKey()
		{
			if (Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.SupportsCacheDependencies)
			{
				Havit.Business.BusinessLayerContexts.BusinessLayerCacheService.InvalidateCacheDependencies(typeof(Currency), GetAnySaveCacheDependencyKey(false));
			}
		}
		#endregion
		
		#region GetFirst, GetList, GetAll
		/// <summary>
		/// Vrátí první nalezený objekt typu Currency dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null.
		/// </summary>
		public static Currency GetFirst(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return Currency.GetFirst(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí první nalezený objekt typu Currency dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null. Data jsou načítána v předané transakci.
		/// </summary>
		public static Currency GetFirst(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			int? originalTopRecords = queryParams.TopRecords;
			queryParams.TopRecords = 1;
			CurrencyCollection getListResult = Currency.GetList(queryParams, transaction);
			queryParams.TopRecords = originalTopRecords;
			return (getListResult.Count == 0) ? null : getListResult[0];
		}
		
		/// <summary>
		/// Vrátí objekty typu Currency dle parametrů v queryParams.
		/// </summary>
		internal static CurrencyCollection GetList(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return Currency.GetList(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí objekty typu Currency dle parametrů v queryParams. Data jsou načítána v předané transakci.
		/// </summary>
		internal static CurrencyCollection GetList(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			queryParams.ObjectInfo = Currency.ObjectInfo;
			if (queryParams.Properties.Count > 0)
			{
				queryParams.Properties.Add(Currency.Properties.ID);
			}
			
			queryParams.PrepareCommand(dbCommand, SqlServerPlatform.SqlServer2008, CommandBuilderOptions.None);
			return Currency.GetList(dbCommand, queryParams.GetDataLoadPower());
		}
		
		private static CurrencyCollection GetList(DbCommand dbCommand, DataLoadPower dataLoadPower)
		{
			if (dbCommand == null)
			{
				throw new ArgumentNullException("dbCommand");
			}
			
			CurrencyCollection result = new CurrencyCollection();
			
			using (DbDataReader reader = DbConnector.Default.ExecuteReader(dbCommand))
			{
				while (reader.Read())
				{
					DataRecord dataRecord = new DataRecord(reader, dataLoadPower);
					Currency currency = Currency.GetObject(dataRecord);
					if (dataLoadPower == DataLoadPower.FullLoad)
					{
						currency.AddDataRecordToCache(dataRecord);
					}
					result.Add(currency);
				}
			}
			
			return result;
		}
		
		private static object lockGetAllCacheAccess = new object();
		
		/// <summary>
		/// Vrátí všechny objekty typu Currency.
		/// </summary>
		public static CurrencyCollection GetAll()
		{
			CurrencyCollection collection = null;
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
						collection = Currency.GetList(queryParams);
						ids = collection.GetIDs();
						
						AddAllIDsToCache(ids);
					}
				}
			}
			if (collection == null)
			{
				collection = new CurrencyCollection();
				collection.AddRange(Currency.GetObjects(ids));
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
				return "Currency(New)";
			}
			
			return String.Format("Currency(ID={0})", this.ID);
		}
		#endregion
		
		#region ObjectInfo
		/// <summary>
		/// Objektová reprezentace metadat typu Currency.
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
		/// Objektová reprezentace metadat vlastností typu Currency.
		/// </summary>
		public static CurrencyProperties Properties
		{
			get
			{
				return properties;
			}
		}
		private static CurrencyProperties properties;
		#endregion
		
	}
}
