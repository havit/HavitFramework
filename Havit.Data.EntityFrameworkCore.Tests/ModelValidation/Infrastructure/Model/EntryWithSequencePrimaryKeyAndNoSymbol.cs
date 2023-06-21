using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class EntryWithSequencePrimaryKeyAndNoSymbol
{
	// ModelValidatingDbContext - HasSequence() + HasDefaultValueSql("NEXT VALUE FOR EntrySequence")
	public int Id { get; set; }

	public enum Entry
	{
		One, Two, Three
	}
}
