using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Business;

/// <summary>
/// Oznamuje změnu vlastnosti vč. hodnot.
/// </summary>
public class PropertyChangedEventArgs : System.ComponentModel.PropertyChangedEventArgs
{
	/// <summary>
	/// Původní hodnota. 
	/// </summary>
	public object OldValue { get; }

	/// <summary>
	/// Nová hodnota.
	/// </summary>
	public object NewValue { get; }

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public PropertyChangedEventArgs(string propertyName, object oldValue, object newValue) : base(propertyName)
	{
		this.OldValue = oldValue;
		this.NewValue = newValue;
	}
}
