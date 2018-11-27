using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal
{
	internal class PairExpressionWithCompilation<TEntity>
		where TEntity : class
	{
		public Expression<Func<TEntity, object>> Expression {get;set; }
		public Func<TEntity, object> CompiledLambda { get; set; }		
	}
}
