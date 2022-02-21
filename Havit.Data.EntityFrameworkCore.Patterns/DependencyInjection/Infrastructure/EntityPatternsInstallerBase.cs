using System;
using System.Linq;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.Patterns.Caching;
using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders;
using Havit.Data.EntityFrameworkCore.Patterns.DataLoaders.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.DataSources;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.PropertyLambdaExpressions.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Repositories;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.EntityValidation;
using Havit.Data.Patterns.Attributes;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataLoaders;
using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Localizations;
using Havit.Data.Patterns.Localizations.Internal;
using Havit.Data.Patterns.Repositories;
using Havit.Data.Patterns.UnitOfWorks;
using Havit.Diagnostics.Contracts;
using Havit.Model.Localizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection.Infrastructure
{
	/// <summary>
	/// Bázová implementace <see cref="IEntityPatternsInstaller"/>u.
	/// Pro použití pro jednotlivé DI kontejnery. Chce se, aby pro každý kontejner byla práce minimální.
	/// </summary>
	public abstract class EntityPatternsInstallerBase<TEntityPatternsInstaller> 
		where TEntityPatternsInstaller : EntityPatternsInstallerBase<TEntityPatternsInstaller>
	{
		private readonly IServiceInstaller installer;
		private readonly ComponentRegistrationOptions componentRegistrationOptions;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="installer">Installer, kterým budou provedeny registrace.</param>
        /// <param name="componentRegistrationOptions">Nastavení registrací.</param>
        protected EntityPatternsInstallerBase(IServiceInstaller installer, ComponentRegistrationOptions componentRegistrationOptions)
		{
			Contract.Requires<ArgumentNullException>(installer != null);
			Contract.Requires<ArgumentNullException>(componentRegistrationOptions != null);

			this.installer = installer;
			this.componentRegistrationOptions = componentRegistrationOptions;
        }

	}
}
