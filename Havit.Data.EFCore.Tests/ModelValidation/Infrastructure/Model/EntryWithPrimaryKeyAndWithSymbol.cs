using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.EFCore.Tests.ModelValidation.Infrastructure.Model
{
	public class EntryWithPrimaryKeyAndWithSymbol
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		[MaxLength(50)]
		public string Symbol { get; set; }

		public enum Entry
		{
			One, Two, Three
		}
	}
}
