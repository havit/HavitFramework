using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;

internal class DataWithPairByValues<TEntity>
	where TEntity : class
{
	public TEntity OriginalItem { get; set; }
	public PairByValues PairByValues { get; set; }
}
