using System;
using System.Collections.Generic;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Výsledek s daty z odpovìdi ze služeb ARES - pøehled ekonomických subjektù, pøehled osob.
	/// </summary>
	public class AresPrehledSubjektuResult
	{
		/// <summary>
		/// Nalezeno pøíliš mnoho výsledkù
		/// </summary>
		public bool PrilisMnohoVysledku { get; set; }

		/// <summary>
		/// Data vyhledaných subjektù
		/// </summary>
		public List<AresPrehledSubjektuItem> Data { get; set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public AresPrehledSubjektuResult()
		{
			Data = new List<AresPrehledSubjektuItem>();
			PrilisMnohoVysledku = false;
		}
	}
}