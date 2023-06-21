using Havit.Model.Collections.Generic;
using System;
using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model;

public class Master
{
	public int Id { get; set; }

	public ICollection<Child> Children { get; set; }

	public List<Child> ChildrenIncludingDeleted { get; } = new List<Child>();

	public DateTime? Deleted { get; set; }

	public Master()
	{
		Children = new FilteringCollection<Child>(ChildrenIncludingDeleted, child => child.Deleted == null);
	}
}
