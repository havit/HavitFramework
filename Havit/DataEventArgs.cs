using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit
{
	/// <summary>
	/// Argumenty události DataEventHandler, kterými si vyvolání události a její obsluha mohou předávat data (a to jedním i druhým směrem).
	/// </summary>
	/// <typeparam name="T">Typ předávaných dat.</typeparam>
	public class DataEventArgs<T> : EventArgs
	{
		/// <summary>
		/// Datový objekt, který se předává mezi volajícím a volaným.
		/// </summary>
		public T Data { get; set; }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DataEventArgs()
		{

		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DataEventArgs(T data)
		{
			this.Data = data;
		}
	}
}
