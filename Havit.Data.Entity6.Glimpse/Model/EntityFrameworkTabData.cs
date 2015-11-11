using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Glimpse.Model
{
	/// <summary>
	/// Data zobrazovaná záložkou Entity Framework (EF).
	/// </summary>
	public class EntityFrameworkTabData
	{
		/// <summary>
		/// Databázové příkazy k zobrazení.
		/// </summary>
		public List<DbCommandMessage> Commands { get; set; }
	}
}
