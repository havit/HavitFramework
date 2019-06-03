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

namespace Havit.BusinessLayerTest.Resources
{
	/// <summary>
	/// Rozšiřující metody.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public static partial class ExtensionMethods
	{
		#region IEnumerable<Havit.BusinessLayerTest.Resources.ResourceClassCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.Resources.ResourceClassCollection z IEnumerable&lt;Havit.BusinessLayerTest.Resources.ResourceClass&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.Resources.ResourceClassCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Resources.ResourceClass> objects)
		{
			return new Havit.BusinessLayerTest.Resources.ResourceClassCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.Resources.ResourceItemCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.Resources.ResourceItemCollection z IEnumerable&lt;Havit.BusinessLayerTest.Resources.ResourceItem&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.Resources.ResourceItemCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Resources.ResourceItem> objects)
		{
			return new Havit.BusinessLayerTest.Resources.ResourceItemCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection z IEnumerable&lt;Havit.BusinessLayerTest.Resources.ResourceItemLocalization&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Resources.ResourceItemLocalization> objects)
		{
			return new Havit.BusinessLayerTest.Resources.ResourceItemLocalizationCollection(objects);
		}
		#endregion
		
	}
}
