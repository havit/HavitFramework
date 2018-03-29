using System;

namespace Havit.Data.Entity.Patterns.Tests.Infrastructure
{
	public class ItemWithDeleted
	{
		public int Id { get; set; }
		public DateTime? Deleted { get; set; }
	}
}