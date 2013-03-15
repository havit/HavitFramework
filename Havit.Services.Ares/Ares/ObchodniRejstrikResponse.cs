using System;

namespace Havit.Services.Ares
{
	/// <summary>
	/// Strongtypová obálko pro data z odpovìdi ze služby ARES - Obchodní rejstøík.
	/// </summary>
	public class ObchodniRejstrikResponse
	{
		/// <summary>
		/// IÈO obchodní firmy zapsané v OR.
		/// </summary>
		public string Ico { get; set; }

		/// <summary>
		/// DIÈ obchodní firmy zapsané v OR.
		/// </summary>
		public string Dic { get; set; }

		/// <summary>
		/// Název pod kterým je firma zapsaná v OR.
		/// </summary>
		public string NazevObchodniFirmy { get; set; }

		/// <summary>
		/// Den zápisu firmy do OR.
		/// </summary>
		public DateTime DenZapisu { get; set; }

		/// <summary>
		/// Název soudu kterým je firma registrovaná v OR.
		/// </summary>
		public string NazevSoudu { get; set; }

		/// <summary>
		/// Kód soudu kterým je firma registrovaná v OR.
		/// </summary>
		public string KodSoudu { get; set; }

		/// <summary>
		/// Spisová znaèka pod kterou je firma v OR vedena (oddíl + vložka).
		/// </summary>
		public string SpisovaZnacka { get; set; }

		/// <summary>
		/// Právní forma firmy.
		/// </summary>
		public string PravniForma { get; set; }

		/// <summary>
		/// Stav subjektu v OR.
		/// </summary>
		public string StavSubjektu { get; set; }

		/// <summary>
		/// Ulice sídla firmy.
		/// </summary>
		public string SidloUlice { get; set; }

		/// <summary>
		/// Popisné èíslo sídla firmy.
		/// </summary>
		public string SidloCisloPopisne { get; set; }

		/// <summary>
		/// Orientaèní èíslo sídla firmy.
		/// </summary>
		public string SidloCisloOrientacni { get; set; }

		/// <summary>
		/// Mìsto sídla firmy.
		/// </summary>
		public string SidloMesto { get; set; }

		/// <summary>
		/// Mìstký èást sídla firmy.
		/// </summary>
		public string SidloMestskaCast { get; set; }

		/// <summary>
		/// PSÈ sídla firmy.
		/// </summary>
		public string SidloPsc { get; set; }

		/// <summary>
		/// Stát sídla firmy.
		/// </summary>
		public string SidloStat { get; set; }

		/// <summary>
		/// Chybová zpráva odpovìdi služby ARES.
		/// </summary>
		public string ResponseErrorMessage { get; set; }

		/// <summary>
		/// Indikuje, zda-li se vyskytla v odpovìdi služby ARES - Obchodní rejstøík  chyba.
		/// </summary>
		public bool HasError
		{
			get { return !String.IsNullOrWhiteSpace(ResponseErrorMessage); }

		}
	}
}
