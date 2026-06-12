using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class EntryWithGuidPrimaryKeyAndWithSymbol
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	[MaxLength(50)]
	public string Symbol { get; set; }

	public enum Entry
	{
		One, Two, Three
	}
}
