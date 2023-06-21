using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.GoPay.DataObjects;

/// <summary>
/// Go pay exception
/// </summary>
public class GoPayResponseException : ApplicationException
{
	/// <summary>
	/// Konstruktor
	/// </summary>
	/// <param name="message">Text vyjímky</param>
	/// <param name="innerException">Vnitřní vyjímka</param>
	public GoPayResponseException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
