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
	/// Třída reprezentující peněžní částky s měnou.
	/// </summary>
	[System.CodeDom.Compiler.GeneratedCode("Havit.BusinessLayerGenerator", "1.0")]
	[System.Diagnostics.DebuggerDisplay("{GetType().FullName,nq} (Amount={Amount, nq}, Currency={Currency == null ? \"null\" : Currency.ID.ToString(), nq})")]
	public class MoneyBase : Havit.Business.MoneyImplementationBase<Currency, Money>
	{
		/// <summary>
		/// Inicializuje třídu money s prázdními hodnotami (Amount i Currency jsou null).
		/// </summary>
		public MoneyBase() : base()
		{
		}
		
		/// <summary>
		/// Inicializuje třídu money zadanými hodnotami.
		/// </summary>
		public MoneyBase(decimal? amount, Currency currency) : base(amount, currency)
		{
		}
	}
}
