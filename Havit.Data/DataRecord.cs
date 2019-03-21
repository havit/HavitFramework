using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Havit.Data
{
	/// <summary>
	/// DataRecord usnadňuje načítání dat objektu z databáze.
	/// Zejména vhodný je pro situace, kdy je možné i částečné načítání.
	/// </summary>
	/// <remarks>
	/// DataRecord pracuje tak, že v constructoru zkopíruje celý datový záznam do slovníku Dictionary&lt;field, value&gt;.
	/// V jednotlivých Loadech pak už jenom načítá data ze slovníku.<br/>
	/// Datový zdroj je tedy potřeba pouze v okamžiku volání constructoru a následně ho můžeme zlikvidovat.<br/>
	/// Stejnětak je vhodné použít na všechny loady jeden DataRecord a předávat si ho mezi objekty.
	/// </remarks>
	[Serializable]
	public class DataRecord
	{
		/// <summary>
		/// Indikuje, zdali je požadována 100% úspěšnost pro načítání položek (true), nebo zdali se mají neúspěchy ignorovat.
		/// </summary>
		public bool FullLoad
		{
			get { return DataLoadPower == DataLoadPower.FullLoad; }
		}

		/// <summary>
		/// Indikuje množství dat, které jsou uloženy v DataRecordu vůči všem možným sloupcům řádků.
		/// </summary>
		public DataLoadPower DataLoadPower
		{
			get { return dataLoadPower; }
			set { dataLoadPower = value; }
		}
		private DataLoadPower dataLoadPower;

		/// <summary>
		/// Data z databáze.
		/// </summary>
		private readonly Dictionary<string, object> dataDictionary;

		/// <summary>
		/// Vytvoří instanci DataRecordu a načte do ní data z <see cref="System.Data.DataRow"/>.
		/// </summary>
		/// <param name="row">datový zdroj typu DataRow</param>
		/// <param name="dataLoadPower">Rozsah dat v datovém zdroji.</param>
		public DataRecord(DataRow row, DataLoadPower dataLoadPower)
		{
			this.dataLoadPower = dataLoadPower;

			// zkopíruje data do dataDictionary
			this.dataDictionary = new Dictionary<string, object>(row.Table.Columns.Count);
			for (int i = 0; i < row.Table.Columns.Count; i++)
			{
				this.dataDictionary[row.Table.Columns[i].ColumnName] = row[i];
			}
		}

		/// <summary>
		/// Vytvoří instanci DataRecordu a načte do ní data z <see cref="System.Data.DataRow"/>.
		/// </summary>
		/// <param name="row">datový zdroj typu DataRow</param>
		/// <param name="fullLoad">true, má-li být při nenalezení parametru vyvolána výjimka</param>
		[Obsolete]
		public DataRecord(DataRow row, bool fullLoad) : this(row, fullLoad ? DataLoadPower.FullLoad : DataLoadPower.PartialLoad)
		{
		}

		/// <summary>
		/// Vytvoří instanci DataRecordu a načte do ní data z <see cref="System.Data.DataRow"/>.
		/// </summary>
		/// <param name="row">datový zdroj typu <see cref="System.Data.DataRow"/></param>
		[Obsolete]
		public DataRecord(DataRow row) : this(row, true)
		{
		}

		/// <summary>
		/// Vytvoří instanci DataRecordu a načte do ní data z <see cref="System.Data.IDataRecord"/>
		/// (např. <see cref="System.Data.SqlClient.SqlDataReader"/>).
		/// </summary>
		/// <param name="record">datový zdroj <see cref="System.Data.IDataRecord"/> (např. <see cref="System.Data.SqlClient.SqlDataReader"/>)</param>
		/// <param name="dataLoadPower">Rozsah dat v datovém zdroji.</param>
		public DataRecord(IDataRecord record, DataLoadPower dataLoadPower)
		{
			this.dataLoadPower = dataLoadPower;

			// zkopíruje data do dataDictionary
			this.dataDictionary = new Dictionary<string, object>(record.FieldCount);
			for (int i = 0; i < record.FieldCount; i++)
			{
				this.dataDictionary[record.GetName(i)] = record[i];
			}
		}

		/// <summary>
		/// Vytvoří instanci DataRecordu a načte do ní data z <see cref="System.Data.IDataRecord"/>
		/// (např. <see cref="System.Data.SqlClient.SqlDataReader"/>).
		/// </summary>
		/// <param name="record">datový zdroj <see cref="System.Data.IDataRecord"/> (např. <see cref="System.Data.SqlClient.SqlDataReader"/>)</param>
		/// <param name="fullLoad">true, má-li být při nenalezení parametru vyvolána výjimka</param>
		[Obsolete]
		public DataRecord(IDataRecord record, bool fullLoad) : this(record, fullLoad ? DataLoadPower.FullLoad : DataLoadPower.PartialLoad)
		{
		}

		/// <summary>
		/// Vytvoří instanci DataRecordu a načte do ní data z <see cref="System.Data.IDataRecord"/>
		/// (např. <see cref="System.Data.SqlClient.SqlDataReader"/>).
		/// </summary>
		/// <param name="record">datový zdroj <see cref="System.Data.IDataRecord"/> (např. <see cref="System.Data.SqlClient.SqlDataReader"/>)</param>
		[Obsolete]
		public DataRecord(IDataRecord record) : this(record, true)
		{			
		}

		/// <summary>
		/// Indexer pro získání k prvku pomocí názvu pole.
		/// </summary>
		/// <param name="field">pole, sloupec</param>
		/// <returns>hodnota</returns>
		public object this[string field]
		{
			get
			{
				return this.dataDictionary[field];
			}
		}

		/// <summary>
		/// Načte parametr zadaného generického typu T.
		/// </summary>
		/// <typeparam name="T">Typ hodnoty.</typeparam>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>
		/// <c>true</c>, pokud byla načtena hodnota;<br/>
		/// <c>false</c>, pokud field v data recordu není a vlastnost <see cref="FullLoad"/> je <c>false</c> (target je pak nastaven na <c>default(T)</c>);<br/>
		/// </returns>
		/// <exception cref="ArgumentException">pokud field v data recordu není a vlastnost <see cref="FullLoad"/> je <c>true</c></exception>
		/// <exception cref="InvalidCastException">pokud nelze převést field na výstupní typ, nebo pokud je field <see cref="DBNull"/> a výstupní typ nemá <c>null</c></exception>
		[SuppressMessage("SonarLint", "S2955", Justification = "Zde chceme podporovat obecné typy, včetně struktur a včetně Nullable, což je struktura \"umožňující\" hodnotu null (Use a comparison to \"default(T)\" instead or add a constraint to \"T\" so that it can't be a value type.)")]
		public bool TryGet<T>(string fieldName, out T target)
		{
			target = default(T);
			object value;
			if (dataDictionary.TryGetValue(fieldName, out value))
			{
				if (value == DBNull.Value)
				{
					if (default(T) != null)
					{
						throw new InvalidCastException("Hodnota NULL nelze převést na ValueType.");
					}
					target = default(T); // null
				}
				else
				{
					if (value is T)
					{
						target = (T)value;
					}
					else
					{
						if (value is IConvertible)
						{
							try
							{
								target = (T)Convert.ChangeType(value, typeof(T));	 // poslední pokus např. pro konverzi decimal -> double
							}
							catch (InvalidCastException e)
							{
								throw new InvalidCastException("Specified cast is not valid, field '" + fieldName + "', from " + value.GetType().FullName, e);
							}
						}
					}
				}

				return true;
			}
			else if (dataLoadPower == DataLoadPower.FullLoad)
			{
				throw new ArgumentException("Parametr požadovaného jména nebyl v DataRecordu nalezen.", fieldName);
			}
			else
			{
				target = default(T);
				return false;
			}
		}

		/// <summary>
		/// Vrátí parametr zadaného generického typu.
		/// </summary>
		/// <typeparam name="T">Typ hodnoty.</typeparam>
		/// <remarks>
		/// Mimo castingu se pokouší i o konverzi typu pomocí IConvertible.
		/// </remarks>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>
		/// vrátí hodnotu typu T;<br/>
		/// pokud parametr neexistuje a není <see cref="FullLoad"/>, pak vrací <c>default(T)</c>, ve FullLoad hází výjimku ArgumentException;<br/>
		/// pokud má parametr hodnotu NULL, pak vrací <c>null</c> pro referenční typy, pro hodnotové typy hází výjimku InvalidCastException<br/>
		/// </returns>
		/// <exception cref="ArgumentException">pokud field v data recordu není a vlastnost <see cref="FullLoad"/> je <c>true</c></exception>
		/// <exception cref="InvalidCastException">pokud nelze převést field na výstupní typ, nebo pokud je field <see cref="DBNull"/> a výstupní typ nemá <c>null</c></exception>
		public T Get<T>(string fieldName)
		{
			T target;
			TryGet<T>(fieldName, out target);
			return target;
		}

		/// <summary>
		/// Načte parametr zadaného generického typu T.
		/// </summary>
		/// <typeparam name="T">Typ hodnoty.</typeparam>
		/// <remarks>
		/// Narozdíl od <see cref="TryGet{T}(string, out T)"/> neindikuje přítomnost fieldu v data recordu, nýbrž je-li field roven <see cref="DBNull"/>.<br/>
		/// Pokud je field <see cref="DBNull"/>, pak parametr <c>target</c> nezmění
		/// </remarks>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>
		/// <c>false</c>, pokud má field hodnotu <see cref="DBNull"/>;<br/>
		/// <c>false</c>, pokud nebyl field nalezen a <see cref="FullLoad"/> je <c>false</c>;
		/// <c>true</c>, pokud byla načtena hodnota
		/// </returns>
		/// <exception cref="ArgumentException">pokud nebyl field nalezen a <see cref="FullLoad"/> je <c>true</c></exception>
		/// <exception cref="InvalidCastException">pokud nelze převést field na výstupní typ</exception>
		[Obsolete("Metoda Load<T>() je obsolete, použijte TryGet<T>().")]
		public bool Load<T>(string fieldName, ref T target)
		{
			object value;
			if (dataDictionary.TryGetValue(fieldName, out value))
			{
				if (value == DBNull.Value)
				{
					// neměníme hodnotu target
					return false;
				}
				else
				{
					if (value is T)
					{
						target = (T)value;
						return true;
					}
					else
					{
						if (value is IConvertible)
						{
							try
							{
								target = (T)Convert.ChangeType(value, typeof(T));	 // poslední pokus např. pro konverzi decimal -> double
								return true;
							}
							catch (InvalidCastException e)
							{
								throw new InvalidCastException("Specified cast is not valid, field '" + fieldName + "', from " + value.GetType().FullName, e);
							}
						}
					}
				}
			}
			else if (dataLoadPower == DataLoadPower.FullLoad)
			{
				throw new ArgumentException("Parametr ve vstupních datech nebyl nalezen", fieldName);
			}
			return false;
		}

		/// <summary>
		/// Načte parametr typu Object.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla načtena hodnota</returns>
		[Obsolete("Metoda LoadObject() je obsolete, použijte TryGet<object>().")]
		public bool LoadObject(string fieldName, ref object target)
		{
			return Load<object>(fieldName, ref target);
		}

		/// <summary>
		/// Vrátí parametr typu Object.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>null, pokud má parametr hodnotu NULL, nebo neexistuje; jinak hodnota typu Object</returns>
		public object GetObject(string fieldName)
		{
			return Get<object>(fieldName);
		}

		/// <summary>
		/// Načte parametr typu string.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla načtena hodnota</returns>
		[Obsolete("Metoda LoadString() je obsolete, použijte TryGet<string>().")]
		public bool LoadString(string fieldName, ref string target)
		{
			return Load<string>(fieldName, ref target);
		}

		/// <summary>
		/// Vrátí parametr typu String.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>null, pokud má parametr hodnotu NULL, nebo neexistuje; jinak hodnota typu String</returns>
		public string GetString(string fieldName)
		{
			return Get<string>(fieldName);
		}

		/// <summary>
		/// Načte parametr typu Int32.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla načtena hodnota</returns>
		[Obsolete("Metoda LoadInt32() je obsolete, použijte TryGet<int>().")]
		public bool LoadInt32(string fieldName, ref Int32 target)
		{
			return Load<Int32>(fieldName, ref target);
		}

		/// <summary>
		/// Vrátí parametr typu Int32?.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>null, pokud má parametr hodnotu NULL, nebo neexistuje; jinak hodnota typu Int32</returns>
		public Int32? GetNullableInt32(string fieldName)
		{
			return Get<Int32?>(fieldName);
		}

		/// <summary>
		/// Načte parametr typu Double.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla načtena hodnota</returns>
		[Obsolete("Metoda LoadDouble() je obsolete, použijte TryGet<double>().")]
		public bool LoadDouble(string fieldName, ref double target)
		{
			return Load<double>(fieldName, ref target);
		}

		/// <summary>
		/// Vrátí parametr typu Double?.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>null, pokud má parametr hodnotu NULL, nebo neexistuje; jinak hodnota typu Double</returns>
		public double? GetNullableDouble(string fieldName)
		{
			return Get<double?>(fieldName);
		}

		/// <summary>
		/// Načte parametr typu Boolean.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla načtena hodnota</returns>
		[Obsolete("Metoda LoadBoolean() je obsolete, použijte TryGet<bool>().")]
		public bool LoadBoolean(string fieldName, ref bool target)
		{
			return Load<Boolean>(fieldName, ref target);
		}

		/// <summary>
		/// Vrátí parametr typu bool?.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>null, pokud má parametr hodnotu NULL, nebo neexistuje; jinak hodnota typu Boolean</returns>
		public bool? GetNullableBoolean(string fieldName)
		{
			return Get<bool?>(fieldName);
		}

		/// <summary>
		/// Načte parametr typu DateTime.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla načtena hodnota</returns>
		[Obsolete("Metoda LoadDateTime() je obsolete, použijte TryGet<DateTime>().")]
		public bool LoadDateTime(string fieldName, ref DateTime target)
		{
			return Load<DateTime>(fieldName, ref target);
		}

		/// <summary>
		/// Vrátí parametr typu DateTime?.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>null, pokud má parametr hodnotu NULL, nebo neexistuje; jinak hodnota typu DateTime</returns>
		public DateTime? GetNullableDateTime(string fieldName)
		{
			return Get<DateTime?>(fieldName);
		}
	}
}
