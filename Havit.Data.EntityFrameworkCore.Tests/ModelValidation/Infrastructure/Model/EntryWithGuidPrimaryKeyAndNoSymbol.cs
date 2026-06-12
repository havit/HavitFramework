using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class EntryWithGuidPrimaryKeyAndNoSymbol
{
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public Guid Id { get; set; }

	public enum Entry
	{
		One, Two, Three
	}
}
