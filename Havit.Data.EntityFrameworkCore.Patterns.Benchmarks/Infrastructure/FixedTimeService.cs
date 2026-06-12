using Havit.Services.TimeServices;

namespace Havit.Data.EntityFrameworkCore.Patterns.Benchmarks.Infrastructure;

/// <summary>
/// Vrací konstantní čas, aby cena získání aktuálního času nezkreslovala výsledky benchmarků.
/// </summary>
public class FixedTimeService : ITimeService
{
	private static readonly DateTime _currentTime = new DateTime(2026, 6, 12, 8, 0, 0);

	public DateTime GetCurrentTime() => _currentTime;

	public DateTime GetCurrentDate() => _currentTime.Date;
}
