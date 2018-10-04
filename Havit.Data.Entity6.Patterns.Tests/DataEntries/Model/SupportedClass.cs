using System;

namespace Havit.Data.Entity.Patterns.Tests.DataEntries.Model
{
	public class SupportedClass
	{
		public int Id { get; set; }
		public string Symbol { get; set; }
		public DateTime? Deleted { get; set; }

		public enum Entry
		{
			First, Second, Third
		}
	}
}
