﻿using System;
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
	/// <summary>
	/// Uživatel.
	/// </summary>
	public partial class Uzivatel : UzivatelBase
	{
		#region Constructors
		/// <summary>
		/// Vytvoří instanci objektu jako nový prvek.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected Uzivatel() : base()
		{
		}
		
		/// <summary>
		/// Vytvoří instanci existujícího objektu.
		/// </summary>
		/// <param name="id">UzivatelID (PK).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected Uzivatel(int id) : base(id)
		{
		}
		
		/// <summary>
		/// Vytvoří instanci objektu na základě dat (i částečných) načtených z databáze.
		/// </summary>
		/// <param name="id">UzivatelID (PK).</param>
		/// <param name="record"><see cref="Havit.Data.DataRecord"/> s daty objektu (i částečnými).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		protected Uzivatel(int id, DataRecord record) : base(id, record)
		{
		}
		#endregion
		
		#region CreateObject (static)
		/// <summary>
		/// Vrátí nový objekt.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		public static Uzivatel CreateObject()
		{
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<Uzivatel>() != null);
			
			Uzivatel result = new Uzivatel();
			return result;
		}
		#endregion
		
		#region GetObject (static)
		
		/// <summary>
		/// Vrátí existující objekt s daným ID.
		/// </summary>
		/// <param name="id">UzivatelID (PK).</param>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		public static Uzivatel GetObject(int id)
		{
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<Uzivatel>() != null);
			
			Uzivatel result;
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			if ((currentIdentityMap != null) && (currentIdentityMap.TryGet<Uzivatel>(id, out result)))
			{
				global::System.Diagnostics.Contracts.Contract.Assume(result != null);
				return result;
			}
			
			result = new Uzivatel(id);
			
			return result;
		}
		
		/// <summary>
		/// Vrátí existující objekt inicializovaný daty z DataReaderu.
		/// </summary>
		[System.Diagnostics.Contracts.ContractVerification(false)]
		[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
		internal static Uzivatel GetObject(DataRecord dataRecord)
		{
			global::System.Diagnostics.Contracts.Contract.Requires(dataRecord != null);
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<Uzivatel>() != null);
			
			Uzivatel result = null;
			
			int id = dataRecord.Get<int>(Uzivatel.Properties.ID.FieldName);
			
			IdentityMap currentIdentityMap = IdentityMapScope.Current;
			if ((currentIdentityMap != null)
				&& ((dataRecord.DataLoadPower == DataLoadPower.Ghost)
					|| (dataRecord.DataLoadPower == DataLoadPower.FullLoad)))
			{
				if (currentIdentityMap.TryGet<Uzivatel>(id, out result))
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
						result = Uzivatel.GetObject(id);
					}
					else
					{
						result = new Uzivatel(id, dataRecord);
					}
				}
			}
			else
			{
				result = new Uzivatel(id, dataRecord);
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
		public static Uzivatel GetObjectOrDefault(int? id, Uzivatel defaultValue = null)
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
		public static UzivatelCollection GetObjects(params int[] ids)
		{
			global::System.Diagnostics.Contracts.Contract.Requires(ids != null);
			global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<UzivatelCollection>() != null);
			
			return new UzivatelCollection(Array.ConvertAll<int, Uzivatel>(ids, id => Uzivatel.GetObject(id)));
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
