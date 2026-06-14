using Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Entity;
using Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Havit.Data.EntityFrameworkCore.Tests;

[TestClass]
public class ModelBuilderExtensionsTests
{
	[TestMethod]
	public void ModelBuilderExtensions_RegisterModelFromAssembly()
	{
		// Arrange
		ConventionSet conventionSet = ((IInfrastructure<IServiceProvider>)new EmptyDbContext()).GetInfrastructure().GetService<IConventionSetBuilder>().CreateConventionSet();
		ModelBuilder modelBuilder = new ModelBuilder(conventionSet);
		Assert.IsNull(modelBuilder.Model.FindEntityType(typeof(ModelClass))); // ověřujeme, že vrací null (jinak je test chybně implementovaný)

		// Act
		modelBuilder.RegisterModelFromAssembly(typeof(ModelBuilderExtensionsTests).Assembly, typeof(ModelClass).Namespace);
		var model = modelBuilder.FinalizeModel();

		// Assert
		Assert.IsNotNull(model.FindEntityType(typeof(ModelClass)), "ModelClass is not a registered entity.");
		Assert.IsNull(model.FindEntityType(typeof(NotMappedClass)), "NotMappedClass is a registered entity.");
		Assert.IsNull(model.FindEntityType(typeof(ComplexClass)), "ComplexClass is a registered entity.");
		Assert.IsNull(model.FindEntityType(typeof(StaticClass)), "StaticClass is a registered entity.");
		Assert.IsNotNull(model.FindEntityType(typeof(KeylessClass)), "KeylessClass is not a registered entity.");
		Assert.IsNull(model.FindEntityType(typeof(KeylessClass)).FindPrimaryKey(), "KeylessClass is not a keyless entity.");
		Assert.IsNotNull(model.FindEntityType(typeof(OwnedClass)), "OwnedClass is not a registered entity.");
		Assert.IsTrue(model.FindEntityType(typeof(OwnedClass)).IsOwned(), "OwnedClass is not an owned entity.");
	}

}
