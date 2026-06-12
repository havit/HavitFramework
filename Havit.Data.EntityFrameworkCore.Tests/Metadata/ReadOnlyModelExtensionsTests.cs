using Havit.Data.EntityFrameworkCore.Metadata;
using Havit.Data.EntityFrameworkCore.Model;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure;
using Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Havit.Data.EntityFrameworkCore.Tests.Metadata;

[TestClass]
public class ReadOnlyModelExtensionsTests
{
	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_IncludesApplicationEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes().ToArray();

		// Assert
		CollectionAssert.Contains(applicationEntityTypes, modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass)));
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_ExcludesSystemEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes().ToArray();

		// Assert
		CollectionAssert.DoesNotContain(applicationEntityTypes, modelValidatingDbContext.Model.FindEntityType(typeof(DataSeedVersion)));
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_ExcludesOwnedEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes().ToArray();

		// Assert
		CollectionAssert.DoesNotContain(applicationEntityTypes, modelValidatingDbContext.Model.FindEntityType(typeof(OwnedType)));
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_ExcludesKeylessEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes().ToArray();

		// Assert
		CollectionAssert.DoesNotContain(applicationEntityTypes, modelValidatingDbContext.Model.FindEntityType(typeof(KeylessClass)));
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_IncludeManyToManyEntities_IncludesManyToManyEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: true).ToArray();

		// Assert
		CollectionAssert.Contains(applicationEntityTypes, modelValidatingDbContext.Model.FindEntityType(typeof(UserRoleMembership)));
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_ExcludeManyToManyEntities_ExcludesManyToManyEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: false).ToArray();

		// Assert
		// Běžná aplikační entita zůstává, M:N vztahová entita je vyloučena.
		CollectionAssert.Contains(applicationEntityTypes, modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass)));
		CollectionAssert.DoesNotContain(applicationEntityTypes, modelValidatingDbContext.Model.FindEntityType(typeof(UserRoleMembership)));
	}
}
