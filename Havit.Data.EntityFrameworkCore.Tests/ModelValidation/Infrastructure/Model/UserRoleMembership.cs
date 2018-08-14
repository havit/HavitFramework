using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
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
