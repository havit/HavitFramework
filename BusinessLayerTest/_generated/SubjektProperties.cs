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
using System.Xml;
using Havit.Business;
using Havit.Business.Query;
using Havit.Collections;
using Havit.Data;
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest
{
	/// <summary>
	/// Objektová reprezentace metadat vlastností typu Subjekt.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public class SubjektProperties
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public SubjektProperties()
		{
			_id = new FieldPropertyInfo();
			_nazev = new FieldPropertyInfo();
			_uzivatel = new ReferenceFieldPropertyInfo();
			_created = new FieldPropertyInfo();
			_deleted = new FieldPropertyInfo();
			_komunikace = new CollectionPropertyInfo();
			_all = new PropertyInfoCollection(_id, _nazev, _uzivatel, _created, _deleted, _komunikace);
		}
		
		/// <summary>
		/// Inicializuje hodnoty metadat.
		/// </summary>
		public void Initialize(ObjectInfo objectInfo)
		{
			_id.Initialize(objectInfo, "ID", "SubjektID", true, SqlDbType.Int, false, 4);
			_nazev.Initialize(objectInfo, "Nazev", "Nazev", false, SqlDbType.NVarChar, true, 50);
			_uzivatel.Initialize(objectInfo, "Uzivatel", "UzivatelID", false, SqlDbType.Int, true, 4, typeof(Havit.BusinessLayerTest.Uzivatel), Havit.BusinessLayerTest.Uzivatel.ObjectInfo);
			_created.Initialize(objectInfo, "Created", "Created", false, SqlDbType.SmallDateTime, false, 4);
			_deleted.Initialize(objectInfo, "Deleted", "Deleted", false, SqlDbType.SmallDateTime, true, 4);
			_komunikace.Initialize(objectInfo, "Komunikace", typeof(Havit.BusinessLayerTest.Komunikace), "(SELECT CAST([_items].[KomunikaceID] AS NVARCHAR(11)) + '|' FROM [dbo].[Komunikace] AS [_items] WHERE ([_items].[SubjektID] = [dbo].[Subjekt].[SubjektID]) FOR XML PATH('')) AS [Komunikace]");
		}
		
		/// <summary>
		/// Identifikátor objektu.
		/// </summary>
		public FieldPropertyInfo ID
		{
			get
			{
				return _id;
			}
		}
		private FieldPropertyInfo _id;
		
		/// <summary>
		/// Název.
		/// </summary>
		public FieldPropertyInfo Nazev
		{
			get
			{
				return _nazev;
			}
		}
		private FieldPropertyInfo _nazev;
		
		/// <summary>
		/// Uživatel (login).
		/// </summary>
		public ReferenceFieldPropertyInfo Uzivatel
		{
			get
			{
				return _uzivatel;
			}
		}
		private ReferenceFieldPropertyInfo _uzivatel;
		
		/// <summary>
		/// Čas vytvoření objektu.
		/// </summary>
		public FieldPropertyInfo Created
		{
			get
			{
				return _created;
			}
		}
		private FieldPropertyInfo _created;
		
		/// <summary>
		/// Čas smazání objektu.
		/// </summary>
		public FieldPropertyInfo Deleted
		{
			get
			{
				return _deleted;
			}
		}
		private FieldPropertyInfo _deleted;
		
		/// <summary>
		/// Komunikace subjektu.
		/// </summary>
		public CollectionPropertyInfo Komunikace
		{
			get
			{
				return _komunikace;
			}
		}
		private CollectionPropertyInfo _komunikace;
		
		/// <summary>
		/// Všechny sloupečky typu Subjekt.
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
