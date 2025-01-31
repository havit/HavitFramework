using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model;

public class EntryWithGeneratedPrimaryKeyAndWithSymbol
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }

	[MaxLength(50)]
	public string Symbol { get; set; }

	public enum Entry
	{
		One, Two, Three
	}
}
