using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.TestSolution.Model;

public class User
{
	public int Id { get; set; }

	[MaxLength(100)]
	public string Username { get; set; }

	public List<UserRole> Roles { get; set; }

	public DateTime? Deleted { get; set; }
}