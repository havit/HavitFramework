using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data;

/// <summary>
/// Rozsah dat v datovém zdroji.
/// </summary>
public enum DataLoadPower
{

	/// <summary>
	/// Datový zdroj obsahuje jen informace pro založení ghosta (primární klíč).
	/// </summary>
	Ghost = 0,

	/// <summary>
	/// Datový zdroj obsahuje kompletní řádek dat (všechny možné sloupce).
	/// </summary>
	FullLoad = 1,

	/// <summary>
	/// Datový zdroj obsahuje nekompletní řádek dat.
	/// </summary>
	PartialLoad = 2
}
