using System.ComponentModel.DataAnnotations.Schema;

namespace Havit.Data.Entity.Tests.ModelValidation.Infrastructure.Model
{
	public class EntryWithPrimaryKeyAndNoSymbol
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }

		public enum Entry
		{
			One, Two, Three
		}
	}
}
