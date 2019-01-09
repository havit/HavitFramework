using System;

namespace Havit.Data.EntityFrameworkCore
{
	/// <summary>
	/// Extension methods for <c>IDbContextFactory</c>.
	/// </summary>
	public static class DbContextFactoryExtensions
	{
		/// <summary>
		/// Executes action using DbContext from a factory. Releases the DbContext after executing the action.
		/// </summary>
		/// <param name="dbContextFactory">Service factory providing the DbContext to be used.</param>
		/// <param name="action">Action to be executed.</param>
		public static void ExecuteAction(this IDbContextFactory dbContextFactory, Action<IDbContext> action)
		{
			global::Havit.Diagnostics.Contracts.Contract.Requires(dbContextFactory != null);

			IDbContext dbContext = dbContextFactory.CreateService();
			try
			{
				action(dbContext);
			}
			finally
			{
				dbContextFactory.ReleaseService(dbContext);
			}
		}
	}
}