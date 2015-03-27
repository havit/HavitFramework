using System;
using System.Collections.Generic;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Výsledek s daty z odpovìdi ze služeb ARES - pøehled ekonomických subjektù, pøehled osob.
	/// </summary>
	public class AresPrehledSubjektuResult
	{
		#region PrilisMnohoVysledku
		/// <summary>
		/// Nalezeno pøíliš mnoho výsledkù
		/// </summary>
		public bool PrilisMnohoVysledku { get; set; }
		#endregion

		#region Data
		/// <summary>
		/// Data vyhledaných subjektù
		/// </summary>
		public List<AresPrehledSubjektuItem> Data { get; set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AresPrehledSubjektuResult()
		{
			Data = new List<AresPrehledSubjektuItem>();
			PrilisMnohoVysledku = false;
		}
		#endregion
	}
}