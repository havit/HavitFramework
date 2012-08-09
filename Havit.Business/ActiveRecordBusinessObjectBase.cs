using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Havit.Data;
using Havit.Data.SqlClient;
using System.Data.SqlClient;

namespace Havit.Business
{
	/// <summary>
	/// Bázová tøída pro všechny business-objekty, která definuje jejich základní chování (Layer Supertype),
	/// zejména ve vztahu k databázi jako Active Record [Fowler].
	/// </summary>
	/// <remarks>
	/// Tøída je základem pro všechny business-objekty a implementuje základní pattern pro komunikaci s databází.
	/// Naèítání z databáze je implementováno jako Lazy Load, kdy je objekt nejprve vytvoøen prázdný jako Ghost se svým ID a teprve
	/// pøi první potøebì je iniciováno jeho úplné naètení z DB.<br/>
	/// Prostøednictvím constructoru BusinessObjectBase(DataRecord record) lze vytvoøit i neúplnì naètenou instanci objektu,
	/// samotná funkènost však není øešena a každý si musí sám ohlídat, aby bylo naèteno vše, co je potøeba.
	/// </remarks>
	[Serializable]
	public abstract class ActiveRecordBusinessObjectBase : BusinessObjectBase
	{
		/*
		#region Properties - Stav objektu		
		/// <summary>
		/// Indikuje, zda-li byla data objektu naètena z databáze èásteènì, tedy zda-li se jednalo o partial-load.
		/// </summary>
		public bool IsLoadedPartially
		{
			get { return _isLoadedPartially; }
			set { _isLoadedPartially = value; }
		}
		private bool _isLoadedPartially;
		#endregion
		*/
		#region Constructors
		/// <summary>
		/// Konstruktor pro nový objekt (bez perzistence v databázi).
		/// </summary>
		protected ActiveRecordBusinessObjectBase()
			: base()
		{
		}

		/// <summary>
		/// Konstruktor pro objekt s obrazem v databázi (perzistentní).
		/// </summary>
		/// <param name="id">primární klíè objektu</param>
		protected ActiveRecordBusinessObjectBase(int id)
			: base(id)
		{
			if (IdentityMapScope.Current != null)
			{
				IdentityMapScope.Current.Store(this);
			}
		}

		/// <summary>
		/// Konstruktor pro objekt s obrazen v databázi, kterým dojde rovnou k naètení dat z <see cref="Havit.Data.DataRecord"/>.
		/// Základní cesta vytvoøení partially-loaded instance.
		/// Pokud se inicializuje Ghost nebo FullLoad objekt, je pøidán do IdentityMapy, pokud existuje.
		/// </summary>
		/// <param name="id">ID naèítaného objektu</param>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu naètenými z databáze</param>
		protected ActiveRecordBusinessObjectBase(int id, DataRecord record)
			: base(
			id,	// ID
			false,	// IsNew
			false,	// IsDirty
			false)	// IsLoaded

		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}

/* nahradil implementaèní constructor base(...)
			this.IsNew = false;
			this.IsLoaded = false;
*/
			if ((IdentityMapScope.Current != null)
				&& ((record.DataLoadPower == DataLoadPower.Ghost) || (record.DataLoadPower == DataLoadPower.FullLoad)))
			{
				IdentityMapScope.Current.Store(this);
			}

			Load(record);

			//this.Load_ParseDataRecord(record);

//			this._isLoadedPartially = !record.FullLoad;
			//this.IsLoaded = true;
			//this.IsDirty = false;
		}
		#endregion

		#region Load logika

		/// <summary>
		/// Nastaví objektu hodnoty z DataRecordu.
		/// Pokud je objekt již naèten, vyhodí výjimku.
		/// </summary>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu naètenými z databáze.</param>
		public void Load(DataRecord record)
		{
			if (this.IsLoaded)
			{
				throw new InvalidOperationException("Nelze nastavit objektu hodnoty z DataRecordu, pokud objekt není ghostem.");
			}
			Load_ParseDataRecord(record);

			if (record.DataLoadPower != DataLoadPower.Ghost)
			{
				this.IsLoaded = true;
			}
			this.IsDirty = false;
		}

		/// <summary>
		/// Výkonná èást nahrání objektu z perzistentního uložištì.
		/// </summary>
		/// <remarks>
		/// Naète objekt z databáze do <see cref="DataRecord"/> a parsuje získaný <see cref="DataRecord"/> do objektu.
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt naèten; null, pokud bez transakce</param>
		protected override void Load_Perform(DbTransaction transaction)
		{
			DataRecord record = Load_GetDataRecord(transaction);
			Load_ParseDataRecord(record);
		}
		/// <summary>
		/// Implementace metody naète DataRecord objektu z databáze.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt naèten; null, pokud bez transakce</param>
		/// <returns><see cref="Havit.Data.DataRecord"/> s daty objektu naètenými z databáze; null, pokud nenalezeno</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract DataRecord Load_GetDataRecord(DbTransaction transaction);

		/// <summary>
		/// Implemetace metody naplní hodnoty objektu z DataRecordu.
		/// </summary>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu naètenými z databáze; null, pokud nenalezeno</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Load_ParseDataRecord(DataRecord record);
		#endregion

		#region Save logika
		/// <summary>
		/// Uloží objekt do databáze, s použitím transakce. Nový objekt je vložen INSERT, existující objekt je aktualizován UPDATE.
		/// </summary>
		/// <remarks>
		/// Metoda neprovede uložení objektu, pokud není nahrán (!IsLoaded), není totiž ani co ukládat,
		/// data nemohla být zmìnìna, když nebyla ani jednou použita.<br/>
		/// Metoda také neprovede uložení, pokud objekt nebyl zmìnìn a souèasnì nejde o nový objekt (!IsDirty &amp;&amp; !IsNew)
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		public override void Save(DbTransaction transaction)
		{
			//if (IsLoadedPartially)
			//{
			//    throw new ApplicationException("Partially-loaded object cannot be saved.");
			//}

			// vynucení transakce nad celou Save() operací (BusinessObjectBase ji pouze oèekává, ale nevynucuje).
			SqlDataAccess.ExecuteTransaction(delegate(SqlTransaction myTransaction)
				{
					Save_BaseInTransaction(myTransaction); // base.Save(myTransaction) hlásí warning
				}, (SqlTransaction)transaction);
		}

		/// <summary>
		/// Voláno z metody Save - øeší warning pøi kompilaci pøi volání base.Save(...) z anonymní metody.
		/// </summary>
		private void Save_BaseInTransaction(SqlTransaction myTransaction)
		{
			base.Save(myTransaction);
		}

		/// <summary>
		/// Výkonná èást uložení objektu do perzistentního uložištì.
		/// </summary>
		/// <remarks>
		/// Pokud je objekt nový, volá Save_Insert_SaveRequiredForFullInsert a Insert, jinak Update.
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected override void Save_Perform(DbTransaction transaction)
		{
			if (IsNew)
			{
				Save_Insert_InsertRequiredForFullInsert(transaction);
			}

			if (IsNew)
			{
				Save_SaveMembers(transaction);
				Save_FullInsert(transaction);
				Save_SaveCollections(transaction);
			}
			else
			{
				Save_SaveMembers(transaction);
				Save_SaveCollections(transaction);
				if (IsDirty)
				{
					Save_Update(transaction);
				}
			}
		}

		/// <summary>
		/// Ukládá member-objekty.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které mají být member-objekty uloženy; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected virtual void Save_SaveMembers(DbTransaction transaction)
		{
			// NOOP
		}

		/// <summary>
		/// Ukládá member-kolekce objektu.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které mají být member-kolekce uloženy; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected virtual void Save_SaveCollections(DbTransaction transaction)
		{
			// NOOP
		}

		/// <summary>
		/// Implementace metody vloží nový objekt do databáze a nastaví novì pøidìlené ID (primární klíè).
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Save_FullInsert(DbTransaction transaction);

		/// <summary>
		/// Implementace metody vloží jen not-null vlastnosti objektu do databáze a nastaví novì pøidìlené ID (primární klíè).
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		public abstract void Save_MinimalInsert(DbTransaction transaction);

		/// <summary>
		/// Implementace metody aktualizuje data objektu v databázi.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Save_Update(DbTransaction transaction);

		/// <summary>
		/// Ukládá hodnoty potøebné pro provedení plného insertu.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected virtual void Save_Insert_InsertRequiredForFullInsert(DbTransaction transaction)
		{
			Save_Insert_InsertRequiredForMinimalInsert(transaction);
			IsMinimalInserting = false;
		}
		
		/// <summary>
		/// Ukládá hodnoty potøebné pro provedení minimálního insertu. Volá Save_Insert_SaveRequiredForMinimalInsert.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected virtual void Save_Insert_InsertRequiredForMinimalInsert(DbTransaction transaction)
		{
			if (IsMinimalInserting)
			{
				throw new InvalidOperationException("Pøi ukládání objektù se vyskytla neøešitelná cyklická závislost stylu 'Co vzniklo první: zrno nebo klas?'");
			}

			IsMinimalInserting = true;
		}

		/// <summary>
		/// Identifikuje, zda probíhá Save_Insert_InsertRequiredForMinimalInsert (nesmí se zacyklit).
		/// </summary>
		protected bool IsMinimalInserting
		{
			get { return isMinimalInserting; }
			set { isMinimalInserting = value; }
		}
		private bool isMinimalInserting = false;

		#endregion
	}
}
