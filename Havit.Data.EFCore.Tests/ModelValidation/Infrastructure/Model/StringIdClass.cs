using System.ComponentModel.DataAnnotations;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model
{
	public class StringIdClass
	{
		[Key]
		public string Id { get; set; }
	}
}
