namespace Havit.Data.Entity.Migrations
{
	/// <summary>
	/// Konfigurace code first migrations.
	/// Stardardně vypíná automatické migrace (AutomaticMigrationsEnabled i AutomaticMigrationDataLossAllowed).
	/// </summary>
	public class DbMigrationsConfiguration<TContext> : System.Data.Entity.Migrations.DbMigrationsConfiguration<TContext>
		where TContext : DbContext
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		public DbMigrationsConfiguration()
		{
			AutomaticMigrationsEnabled = false;
			AutomaticMigrationDataLossAllowed = false;
		}
	}
}
