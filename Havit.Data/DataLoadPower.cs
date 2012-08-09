using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data
{
	/// <summary>
	/// Rozsah dat v datovém zdroji.
	/// </summary>
	public enum DataLoadPower
	{

		/// <summary>
		/// Datový zdroj obsahuje jen informace pro založení ghosta (primární klíè).
		/// </summary>
		Ghost = 0,

		/// <summary>
		/// Datový zdroj obsahuje kompletní øádek dat (všechny možné sloupce).
		/// </summary>
		FullLoad = 1,

		/// <summary>
		/// Datový zdroj obsahuje nekompletní øádek dat.
		/// </summary>
		PartialLoad = 2
	}
}
