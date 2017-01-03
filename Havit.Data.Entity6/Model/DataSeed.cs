using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Model
{
	/// <summary>
	/// Informace o naposledy spuštěné verzi seedování dat.
	/// </summary>
	public class DataSeed
	{
		/// <summary>
		/// Identifikátor. Používá se jen s hodnotou 1.
		/// </summary>
		public int Id { get; set; }		

		/// <summary>
		/// Naposledy spuštěná verze seedování dat.
		/// </summary>
		public string Version { get; set; }
	}
}
