using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
{
	public class StringIdClass
	{
		[Key]
		public string Id { get; set; }
	}
}
