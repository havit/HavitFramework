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
using Havit.Collections;
using Havit.Business;
using Havit.Business.Query;
using Havit.Data;
using Havit.Data.SqlClient;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest
{
	public partial class ObjednavkaSepsani : ObjednavkaSepsaniBase
	{
		#region Constructors
		/// <summary>
		/// Vytvoří instanci objektu jako nový prvek.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected ObjednavkaSepsani() : base()
		{
		}
		
		/// <summary>
		/// Vytvoří instanci existujícího objektu.
		/// </summary>
		/// <param name="id">ObjednavkaSepsaniID (PK).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected ObjednavkaSepsani(int id) : base(id)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.
		/// </summary>
		/// <param name="id">ObjednavkaSepsaniID (PK).</param>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu (i částečnými).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected ObjednavkaSepsani(int id, DataRecord record) : base(id, record)
		{
		}
		#endregion
		
		#region CreateObject (static)
		/// <summary>
		/// Vrátí nový objekt.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		public static ObjednavkaSepsani CreateObject()
		{
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<ObjednavkaSepsani>() != null);
			
			ObjednavkaSepsani result = new ObjednavkaSepsani();
			return result;
		}
		#endregion
		
		#region GetObject (static)
		
		/// <summary>
		/// Vrátí existující objekt s daným ID.
		/// </summary>
		/// <param name="id">ObjednavkaSepsaniID (PK).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		public static ObjednavkaSepsani GetObject(int id)
		{
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<ObjednavkaSepsani>() != null);
			
			ObjednavkaSepsani result;
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			if ((currentIdentityMap != null) && (currentIdentityMap.TryGet<ObjednavkaSepsani>(id, out result)))
			{
				global::System.Diagnostics.Contracts.Contract.Assume(result != null);
				return result;
			}
			
			result = new ObjednavkaSepsani(id);
			
			return result;
		}
		
		/// <summary>
		/// Vrátí existující objekt inicializovaný daty z DataReaderu.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		internal static ObjednavkaSepsani GetObject(DataRecord dataRecord)
		{
			global::System.Diagnostics.Contracts.Contract.Requires(dataRecord != null);
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<ObjednavkaSepsani>() != null);
			
			ObjednavkaSepsani result = null;
			
			int id = dataRecord.Get<int>(ObjednavkaSepsani.Properties.ID.FieldName);
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			if ((currentIdentityMap != null)
				&& ((dataRecord.DataLoadPower == DataLoadPower.Ghost)
					|| (dataRecord.DataLoadPower == DataLoadPower.FullLoad)))
			{
				if (currentIdentityMap.TryGet<ObjednavkaSepsani>(id, out result))
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
						result = ObjednavkaSepsani.GetObject(id);
					}
					else
					{
						result = new ObjednavkaSepsani(id, dataRecord);
					}
				}
			}
			else
			{
				result = new ObjednavkaSepsani(id, dataRecord);
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
		public static ObjednavkaSepsani GetObjectOrDefault(int? id, ObjednavkaSepsani defaultValue = null)
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
		public static ObjednavkaSepsaniCollection GetObjects(params int[] ids)
		{
			global::System.Diagnostics.Contracts.Contract.Requires(ids != null);
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<ObjednavkaSepsaniCollection>() != null);
			
			return new ObjednavkaSepsaniCollection(Array.ConvertAll<int, ObjednavkaSepsani>(ids, id => ObjednavkaSepsani.GetObject(id)));
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
