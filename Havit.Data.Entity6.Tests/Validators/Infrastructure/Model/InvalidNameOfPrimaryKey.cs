using System.ComponentModel.DataAnnotations;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model;

public class InvalidNameOfPrimaryKey
{
	[Key]
	public int PrimaryKey { get; set; }

	public int Id { get; set; }

	public int ExternalId { get; set; }

}
