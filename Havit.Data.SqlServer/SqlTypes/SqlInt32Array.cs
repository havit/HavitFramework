using System;
using System.Data;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Havit.Data.SqlTypes
{
	/// <summary>
	/// UDT reprezentující pole SqlInt32 (T-SQL: int) hodnot.
	/// </summary>
	/// <remarks>
	/// Vzhledem k tomu, že maximální velikost UDT je 8KB, maximem je 1999 hodnot (jedna je stavová).<br/>
	/// <br/>
	/// Binární serializace je takováto:<br/>
	/// byte 1-4 ~ Int32 Length (velikost pole, pokud je 0, pak je hodnota NULL)<br/>
	/// byte 5-(8000) ~ values<br/>
	/// </remarks>
	/// <example>
	/// Vytvoření UDT typu:<br/>
	/// <code>
	/// CREATE TYPE [dbo].IntArray<br/>
	/// EXTERNAL NAME [Havit.Data.SqlServer].[Havit.Data.SqlTypes.SqlInt32Array]<br/>
	/// </code>
	/// Vytvoření funkce:<br/>
	/// <code>
	/// CREATE FUNCTION IntArrayToTable<br/>
	/// (<br/>
	///     @array dbo.IntArray<br/>
	/// )<br/>
	/// RETURNS TABLE<br/>
	/// (<br/>
	///     [Value] int<br/>
	/// )<br/>
	/// AS EXTERNAL NAME [Havit.Data.SqlServer].[Havit.Data.SqlTypes.SqlInt32Array].[GetInt32Values]<br/>
	/// </code>
	/// Použití ve filtru:<br/>
	/// <code>
	/// CREATE PROCEDURE Filter<br/>
	/// (<br/>
	///     @Vlastnosti dbo.IntArray = NULL<br/>
	/// )<br/>
	/// AS<br/>
	///     SELECT col FROM tab<br/>
	///         WHERE ((@Vlastnosti IS NULL) OR (VlastnostID IN (SELECT Value FROM dbo.IntArrayToTable(@Vlastnosti))))<br/>
	/// </code>
	/// </example>
	[Serializable]
	[SqlUserDefinedType(
		Format.UserDefined,
		Name = "IntArray",
		IsByteOrdered = true,
		MaxByteSize = -1)]
	public class SqlInt32Array : INullable, IBinarySerialize
	{
		#region private value holder
		private List<SqlInt32> values = null;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoří instanci s hodnotou NULL.
		/// </summary>
		public SqlInt32Array()
		{
			this.values = null;
		}

		/// <summary>
		/// Vytvoří instanci a naplní ji předanými hodnotami.
		/// </summary>
		/// <param name="values">hodnoty, které mají instance reprezentovat</param>
		public SqlInt32Array(int[] values)
		{
			if ((values == null) || (values.Length == 0))
			{
				this.values = null;
				return;
			}
			//if (values.Length > 1999)
			//{
			//    throw new ArgumentException(String.Format("Maximální velikost pole je 1999 hodnot, požadováno je však {0} hodnot.",
			//        values.Length));
			//}

			this.values = new List<SqlInt32>();
			for (int i = 0; i < values.Length; i++)
			{
				this.values.Add(new SqlInt32(values[i]));
			}
		}
		#endregion

		#region Add
		/// <summary>
		/// Přidá prvek do pole.
		/// </summary>
		public void Add(SqlInt32 value)
		{
			if (!value.IsNull && (value.Value == Int32.MinValue))
			{
				throw new ArgumentException("Prvek nesmí mít vyhrazenou hodnotu Int32.MinValue.");
			}

			if (this.values == null)
			{
				this.values = new List<SqlInt32>();
			}
			values.Add(value);
		}
		#endregion

		#region Count
		/// <summary>
		/// Počet prvků v seznamu.
		/// </summary>
		public int Count
		{
			get { return values.Count; }
		}
		#endregion

		#region Indexer
		/// <summary>
		/// Indexer pro přístup k prvkům podle jejich pořadí.
		/// </summary>
		/// <param name="index">index (pořadí) prvku</param>
		/// <returns>hodnota <see cref="SqlInt32"/></returns>
		public SqlInt32 this[int index]
		{
			get { return values[index]; }
		}
		#endregion

		#region Merge
		/// <summary>
		/// Spojí dvě pole v jedno.
		/// </summary>
		/// <param name="array">přidávané pole</param>
		public void Merge(SqlInt32Array array)
		{
			if (!array.IsNull)
			{
				for (int i = 0; i < array.values.Count; i++)
				{
					this.values.Add(array.values[i]);
				}
			}
		}
		#endregion

		#region Accessors
		/// <summary>
		/// Vrátí pole SqlInt32[] s hodnotami.
		/// </summary>
		/// <returns>Pole SqlInt32[] s hodnotami.</returns>
		public SqlInt32[] GetSqlInt32Array()
		{
			if (this.IsNull)
			{
				return null;
			}
			return (SqlInt32[])values.ToArray();
		}

		/// <summary>
		/// Vrací tabulku Int32 hodnot.
		/// Metoda určená pro mapování do T-SQL na table-valued function (TVF).
		/// </summary>
		/// <param name="values">Proměnná, která má být rozbalena do tabulky hodnot Int32.</param>
		/// <returns>tabulka Int32 hodnot (pomocí FillInt32Row)</returns>
		[SqlFunctionAttribute(
			DataAccess= DataAccessKind.None,
			IsDeterministic= true,
			IsPrecise= true,
			Name= "IntArrayToTable",
			TableDefinition = "[Value] int",
			FillRowMethodName = "FillSqlInt32Row")]
		public static IEnumerable GetSqlInt32Values(SqlInt32Array values)
		{
			if ((values != null) && (!values.IsNull))
			{
				return values.GetSqlInt32Array();
			}
			return null;
		}

		/// <summary>
		/// Metoda zajišťující převod řádku v table-valued function (TVF).
		/// </summary>
		/// <param name="sqlInt32ArrayElement">vstupní hodnota řádku</param>
		/// <param name="value">výstupní hodnota řádku</param>
		public static void FillSqlInt32Row(object sqlInt32ArrayElement, out SqlInt32 value)
		{
			if (sqlInt32ArrayElement is SqlInt32)
			{
				value = (SqlInt32)sqlInt32ArrayElement;
			}
			else
			{
				value = SqlInt32.Null;
			}
		}
		#endregion

		#region Parse
		/// <summary>
		/// Vytvoří z CSV textové reprezentace hodnotu pole.
		/// </summary>
		/// <param name="text">CSV text hodnot</param>
		/// <returns>pole s hodnotami dle CSV</returns>
		[SqlMethod(DataAccess = DataAccessKind.None, SystemDataAccess = SystemDataAccessKind.None)]
		public static SqlInt32Array Parse(SqlString text)
		{
			if (text.IsNull)
			{
				return Null;
			}
			string[] parts = text.Value.Split(',');
			int length = parts.Length;
			SqlInt32Array result = new SqlInt32Array();
			for (int i = 0; i < length; i++)
			{
				if (String.Compare(parts[i].Trim(), "NULL", true) == 0)
				{
					result.Add(SqlInt32.Null);
				}
				else
				{
					result.Add(new SqlInt32(Convert.ToInt32(parts[i])));
				}
			}
			return result;
		}
		#endregion

		#region ToString
		/// <summary>
		/// Převede hodnotu na CSV textovou reprezentaci string
		/// </summary>
		/// <returns>CSV seznam hodnot</returns>
		public override string ToString()
		{
			if (this.IsNull)
			{
				return null;
			}

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < this.values.Count; i++)
			{
				if (this.values[i].IsNull)
				{
					sb.Append("NULL");
				}
				else
				{
					sb.Append(this.values[i].Value);
				}
				if (i < this.values.Count - 1)
				{
					sb.Append(",");
				}
			}
			return sb.ToString();
		}
		#endregion

		#region Null (static)
		/// <summary>
		/// Hodnota NULL.
		/// </summary>
		public static SqlInt32Array Null
		{
			get
			{
				return new SqlInt32Array();
			}
		}
		#endregion

		#region INullable Members
		/// <summary>
		/// Indikuje, zda-li je hodnota NULL.
		/// </summary>
		public bool IsNull
		{
			get
			{
				return ((this.values == null) || (this.values.Count == 0));
			}
		}
		#endregion

		#region IBinarySerialize Members
		/// <summary>
		/// Načte hodnotu z binární reprezentace.
		/// </summary>
		/// <remarks>
		/// Binární serializace je takováto:
		/// byte 1-4 ~ Int32 Length (velikost pole, pokud je 0, pak je hodnota NULL)
		/// byte 5-(8000) ~ values (NULL hodnoty reprezentuje Int32.MinValue)
		/// </remarks>
		/// <param name="r"><see cref="System.IO.BinaryReader"/> s binární reprezentací hodnoty</param>
		public void Read(System.IO.BinaryReader r)
		{
			// byte 1 - počet hodnot
			Int32 length = r.ReadInt32();
			if (length == 0)
			{
				// NULL
				this.values = null;
			}
			else
			{
				// hodnoty
				this.values = new List<SqlInt32>();
				for (int i = 0; i < length; i++)
				{
					Int32 temp = r.ReadInt32();
					if (temp == Int32.MinValue)
					{
						this.values.Add(SqlInt32.Null);
					}
					else
					{
						this.values.Add(new SqlInt32(temp));
					}
				}
			}
		}

		/// <summary>
		/// Vytvoří binární reprezentace hodnoty.
		/// </summary>
		/// <remarks>
		/// Binární serializace je takováto:
		/// byte 1-4 ~ Int32 Length (velikost pole, pokud je 0, pak je hodnota NULL)
		/// byte 5-(8000) ~ values (NULL hodnoty implementuje Int32.MinValue)
		/// </remarks>
		/// <param name="w"><see cref="System.IO.BinaryWriter"/> do kterého má být binární reprezentace zapsána</param>
		public void Write(System.IO.BinaryWriter w)
		{
			// byte 1 - počet hodnot
			if (this.IsNull)
			{
				w.Write(0);
			}
			else
			{
				w.Write(this.values.Count);

				// hodnoty
				for (int i = 0; i < this.values.Count; i++)
				{
					if (this.values[i].IsNull)
					{
						w.Write(Int32.MinValue);
					}
					else
					{
						w.Write(this.values[i].Value);
					}
				}
			}
		}
		#endregion
	}
}

