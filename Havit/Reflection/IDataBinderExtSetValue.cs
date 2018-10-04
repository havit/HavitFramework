using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Reflection
{
	/// <summary>
	/// Interface, kterým se označuje typ, jehož DataBinderExt.SetValue má fungovat nestandardním způsobem.
	/// Určeno pro nastavení hodnoty business object kolekcím, jejich setter nechceme volat, ale chceme provést vyčištění a naplnění kolekce.
	/// </summary>
	public interface IDataBinderExtSetValue
	{
		/// <summary>
		/// Nastaví objektu hodnotu z parametru.
		/// </summary>
		void SetValue(object value);
	}
}
