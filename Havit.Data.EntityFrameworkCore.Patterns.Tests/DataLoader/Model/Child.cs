using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataLoader.Model
{
	public class Child
	{
		public int Id { get; set; }

		// Reference
		public Master Parent { get; set; }
		public int? ParentId { get; set; }

		public DateTime? Deleted { get; set; }
	}
}