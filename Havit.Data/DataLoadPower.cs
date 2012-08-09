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
		Ghosts,

		/// <summary>
		/// Datový zdroj obsahuje kompletní øádek dat (všechny možné sloupce).
		/// </summary>
		FullLoad,

		/// <summary>
		/// Datový zdroj obsahuje nekompletní øádek dat.
		/// </summary>
		PartialLoad
	}
}
