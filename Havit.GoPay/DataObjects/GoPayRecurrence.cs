using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.GoPay.Codebooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Havit.GoPay.DataObjects
{
	/// <summary>
	/// Nastavení opakované platby
	/// </summary>
	public class GoPayRecurrence
	{
		/// <summary>
		/// Časový úsek opakování
		/// </summary>
		[JsonProperty("recurrence_cycle")]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPayRecurrenceCycle Cycle { get; set; }

		/// <summary>
		/// Perioda opakování opakované platby
		/// </summary>
		[JsonProperty("recurrence_period", NullValueHandling = NullValueHandling.Ignore)]
		public long? Period { get; set; }

		/// <summary>
		/// Doba platnosti opakované platby
		/// </summary>
		[JsonProperty("recurrence_date_to")]
		[JsonConverter(typeof(RecurrenceDateConverter))]
		public DateTime DateTo { get; set; }

		/// <summary>
		/// Stav opakované platby
		/// </summary>
		[JsonProperty("recurrence_state", NullValueHandling = NullValueHandling.Ignore)]
		[JsonConverter(typeof(StringEnumConverter))]
		public GoPayRecurrenceState? State { get; set; }
	}
}
