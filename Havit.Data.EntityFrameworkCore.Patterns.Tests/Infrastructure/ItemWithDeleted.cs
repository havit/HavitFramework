using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure
{
	public class ItemWithDeleted
	{
		public int Id { get; set; }
		public DateTime? Deleted { get; set; }
		public string Symbol { get; set; }
	}
}