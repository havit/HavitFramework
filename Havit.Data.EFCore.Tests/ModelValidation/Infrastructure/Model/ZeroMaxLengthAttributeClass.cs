using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
{
	public class ZeroMaxLengthAttributeClass
	{
		public int Id { get; set; }

		[MaxLength(0)]
		public string Value { get; set; }
	}
}
