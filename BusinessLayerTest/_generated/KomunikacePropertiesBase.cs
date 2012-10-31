﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
	/// Objektová reprezentace metadat vlastností typu Komunikace.
	/// </summary>
	[System.Diagnostics.Contracts.ContractVerification(false)]
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public class KomunikacePropertiesBase
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public KomunikacePropertiesBase()
		{
			_id = new FieldPropertyInfo();
			_subjekt = new ReferenceFieldPropertyInfo();
			_objednavkaSepsani = new ReferenceFieldPropertyInfo();
			_all = new PropertyInfoCollection(_id, _subjekt, _objednavkaSepsani);
		}
		
		/// <summary>
		/// Inicializuje hodnoty metadat.
		/// </summary>
		public void Initialize(ObjectInfo objectInfo)
		{
			_id.Initialize(objectInfo, "ID", "KomunikaceID", true, SqlDbType.Int, false, 4);
			_subjekt.Initialize(objectInfo, "Subjekt", "SubjektID", false, SqlDbType.Int, false, 4, typeof(Havit.BusinessLayerTest.Subjekt), Havit.BusinessLayerTest.Subjekt.ObjectInfo);
			_objednavkaSepsani.Initialize(objectInfo, "ObjednavkaSepsani", "ObjednavkaSepsaniID", false, SqlDbType.Int, true, 4, typeof(Havit.BusinessLayerTest.ObjednavkaSepsani), Havit.BusinessLayerTest.ObjednavkaSepsani.ObjectInfo);
		}
		
		/// <summary>
		/// Identifikátor objektu.
		/// </summary>
		public FieldPropertyInfo ID
		{
			get
			{
				global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<FieldPropertyInfo>() != null);
				return _id;
			}
		}
		private FieldPropertyInfo _id;
		
		public ReferenceFieldPropertyInfo Subjekt
		{
			get
			{
				global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<ReferenceFieldPropertyInfo>() != null);
				return _subjekt;
			}
		}
		private ReferenceFieldPropertyInfo _subjekt;
		
		public ReferenceFieldPropertyInfo ObjednavkaSepsani
		{
			get
			{
				global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<ReferenceFieldPropertyInfo>() != null);
				return _objednavkaSepsani;
			}
		}
		private ReferenceFieldPropertyInfo _objednavkaSepsani;
		
		/// <summary>
		/// Všechny sloupečky typu Komunikace.
		/// </summary>
		public PropertyInfoCollection All
		{
			get
			{
				return _all;
			}
		}
		private PropertyInfoCollection _all;
		
	}
}
