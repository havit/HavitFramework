using System;
using System.Collections.Generic;
using System.Text;
using Havit.Data;
using System.Data.Common;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Havit.Business
{
	/// <summary>
	/// Bázová tøída pro všechny business-objekty, která definuje jejich základní chování, zejména ve vztahu k databázi (Layer Supertype).
	/// </summary>
	/// <remarks>
	/// Tøída je základem pro všechny business-objekty a implementuje základní pattern pro komunikaci s perzistentními uložišti.
	/// Naèítání je implementováno jako lazy-load, kdy je objekt nejprve vytvoøen prázdný jako ghost se svým ID a teprve
	/// pøi první potøebì je iniciováno jeho úplné naètení.<br/>
	/// </remarks>
	[DebuggerDisplay("{GetType().FullName,nq} (ID={IsNew ? \"New\" : ID.ToString(),nq}, IsLoaded={IsLoaded,nq}, IsDirty={IsDirty,nq})")]
	public abstract class BusinessObjectBase
	{
		#region Consts
		/// <summary>
		/// Hodnota, kterou má ID objektu neuloženého v databázi (bez perzistence).
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
		public const int NoID = Int32.MinValue;
		#endregion

		#region Property - ID
		/// <summary>
		/// Primární klíè objektu.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
		public int ID
		{
			get { return _id; }
			protected set { _id = value; }
		}
		private int _id;
		#endregion

		#region Properties - Stav objektu (IsDirty, IsLoaded, IsNew, IsDeleted)
		/// <summary>
		/// Indikuje, zda-li byla data objektu zmìnìna oproti datùm v databázi.
		/// Pøi nastavení na false zruší pøíznak zmìn všem PropertyHolderùm.
		/// </summary>
		public bool IsDirty
		{
			get { return _isDirty; }
			protected internal set
			{
				_isDirty = value;
				if (!value)
					foreach (PropertyHolderBase propertyHolder in PropertyHolders)
						propertyHolder.IsDirty = false;
			}
		}
		private bool _isDirty;

		/// <summary>
		/// Indikuje, zda-li byla data objektu naètena z databáze,
		/// resp. zda-li je potøeba objekt nahrávat z databáze.
		/// </summary>
		public bool IsLoaded
		{
			get { return _isLoaded; }
			protected set { _isLoaded = value; }
		}
		private bool _isLoaded;

		/// <summary>
		/// Indikuje, zda-li jde o nový objekt bez perzistence, který nebyl dosud uložen do databáze.
		/// Èeká na INSERT.
		/// </summary>
		public bool IsNew
		{
			get { return _isNew; }
			protected set { _isNew = value; }
		}
		private bool _isNew;

		/// <summary>
		/// Indikuje, zda-li je objekt smazán z databáze, pøípadnì je v ní oznaèen jako smazaný.
		/// </summary>
		public bool IsDeleted
		{
			get { return _isDeleted; }
			protected set { _isDeleted = value; }
		}
		private bool _isDeleted;

		/// <summary>
		/// Indikuje, zda-li je objekt zrovna ukládán (hlídá cyklické reference pøi ukládání).
		/// </summary>
		protected internal bool IsSaving
		{
			get { return _isSaving; }
			set { _isSaving = value; }
		}
		private bool _isSaving = false;
		#endregion

		#region PropertyHolders
		/// <summary>
		/// Kolekce referencí na jednotlivé property-holder objekty.
		/// </summary>
		/// <remarks>
		/// Kolekce je urèena pro hromadné operace s property-holdery. Jednotlivé property si reference na své property-holdery udržují v private fieldu.
		/// </remarks>
		internal protected List<PropertyHolderBase> PropertyHolders
		{
			get
			{
				return _propertyHolders;
			}
		}
		private List<PropertyHolderBase> _propertyHolders = new List<PropertyHolderBase>(16);
		#endregion

		#region Constructors
		/// <summary>
		/// Implementaèní konstruktor.
		/// </summary>
		/// <param name="id">ID objektu (PK)</param>
		/// <param name="isNew">indikuje nový objekt</param>
		/// <param name="isDirty">indikuje objekt zmìnìný vùèi perzistentnímu uložišti</param>
		/// <param name="isLoaded">indikuje naètený objekt</param>
		protected internal BusinessObjectBase(int id, bool isNew, bool isDirty, bool isLoaded)
		{
			this._id = id;
			this._isNew = isNew;
			this._isDirty = isDirty;
			this._isLoaded = isLoaded;

			Init();
		}

		/// <summary>
		/// Konstruktor pro nový objekt (bez perzistence v databázi).
		/// </summary>
		protected BusinessObjectBase()
			: this(
			NoID,		// ID
			true,		// IsNew
			false,		// IsDirty
			true)		// IsLoaded

		{
			/*
			this._id = NoID;
			this._isNew = true;
			this._isDirty = false;
			this._isLoaded = true;

			Init();
			*/
		}

		/// <summary>
		/// Konstruktor pro objekt s obrazem v databázi (perzistentní).
		/// </summary>
		/// <param name="id">primární klíè objektu</param>
		protected BusinessObjectBase(int id)
			: this(
			id,		// ID
			false,	// IsNew
			false,	// IsDirty
			false)	// IsLoaded
		{
			if (id == NoID)
			{
				throw new InvalidOperationException("Nelze vytvoøit objekt, který by nebyl nový a mìl NoID.");
			}

			/*
			this._id = id;
			this._isLoaded = false;
			this._isDirty = false;

			Init();
			 */
		}
		#endregion

		#region Load logika
		// zámek pro naèítání objektù
		private object loadLock = new object();

		/// <summary>
		/// Nahraje objekt z perzistentního uložištì, bez transakce.
		/// </summary>
		/// <remarks>
		/// Pozor, pokud je již objekt naèten a není urèena transakce (null), znovu se nenahrává.
		/// Pokud je transakce urèena, naète se znovu.
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt naèten; null, pokud bez transakce</param>
		public virtual void Load(DbTransaction transaction)
		{
			if (this.IsLoaded && (transaction == null))
			{
				// pokud je již objekt naèten, nenaèítáme ho znovu
				return;
			}

			// naèítání se zamyká kvùli cachovaným readonly objektùm
			// tam je sdílena instance, která by mohla být naèítána najednou ze dvou threadù
			lock (loadLock)
			{
				if (!this.IsLoaded)
				{
					Load_Perform(transaction);
					this.IsLoaded = true;
					this.IsDirty = false; // naètený objekt není Dirty.			
				}
			}
		}

		/// <summary>
		/// Nahraje objekt z perzistentního uložištì, bez transakce.
		/// </summary>
		/// <remarks>
		/// Pozor, pokud je již objekt naèten, znovu se nenahrává.
		/// </remarks>
		public void Load()
		{
			Load(null);
		}

		/// <summary>
		/// Výkonná èást nahrání objektu z perzistentního uložištì.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt naèten; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Load_Perform(DbTransaction transaction);
		#endregion

		#region Save logika
		/// <summary>
		/// Uloží objekt do databáze, s pøípadným použitím VNÌJŠÍ transakce.
		/// </summary>
		/// <remarks>
		/// Metoda neprovede uložení objektu, pokud není nahrán (!IsLoaded), není totiž ani co ukládat,
		/// data nemohla být zmìnìna, když nebyla ani jednou použita.<br/>
		/// Metoda také neprovede uložení, pokud objekt nebyl zmìnìn a souèasnì nejde o nový objekt (!IsDirty &amp;&amp; !IsNew).<br/>
		/// Metoda nezakládá vlastní transakci, která by sdružovala uložení kolekcí, èlenských objektù a vlastních dat!!!
		/// Pøíslušná transakce musí být pøedána (explicitní transakci doplòuje až ActiveRecordBusinessObjectbase).<br/>
		/// Mazání objektù rovnìž ukládá pøes tuto metodu.
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		public virtual void Save(DbTransaction transaction)
		{			
			if (!IsLoaded || IsSaving)
			{
				return;
			}

			IsSaving = true; // øeší cyklické reference pøi ukládání objektových struktur

			bool wasNew = IsNew;
			bool callBeforeAfterSave = IsDirty; // pokrývá i situaci, kdy je objekt nový
			if (callBeforeAfterSave) // zavoláno pro zmìnìné (a tedy i nové) objekty
			{
				OnBeforeSave(new BeforeSaveEventArgs(transaction));
			}

			if (IsDirty && !IsDeleted)
			{
				CheckConstraints();
			}

			Save_Perform(transaction);
			// Uložený objekt není už nový, dostal i pøidìlené ID.
			// Pro generovaný kód BL je zbyteèné, ten IsNew nastavuje i ve vygenerovaných
			// metodách pro MinimalInsert a FullInsert.
			IsNew = false; 
			IsDirty = false; // uložený objekt je aktuální

			if (callBeforeAfterSave)
			{
				OnAfterSave(new AfterSaveEventArgs(transaction, wasNew));
			}
			IsSaving = false;
		}

		/// <summary>
		/// Uloží objekt do databáze, bez transakce. Nový objekt je vložen INSERT, existující objekt je aktualizován UPDATE.
		/// </summary>
		/// <remarks>
		/// Metoda neprovede uložení objektu, pokud není nahrán (!IsLoaded), není totiž ani co ukládat,
		/// data nemohla být zmìnìna, když nebyla ani jednou použita.<br/>
		/// Metoda také neprovede uložení, pokud objekt nebyl zmìnìn a souèasnì nejde o nový objekt (!IsDirty &amp;&amp; !IsNew)
		/// </remarks>
		public void Save()
		{
			Save(null);
		}

		/// <summary>
		/// Výkonná èást uložení objektu do perzistentního uložištì.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které má být objekt uložen; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Save_Perform(DbTransaction transaction);

		/// <summary>
		/// Volá se pøed jakýmkoliv uložením objektu, tj. i pøed smazáním.
		/// V každém spuštìní uložení grafu objektù se metoda volá právì jednou, na rozdíl od Save, který mùže být (a je) spouštìn opakovanì v pøípadì ukládání stromù objektù.
		/// Metoda se volá pøed zavoláním validaèní metody CheckConstrains.
		/// </summary>
		protected virtual void OnBeforeSave(BeforeSaveEventArgs transactionEventArgs)
		{
			// metoda vznikla jako reseni problemu opakovaneho volani Save a logiky, kterou jsme do Save vsude psali
			// az budeme potrebovat, implementujeme udalost oznamujici okamzik pred ulozeni objektu (a pred jeho validaci).
		}

		/// <summary>
		/// Volá se po zakémkoliv uložení objektu, tj. i po smazání objektu.
		/// V každém spuštìní uložení grafu objektù se metoda volá právì jednou, na rozdíl od Save, který mùže být (a je) spouštìn opakovanì v pøípadì ukládání stromù objektù.
		/// Metoda se volá po nastavení pøíznakù IsDirty, IsNew, apod.
		/// </summary>        
		protected virtual void OnAfterSave(AfterSaveEventArgs transactionEventArgs)
		{
			// metoda vznikla jako reseni problemu opakovaneho volani Save a logiky, kterou jsme do Save vsude psali
			// az budeme potrebovat, implementujeme udalost oznamujici okamzik pred ulozeni objektu (a pred jeho validaci).
		}

		#endregion

		#region Delete logika
		/// <summary>
		/// Smaže objekt, nebo ho oznaèí jako smazaný, podle zvolené logiky. Zmìnu uloží do databáze, v transakci.
		/// </summary>
		/// <remarks>
		/// Neprovede se, pokud je již objekt smazán.
		/// </remarks>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které se smazání provede; null, pokud bez transakce</param>
		public virtual void Delete(DbTransaction transaction)
		{
			if (IsNew)
			{
				throw new InvalidOperationException("Nový objekt nelze smazat.");
			}
			
			EnsureLoaded();

			if (IsDeleted)
			{
				return;
			}

			IsDirty = true;
			IsDeleted = true;
			Save(transaction);
		}

		/// <summary>
		/// Smaže objekt, nebo ho oznaèí jako smazaný, podle zvolené logiky. Zmìnu uloží do databáze, bez transakce.
		/// </summary>
		/// <remarks>
		/// Neprovede se, pokud je již objekt smazán.
		/// </remarks>
		public void Delete()
		{
			Delete(null);
		}

		/// <summary>
		/// Implementace metody vymaže objekt z perzistentního uložištì nebo ho oznaèí jako smazaný.
		/// </summary>
		/// <param name="transaction">transakce <see cref="DbTransaction"/>, v rámci které se smazání provede; null, pokud bez transakce</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
		protected abstract void Delete_Perform(DbTransaction transaction);
		#endregion

		#region Implementaèní metody - EnsureLoaded, CheckChange
		/// <summary>
		/// Ovìøí, jestli jsou data objektu naètena z databáze (IsLoaded). Pokud nejsou, provede jejich naètení.
		/// </summary>
		/// <remarks>
		/// Metoda EnsureLoaded se volá pøed každou operací, která potøebuje data objektu. Zajištuje lazy-load.
		/// </remarks>
		protected void EnsureLoaded()
		{
			if (IsLoaded || IsNew)
			{
				return;
			}

			Load();
		}

		/// <summary>
		/// Metoda zkontroluje rovnost dvou objektù - jestliže nejsou stejné, je objekt oznaèen jako zmìnìný (IsDirty = true).
		/// </summary>
		/// <remarks>
		/// Metoda se používá zejména v set-accesorech properties, kde hlídá, jestli dochází ke zmìnì,
		/// kterou bude potøeba uložit.
		/// </remarks>
		/// <param name="currentValue">dosavadní hodnota</param>
		/// <param name="newValue">nová hodnota</param>
		/// <returns>false, pokud jsou hodnoty stejné; true, pokud dochází ke zmìnì</returns>
		protected bool CheckChange(object currentValue, object newValue)
		{
			if (!Object.Equals(currentValue, newValue))
			{
				IsDirty = true;
				return true;
			}
			return false;
		}
		#endregion

		#region Equals, GetHashCode, operátory == a != (override)
		/// <summary>
		/// Zjistí rovnost druhého objektu s instancí. Základní implementace porovná jejich ID.
		/// Nové objekty jsou si rovny v pøípadì identity (stejná reference).
		/// </summary>
		/// <param name="obj">objekt k porovnání</param>
		/// <returns>true, pokud jsou si rovny; jinak false</returns>
		public virtual bool Equals(BusinessObjectBase obj)
		{
			if ((obj == null) || (this.GetType() != obj.GetType()))
			{
				return false;
			}
			
			// nové objekty jsou si rovny pouze v pøípadì identity (stejná reference)
			if (this.IsNew || obj.IsNew)
			{
				return Object.ReferenceEquals(this, obj);
			}
			
			// bìžné objekty jsou si rovny, pokud mají stejné ID
			if (!Object.Equals(this.ID, obj.ID))
			{
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Zjistí rovnost druhého objektu s instancí. Základní implementace porovná jejich ID.
		/// </summary>
		/// <param name="obj">objekt k porovnání</param>
		/// <returns>true, pokud jsou si rovny; jinak false</returns>
		public override bool Equals(object obj)
		{
			BusinessObjectBase bob = obj as BusinessObjectBase;
			if (bob != null)
			{
				return this.Equals(bob);
			}
			return false;
		}

		/// <summary>
		/// Operátor ==, ovìøuje rovnost ID.
		/// </summary>
		/// <param name="objA">první objekt</param>
		/// <param name="objB">druhý objekt</param>
		/// <returns>true, pokud mají objekty stejné ID; jinak false</returns>
		public static bool operator ==(BusinessObjectBase objA, BusinessObjectBase objB)
		{
			return Object.Equals(objA, objB);
		}

		/// <summary>
		/// Operátor !=, ovìøuje rovnost ID.
		/// </summary>
		/// <param name="objA">první objekt</param>
		/// <param name="objB">druhý objekt</param>
		/// <returns>false, pokud mají objekty stejné ID; jinak true</returns>
		public static bool operator !=(BusinessObjectBase objA, BusinessObjectBase objB)
		{
			return !Object.Equals(objA, objB);
		}

		/// <summary>
		/// Vrací ID jako HashCode.
		/// </summary>
		public override int GetHashCode()
		{
			return this.ID;
		}
		#endregion

		#region CheckConstraints
		/// <summary>
		/// Kontroluje konzistenci objektu jako celku.
		/// </summary>
		/// <remarks>
		/// Automaticky je voláno pøed ukládáním objektu Save(), pokud je objekt opravdu ukládán.
		/// </remarks>
		protected virtual void CheckConstraints()
		{
		}
		#endregion

		#region Init
		/// <summary>
		/// Inicializaèní metoda, která je volána pøi vytvoøení objektu (pøímo z konstruktorù).
		/// Pøipravena pro override potomky.
		/// </summary>
		/// <remarks>
		/// Metoda Init() je zamýšlena mj. pro incializaci PropertyHolderù (vytvoøení instance) a kolekcí (vytvoøení instance, navázání událostí).
		/// </remarks>
		protected virtual void Init()
		{
			// NOOP
		}
		#endregion

		#region RegisterPropertyHolder (internal)
		/// <summary>
		/// Zaregistruje PropertyHolder do kolekce PropertyHolders.
		/// </summary>
		/// <remarks>
		/// Touto metodou se k objektu registrují sami PropertyHoldery ve svých constructorech.
		/// </remarks>
		/// <param name="propertyHolder">PropertyHolder k zaregistrování</param>
		internal void RegisterPropertyHolder(PropertyHolderBase propertyHolder)
		{
			_propertyHolders.Add(propertyHolder);
		}
		#endregion

		/**********************************************************************************/

		#region GetNullableID (static)
		/// <summary>
		/// Vrátí ID objektu, nebo null, pokud je vstupní objekt null.
		/// Urèeno pro pøehledné získávání ID, obvykle pøi pøedávání do DB.
		/// </summary>
		/// <param name="businessObject">objekt, jehož ID chceme</param>
		/// <returns>ID objektu, nebo null, pokud je vstupní objekt null</returns>
		public static int? GetNullableID(BusinessObjectBase businessObject)
		{
			if (businessObject == null)
			{
				return null;
			}
			return businessObject.ID;
		}
		#endregion
	}
}
