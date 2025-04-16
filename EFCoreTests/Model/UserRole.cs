using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.EFCoreTests.Model;

[NotMapped]
public class UserRole
{
	public Guid UserId { get; set; }
	public int RoleId { get; set; }
}
