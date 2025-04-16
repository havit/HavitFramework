using Havit.Data.EntityFrameworkCore.Attributes;

namespace Havit.EFCoreTests.Model;

[Cache]
public class User
{
	public Guid Id { get; set; }
	public string Username { get; set; } = new Guid().ToString();

	public List<Role> PrimaryRoles { get; set; }

	public List<Role> SecondaryRoles { get; set; }

	public List<Role> AdditionalRoles { get; set; }
	public List<UserRole> AdditionalUserRoles { get; set; }

	public override string ToString()
	{
		return "User.Id=" + Id;
	}
}
