using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.GoPay.Codebooks;
using Newtonsoft.Json;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Identfikace příjemce platby
	/// </summary>
	public class GoPayTarget
	{
		/// <summary>
		/// Typ příjemce platby
		/// </summary>
		[JsonProperty("type")]
		public GoPayTargetType Type { get; set; }

		/// <summary>
		/// Identifikátor příjemce u GoPay
		/// </summary>
		[JsonProperty("goid")]
		public long GoId { get; set; }
	}
}
