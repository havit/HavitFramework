namespace Havit.Data.Patterns.DataSeeds;

/// <summary>
/// Výjimka hlásí chybu při asynchronním seedování dat z synchronního spuštění data seed runneru.
/// </summary>
/// <remarks>
/// Jde o chybu použití (programátorskou chybu), kterou je třeba odhalit a opravit během vývoje/ladění:
/// synchronní <see cref="IDataSeedRunner.SeedData(System.Type, bool)"/> byl použit na data seed, který přepisuje (overriduje)
/// asynchronní <c>SeedDataAsync</c> tak, že vrací nedokončený <see cref="System.Threading.Tasks.Task"/>.
/// Řešením je spouštět seedování asynchronně přes <see cref="IDataSeedRunner.SeedDataAsync(System.Type, bool, System.Threading.CancellationToken)"/>.
/// <para>
/// Pozn.: V okamžiku vyhození této výjimky je rozběhnutý (nedokončený) task seedu opuštěn bez dokončení
/// a jeho persister je uvolněn. Na chování takto opuštěného tasku se nelze spoléhat - výjimka je hlášena právě proto,
/// aby k tomuto scénáři v korektně napsané aplikaci nikdy nedošlo.
/// </para>
/// </remarks>
public class SeedAsyncFromSyncSeedDataException(string message) : InvalidOperationException(message)
{
}
