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

namespace Havit.BusinessLayerTest.Resources
{
	/// <summary>
	/// Položka resourců, lokalizace. [cached]
	/// </summary>
	/// <remarks>
	/// <code>
	/// CREATE TABLE [dbo].[ResourceItem](
	/// 	[ResourceItemID] [int] IDENTITY(1,1) NOT NULL,
	/// 	[ResourceClassID] [int] NOT NULL,
	/// 	[ResourceKey] [varchar](100) COLLATE Czech_CI_AS NOT NULL,
	/// 	[Description] [nvarchar](200) COLLATE Czech_CI_AS NULL,
	///  CONSTRAINT [PK_ResourceItem] PRIMARY KEY CLUSTERED 
	/// (
	/// 	[ResourceItemID] ASC
	/// )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	///  CONSTRAINT [UQ_ResourceItem_ResourceKey_ResourceClassID] UNIQUE NONCLUSTERED 
	/// (
	/// 	[ResourceClassID] ASC,
	/// 	[ResourceKey] ASC
	/// )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	/// ) ON [PRIMARY]
	/// ALTER TABLE [dbo].[ResourceItem] ADD  CONSTRAINT [DF_ResourceItem_ResourceKey]  DEFAULT ('') FOR [ResourceKey]
	/// ALTER TABLE [dbo].[ResourceItem] ADD  CONSTRAINT [DF_ResourceItem_Description]  DEFAULT ('') FOR [Description]
	/// ALTER TABLE [dbo].[ResourceItem]  WITH NOCHECK ADD  CONSTRAINT [FK_ResourceItem_ResourceClass] FOREIGN KEY([ResourceClassID])
	/// REFERENCES [dbo].[ResourceClass] ([ResourceClassID])
	/// ALTER TABLE [dbo].[ResourceItem] CHECK CONSTRAINT [FK_ResourceItem_ResourceClass]
	/// </code>
	/// </remarks>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public abstract class ResourceItemBase : ActiveRecordBusinessObjectBase, ILocalizable
	{
		#region Static constructor
		static ResourceItemBase()
		{
			objectInfo = new ObjectInfo();
			properties = new ResourceItemProperties();
			objectInfo.Initialize("dbo", "ResourceItem", "ResourceItem", "Havit.BusinessLayerTest.Resources", false, ResourceItem.CreateObject, ResourceItem.GetObject, ResourceItem.GetAll, null, properties.All);
			properties.Initialize(objectInfo);
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Vytvoří instanci objektu jako nový prvek.
		/// </summary>
		protected ResourceItemBase() : base()
		{
		}
		
		/// <summary>
		/// Vytvoří instanci existujícího objektu.
		/// </summary>
		/// <param name="id">ResourceItemID (PK).</param>
		protected ResourceItemBase(int id) : base(id)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.
		/// </summary>
		/// <param name="id">ResourceItemID (PK).</param>
		/// <param name="record">DataRecord s daty objektu (i částečnými).</param>
		protected ResourceItemBase(int id, DataRecord record) : base(id, record)
		{
		}
		#endregion
		
		#region Properties dle sloupců databázové tabulky
		/// <summary>
		/// Třída resources. [int, not-null]
		/// </summary>
		public virtual Havit.BusinessLayerTest.Resources.ResourceClass ResourceClass
		{
			get
			{
				EnsureLoaded();
				return _ResourceClassPropertyHolder.Value;
			}
			set
			{
				EnsureLoaded();
				_ResourceClassPropertyHolder.Value = value;
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost ResourceClass.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<Havit.BusinessLayerTest.Resources.ResourceClass> _ResourceClassPropertyHolder;
		
		/// <summary>
		/// Klíč položky v rámci ResourceClass [varchar(100), not-null, default '']
		/// </summary>
		public virtual string ResourceKey
		{
			get
			{
				EnsureLoaded();
				return _ResourceKeyPropertyHolder.Value;
			}
			set
			{
				EnsureLoaded();
				if (value == null)
				{
					_ResourceKeyPropertyHolder.Value = String.Empty;
				}
				else
				{
					_ResourceKeyPropertyHolder.Value = value;
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost ResourceKey.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<string> _ResourceKeyPropertyHolder;
		
		/// <summary>
		/// Popis pro administraci. [nvarchar(200), nullable, default '']
		/// </summary>
		public virtual string Description
		{
			get
			{
				EnsureLoaded();
				return _DescriptionPropertyHolder.Value;
			}
			set
			{
				EnsureLoaded();
				if (value == null)
				{
					_DescriptionPropertyHolder.Value = String.Empty;
				}
				else
				{
					_DescriptionPropertyHolder.Value = value;
				}
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Description.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected PropertyHolder<string> _DescriptionPropertyHolder;
		
		/// <summary>
		/// Lokalizované hodnoty.
		/// </summary>
		public virtual Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection Localizations
		{
			get
			{
				EnsureLoaded();
				return _LocalizationsPropertyHolder.Value;
			}
		}
		/// <summary>
		/// PropertyHolder pro vlastnost Localizations.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected CollectionPropertyHolder<Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection, Havit.BusinessLayerTest.Resources.ResourceItemLocalization> _LocalizationsPropertyHolder;
		private Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection _loadedLocalizationsValues;
		
		#region CreateLocalization
		/// <summary>
		/// Vytvoří položku lokalizace pro daný jazyk.
		/// </summary>
		public ResourceItemLocalization CreateLocalization(Havit.BusinessLayerTest.Language language)
		{
			return ResourceItemLocalization.CreateObject((ResourceItem)this, language);
		}
		#endregion
		
		#region ILocalizable interface implementation
		/// <summary>
		/// Vytvoří položku lokalizace pro daný jazyk.
		/// </summary>
		BusinessObjectBase ILocalizable.CreateLocalization(ILanguage language)
		{
			return this.CreateLocalization((Havit.BusinessLayerTest.Language)language);
		}
		/// <summary>
		/// Vytvoří položku lokalizace pro daný jazyk.
		/// </summary>
		ILocalizationCollection ILocalizable.Localizations
		{
			get
			{
				return this.Localizations;
			}
		}
		#endregion
		
		#endregion
		
		#region Init
		/// <summary>
		/// Inicializuje třídu (vytvoří instance PropertyHolderů).
		/// </summary>
		protected override void Init()
		{
			_ResourceClassPropertyHolder = new PropertyHolder<Havit.BusinessLayerTest.Resources.ResourceClass>(this);
			_ResourceKeyPropertyHolder = new PropertyHolder<string>(this);
			_DescriptionPropertyHolder = new PropertyHolder<string>(this);
			_LocalizationsPropertyHolder = new CollectionPropertyHolder<Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection, Havit.BusinessLayerTest.Resources.ResourceItemLocalization>(this);
			
			if (IsNew)
			{
				_ResourceClassPropertyHolder.Value = null;
				_ResourceKeyPropertyHolder.Value = String.Empty;
				_DescriptionPropertyHolder.Value = String.Empty;
				_LocalizationsPropertyHolder.Initialize();
			}
			
			base.Init();
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
			
			if (_ResourceClassPropertyHolder.IsDirty && (_ResourceClassPropertyHolder.Value == null))
			{
				throw new ConstraintViolationException(this, "Vlastnost \"ResourceClass\" nesmí nabývat hodnoty null.");
			}
			
			if (_ResourceKeyPropertyHolder.IsDirty && (_ResourceKeyPropertyHolder.Value != null) && (_ResourceKeyPropertyHolder.Value.Length > 100))
			{
				throw new ConstraintViolationException(this, "Vlastnost \"ResourceKey\" - řetězec přesáhl maximální délku 100 znaků.");
			}
			
			if (_DescriptionPropertyHolder.IsDirty && (_DescriptionPropertyHolder.Value != null) && (_DescriptionPropertyHolder.Value.Length > 200))
			{
				throw new ConstraintViolationException(this, "Vlastnost \"Description\" - řetězec přesáhl maximální délku 200 znaků.");
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
			dbCommand.CommandText = "SELECT [ResourceItemID], [ResourceClassID], [ResourceKey], [Description], (SELECT CAST([_items].[ResourceItemLocalizationID] AS NVARCHAR(11)) + '|' FROM [dbo].[ResourceItemLocalization] AS [_items] WHERE ([_items].[ResourceItemID] = @ResourceItemID) FOR XML PATH('')) AS [Localizations] FROM [dbo].[ResourceItem] WHERE [ResourceItemID] = @ResourceItemID";
			dbCommand.Transaction = transaction;
			
			DbParameter dbParameterResourceItemID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceItemID.DbType = DbType.Int32;
			dbParameterResourceItemID.Direction = ParameterDirection.Input;
			dbParameterResourceItemID.ParameterName = "ResourceItemID";
			dbParameterResourceItemID.Value = this.ID;
			dbCommand.Parameters.Add(dbParameterResourceItemID);
			
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
			this.ID = record.Get<int>("ResourceItemID");
			
			int _tempResourceClass;
			if (record.TryGet<int>("ResourceClassID", out _tempResourceClass))
			{
				_ResourceClassPropertyHolder.Value = Havit.BusinessLayerTest.Resources.ResourceClass.GetObject(_tempResourceClass);
			}
			
			string _tempResourceKey;
			if (record.TryGet<string>("ResourceKey", out _tempResourceKey))
			{
				_ResourceKeyPropertyHolder.Value = _tempResourceKey ?? String.Empty;
			}
			
			string _tempDescription;
			if (record.TryGet<string>("Description", out _tempDescription))
			{
				_DescriptionPropertyHolder.Value = _tempDescription ?? String.Empty;
			}
			
			string _tempLocalizations;
			if (record.TryGet<string>("Localizations", out _tempLocalizations))
			{
				_LocalizationsPropertyHolder.Initialize();
				_LocalizationsPropertyHolder.Value.Clear();
				if (_tempLocalizations != null)
				{
					_LocalizationsPropertyHolder.Value.AllowDuplicates = true; // Z výkonových důvodů. Víme, že duplicity nepřidáme.
					string[] _tempLocalizationsItems = _tempLocalizations.Split('|');
					int _tempLocalizationsItemsLength = _tempLocalizationsItems.Length - 1; // za každou (i za poslední) položkou je oddělovač
					for (int i = 0; i < _tempLocalizationsItemsLength; i++)
					{
						_LocalizationsPropertyHolder.Value.Add(Havit.BusinessLayerTest.Resources.ResourceItemLocalization.GetObject(BusinessObjectBase.FastIntParse(_tempLocalizationsItems[i])));
					}
					_LocalizationsPropertyHolder.Value.AllowDuplicates = false;
					_loadedLocalizationsValues = new Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection(_LocalizationsPropertyHolder.Value);
				}
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
			
			if (_ResourceClassPropertyHolder.IsInitialized && (_ResourceClassPropertyHolder.Value != null))
			{
				_ResourceClassPropertyHolder.Value.Save(transaction);
			}
			
		}
		
		/// <summary>
		/// Ukládá member-kolekce objektu.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Save_SaveCollections(DbTransaction transaction)
		{
			base.Save_SaveCollections(transaction);
			
			if (_LocalizationsPropertyHolder.IsInitialized)
			{
				_LocalizationsPropertyHolder.Value.SaveAll(transaction);
			}
			
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
			
			DbParameter dbParameterResourceClassID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceClassID.DbType = DbType.Int32;
			dbParameterResourceClassID.Direction = ParameterDirection.Input;
			dbParameterResourceClassID.ParameterName = "ResourceClassID";
			dbParameterResourceClassID.Value = (_ResourceClassPropertyHolder.Value == null) ? DBNull.Value : (object)_ResourceClassPropertyHolder.Value.ID;
			dbCommand.Parameters.Add(dbParameterResourceClassID);
			_ResourceClassPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterResourceKey = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceKey.DbType = DbType.AnsiString;
			dbParameterResourceKey.Size = 100;
			dbParameterResourceKey.Direction = ParameterDirection.Input;
			dbParameterResourceKey.ParameterName = "ResourceKey";
			dbParameterResourceKey.Value = _ResourceKeyPropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterResourceKey);
			_ResourceKeyPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterDescription = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterDescription.DbType = DbType.String;
			dbParameterDescription.Size = 200;
			dbParameterDescription.Direction = ParameterDirection.Input;
			dbParameterDescription.ParameterName = "Description";
			dbParameterDescription.Value = _DescriptionPropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterDescription);
			_DescriptionPropertyHolder.IsDirty = false;
			
			dbCommand.CommandText = "DECLARE @ResourceItemID INT; INSERT INTO [dbo].[ResourceItem] ([ResourceClassID], [ResourceKey], [Description]) VALUES (@ResourceClassID, @ResourceKey, @Description); SELECT @ResourceItemID = SCOPE_IDENTITY(); SELECT @ResourceItemID; ";
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
			
			DbParameter dbParameterResourceClassID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceClassID.DbType = DbType.Int32;
			dbParameterResourceClassID.Direction = ParameterDirection.Input;
			dbParameterResourceClassID.ParameterName = "ResourceClassID";
			dbParameterResourceClassID.Value = (_ResourceClassPropertyHolder.Value == null) ? DBNull.Value : (object)_ResourceClassPropertyHolder.Value.ID;
			dbCommand.Parameters.Add(dbParameterResourceClassID);
			_ResourceClassPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterResourceKey = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceKey.DbType = DbType.AnsiString;
			dbParameterResourceKey.Size = 100;
			dbParameterResourceKey.Direction = ParameterDirection.Input;
			dbParameterResourceKey.ParameterName = "ResourceKey";
			dbParameterResourceKey.Value = _ResourceKeyPropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterResourceKey);
			_ResourceKeyPropertyHolder.IsDirty = false;
			
			DbParameter dbParameterDescription = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterDescription.DbType = DbType.String;
			dbParameterDescription.Size = 200;
			dbParameterDescription.Direction = ParameterDirection.Input;
			dbParameterDescription.ParameterName = "Description";
			dbParameterDescription.Value = _DescriptionPropertyHolder.Value ?? String.Empty;
			dbCommand.Parameters.Add(dbParameterDescription);
			_DescriptionPropertyHolder.IsDirty = false;
			
			dbCommand.CommandText = "DECLARE @ResourceItemID INT; INSERT INTO [dbo].[ResourceItem] ([ResourceClassID], [ResourceKey], [Description]) VALUES (@ResourceClassID, @ResourceKey, @Description); SELECT @ResourceItemID = SCOPE_IDENTITY(); SELECT @ResourceItemID; ";
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
			commandBuilder.Append("UPDATE [dbo].[ResourceItem] SET ");
			
			bool dirtyFieldExists = false;
			if (_ResourceClassPropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[ResourceClassID] = @ResourceClassID");
				
				DbParameter dbParameterResourceClassID = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterResourceClassID.DbType = DbType.Int32;
				dbParameterResourceClassID.Direction = ParameterDirection.Input;
				dbParameterResourceClassID.ParameterName = "ResourceClassID";
				dbParameterResourceClassID.Value = (_ResourceClassPropertyHolder.Value == null) ? DBNull.Value : (object)_ResourceClassPropertyHolder.Value.ID;
				dbCommand.Parameters.Add(dbParameterResourceClassID);
				
				dirtyFieldExists = true;
			}
			
			if (_ResourceKeyPropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[ResourceKey] = @ResourceKey");
				
				DbParameter dbParameterResourceKey = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterResourceKey.DbType = DbType.AnsiString;
				dbParameterResourceKey.Size = 100;
				dbParameterResourceKey.Direction = ParameterDirection.Input;
				dbParameterResourceKey.ParameterName = "ResourceKey";
				dbParameterResourceKey.Value = _ResourceKeyPropertyHolder.Value ?? String.Empty;
				dbCommand.Parameters.Add(dbParameterResourceKey);
				
				dirtyFieldExists = true;
			}
			
			if (_DescriptionPropertyHolder.IsDirty)
			{
				if (dirtyFieldExists)
				{
					commandBuilder.Append(", ");
				}
				commandBuilder.Append("[Description] = @Description");
				
				DbParameter dbParameterDescription = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterDescription.DbType = DbType.String;
				dbParameterDescription.Size = 200;
				dbParameterDescription.Direction = ParameterDirection.Input;
				dbParameterDescription.ParameterName = "Description";
				dbParameterDescription.Value = _DescriptionPropertyHolder.Value ?? String.Empty;
				dbCommand.Parameters.Add(dbParameterDescription);
				
				dirtyFieldExists = true;
			}
			
			if (dirtyFieldExists)
			{
				// objekt je sice IsDirty (volá se tato metoda), ale může být změněná jen kolekce
				commandBuilder.Append(" WHERE [ResourceItemID] = @ResourceItemID; ");
			}
			else
			{
				commandBuilder = new StringBuilder();
			}
			
			bool dirtyCollectionExists = false;
			if (_LocalizationsPropertyHolder.IsDirty && (_loadedLocalizationsValues != null))
			{
				Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection _localizationsToRemove = new Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection(_loadedLocalizationsValues.Except(_LocalizationsPropertyHolder.Value).Where(item => !item.IsLoaded || (!item.IsDeleted && (item.ResourceItem == this))));
				if (_localizationsToRemove.Count > 0)
				{
					dirtyCollectionExists = true;
					commandBuilder.AppendFormat("DELETE FROM [dbo].[ResourceItemLocalization] WHERE ([ResourceItemID] = @ResourceItemID) AND [ResourceItemLocalizationID] IN (SELECT [Value] FROM @Localizations);");
					SqlParameter dbParameterLocalizations = new SqlParameter("Localizations", SqlDbType.Structured);
					dbParameterLocalizations.TypeName = "dbo.IntTable";
					dbParameterLocalizations.Value = IntTable.GetSqlParameterValue(_localizationsToRemove.GetIDs());
					dbCommand.Parameters.Add(dbParameterLocalizations);
					_loadedLocalizationsValues = new Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection(_LocalizationsPropertyHolder.Value);
				}
			}
			
			// pokud je objekt dirty, ale žádná property není dirty (Save_MinimalInsert poukládal všechno), neukládáme
			if (dirtyFieldExists || dirtyCollectionExists)
			{
				DbParameter dbParameterResourceItemID = DbConnector.Default.ProviderFactory.CreateParameter();
				dbParameterResourceItemID.DbType = DbType.Int32;
				dbParameterResourceItemID.Direction = ParameterDirection.Input;
				dbParameterResourceItemID.ParameterName = "ResourceItemID";
				dbParameterResourceItemID.Value = this.ID;
				dbCommand.Parameters.Add(dbParameterResourceItemID);
				dbCommand.CommandText = commandBuilder.ToString();
				DbConnector.Default.ExecuteNonQuery(dbCommand);
			}
			
			HttpRuntime.Cache.Remove(GetDataRecordCacheKey(this.ID));
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
			
			if (_ResourceClassPropertyHolder.Value.IsNew)
			{
				_ResourceClassPropertyHolder.Value.Save_MinimalInsert(transaction);
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
		/// Metoda vymaže objekt z perzistentního uložiště.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		protected override sealed void Delete_Perform(DbTransaction transaction)
		{
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			StringBuilder commandBuilder = new StringBuilder();
			if (_LocalizationsPropertyHolder.IsDirty && (_loadedLocalizationsValues != null))
			{
				Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection _localizationsToRemove = new Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection(_loadedLocalizationsValues.Except(_LocalizationsPropertyHolder.Value).Where(item => !item.IsLoaded || (!item.IsDeleted && (item.ResourceItem == this))));
				if (_localizationsToRemove.Count > 0)
				{
					commandBuilder.AppendFormat("DELETE FROM [dbo].[ResourceItemLocalization] WHERE ([ResourceItemID] = @ResourceItemID) AND [ResourceItemLocalizationID] IN (SELECT [Value] FROM @Localizations);");
					SqlParameter dbParameterLocalizations = new SqlParameter("Localizations", SqlDbType.Structured);
					dbParameterLocalizations.TypeName = "dbo.IntTable";
					dbParameterLocalizations.Value = IntTable.GetSqlParameterValue(_localizationsToRemove.GetIDs());
					dbCommand.Parameters.Add(dbParameterLocalizations);
					_loadedLocalizationsValues = new Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection(_LocalizationsPropertyHolder.Value);
				}
			}
			
			commandBuilder.AppendFormat("DELETE FROM [dbo].[ResourceItem] WHERE [ResourceItemID] = @ResourceItemID");
			
			DbParameter dbParameterResourceItemID = DbConnector.Default.ProviderFactory.CreateParameter();
			dbParameterResourceItemID.DbType = DbType.Int32;
			dbParameterResourceItemID.Direction = ParameterDirection.Input;
			dbParameterResourceItemID.ParameterName = "ResourceItemID";
			dbParameterResourceItemID.Value = this.ID;
			dbCommand.Parameters.Add(dbParameterResourceItemID);
			
			dbCommand.CommandText = commandBuilder.ToString();
			DbConnector.Default.ExecuteNonQuery(dbCommand);
			
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
			
			return "Resources.ResourceItem.DataRecords|ID=" + id.ToString();
		}
		
		/// <summary>
		/// Přidá DataRecord do cache.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal void AddDataRecordToCache(DataRecord dataRecord)
		{
			string cacheKey = GetDataRecordCacheKey(this.ID);
			HttpRuntime.Cache.Insert(
				cacheKey,
				dataRecord,
				null, // dependencies
				Cache.NoAbsoluteExpiration,
				Cache.NoSlidingExpiration,
				CacheItemPriority.Default,
				null); // callback
		}
		
		/// <summary>
		/// Vyhledá v cache DataRecord pro objekt daného ID a vrátí jej. Není-li v cache nalezen, vrací null.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static DataRecord GetDataRecordFromCache(int id)
		{
			return (DataRecord)HttpRuntime.Cache[GetDataRecordCacheKey(id)];
		}
		#endregion
		
		#region Cache dependencies methods
		/// <summary>
		/// Vrátí klíč pro tvorbu závislostí cache na objektu. Při uložení objektu jsou závislosti invalidovány.
		/// </summary>
		public string GetSaveCacheDependencyKey(bool ensureInCache = true)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(!this.IsNew, "!this.IsNew");
			
			string key = "Resources.ResourceItem.SaveCacheDependencyKey|ID=" + this.ID.ToString();
			if (ensureInCache && (HttpRuntime.Cache[key] == null))
			{
				HttpRuntime.Cache[key] = new object();
			}
			return key;
		}
		
		/// <summary>
		/// Odstraní z cache závislosti na klíči CacheDependencyKey.
		/// </summary>
		protected void InvalidateSaveCacheDependencyKey()
		{
			HttpRuntime.Cache.Remove(GetSaveCacheDependencyKey(false));
		}
		
		/// <summary>
		/// Vrátí klíč pro tvorbu závislostí cache. Po uložení jakéhokoliv objektu této třídy jsou závislosti invalidovány.
		/// </summary>
		public static string GetAnySaveCacheDependencyKey(bool ensureInCache = true)
		{
			string key = "Resources.ResourceItem.AnySaveCacheDependencyKey";
			if (ensureInCache && (HttpRuntime.Cache[key] == null))
			{
				HttpRuntime.Cache[key] = new object();
			}
			return key;
		}
		
		/// <summary>
		/// Odstraní z cache závislosti na klíči AnySaveCacheDependencyKey.
		/// </summary>
		[System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		private static void InvalidateAnySaveCacheDependencyKey()
		{
			HttpRuntime.Cache.Remove(GetAnySaveCacheDependencyKey(false));
		}
		#endregion
		
		#region GetFirst, GetList, GetAll
		/// <summary>
		/// Vrátí první nalezený objekt typu ResourceItem dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null.
		/// </summary>
		public static ResourceItem GetFirst(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return ResourceItem.GetFirst(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí první nalezený objekt typu ResourceItem dle parametrů v queryParams.
		/// Pokud není žádný objekt nalezen, vrací null. Data jsou načítána v předané transakci.
		/// </summary>
		public static ResourceItem GetFirst(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			int? originalTopRecords = queryParams.TopRecords;
			queryParams.TopRecords = 1;
			ResourceItemCollection getListResult = ResourceItem.GetList(queryParams, transaction);
			queryParams.TopRecords = originalTopRecords;
			return (getListResult.Count == 0) ? null : getListResult[0];
		}
		
		/// <summary>
		/// Vrátí objekty typu ResourceItem dle parametrů v queryParams.
		/// </summary>
		public static ResourceItemCollection GetList(QueryParams queryParams)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			return ResourceItem.GetList(queryParams, null);
		}
		
		/// <summary>
		/// Vrátí objekty typu ResourceItem dle parametrů v queryParams. Data jsou načítána v předané transakci.
		/// </summary>
		public static ResourceItemCollection GetList(QueryParams queryParams, DbTransaction transaction)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(queryParams != null, "queryParams != null");
			
			DbCommand dbCommand = DbConnector.Default.ProviderFactory.CreateCommand();
			dbCommand.Transaction = transaction;
			
			queryParams.ObjectInfo = ResourceItem.ObjectInfo;
			if (queryParams.Properties.Count > 0)
			{
				queryParams.Properties.Add(ResourceItem.Properties.ID);
			}
			
			queryParams.PrepareCommand(dbCommand, SqlServerPlatform.SqlServer2008, CommandBuilderOptions.None);
			return ResourceItem.GetList(dbCommand, queryParams.GetDataLoadPower());
		}
		
		private static ResourceItemCollection GetList(DbCommand dbCommand, DataLoadPower dataLoadPower)
		{
			if (dbCommand == null)
			{
				throw new ArgumentNullException("dbCommand");
			}
			
			ResourceItemCollection result = new ResourceItemCollection();
			
			using (DbDataReader reader = DbConnector.Default.ExecuteReader(dbCommand))
			{
				while (reader.Read())
				{
					DataRecord dataRecord = new DataRecord(reader, dataLoadPower);
					ResourceItem resourceItem = ResourceItem.GetObject(dataRecord);
					if (dataLoadPower == DataLoadPower.FullLoad)
					{
						resourceItem.AddDataRecordToCache(dataRecord);
					}
					result.Add(resourceItem);
				}
			}
			
			return result;
		}
		
		private static object lockGetAllCacheAccess = new object();
		
		/// <summary>
		/// Vrátí všechny objekty typu ResourceItem.
		/// </summary>
		public static ResourceItemCollection GetAll()
		{
			ResourceItemLocalization.GetAll();
			
			ResourceItemCollection collection = null;
			int[] ids = null;
			string cacheKey = "Havit.BusinessLayerTest.Resources.ResourceItem.GetAll";
			
			ids = (int[])HttpRuntime.Cache.Get(cacheKey);
			if (ids == null)
			{
				lock (lockGetAllCacheAccess)
				{
					ids = (int[])HttpRuntime.Cache.Get(cacheKey);
					if (ids == null)
					{
						QueryParams queryParams = new QueryParams();
						collection = ResourceItem.GetList(queryParams);
						ids = collection.GetIDs();
						
						HttpRuntime.Cache.Insert(
							cacheKey,
							ids,
							new CacheDependency(null, new string[] { Havit.BusinessLayerTest.Resources.ResourceItem.GetAnySaveCacheDependencyKey() }), 
							Cache.NoAbsoluteExpiration,
							Cache.NoSlidingExpiration,
							CacheItemPriority.Default,
							null); // callback
					}
				}
			}
			if (collection == null)
			{
				collection = new ResourceItemCollection();
				collection.AddRange(ResourceItem.GetObjects(ids));
				collection.LoadAll();
			}
			
			return collection;
		}
		
		#endregion
		
		#region ObjectInfo
		/// <summary>
		/// Objektová reprezentace metadat typu ResourceItem.
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
		/// Objektová reprezentace metadat vlastností typu ResourceItem.
		/// </summary>
		public static ResourceItemProperties Properties
		{
			get
			{
				return properties;
			}
		}
		private static ResourceItemProperties properties;
		#endregion
		
	}
}
