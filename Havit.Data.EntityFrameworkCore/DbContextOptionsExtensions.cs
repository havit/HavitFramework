using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore;

/// <summary>
/// Extensions pro konfigurace DbContextOptions.
/// </summary>
public static class DbContextOptionsExtensions
{
	/// <summary>
	/// Zaregistruje k DbContextu výchozí konvence.
	/// </summary>
	public static DbContextOptionsBuilder UseDefaultHavitConventions(this DbContextOptionsBuilder optionsBuilder)
	{
		// TODO EF Core: Předělat na konvence?
		optionsBuilder.UseFrameworkConventions();
		optionsBuilder.UseDbLockedMigrator();

		return optionsBuilder;
	}
}
