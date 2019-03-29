using System;
using System.Collections.Generic;

namespace Havit.Services.Ares
{
    /// <summary>
    /// Výsledek s daty z odpovědi ze služeb ARES - přehled ekonomických subjektů, přehled osob.
    /// </summary>
    public class AresPrehledSubjektuResult
    {
        /// <summary>
        /// Nalezeno příliš mnoho výsledků
        /// </summary>
        public bool PrilisMnohoVysledku { get; set; }

        /// <summary>
        /// Data vyhledaných subjektů
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