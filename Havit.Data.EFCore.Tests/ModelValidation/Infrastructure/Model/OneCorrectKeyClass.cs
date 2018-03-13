using System.ComponentModel.DataAnnotations;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model
{
	public class OneCorrectKeyClass
	{
		[Key]
		public int Id { get; set; }

		public int ExternalId { get; set; }
	}
}
