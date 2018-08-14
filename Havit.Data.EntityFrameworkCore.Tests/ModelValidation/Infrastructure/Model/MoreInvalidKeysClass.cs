namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
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
