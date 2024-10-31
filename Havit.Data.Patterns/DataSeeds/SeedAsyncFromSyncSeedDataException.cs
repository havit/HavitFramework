namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Výjimka hlásí chybu při asynchronním seedování dat z synchronního spuštění data seed runneru.
/// </summary>
public class SeedAsyncFromSyncSeedDataException(string message) : InvalidOperationException(message)
{
}
