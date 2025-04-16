using System.ComponentModel.DataAnnotations.Schema;
using Havit.Data.EntityFrameworkCore.Attributes;

namespace Havit.EFCoreTests.Model;

[Cache]
public class Role
{
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int Id { get; set; }

	public string Name { get; set; }
}
