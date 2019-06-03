using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Třída pro objekt, který nese kolekci property BusinessObjectu.
	/// </summary>
	/// <typeparam name="CollectionType">typ kolekce, jíž je CollectionPropertyHolder nosičem</typeparam>
	/// <typeparam name="BusinessObjectType">typ prvku kolekce</typeparam>
	public class CollectionPropertyHolder<CollectionType, BusinessObjectType> : PropertyHolderBase
		where BusinessObjectType : BusinessObjectBase
		where CollectionType : BusinessObjectCollection<BusinessObjectType, CollectionType>, new()
	{
		private readonly Func<int, BusinessObjectType> getObjectFunc;

		private CollectionType _value;
		private BusinessObjectType[] _loadedValue;
		private string itemIDsWithDelemiter;

		/// <summary>
		/// Založí instanci CollectionPropertyHolderu.
		/// </summary>
		/// <param name="owner">Objekt, kterému CollectionPropertyHolder patří.</param>
		/// <param name="getObjectFunc">Delegát na funkci GetObject příslušného business objektu.</param>
		public CollectionPropertyHolder(BusinessObjectBase owner, Func<int, BusinessObjectType> getObjectFunc)
			: base(owner)
		{
			this.getObjectFunc = getObjectFunc;
		}

		/// <summary>
		/// Hodnota, kterou CollectionPropertyHolder nese.
		/// </summary>
		public CollectionType Value
		{
			get
			{
				// Zde jen zkontrolujeme, zda došlo k požadavku inicializaci.
				// Díky odloženému provedení inicializace ve skutečnosti nemusí být hodnoty kolekce ještě nastaveny.
				// Díky konvenci pojmenování v předkovi zde může dojík ke zmatení: initialization vs. value initialization
				// CheckInitialization - zkontroluje, zda byla hodnota nastavena zvenčí, nemusí být nutně ve value, to zajistí následující
				// EnsureLazyValueInitialization - zajistí, aby se hodnota nastavená z venčí dostala do _value (a _loadedValues)
				
				CheckInitialization();

				EnsureLazyValueInitialization();

				return _value;
			}
		}

		/// <summary>
		/// Originální hodnoty (prvky kolekce) načtené z databáze.
		/// </summary>
		public BusinessObjectType[] LoadedValue
		{
			get
			{
				EnsureLazyValueInitialization();
				return _loadedValue;
			}
		}

		/// <summary>
		/// Inicilizuje hodnotu property holderu.
		/// Určeno pro instance nových objektů (objektů k uložení).
		/// </summary>
		public void Initialize()
		{
			if (_value == null)
			{
				_value = new CollectionType();
				_value.DisallowDuplicatesWithoutCheckDuplicates();
				_value.CollectionChanged += delegate (object sender, EventArgs e)
				{
					IsDirty = true;
				};
			}
			else
			{
				_value.Clear();
			}
			_loadedValue = new BusinessObjectType[0];
			IsInitialized = true;
		}

		/// <summary>
		/// Inicilizuje hodnotu property holderu.
		/// Určeno pro instance objektů načtených z databáze.
		/// </summary>
		/// <param name="itemIDsWithDelemiter">Identifikátory objektů v kolekci serializované pomocí FOR XML (např. "1|2|3|4|" - obsahuje na separátor i na konci).</param>
		public void Initialize(string itemIDsWithDelemiter) // může být null
		{
			if (itemIDsWithDelemiter == null)
			{
				// i když nemáme v databázi žádnou hodnotu, musíme buď
				// zajistit lazy initialization kolekce
				// nebo kolekci rovnou inicializovat (zvoleno toto řešení).
				Initialize();
			}
			else
			{
				// řekneme, že property holder byl inicializován
				IsInitialized = true;

				// Avšak nestaráme se o kolekci - použijeme strategii odložení inicializace do okamžiku prvníhop použití.
				// Řekneme jen, jaké hodnoty mají být nastaveny, až (a pokud) budou potřeba.
				this.itemIDsWithDelemiter = itemIDsWithDelemiter;
			}
		}

		/// <summary>
		/// Zajistí nastavení hodnoty do _value a _lazyValue.
		/// </summary>
		private void EnsureLazyValueInitialization()
		{
			if (itemIDsWithDelemiter != null)
			{
				_value = new CollectionType();

				if (itemIDsWithDelemiter.Length > 25)
				{
					Span<byte> itemIDsSpan = Encoding.UTF8.GetBytes(itemIDsWithDelemiter);
					while (itemIDsSpan.Length > 0)
					{
						System.Buffers.Text.Utf8Parser.TryParse(itemIDsSpan, out int id, out int bytesConsumed);
						_value.Add(getObjectFunc(id));

						itemIDsSpan = itemIDsSpan.Slice(bytesConsumed + 1); // za každou (i za poslední) položkou je oddělovač
					}
				}
				else
				{
					string[] itemIDs = itemIDsWithDelemiter.Split('|');
					int itemIDsLength = itemIDs.Length - 1; // za každou (i za poslední) položkou je oddělovač
					for (int i = 0; i < itemIDsLength; i++)
					{
						_value.Add(getObjectFunc(BusinessObjectBase.FastIntParse(itemIDs[i])));
					}
				}

				_value.DisallowDuplicatesWithoutCheckDuplicates();
				_value.CollectionChanged += delegate (object sender, EventArgs e)
				{
					IsDirty = true;
				};

				itemIDsWithDelemiter = null;

				UpdateLoadedValue(); // musí být až na konci - implementace volá Value a pokud by nebylo až za předchozím řádkem, provedli bychom StackOverflowException				}
			}
		}

		/// <summary>
		/// Nastaví hodnotu LoadedValue dle Value.
		/// </summary>
		public void UpdateLoadedValue()
		{
			_loadedValue = Value.ToArray();
		}

	}
}