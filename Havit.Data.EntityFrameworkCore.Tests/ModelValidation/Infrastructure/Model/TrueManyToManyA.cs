using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class TrueManyToManyA
{
	public int Id { get; set; }

	public List<TrueManyToManyB> Items { get; set; }
}
