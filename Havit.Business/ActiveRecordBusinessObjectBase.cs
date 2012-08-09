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
	/// Bázová třída pro všechny business-objekty, která definuje jejich základní chování (Layer Supertype),
	/// zejména ve vztahu k databázi jako Active Record [Fowler].
	/// </summary>
	/// <remarks>
	/// Třída je základem pro všechny business-objekty a implementuje základní pattern pro komunikaci s databází.
	/// Načítání z databáze je implementováno jako Lazy Load, kdy je objekt nejprve vytvořen prázdný jako Ghost se svým ID a teprve
	/// při první potřebě je iniciováno jeho úplné načtení z DB.<br/>
	/// Prostřednictvím constructoru BusinessObjectBase(DataRecord record) lze vytvořit i neúplně načtenou instanci objektu,
	/// samotná funkčnost však není řešena a každý si musí sám ohlídat, aby bylo načteno vše, co je potřeba.
	/// </remarks>
	public abstract class ActiveRecordBusinessObjectBase : BusinessObjectBase
	{
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
		/// <param name="id">primární klíč objektu</param>
		protected ActiveRecordBusinessObjectBase(int id)
			: base(id)
		{
			if (IdentityMapScope.Current != null)
			{
				IdentityMapScope.Current.Store(this);
			}
		}

		/// <summary>
		/// Konstruktor pro objekt s obrazen v databázi, kterým dojde rovnou k načtení dat z <see cref="Havit.Data.DataRecord"/>.
		/// Základní cesta vytvoření partially-loaded instance.
		/// Pokud se inicializuje Ghost nebo FullLoad objekt, je přidán do IdentityMapy, pokud existuje.
		/// </summary>
		/// <param name="id">ID načítaného objektu</param>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu načtenými z databáze</param>
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

			/* nahradil implementační constructor base(...)
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
		/// Pokud je objekt již načten, vyhodí výjimku.
		/// </summary>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu načtenými z databáze.</param>
		public void Load(DataRecord record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}

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
		/// Výkonná část nahrání objektu z perzistentního uložiště.
		/// </summary>
		/// <remarks>
		/// Načte objekt z databáze do <see cref="DataRecord"/> a parsuje získaný <see cref="DataRecord"/> do objektu.
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt načten; null, pokud bez transakce</param>
		protected override sealed bool TryLoad_Perform(DbTransaction transaction)
		{
			DataRecord record = Load_GetDataRecord(transaction);

			if (record == null)
			{
				return false;
			}

			Load_ParseDataRecord(record);
			return true;
		}
		/// <summary>
		/// Implementace metody načte DataRecord objektu z databáze.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt načten; null, pokud bez transakce</param>
		/// <returns><see cref="Havit.Data.DataRecord"/> s daty objektu načtenými z databáze; null, pokud nenalezeno</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract DataRecord Load_GetDataRecord(DbTransaction transaction);

		/// <summary>
		/// Implemetace metody naplní hodnoty objektu z DataRecordu.
		/// </summary>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu načtenými z databáze; null, pokud nenalezeno</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Load_ParseDataRecord(DataRecord record);
		#endregion

		#region Save logika
		private WeakReference lastSaveTransaction;

		/// <summary>
		/// Uloží objekt do databáze, s použitím transakce. Nový objekt je vložen INSERT, existující objekt je aktualizován UPDATE.
		/// </summary>
		/// <remarks>
		/// Metoda neprovede uložení objektu, pokud není nahrán (!IsLoaded), není totiž ani co ukládat,
		/// data nemohla být změněna, když nebyla ani jednou použita.<br/>
		/// Metoda také neprovede uložení, pokud objekt nebyl změněn a současně nejde o nový objekt (!IsDirty &amp;&amp; !IsNew)
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		public override sealed void Save(DbTransaction transaction)
		{
			// vynucení transakce nad celou Save() operací (BusinessObjectBase ji pouze očekává, ale nevynucuje).
			DbConnector.Default.ExecuteTransaction(delegate(DbTransaction myTransaction)
				{

					// nechceme dvojí Save v rámci jedné transakce, proto si transakci ukládáme jako scope sejvu a v rámci stejného scope vykopneme Save
					// ovšem pokud jsme dirty, tak budeme pokračovat ukládáním
					if (!IsDirty && (lastSaveTransaction != null) && (object.ReferenceEquals(lastSaveTransaction.Target, myTransaction)))
					{
						return;
					}
					lastSaveTransaction = new WeakReference(myTransaction);

					// Řeší cyklické ukládání, kdy mne ukládá existující objekt díky cyklu.
					// Přeze mne již Save jednou prošel (já jsem ho nejspíš inicioval), proto jsem IsSaving
					// Save se na mne dostal podruhé, tzn. někdo mě potřebuje.
					// a pokud jsem tedy nový, nutně je vyžadován MinimalInsert
					// Neřeší zřejmě situaci, že ten kdo mě ukládá, by mě nemusel potřebovat a mohl se uložit
					// sám dvoufázově.
					if (this.IsNew && this.IsSaving)
					{
						// jsem New, ukládám se a zase jsem cyklem došel sám na sebe
						// nezbývá, než se zkusit uložit.
						this.Save_MinimalInsert(myTransaction);
					}

					Save_BaseInTransaction(myTransaction); // base.Save(myTransaction) hlásí warning
				}, transaction);
		}

		/// <summary>
		/// Voláno z metody Save - řeší warning při kompilaci při volání base.Save(...) z anonymní metody.
		/// </summary>
		private void Save_BaseInTransaction(DbTransaction myTransaction)
		{
			base.Save(myTransaction);
		}

		/// <summary>
		/// Výkonná část uložení objektu do perzistentního uložiště.
		/// </summary>
		/// <remarks>
		/// Pokud je objekt nový, volá Save_Insert_SaveRequiredForFullInsert a Insert, jinak Update.
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected override sealed void Save_Perform(DbTransaction transaction)
		{
			// transakce je zajištěna v override Save(DbTransaction), zde není potřeba zakládat další
			
			if (IsNew)
			{
				Save_Insert_InsertRequiredForFullInsert(transaction);
			}

			// neslučovat do jedné podmínky, InsertRequiredForFullInsert může zavolat můj MinimalInsert a pak už nejsem New

			if (IsNew && IsDeleted)
			{
				throw new InvalidOperationException("Nový objekt nemůže být smazán.");
			}

			Save_SaveMembers(transaction);
			if (IsNew)
			{
				Save_FullInsert(transaction);
				Save_SaveCollections(transaction);
			}
			else
			{
				Save_SaveCollections(transaction);
				if (IsDirty)
				{
					if (IsDeleted)
					{
						Delete_Perform(transaction);
					}
					else
					{
						Save_Update(transaction);
					}
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
		/// Implementace metody vloží nový objekt do databáze a nastaví nově přidělené ID (primární klíč).
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Save_FullInsert(DbTransaction transaction);

		/// <summary>
		/// Implementace metody vloží jen not-null vlastnosti objektu do databáze a nastaví nově přidělené ID (primární klíč).
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		public virtual void Save_MinimalInsert(DbTransaction transaction)
		{
			CheckConstraints();
		}


		/// <summary>
		/// Implementace metody aktualizuje data objektu v databázi.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Save_Update(DbTransaction transaction);

		/// <summary>
		/// Ukládá hodnoty potřebné pro provedení plného insertu.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected virtual void Save_Insert_InsertRequiredForFullInsert(DbTransaction transaction)
		{
			Save_Insert_InsertRequiredForMinimalInsert(transaction);
			IsMinimalInserting = false;
		}

		/// <summary>
		/// Ukládá hodnoty potřebné pro provedení minimálního insertu. Volá Save_Insert_SaveRequiredForMinimalInsert.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected virtual void Save_Insert_InsertRequiredForMinimalInsert(DbTransaction transaction)
		{
			if (IsMinimalInserting)
			{
				throw new InvalidOperationException("Při ukládání objektů se vyskytla neřešitelná cyklická závislost stylu 'Co vzniklo první: zrno nebo klas?'");
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
