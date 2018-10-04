using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model
{
	public class MoreInvalidKeysClass
	{
		[Key]
		[Column(Order = 1)]
		public int Id1 { get; set; }

		[Key]
		[Column(Order = 2)]
		public int Id2 { get; set; }
	}
}
