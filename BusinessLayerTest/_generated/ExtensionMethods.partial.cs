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
	/// Rozšiřující metody.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	public static partial class ExtensionMethods
	{
		#region IEnumerable<Havit.BusinessLayerTest.CenikItemCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.CenikItemCollection z IEnumerable&lt;Havit.BusinessLayerTest.CenikItem&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.CenikItemCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.CenikItem> objects)
		{
			return new Havit.BusinessLayerTest.CenikItemCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.CurrencyCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.CurrencyCollection z IEnumerable&lt;Havit.BusinessLayerTest.Currency&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.CurrencyCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Currency> objects)
		{
			return new Havit.BusinessLayerTest.CurrencyCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.KomunikaceCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.KomunikaceCollection z IEnumerable&lt;Havit.BusinessLayerTest.Komunikace&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.KomunikaceCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Komunikace> objects)
		{
			return new Havit.BusinessLayerTest.KomunikaceCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.LanguageCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.LanguageCollection z IEnumerable&lt;Havit.BusinessLayerTest.Language&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.LanguageCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Language> objects)
		{
			return new Havit.BusinessLayerTest.LanguageCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.ObjednavkaSepsaniCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.ObjednavkaSepsaniCollection z IEnumerable&lt;Havit.BusinessLayerTest.ObjednavkaSepsani&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.ObjednavkaSepsaniCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.ObjednavkaSepsani> objects)
		{
			return new Havit.BusinessLayerTest.ObjednavkaSepsaniCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.RoleCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.RoleCollection z IEnumerable&lt;Havit.BusinessLayerTest.Role&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.RoleCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Role> objects)
		{
			return new Havit.BusinessLayerTest.RoleCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.RoleLocalizationCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.RoleLocalizationCollection z IEnumerable&lt;Havit.BusinessLayerTest.RoleLocalization&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.RoleLocalizationCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.RoleLocalization> objects)
		{
			return new Havit.BusinessLayerTest.RoleLocalizationCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.SoftDeleteWithDateTime2Collection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.SoftDeleteWithDateTime2Collection z IEnumerable&lt;Havit.BusinessLayerTest.SoftDeleteWithDateTime2&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.SoftDeleteWithDateTime2Collection ToCollection(this IEnumerable<Havit.BusinessLayerTest.SoftDeleteWithDateTime2> objects)
		{
			return new Havit.BusinessLayerTest.SoftDeleteWithDateTime2Collection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.SoftDeleteWithDateTimeOffsetCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.SoftDeleteWithDateTimeOffsetCollection z IEnumerable&lt;Havit.BusinessLayerTest.SoftDeleteWithDateTimeOffset&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.SoftDeleteWithDateTimeOffsetCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.SoftDeleteWithDateTimeOffset> objects)
		{
			return new Havit.BusinessLayerTest.SoftDeleteWithDateTimeOffsetCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.SubjektCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.SubjektCollection z IEnumerable&lt;Havit.BusinessLayerTest.Subjekt&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.SubjektCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Subjekt> objects)
		{
			return new Havit.BusinessLayerTest.SubjektCollection(objects);
		}
		#endregion
		
		#region IEnumerable<Havit.BusinessLayerTest.UzivatelCollection>.ToCollection
		/// <summary>
		/// Vytvoří Havit.BusinessLayerTest.UzivatelCollection z IEnumerable&lt;Havit.BusinessLayerTest.Uzivatel&gt;.
		/// </summary>
		public static Havit.BusinessLayerTest.UzivatelCollection ToCollection(this IEnumerable<Havit.BusinessLayerTest.Uzivatel> objects)
		{
			return new Havit.BusinessLayerTest.UzivatelCollection(objects);
		}
		#endregion
		
	}
}
