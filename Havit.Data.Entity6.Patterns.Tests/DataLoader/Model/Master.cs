using System;
using System.Collections.Generic;

namespace Havit.Data.Entity.Patterns.Tests.DataLoader.Model
{
	public class Master
	{
		public int Id { get; set; }

		public ICollection<Child> Children { get; set; }

		public List<Child> ChildrenWithDeleted { get; set; }

		public DateTime? Deleted { get; set; }
	}
}
