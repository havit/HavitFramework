using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;
using Havit.Data.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation
{
	[TestClass]
	public class EntityTypeExtensionsTests
	{			
		[TestMethod]
		public void EntityTypeExtensions_IsSystemEntity()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

			// Act + Assert
			Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass)).IsSystemEntity());
		}

		[TestMethod]
		public void EntityTypeExtensions_IsManyToManyEntity()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

			// Act + Assert
			Assert.IsTrue(modelValidatingDbContext.Model.FindEntityType(typeof(UserRoleMembership)).IsManyToManyEntity(), typeof(UserRoleMembership).Name);
			Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(User)).IsManyToManyEntity(), typeof(User).Name);
			Assert.IsFalse(modelValidatingDbContext.Model.FindEntityType(typeof(MoreInvalidKeysClass)).IsManyToManyEntity(), typeof(MoreInvalidKeysClass).Name);
		}
	}
}

