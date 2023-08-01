namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class ExternalIdWithNoForeignKey
{
	public int Id { get; set; }
	public int SomethinhWithExternalId { get; set; } // is not a foreign key and it is correct
}
