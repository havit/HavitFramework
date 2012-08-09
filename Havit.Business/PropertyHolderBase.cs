using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Pøedek generického typu <see cref="PropertyHolder{T}"/>. 
	/// Potøebujeme kolekci PropertyHolderù a kolekci generických typù nelze udìlat.
	/// </summary>
	public abstract class PropertyHolderBase
	{
		/// <summary>
		/// Zruší pøíznak zmìny hodnoty, hodnota se poté chová jako nezmìnìná.
		/// </summary>
		internal abstract void ClearDirty();
	}
}
