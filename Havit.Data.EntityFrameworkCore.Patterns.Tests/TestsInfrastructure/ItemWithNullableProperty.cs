namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure
{
	public class ItemWithNullableProperty
	{
		public int Id { get; set; }
		public int RequiredValue { get; set; }
		public int? NullableValue { get; set; }
	}
}
