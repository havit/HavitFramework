using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model
{
	public class StringIdClass
	{
		[Key]
		public string Id { get; set; }
	}
}
