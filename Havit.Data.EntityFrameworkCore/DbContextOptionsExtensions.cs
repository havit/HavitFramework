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
		// No code right now

		return optionsBuilder;
	}
}
