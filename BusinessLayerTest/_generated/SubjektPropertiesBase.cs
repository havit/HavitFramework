//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Havit.Data;
using Havit.Data.SqlClient;
using Havit.Data.SqlTypes;
using Havit.Business;
using Havit.Business.Query;

namespace Havit.BusinessLayerTest
{
	public class SubjektPropertiesBase
	{
		public SubjektPropertiesBase()
		{
			_id = new FieldPropertyInfo();
			_nazev = new FieldPropertyInfo();
			_uzivatel = new ReferenceFieldPropertyInfo();
			_created = new FieldPropertyInfo();
			_deleted = new FieldPropertyInfo();
			_all = new PropertyInfoCollection(_id, _nazev, _uzivatel, _created, _deleted);
		}
		
		public void Initialize(ObjectInfo objectInfo)
		{
			_id.Initialize(objectInfo, "ID", "SubjektID", true, SqlDbType.Int, false, 4);
			_nazev.Initialize(objectInfo, "Nazev", "Nazev", false, SqlDbType.NVarChar, true, 50);
			_uzivatel.Initialize(objectInfo, "Uzivatel", "UzivatelID", false, SqlDbType.Int, true, 4, typeof(Havit.BusinessLayerTest.Uzivatel), Havit.BusinessLayerTest.Uzivatel.ObjectInfo);
			_created.Initialize(objectInfo, "Created", "Created", false, SqlDbType.SmallDateTime, false, 4);
			_deleted.Initialize(objectInfo, "Deleted", "Deleted", false, SqlDbType.SmallDateTime, true, 4);
		}
		
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
