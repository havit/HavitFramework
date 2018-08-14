namespace Havit.Data.Entity.Tests.ModelValidation.Infrastructure.Model
{
    public class UserRoleMembership
    {
	    /// <summary>
	    /// Součástí primárního klíče (viz ModelValidationDbContext).
	    /// </summary>
		public int UserId { get; set; }
	    public User User { get; set; }

	    /// <summary>
	    /// Součástí primárního klíče (viz ModelValidationDbContext).
		/// </summary>
		public int RoleId { get; set; }
	    public Role Role { get; set; }
    }
}
