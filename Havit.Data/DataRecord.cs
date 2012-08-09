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
			get { return fullLoad; }
			set { fullLoad = value; }
		}
		private bool fullLoad;
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
		/// <param name="fullLoad">true, má-li být pøi nenalezení parametru vyvolána výjimka</param>
		public DataRecord(DataRow row, bool fullLoad)
		{
			this.fullLoad = fullLoad;

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
		/// <param name="row">datový zdroj typu <see cref="System.Data.DataRow"/></param>
		public DataRecord(DataRow row)
		{
			this.fullLoad = true;

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
		/// Vytvoøí instanci DataRecordu a naète do ní data z <see cref="System.Data.IDataRecord"/>
		/// (napø. <see cref="System.Data.SqlClient.SqlDataReader"/>).
		/// </summary>
		/// <param name="record">datový zdroj <see cref="System.Data.IDataRecord"/> (napø. <see cref="System.Data.SqlClient.SqlDataReader"/>)</param>
		/// <param name="fullLoad">true, má-li být pøi nenalezení parametru vyvolána výjimka</param>
		public DataRecord(IDataRecord record, bool fullLoad)
		{
			this.fullLoad = fullLoad;

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
		public DataRecord(IDataRecord record)
		{
			this.fullLoad = true;

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

		#region Load<T>, Get<T>
		/// <summary>
		/// Naète parametr zadaného generického typu T.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
		public bool Load<T>(string fieldName, ref T target)
		{
			if (dataDictionary.ContainsKey(fieldName))
			{
				if (dataDictionary[fieldName] != DBNull.Value)
				{
					try
					{
						target = (T)dataDictionary[fieldName];
					}
					catch (InvalidCastException e)
					{
						throw new InvalidCastException("Specified cast is not valid, field type is " + dataDictionary[fieldName].GetType().FullName, e);
					}
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (fullLoad)
			{
				throw new ArgumentException("Parametr ve vstupních datech nebyl nalezen", fieldName);
			}
			return false;
		}

		/// <summary>
		/// Vrátí parametr zadaného generického typu.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <returns>
		/// vrátí hodnotu typu T;<br/>
		/// pokud parametr neexistuje a není FullLoad, pak vrací default(T), ve FullLoad hází výjimku ArgumentException;<br/>
		/// pokud má parametr hodnotu NULL, pak vrací <c>null</c> pro referenèní typy, pro hodnotové typy hází výjimku InvalidCastException<br/>
		/// </returns>
		public T Get<T>(string fieldName)
		{
			object value;
			if (dataDictionary.TryGetValue(fieldName, out value))
			{
				if (value == DBNull.Value)
				{
					if (default(T) != null)
					{
						throw new InvalidCastException("Hodnota NULL nelze pøevést na ValueType.");
					}
					return default(T);
				}
				else
				{
					if (value is T)
					{
						return (T)value;
					}
					else if (value is IConvertible)
					{
						return (T)Convert.ChangeType(value, typeof(T));
					}
					else
					{
						try
						{
							return (T)dataDictionary[fieldName];
						}
						catch (InvalidCastException e)
						{
							throw new InvalidCastException("Specified cast is not valid, field '" + fieldName + "', type " + dataDictionary[fieldName].GetType().FullName, e);
						}
					}
				}
			}
			else if (fullLoad)
			{
				throw new ArgumentException("Parametr požadovaného jména nebyl v DataRecordu nalezen.", fieldName);
			}

			return default(T);
		}
		#endregion

		#region LoadObject, GetObject
		/// <summary>
		/// Naète parametr typu Object.
		/// </summary>
		/// <param name="fieldName">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
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
		/// <param name="field">jméno parametru</param>
		/// <param name="target">cíl, kam má být parametr uložen</param>
		/// <returns>false, pokud má parametr hodnotu NULL; true, pokud byla naètena hodnota</returns>
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
