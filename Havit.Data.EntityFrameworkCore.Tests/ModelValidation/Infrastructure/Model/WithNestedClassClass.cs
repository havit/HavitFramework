namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class WithNestedClassClass
	{
		public int Id { get; set; }

		public class NestedClass
		{
			public int Id { get; set; }
		}
	}
}
