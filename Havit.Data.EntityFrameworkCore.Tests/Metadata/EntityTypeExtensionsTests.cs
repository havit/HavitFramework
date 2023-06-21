using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Data.EntityFrameworkCore.Tests.Metadata.Infrastructure.Entity;
using Havit.Data.EntityFrameworkCore.Tests.Metadata.Infrastructure.Model;

namespace Havit.Data.EntityFrameworkCore.Tests.Metadata;

[TestClass]
public class EntityTypeExtensionsTests
{
	[TestMethod]
	public void EntityTypeExtensions_IsSystemEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act + Assert
		Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass)).IsSystemType());
	}

	[TestMethod]
	public void EntityTypeExtensions_IsManyToManyEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act + Assert
		Assert.IsTrue(modelValidatingDbContext.Model.FindEntityType(typeof(UserRoleMembership)).IsManyToManyEntity(), typeof(UserRoleMembership).Name);
		Assert.IsTrue(modelValidatingDbContext.Model.FindEntityType(typeof(GroupToGroup)).IsManyToManyEntity(), typeof(GroupToGroup).Name);
		Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(Group)).IsManyToManyEntity(), typeof(Group).Name);
		Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(User)).IsManyToManyEntity(), typeof(User).Name);
		Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(MoreInvalidKeysClass)).IsManyToManyEntity(), typeof(MoreInvalidKeysClass).Name);
	}

	[TestMethod]
	public void EntityTypeExtensions_IsKeyless()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act + Assert
		Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(GroupToGroup)).IsKeyless(), typeof(GroupToGroup).Name);
		Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(Group)).IsKeyless(), typeof(Group).Name);
		Assert.IsTrue(modelValidatingDbContext.Model.FindEntityType(typeof(KeylessClass)).IsKeyless(), typeof(KeylessClass).Name);
	}

	[TestMethod]
	public void EntityTypeExtensions_IsApplicationEntity_WhenNotExplicitlySet()
	{
		// Arrange
		ApplicationEntityTestDbContext applicationEntityTestDbContext = new ApplicationEntityTestDbContext();

		// Act + Assert
		Assert.IsTrue(applicationEntityTestDbContext.Model.FindEntityType(typeof(DefaultApplicationEntity)).IsApplicationEntity());
	}

	[TestMethod]
	public void EntityTypeExtensions_IsApplicationEntity_WhenExplicitlySetToTrue()
	{
		// Arrange
		ApplicationEntityTestDbContext applicationEntityTestDbContext = new ApplicationEntityTestDbContext();

		// Act + Assert
		Assert.IsTrue(applicationEntityTestDbContext.Model.FindEntityType(typeof(ExplicitApplicationEntity)).IsApplicationEntity());
	}

	[TestMethod]
	public void EntityTypeExtensions_IsApplicationEntity_WhenExplicitlySetToFalse()
	{
		// Arrange
		ApplicationEntityTestDbContext applicationEntityTestDbContext = new ApplicationEntityTestDbContext();

		// Act + Assert
		Assert.IsFalse(applicationEntityTestDbContext.Model.FindEntityType(typeof(NotApplicationEntity)).IsApplicationEntity());
	}

}

