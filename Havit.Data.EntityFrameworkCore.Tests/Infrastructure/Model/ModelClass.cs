using System.ComponentModel.DataAnnotations;

namespace Havit.Data.Entity.Tests.Infrastructure.Model
{
	public class ModelClass
	{
		public int Id { get; set; }
		
		[MaxLength(100)]
		public string Name { get; set; }
	}
}
