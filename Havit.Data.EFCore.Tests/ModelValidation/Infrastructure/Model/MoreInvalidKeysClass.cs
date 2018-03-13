using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model
{
	public class MoreInvalidKeysClass
	{
		/// <summary>
		/// Součástí primárního klíče (viz ModelValidationDbContext).
		/// </summary>
		public int Id1 { get; set; }

		/// <summary>
		/// Součástí primárního klíče (viz ModelValidationDbContext).
		/// </summary>
		public int Id2 { get; set; }
	}
}
