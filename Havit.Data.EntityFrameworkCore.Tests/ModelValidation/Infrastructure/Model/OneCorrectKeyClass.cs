using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
{
	public class OneCorrectKeyClass
	{
		[Key]
		public int Id { get; set; }

		public int ExternalId { get; set; }
	}
}
