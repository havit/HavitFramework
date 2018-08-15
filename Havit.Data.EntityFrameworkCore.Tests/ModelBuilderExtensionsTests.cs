using Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Configurations;
using Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Tests
{
	[TestClass]
	public class ModelBuilderExtensionsTests
	{
		[TestMethod]
		public void ModelBuilderExtensions_RegisterModelFromAssembly()
		{
			// Arrange
			ModelBuilder modelBuilder = new ModelBuilder(new ConventionSet());
			Assert.IsNull(modelBuilder.Model.FindEntityType(typeof(ModelClass))); // ověřujeme, že vrací null (jinak je test chybně implementovaný)

			// Act
			modelBuilder.RegisterModelFromAssembly(typeof(ModelBuilderExtensionsTests).Assembly, typeof(ModelClass).Namespace);

			// Assert
			Assert.IsNotNull(modelBuilder.Model.FindEntityType(typeof(ModelClass)), "ModelClass is not a registered entity.");
			Assert.IsNull(modelBuilder.Model.FindEntityType(typeof(NotMappedClass)), "NotMappedClass is a registered entity.");
		}

		[TestMethod]
		public void ModelBuilderExtensions_ApplyConfigurationsFromAssembly()
		{
			// Arrange
			Mock<ModelBuilder> modelBuilderMock = new Mock<ModelBuilder>(MockBehavior.Strict, new ConventionSet());
			modelBuilderMock.Setup(m => m.ApplyConfiguration<ModelClass>(It.IsAny<IEntityTypeConfiguration<ModelClass>>())).Returns(modelBuilderMock.Object);

			// Act
			ModelBuilderExtensions.ApplyConfigurationsFromAssembly(modelBuilderMock.Object, typeof(ModelBuilderExtensionsTests).Assembly, typeof(ModelClassConfiguration).Namespace);
			
			// Assert
			modelBuilderMock.Verify(m => m.ApplyConfiguration<ModelClass>(It.IsAny<IEntityTypeConfiguration<ModelClass>>()), Times.Once);
		}
	}
}
