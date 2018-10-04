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
        /// <remarks>
        /// Pro možnost použití s Moq - Throws vyžaduje typ výjimky s bez parametrickým konstruktorem.
        /// </remarks>
        public ObjectNotFoundException()
        {
        }

		/// <summary>
		/// Konstruktor.
		/// </summary>
		/// <param name="message">Popis výjimky.</param>
		public ObjectNotFoundException(string message) : base(message)
		{
		}
	}
}
