using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;

/// <summary>
/// Priorita položky v cache.
/// </summary>
public enum CacheItemPriority
{
	/// <summary>
	/// Low.
	/// </summary>
	Low,

	/// <summary>
	/// Normal.
	/// </summary>
	Normal,

	/// <summary>
	/// High.
	/// </summary>
	High,

	/// <summary>
	/// NotRemovable.
	/// </summary>
	NotRemovable
}
