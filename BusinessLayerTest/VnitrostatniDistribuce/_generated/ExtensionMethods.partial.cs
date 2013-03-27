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
	/// Rozšiřující metody.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public static partial class ExtensionMethods
	{
		#region IEnumerable<Havit.BusinessLayerTest.VnitrostatniDistribuce.TarifHmotnostItemCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.VnitrostatniDistribuce.TarifHmotnostItemCollection z IEnumerable&lt;Havit.BusinessLayerTest.VnitrostatniDistribuce.TarifHmotnostItem&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.VnitrostatniDistribuce.TarifHmotnostItemCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.VnitrostatniDistribuce.TarifHmotnostItem> objects)
		{
			return new Havit.BusinessLayerTest.VnitrostatniDistribuce.TarifHmotnostItemCollection(objects);
		}
		#endregion
		
	}
}
