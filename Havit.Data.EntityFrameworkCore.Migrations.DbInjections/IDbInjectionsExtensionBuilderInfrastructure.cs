using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    /// <summary>
    ///     Explicitly implemented by <see cref="DbInjectionsExtensionBuilderBase{TBuilder}" /> to hide
    ///     methods that are used by DbInjections extension methods but not intended to be called by application
    ///     developers.
    /// </summary>
    public interface IDbInjectionsExtensionBuilderInfrastructure
    {
        /// <summary>
        ///     Gets the core options builder.
        /// </summary>
        DbContextOptionsBuilder OptionsBuilder { get; }
    }
}
