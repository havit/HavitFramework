using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Security.Permissions;

namespace Havit.Business
{
	/// <summary>
	/// Výjimka reprezentující porušení business pravidla.
	/// </summary>
	[Serializable]
	public class ConstraintViolationException : Exception, ISerializable
	{
		#region BusinessObject
		/// <summary>
		/// BusinessObject, ve kterém došlo k porušení pravidla.
		/// </summary>
		public BusinessObjectBase BusinessObject
		{
			get { return _businessObject; }
		}
		private BusinessObjectBase _businessObject;
		#endregion

		#region Constructors
		/// <summary>
		/// Vytvoří instanci výjimky
		/// </summary>
		public ConstraintViolationException()
			: base()
		{
		}
		
		/// <summary>
		/// Vytvoří instanci výjimky.
		/// </summary>
		/// <param name="businessObject">Business object, ve kterém došlo k porušení pravidla.</param>
		/// <param name="message">Popis výjimky.</param>
		public ConstraintViolationException(BusinessObjectBase businessObject, string message)
			: this(businessObject, message, null)
		{
		}

		/// <summary>
		/// Vytvoří instanci výjimky.
		/// </summary>
		/// <param name="businessObject">Business object, ve kterém došlo k porušení pravidla.</param>
		/// <param name="message">Popis výjimky.</param>
		/// <param name="innerException">Vnořená výjimka.</param>
		public ConstraintViolationException(BusinessObjectBase businessObject, string message, Exception innerException)
			: base(message, innerException)
		{
			this._businessObject = businessObject;
		}

		/// <summary>
		/// Vytvoří instanci výjimky.
		/// </summary>
		/// <param name="message">Popis výjimky.</param>
		public ConstraintViolationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Vytvoří instanci výjimky.
		/// </summary>
		/// <param name="message">Popis výjimky.</param>
		/// <param name="innerException">Vnořená výjimka.</param>
		public ConstraintViolationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Vytvoří instanci výjimky deserializací.
		/// </summary>
		/// <param name="info">data výjimky</param>
		/// <param name="context">context serializace</param>
		protected ConstraintViolationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			_businessObject = (BusinessObjectBase)info.GetValue("BusinessObject", typeof(BusinessObjectBase));
		}
		#endregion

		#region ISerializable
		/// <summary>
		/// Vrátí data pro serializaci výjimky.
		/// </summary>
		/// <param name="info">data výjimky</param>
		/// <param name="context">context serializace</param>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			base.GetObjectData(info, context);

			info.AddValue("BusinessObject", _businessObject);
		}
		#endregion
	}
}
