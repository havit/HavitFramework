using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Model;

[NotMapped]
public class NotMappedClass
{
	public int Id { get; set; }

	[MaxLength(100)]
	public string Name { get; set; }
}
