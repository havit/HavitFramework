using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using Havit.Data;

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
	public abstract class ActiveRecordBusinessObjectBase : BusinessObjectBase
	{
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
		}

		/// <summary>
		/// Konstruktor pro objekt s obrazen v databázi, kterým dojde rovnou k naètení dat z <see cref="Havit.Data.DataRecord"/>.
		/// Základní cesta vytvoøení partially-loaded instance.
		/// </summary>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu naètenými z databáze</param>
		public ActiveRecordBusinessObjectBase(DataRecord record)
			: base()
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}

			this.IsNew = false;
			this.IsDirty = false;
			this.IsLoaded = false;
		
			this.Load_ParseDataRecord(record);

			this._isLoadedPartially = !record.FullLoad;
			this.IsLoaded = true;
		}
		#endregion

		#region Load logika
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
		protected abstract DataRecord Load_GetDataRecord(DbTransaction transaction);

		/// <summary>
		/// Implemetace metody naplní hodnoty objektu z DataRecordu.
		/// </summary>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu naètenými z databáze; null, pokud nenalezeno</param>
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
			if (IsLoadedPartially)
			{
				throw new ApplicationException("Partially-loaded object cannot be saved.");
			}

			base.Save(transaction);
		}

		/// <summary>
		/// Výkonná èást uložení objektu do perzistentního uložištì.
		/// </summary>
		/// <remarks>
		/// Pokud je objekt nový, volá Insert, jinak Update.
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected override void Save_Perform(DbTransaction transaction)
		{
			if (this.IsNew)
			{
				Save_Insert(transaction);
			}
			else
			{
				Save_Update(transaction);
			}
		}

		/// <summary>
		/// Implementace metody vloží nový objekt do databáze a nastaví novì pøidìlené ID (primární klíè).
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected abstract void Save_Insert(DbTransaction transaction);

		/// <summary>
		/// Implementace metody aktualizuje data objektu v databázi.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		protected abstract void Save_Update(DbTransaction transaction);
		#endregion
	}
}
