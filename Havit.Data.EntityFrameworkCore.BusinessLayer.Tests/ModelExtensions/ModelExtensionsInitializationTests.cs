using System.Linq;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ModelExtensions
{
    /// <summary>
    /// Tests for initializing Model Extensions in <see cref="BusinessLayerDbContext"/>.
    /// </summary>
    [TestClass]
    public class ModelExtensionsInitializationTests
    {
        [TestMethod]
        public void BusinessLayerDbContext_ModelExtensionsInitializedThroughConvention()
        {
            IModelExtensionAnnotationProvider annotationProvider = new StoredProcedureAnnotationProvider();
            using (var dbContext = new TestDbContext())
            {
                var modelExtensions = annotationProvider.GetModelExtensions(dbContext.Model.GetAnnotations().ToList());

                Assert.AreNotEqual(0, modelExtensions.Count);
            }
        }

        private class DummyEntity
        {
            public int Id { get; set; }
        }

        private class TestDbContext : BusinessLayerDbContext
        {
            public TestDbContext()
                : base(new DbContextOptionsBuilder<TestDbContext>().UseSqlServer("Server=Dummy").Options)
            {
            }

            protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<DummyEntity>();
            }
        }
    }
}
