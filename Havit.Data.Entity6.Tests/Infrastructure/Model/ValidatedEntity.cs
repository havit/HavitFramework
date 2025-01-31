using System.ComponentModel.DataAnnotations;

namespace Havit.Data.Entity.Tests.Infrastructure.Model;

public class ValidatedEntity
{
	public int Id { get; set; }

	[Required]
	public string RequiredValue { get; set; }
}
