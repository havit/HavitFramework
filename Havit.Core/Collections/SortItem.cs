using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace Havit.Collections;

/// <summary>
/// Represents a sort item.
/// </summary>
[Serializable]
[DataContract]
public class SortItem
{
	/// <summary>
	/// The expression used for sorting.
	/// </summary>
	[DataMember(Order = 1)]
	public virtual string Expression { get; set; }

	/// <summary>
	/// The sort direction.
	/// </summary>
	[DataMember(Order = 2)]
	public virtual SortDirection Direction { get; set; }

	/// <summary>
	/// Creates an empty sort item instance.
	/// </summary>
	public SortItem()
	{
	}

	/// <summary>
	/// Creates a sort item based on the expression and sort direction.
	/// </summary>
	public SortItem(string expression, SortDirection direction)
		: this()
	{
		this.Expression = expression;
		this.Direction = direction;
	}
}
