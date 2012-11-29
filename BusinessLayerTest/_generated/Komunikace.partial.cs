using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Havit.Business;
using Havit.Business.Query;
using Havit.Collections;
using Havit.Data;
using Havit.Data.SqlClient;
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest
{
	/// <summary>
	/// Komunikace.
	/// </summary>
	public partial class Komunikace : KomunikaceBase
	{
		#region Constructors
		/// <summary>
		/// Vytvoří instanci objektu jako nový prvek.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected Komunikace() : base()
		{
		}
		
		/// <summary>
		/// Vytvoří instanci existujícího objektu.
		/// </summary>
		/// <param name="id">KomunikaceID (PK).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected Komunikace(int id) : base(id)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.
		/// </summary>
		/// <param name="id">KomunikaceID (PK).</param>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu (i částečnými).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected Komunikace(int id, DataRecord record) : base(id, record)
		{
		}
		#endregion
		
		#region CreateObject (static)
		/// <summary>
		/// Vrátí nový objekt.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		public static Komunikace CreateObject()
		{
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<Komunikace>() != null);
			
			Komunikace result = new Komunikace();
			return result;
		}
		#endregion
		
		#region GetObject (static)
		
		/// <summary>
		/// Vrátí existující objekt s daným ID.
		/// </summary>
		/// <param name="id">KomunikaceID (PK).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		public static Komunikace GetObject(int id)
		{
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<Komunikace>() != null);
			
			Komunikace result;
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			if ((currentIdentityMap != null) && (currentIdentityMap.TryGet<Komunikace>(id, out result)))
			{
				global::System.Diagnostics.Contracts.Contract.Assume(result != null);
				return result;
			}
			
			result = new Komunikace(id);
			
			return result;
		}
		
		/// <summary>
		/// Vrátí existující objekt inicializovaný daty z DataReaderu.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		internal static Komunikace GetObject(DataRecord dataRecord)
		{
			global::System.Diagnostics.Contracts.Contract.Requires(dataRecord != null);
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<Komunikace>() != null);
			
			Komunikace result = null;
			
			int id = dataRecord.Get<int>(Komunikace.Properties.ID.FieldName);
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			if ((currentIdentityMap != null)
				&& ((dataRecord.DataLoadPower == DataLoadPower.Ghost)
					|| (dataRecord.DataLoadPower == DataLoadPower.FullLoad)))
			{
				if (currentIdentityMap.TryGet<Komunikace>(id, out result))
				{
					if (!result.IsLoaded && (dataRecord.DataLoadPower == DataLoadPower.FullLoad))
					{
						result.Load(dataRecord);
					}
				}
				else
				{
					if (dataRecord.DataLoadPower == DataLoadPower.Ghost)
					{
						result = Komunikace.GetObject(id);
					}
					else
					{
						result = new Komunikace(id, dataRecord);
					}
				}
			}
			else
			{
				result = new Komunikace(id, dataRecord);
			}
			
			return result;
		}
		
		#endregion
		
		#region GetObjectOrDefault (static)
		
		/// <summary>
		/// Pokud je zadáno ID objektu (not-null), vrátí existující objekt s daným ID. Jinak vrací výchozí hodnotu (není-li zadána, pak vrací null).
		/// </summary>
		/// <param name="id">ID objektu.</param>
		/// <param name="defaultValue">Výchozí hodnota.</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		public static Komunikace GetObjectOrDefault(int? id, Komunikace defaultValue = null)
		{
			return (id != null) ? GetObject(id.Value) : defaultValue;
		}
		
		#endregion
		
		#region GetObjects (static)
		
		/// <summary>
		/// Vrátí kolekci obsahující objekty danými ID.
		/// </summary>
		/// <param name="ids">Identifikátory objektů.</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		public static KomunikaceCollection GetObjects(params int[] ids)
		{
			global::System.Diagnostics.Contracts.Contract.Requires(ids != null);
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<KomunikaceCollection>() != null);
			
			return new KomunikaceCollection(Array.ConvertAll<int, Komunikace>(ids, id => Komunikace.GetObject(id)));
		}
		
		#endregion
		
		//------------------------------------------------------------------------------
		// <auto-generated>
		//     This code was generated by a tool.
		//     Changes to this file will be lost if the code is regenerated.
		// </auto-generated>
		//------------------------------------------------------------------------------
		
	}
}
