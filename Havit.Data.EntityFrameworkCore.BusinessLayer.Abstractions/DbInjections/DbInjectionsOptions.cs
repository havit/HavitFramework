namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections
{
	/// <summary>
	/// Nastavenia pre DbInjections
	/// </summary>
	public class DbInjectionsOptions
	{
		/// <summary>
		/// Príznak, ktorý určuje, či migrácie scaffoldované EF Core budú obsahovať všetky anotácie pre model (AlterDatabaseOperation),
		/// alebo iba tie, ktoré boli zmenené. Štandardne je toto nastavenie zapnuté (pre sprehľadnenie kódu migrácii v prípade použia
		/// DbInjections pre uložené procedúry).
		/// </summary>
		public bool RemoveUnnecessaryStatementsForMigrationsAnnotationsForModel { get; set; } = true;
	}
}