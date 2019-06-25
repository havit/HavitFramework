using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.Migrations.DbInjections
{
    /// <summary>
    ///     Explicitly implemented by <see cref="ExtendedMigrationsExtensionBuilder" /> and <see cref="ExtendedMigrationsExtensionBuilderBase"/> to hide
    ///     methods that are used by DbInjections extension methods but not intended to be called by application
    ///     developers.
    /// </summary>
    public interface IExtendedMigrationsExtensionBuilderInfrastructure
    {
        /// <summary>
        ///     Gets the core options builder.
        /// </summary>
        DbContextOptionsBuilder OptionsBuilder { get; }
    }
}
