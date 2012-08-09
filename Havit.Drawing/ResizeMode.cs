using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Drawing
{
	/// <summary>
	/// Režimy pro ImageExt.Resize()
	/// </summary>
	public enum ResizeMode
	{
		/// <summary>
		/// Upraví rozmìry obrázku tak, aby se jeho vnìjší obrys vešel do požadovaných rozmìrù. Pomìr stran zachová.
		/// </summary>
		PreserveAspectRatioFitBox = 1,

		/// <summary>
		/// Pokud je obrázek vìtší než požadované rozmìry, je redukován tak, aby se jeho vnìjší rozmìr vešel do požadovaného boxu.
		/// Pomìr stran je zachován. Pokud je obrázek menší, není zvìtšen, ale zùstává nezmìnìn.
		/// </summary>
		PreserveAspectRatioFitBoxReduceOnly = 2,

		/// <summary>
		/// Obrázek se upraví na pøesnì požadované rozmìry. V pøípadì potøeby je natáhnut, nemusí být zachován pomìr stran.
		/// </summary>
		AdjustToBox = 3
	}
}
