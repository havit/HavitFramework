using System.Data.Entity;

namespace Havit.Data.Entity.CodeGenerator.Entity
{
	internal class DoNothingInitializer<TContext> : IDatabaseInitializer<TContext>
		where TContext : System.Data.Entity.DbContext
	{
		public void InitializeDatabase(TContext context)
		{
			// NOOP
		}
	}
}
