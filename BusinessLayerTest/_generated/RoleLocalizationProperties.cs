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
	/// Objektová reprezentace metadat vlastností typu RoleLocalization.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public class RoleLocalizationProperties
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public RoleLocalizationProperties()
		{
			_id = new FieldPropertyInfo();
			_role = new ReferenceFieldPropertyInfo();
			_language = new ReferenceFieldPropertyInfo();
			_nazev = new FieldPropertyInfo();
			_all = new PropertyInfoCollection(_id, _role, _language, _nazev);
		}
		
		/// <summary>
		/// Inicializuje hodnoty metadat.
		/// </summary>
		public void Initialize(ObjectInfo objectInfo)
		{
			_id.Initialize(objectInfo, "ID", "RoleLocalizationID", true, SqlDbType.Int, false, 4);
			_role.Initialize(objectInfo, "Role", "RoleID", false, SqlDbType.Int, false, 4, typeof(Havit.BusinessLayerTest.Role), Havit.BusinessLayerTest.Role.ObjectInfo);
			_language.Initialize(objectInfo, "Language", "LanguageID", false, SqlDbType.Int, false, 4, typeof(Havit.BusinessLayerTest.Language), Havit.BusinessLayerTest.Language.ObjectInfo);
			_nazev.Initialize(objectInfo, "Nazev", "Nazev", false, SqlDbType.NVarChar, false, 50);
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
		/// Lokalizovaný objekt.
		/// </summary>
		public ReferenceFieldPropertyInfo Role
		{
			get
			{
				return _role;
			}
		}
		private ReferenceFieldPropertyInfo _role;
		
		/// <summary>
		/// Jazyk lokalizovaných dat.
		/// </summary>
		public ReferenceFieldPropertyInfo Language
		{
			get
			{
				return _language;
			}
		}
		private ReferenceFieldPropertyInfo _language;
		
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
		/// Všechny sloupečky typu RoleLocalization.
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
