using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.Security.Permissions;
using Havit.Diagnostics.Contracts;

namespace Havit.Business;

/// <summary>
/// Výjimka reprezentující porušení business pravidla.
/// </summary>
[Serializable]
public class ConstraintViolationException : Exception, ISerializable
{
	/// <summary>
	/// BusinessObject, ve kterém došlo k porušení pravidla.
	/// </summary>
	public BusinessObjectBase BusinessObject
	{
		get { return _businessObject; }
	}
	private readonly BusinessObjectBase _businessObject;

	/// <summary>
	/// Vytvoří instanci výjimky
	/// </summary>
	public ConstraintViolationException()		
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
		Contract.Requires<ArgumentNullException>(info != null, nameof(info));

		_businessObject = (BusinessObjectBase)info.GetValue("BusinessObject", typeof(BusinessObjectBase));
	}

	/// <summary>
	/// Vrátí data pro serializaci výjimky.
	/// </summary>
	/// <param name="info">data výjimky</param>
	/// <param name="context">context serializace</param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
	{
		Contract.Requires<ArgumentNullException>(info != null, nameof(info));

		base.GetObjectData(info, context);

		info.AddValue("BusinessObject", _businessObject);
	}
}
