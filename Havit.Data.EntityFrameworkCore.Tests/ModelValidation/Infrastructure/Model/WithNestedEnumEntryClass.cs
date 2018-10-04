namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class WithNestedEnumEntryClass
	{
		public int Id { get; set; }

		public enum Entry
		{
			One, Two, Three
		}
	}
}
