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
	/// <param name="e">Argumenty události předávající datový objekt.</param>
	public delegate void DataEventHandler<T>(object sender, DataEventArgs<T> e);
}
