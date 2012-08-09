using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Business
{
	/// <summary>
	/// Výjimka reprezentující porušení business pravidla.
	/// </summary>
	public class ConstraintViolationException: ApplicationException
	{
		/// <summary>
		/// Vytvoøí instanci výjimky.
		/// </summary>
		/// <param name="businessObject">Business object, ve kterém došlo k porušení pravidla.</param>
		/// <param name="message">Popis výjimky.</param>
		public ConstraintViolationException(BusinessObjectBase businessObject, string message)
			: this(businessObject, message, null)
		{
		}

		/// <summary>
		/// Vytvoøí instanci výjimky.
		/// </summary>
		/// <param name="businessObject">Business object, ve kterém došlo k porušení pravidla.</param>
		/// <param name="message">Popis výjimky.</param>
		/// <param name="innerException">Vnoøená výjimka.</param>
		public ConstraintViolationException(BusinessObjectBase businessObject, string message, Exception innerException): base(message, innerException)
		{
			this.businessObject = businessObject;
		}

		/// <summary>
		/// BusinessObject, ve kterém došlo k porušení pravidla.
		/// </summary>
		public BusinessObjectBase BusinessObject
		{
			get { return businessObject; }
		}
		private BusinessObjectBase businessObject;

	}
}
