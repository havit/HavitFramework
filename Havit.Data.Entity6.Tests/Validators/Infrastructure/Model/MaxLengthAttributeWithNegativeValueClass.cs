using System.ComponentModel.DataAnnotations;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model
{
	public class MaxLengthAttributeWithNegativeValueClass
	{
		public int Id { get; set; }

		[MaxLength(-1)]
		public string Value { get; set; }
	}
}
