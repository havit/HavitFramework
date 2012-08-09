using System;
using System.Collections.Generic;
using System.Data;

namespace Havit.Data
{
	/// <summary>
	/// DataRecord usnadòuje naèítání dat objektu z databáze.
	/// Zejména vhodný je pro situace, kdy je možné i èásteèné naèítání.
	/// </summary>
	/// <remarks>
	/// DataRecord pracuje tak, že v constructoru zkopíruje celý datový záznam do slovníku Dictionary&lt;field, value&gt;.
	/// V jednotlivých Loadech pak už jenom naèítá data ze slovníku.<br/>
	/// Datový zdroj je tedy potøeba pouze v okamžiku volání constructoru a následnì ho mùžeme zlikvidovat.<br/>
	/// Stejnìtak je vhodné použít na všechny loady jeden DataRecord a pøedávat si ho mezi objekty.
	/// </remarks>
	public class DataRecord
	{
		#region Properties
		/// <summary>
		/// Indikuje, zda-li je požadována 100% úspìšnost pro naèítání položek (true), nebo zda-li se mají neúspìchy ignorovat.
		/// </summary>
		public bool FullLoad
		{
			get { return DataLoadPower == DataLoadPower.FullLoad; }
//			set { 
//				fullLoad = value; }
		}
		//private bool fullLoad;

		/// <summary>
		/// Indikuje množství dat, které jsou uloženy v DataRecordu vùèi všem možným sloupcùm øádkù.
		/// </summary>
		public DataLoadPower DataLoadPower
		{
			get { return dataLoadPower; }
			set { dataLoadPower = value; }
		}
		private DataLoadPower dataLoadPower;
		
		#endregion

		#region private data fields
		/// <summary>
		/// Data z databáze.
		/// </summary>
		private Dictionary<string, object> dataDictionary;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoøí instanci DataRecordu a naète do ní data z <see cref="System.Data.DataRow"/>.
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
				if (!this.dataDictionary.ContainsKey(row.Table.Columns[i].ColumnName))
				{
					this.dataDictionary.Add(row.Table.Columns[i].ColumnName, row[i]);
				}
			}
		}

		/// <summary>
		/// Vytvoøí instanci DataRecordu a naète do ní data z <see cref="System.Data.DataRow"/>.
		/// </summary>
		/// <param name="row">datový zdroj typu DataRow</param>
		/// <param name="fullLoad">true, má-li být pøi nenalezení parametru vyvolána výjimka</param>
		[Obsolete]
		public DataRecord(DataRow row, bool fullLoad): this(row, fullLoad ? DataLoadPower.FullLoad : DataLoadPower.PartialLoad)
		{
		}

		/// <summary>
		/// Vytvoøí instanci DataRecordu a naète do ní data z <see cref="System.Data.DataRow"/>.
		/// </summary>
		/// <param name="row">datový zdroj typu <see cref="System.Data.DataRow"/></param>
		[Obsolete]
		public DataRecord(DataRow row): this(row, true)
		{
		}

		/// <summary>
		/// Vytvoøí instanci DataRecordu a naète do ní data z <see cref="System.Data.IDataRecord"/>
		/// (napø. <see cref="System.Data.SqlClient.SqlDataReader"/>).
		/// </summary>
		/// <param name="record">datový zdroj <see cref="System.Data.IDataRecord"/> (napø. <see cref="System.Data.SqlClient.SqlDataReader"/>)</param>
		/// <param name="dataLoadPower">Rozsah dat v datovém zdroji.</param>
		public DataRecord(IDataRecord record, DataLoadPower dataLoadPower)
		{
			this.dataLoadPower = dataLoadPower;

			// zkopíruje data do dataDictionary
			this.dataDictionary = new Dictionary<string, object>(record.FieldCount);
			for (int i = 0; i < record.FieldCount; i++)
			{
				if (!this.dataDictionary.ContainsKey(record.GetName(i)))
				{
					this.dataDictionary.Add(record.GetName(i), record[i]);
				}
			}
		}

		/// <summary>
		/// Vytvoøí instanci DataRecordu a naète do ní data z <see cref="System.Data.IDataRecord"/>
		/// (napø. <see cref="System.Data.SqlClient.SqlDataReader"/>).
		/// </summary>
		/// <param name="record">datový zdroj <see cref="System.Data.IDataRecord"/> (napø. <see cref="System.Data.SqlClient.SqlDataReader"/>)</param>
		/// <param name="fullLoad">true, má-li být pøi nenalezení parametru vyvolána výjimka</param>
		[Obsolete]
		public DataRecord(IDataRecord record, bool fullLoad): this(record, fullLoad ? DataLoadPower.FullLoad : DataLoadPower.PartialLoad)
		{
		}

		/// <summary>
		/// Vytvoøí instanci DataRecordu a naète do ní data z <see cref="System.Data.IDataRecord"/>
		/// (napø. <see cref="System.Data.SqlClient.SqlDataReader"/>).
		/// </summary>
		/// <param name="record">datový zdroj <see cref="System.Data.IDataRecord"/> (napø. <see cref="System.Data.SqlClient.SqlDataReader"/>)</param>
		[Obsolete]
		public DataRecord(IDataRecord record): this(record, true)
		{			
		}
		#endregion

		#region Indexer
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
		#endregion

		#region Get<T>, TryGet<T>, Load<T>
		/// <summary>
		/// Naète parametr zadaného generického typu T.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>
		/// <c>true</c>, pokud byla naètena hodnota;<br/>
		/// <c>false</c>, pokud field v data recordu není a vlastnost <see cref="FullLoad"/> je <c>false</c> (target je pak nastaven na <c>default(T)</c>);<br/>
		/// </returns>
		/// <exception cref="ArgumentException">pokud field v data recordu není a vlastnost <see cref="FullLoad"/> je <c>true</c></exception>
		/// <exception cref="InvalidCastException">pokud nelze pøevést field na výstupní typ, nebo pokud je field <see cref="DBNull"/> a výstupní typ nemá <c>null</c></exception>
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
						throw new InvalidCastException("Hodnota NULL nelze pøevést na ValueType.");
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
								target = (T)Convert.ChangeType(value, typeof(T));	 // poslední pokus napø. pro konverzi decimal -> double
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
		/// <remarks>
		/// Mimo castingu se pokouší i o konverzi typu pomocí IConvertible.
		/// </remarks>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>
		/// vrátí hodnotu typu T;<br/>
		/// pokud parametr neexistuje a není <see cref="FullLoad"/>, pak vrací <c>default(T)</c>, ve FullLoad hází výjimku ArgumentException;<br/>
		/// pokud má parametr hodnotu NULL, pak vrací <c>null</c> pro referenèní typy, pro hodnotové typy hází výjimku InvalidCastException<br/>
		/// </returns>
		/// <exception cref="ArgumentException">pokud field v data recordu není a vlastnost <see cref="FullLoad"/> je <c>true</c></exception>
		/// <exception cref="InvalidCastException">pokud nelze pøevést field na výstupní typ, nebo pokud je field <see cref="DBNull"/> a výstupní typ nemá <c>null</c></exception>
		public T Get<T>(string fieldName)
		{
			T target;
			TryGet<T>(fieldName, out target);
			return target;
		}

		/// <summary>
		/// Naète parametr zadaného generického typu T.
		/// </summary>
		/// <remarks>
		/// Narozdíl od <see cref="TryGet{T}(string, out T)"/> neindikuje pøítomnost fieldu v data recordu, nýbrž je-li field roven <see cref="DBNull"/>.<br/>
		/// Pokud je field <see cref="DBNull"/>, pak parametr <c>target</c> nezmìní
		/// </remarks>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>
		/// <c>false</c>, pokud má field hodnotu <see cref="DBNull"/>;<br/>
		/// <c>false</c>, pokud nebyl field nalezen a <see cref="FullLoad"/> je <c>false</c>;
		/// <c>true</c>, pokud byla naètena hodnota
		/// </returns>
		/// <exception cref="ArgumentException">pokud nebyl field nalezen a <see cref="FullLoad"/> je <c>true</c></exception>
		/// <exception cref="InvalidCastException">pokud nelze pøevést field na výstupní typ</exception>
		[Obsolete("Metoda Load<T>() je obsolete, použijte TryGet<T>().")]
		public bool Load<T>(string fieldName, ref T target)
		{
			object value;
			if (dataDictionary.TryGetValue(fieldName, out value))
			{
				if (value == DBNull.Value)
				{
					// nemìníme hodnotu target
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
								target = (T)Convert.ChangeType(value, typeof(T));	 // poslední pokus napø. pro konverzi decimal -> double
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
		#endregion

		#region LoadObject, GetObject
		/// <summary>
		/// Naète parametr typu Object.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
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
		#endregion

		#region LoadString, GetString
		/// <summary>
		/// Naète parametr typu string.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
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
		#endregion

		#region LoadInt32, GetNullableInt32
		/// <summary>
		/// Naète parametr typu Int32.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
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
		#endregion

		#region LoadDouble, GetNullableDouble
		/// <summary>
		/// Naète parametr typu Double.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
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

		#endregion

		#region LoadBoolean, GetNullableBoolean
		/// <summary>
		/// Naète parametr typu Boolean.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
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
		#endregion

		#region LoadDateTime, GetNullableDateTime
		/// <summary>
		/// Naète parametr typu DateTime.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
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
		#endregion
	}
}
