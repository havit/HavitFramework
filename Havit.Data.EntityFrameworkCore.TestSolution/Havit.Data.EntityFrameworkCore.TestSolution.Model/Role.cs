using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.TestSolution.Model;

public class Role
{
	public int Id { get; set; }

	[MaxLength(100)]
	public string Name { get; set; }
}