using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.IO;

namespace Havit.Data.SqlTypes
{
	/// <summary>
	/// Aggregate k UDT SqlInt32Array, který zajišťuje převod tabulky hodnot na pole.
	/// </summary>
	/// <example>
	/// Vytvoření agregátu typu:<br/>
	/// <code>
	/// CREATE AGGREGATE [dbo].IntArrayAggregate<br/>
	/// RETURNS IntArray<br/>
	/// EXTERNAL NAME [Havit.Data.SqlServer].[Havit.Data.SqlTypes.SqlInt32ArrayAggregate]<br/>
	/// </code>
	/// Použití agregátu pro vytvoření pole hodnot:<br/>
	/// <code>
	/// SELECT IntArrayAggreagate(ItemID) AS Items FROM dbo.Item WHERE ...<br/>
	/// </code>
	/// </example>
	[Serializable]
	[Microsoft.SqlServer.Server.SqlUserDefinedAggregate(
		Format.UserDefined,
		IsInvariantToDuplicates = false,
		IsInvariantToNulls = false,
		IsInvariantToOrder = true,
		IsNullIfEmpty = true,
		MaxByteSize = -1,
		Name = "IntArrayAggregate")]
	public class SqlInt32ArrayAggregate : IBinarySerialize
	{
		#region private value holder
		/// <summary>
		/// Uchovává mezivýsledek.
		/// </summary>
		private SqlInt32Array array;
		#endregion

		#region Init
		/// <summary>
		/// Inicializace agregátoru.
		/// </summary>
		public void Init()
		{
			array = new SqlInt32Array();
		}
		#endregion

		#region Accumulate
		/// <summary>
		/// Přidá další hodnotu do agregace.
		/// </summary>
		/// <param name="value">přidávaná hodnota</param>
		public void Accumulate(SqlInt32 value)
		{
			if (!value.IsNull)
			{
				array.Add(value);
			}
		}
		#endregion

		#region Merge
		/// <summary>
		/// Spojí dva agregáty v jeden
		/// </summary>
		/// <param name="group">druhá agregace</param>
		public void Merge(SqlInt32ArrayAggregate group)
		{
			if (group != null)
			{
				group.array.Merge(group.array);
			}
		}
		#endregion

		#region Terminate
		/// <summary>
		/// Vrátí výsledek agregace.
		/// </summary>
		public SqlInt32Array Terminate()
		{
			return this.array;
		}
		#endregion

		#region IBinarySerialize Members
		/// <summary>
		/// De-serializuje agregaci.
		/// </summary>
		/// <param name="r">BinaryReader</param>
		public void Read(BinaryReader r)
		{
			this.array = new SqlInt32Array();
			this.array.Read(r);
		}

		/// <summary>
		/// Serializuje agregaci.
		/// </summary>
		/// <param name="w">BinaryWriter</param>
		public void Write(BinaryWriter w)
		{
			this.array.Write(w);
		}
		#endregion
	}
}