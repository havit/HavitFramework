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

namespace Havit.BusinessLayerTest.VnitrostatniDistribuce
{
	/// <summary>
	/// Cena v tarifu pro danou hmotnost.
	/// </summary>
	public partial class TarifHmotnostItem : TarifHmotnostItemBase
	{
		#region CenaAmount
		public new decimal CenaAmount
		{
			get
			{
				return base.CenaAmount;
			}
			set
			{
				base.CenaAmount = value;
			}
		}
		#endregion

		#region CenaCurrency
		public new Currency CenaCurrency
		{
			get
			{
				return base.CenaCurrency;
			}
			set
			{
				base.CenaCurrency = value;
			}
		}
		#endregion

	}
}
