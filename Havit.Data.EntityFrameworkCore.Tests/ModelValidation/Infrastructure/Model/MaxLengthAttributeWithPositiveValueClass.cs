using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class MaxLengthAttributeWithPositiveValueClass
	{
		public int Id { get; set; }

		[MaxLength(500)]
		public string Value { get; set; }
	}
}
