using System;
using System.Data;

namespace Havit.Data.Patterns.Exceptions
{
	/// <summary>
	/// Výjimka reprezentuje chybu, kdy se nepodaří nalézt záznam. 
	/// </summary>
	public class ObjectNotFoundException : DataException
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="message">Popis výjimky.</param>
		public ObjectNotFoundException(string message) : base(message)
		{
		}
	}
}
