using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit
{
	/// <summary>
	/// Delegát pro událost předávající mezi volajícím a volaným datový objekt. 
	/// </summary>
	/// <typeparam name="T">Typ předávaného objektu.</typeparam>
	/// <param name="sender">Volající.</param>
	/// <param name="dataEventArgs">Argumenty události předávající datový objekt (pro jeden směr) nebo očekávající nastavení datového objektu (pro druhý směr).</param>
	public delegate void DataEventHandler<T>(object sender, DataEventArgs<T> dataEventArgs);
}
