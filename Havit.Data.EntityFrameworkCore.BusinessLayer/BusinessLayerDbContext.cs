using System.Collections.Generic;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Infrastructure;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using DbContext = Havit.Data.EntityFrameworkCore.DbContext;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer
{
    public class BusinessLayerDbContext : DbContext
	{
	    public BusinessLayerDbContext()
		{
		}

		public BusinessLayerDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseCodeMigrationsInfrastructure();
			optionsBuilder.UseDbInjections();
			optionsBuilder.UseSqlServerExtendedProperties();
		}

		/// <inheritdoc />
		protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
		{
			base.CustomizeModelCreating(modelBuilder);

			modelBuilder.ForSqlServerExtendedPropertiesAttributes();
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
			yield return new DefaultsForStringsConvention();
			yield return new NamespaceExtendedPropertyConvention();
			yield return new CollectionExtendedPropertiesConvention();
			yield return new XmlCommentsForDescriptionPropertyConvention();
			yield return new SequencesForEnumClassesConvention(); 
			yield return new BusinessLayerIndexesConventions();
            yield return new CharColumnTypeForCharPropertyConvention();
		}

	    protected void RegisterDbInjections(ModelBuilder modelBuilder, Assembly injectionsAssembly = default)
	    {
            modelBuilder.ForDbInjections(this.GetService<IDbInjectionAnnotationProvider>(), injectionsAssembly ?? Assembly.GetCallingAssembly());
	    }
	}
}
