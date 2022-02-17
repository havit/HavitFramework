using Microsoft.EntityFrameworkCore.Storage;

namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal
{
    /// <inheritdoc />
    public class DbDataSeedContext : IDbDataSeedContext
    {
        /// <inheritdoc />
        public IDbContextTransaction CurrentTransaction { get; set; }
    }
}