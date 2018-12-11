using System.Collections.Generic;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
	/// <summary>
	/// Bázová trieda pre <see cref="DbContext"/> používaný v Business Layer projektoch. Mal by sa používať výhradne pre správu schémy DB.
	/// 
	/// <remarks>
	/// Pridáva podporu pre extended properties a DB Injections a ich spoločnú infraštruktúru. Definuje rôzne konvencie používané na Business Layer projektoch.
	/// </remarks>
	/// </summary>
	public abstract class BusinessLayerDbContext : DbContext
	{
		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected BusinessLayerDbContext()
		{
		}

		/// <summary>
		/// Konstruktor.
		/// </summary>
		protected BusinessLayerDbContext(DbContextOptions options)
			: base(options)
		{
		}

		/// <inheritdoc />
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseCodeMigrationsInfrastructure();
			optionsBuilder.UseDbInjections();
			optionsBuilder.UseSqlServerExtendedProperties();
		}

		/// <inheritdoc />
		protected override void ModelCreatingCompleting(ModelBuilder modelBuilder)
		{
			modelBuilder.ForSqlServerExtendedPropertiesAttributes();
			base.ModelCreatingCompleting(modelBuilder);
		}

		/// <inheritdoc />
		protected override IEnumerable<IModelConvention> GetModelConventions()
		{
			foreach (var convention in base.GetModelConventions())
			{
				yield return convention;
			}

			yield return new PrefixedTablePrimaryKeysConvention();
			yield return new ForeignKeysColumnNamesConvention();
			yield return new LocalizationTablesParentEntitiesConvention();
			yield return new StringPropertiesDefaultValueConvention();
			yield return new NamespaceExtendedPropertyConvention();
			yield return new CollectionExtendedPropertiesConvention();
			yield return new XmlCommentsForDescriptionPropertyConvention();
			yield return new BusinessLayerIndexesConventions();
            yield return new CharColumnTypeForCharPropertyConvention();
		}

		/// <summary>
		/// Registruje DB injectors z <paramref name="injectionsAssembly"/>. Vyžaduje, aby v DbContexte bola zaregistrovaná služba <see cref="IDbInjectionAnnotationProvider"/> (štandardne je registrovaná v <see cref="OnConfiguring"/>.
		/// </summary>
	    protected void RegisterDbInjections(ModelBuilder modelBuilder, Assembly injectionsAssembly = default)
	    {
            modelBuilder.ForDbInjections(this.GetService<IDbInjectionAnnotationProvider>(), injectionsAssembly ?? Assembly.GetCallingAssembly());
	    }
	}
}
