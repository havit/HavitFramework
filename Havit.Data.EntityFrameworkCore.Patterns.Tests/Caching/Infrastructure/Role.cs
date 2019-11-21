using Havit.Data.EntityFrameworkCore.Attributes;
using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure
{
	public class Role
	{
		public int Id { get; set; }

        public string Name { get; set; }

		public DateTime? Deleted { get; set; }
	}
}
