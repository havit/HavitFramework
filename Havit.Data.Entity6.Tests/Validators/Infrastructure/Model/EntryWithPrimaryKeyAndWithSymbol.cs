using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model;

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
