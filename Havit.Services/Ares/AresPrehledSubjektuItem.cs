using System;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Obálka pro data z odpovìdi ze služeb ARES - pøehled ekonomických subjektù, pøehled osob.
	/// </summary>
	public class AresPrehledSubjektuItem
	{
		/// <summary>
		/// IÈO obchodní firmy zapsané v OR.
		/// </summary>
		public string Ico { get; set; }

		/// <summary>
		/// Název osoby nebo ekonomického subjektu
		/// </summary>
		public string Nazev { get; set; }

		/// <summary>
		/// Adresa/Kontaktní osoba subjektu
		/// </summary>
		public string Kontakt { get; set; }
	}
}
