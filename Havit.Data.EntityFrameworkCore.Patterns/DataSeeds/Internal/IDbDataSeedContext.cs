using Microsoft.EntityFrameworkCore.Storage;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal
{
     /// <summary>
    /// Context běhu seedování dat. Slouží k předání transakce z DbDataSeedRunneru do DbDataSeedPersisteru.
    /// </summary>
    public interface IDbDataSeedContext
    {
        /// <summary>
        /// Aktuální transakce definovaná DbDataSeedRunnerem a určená pro DbDataSeedPersister.
        /// Může být null.
        /// </summary>
        IDbContextTransaction CurrentTransaction { get; set; }
    }
}