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
using Havit.Data.SqlServer;
using Havit.Data.SqlTypes;

namespace Havit.BusinessLayerTest
{
	/// <summary>
	/// Objektová reprezentace metadat vlastností typu Language.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public class LanguageProperties
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public LanguageProperties()
		{
			_id = new FieldPropertyInfo();
			_uICulture = new FieldPropertyInfo();
			_culture = new FieldPropertyInfo();
			_name = new FieldPropertyInfo();
			_aktivni = new FieldPropertyInfo();
			_editacePovolena = new FieldPropertyInfo();
			_poradi = new FieldPropertyInfo();
			_all = new PropertyInfoCollection(_id, _uICulture, _culture, _name, _aktivni, _editacePovolena, _poradi);
		}
		
		/// <summary>
		/// Inicializuje hodnoty metadat.
		/// </summary>
		public void Initialize(ObjectInfo objectInfo)
		{
			_id.Initialize(objectInfo, "ID", "LanguageID", true, SqlDbType.Int, false, 4);
			_uICulture.Initialize(objectInfo, "UICulture", "UICulture", false, SqlDbType.VarChar, false, 6);
			_culture.Initialize(objectInfo, "Culture", "Culture", false, SqlDbType.VarChar, false, 6);
			_name.Initialize(objectInfo, "Name", "Name", false, SqlDbType.NVarChar, true, 50);
			_aktivni.Initialize(objectInfo, "Aktivni", "Aktivni", false, SqlDbType.Bit, false, 1);
			_editacePovolena.Initialize(objectInfo, "EditacePovolena", "EditacePovolena", false, SqlDbType.Bit, false, 1);
			_poradi.Initialize(objectInfo, "Poradi", "Poradi", false, SqlDbType.Int, false, 4);
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
		/// CultrueName pro UICulture v podobě jako resources. Tedypro výchozí jazyk prázdné, &quot;en&quot; pro všechny angličtiny, &quot;en-US&quot; pro americkou angličtinu.
		/// </summary>
		public FieldPropertyInfo UICulture
		{
			get
			{
				return _uICulture;
			}
		}
		private FieldPropertyInfo _uICulture;
		
		/// <summary>
		/// CultureName v plné podobě, např. cs-CZ. Formát &quot;&lt;languagecode2&gt;-&lt;country/regioncode2&gt;&quot;.
		/// </summary>
		public FieldPropertyInfo Culture
		{
			get
			{
				return _culture;
			}
		}
		private FieldPropertyInfo _culture;
		
		/// <summary>
		/// Název pro UI
		/// </summary>
		public FieldPropertyInfo Name
		{
			get
			{
				return _name;
			}
		}
		private FieldPropertyInfo _name;
		
		/// <summary>
		/// Indikuje, zda je jazyk pro daného uživatele aktivní, či nikoliv.
		/// </summary>
		public FieldPropertyInfo Aktivni
		{
			get
			{
				return _aktivni;
			}
		}
		private FieldPropertyInfo _aktivni;
		
		/// <summary>
		/// Indikuje, zda-li je povolena editace lokalizovaných hodnot pro jazyk.
		/// </summary>
		public FieldPropertyInfo EditacePovolena
		{
			get
			{
				return _editacePovolena;
			}
		}
		private FieldPropertyInfo _editacePovolena;
		
		/// <summary>
		/// Pořadí při výpisu jazyků.
		/// </summary>
		public FieldPropertyInfo Poradi
		{
			get
			{
				return _poradi;
			}
		}
		private FieldPropertyInfo _poradi;
		
		/// <summary>
		/// Všechny sloupečky typu Language.
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
