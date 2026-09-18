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
		Assert.Contains(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass)), applicationEntityTypes);
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_ExcludesSystemEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes().ToArray();

		// Assert
		Assert.DoesNotContain(modelValidatingDbContext.Model.FindEntityType(typeof(DataSeedVersion)), applicationEntityTypes);
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_ExcludesOwnedEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes().ToArray();

		// Assert
		Assert.DoesNotContain(modelValidatingDbContext.Model.FindEntityType(typeof(OwnedType)), applicationEntityTypes);
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_ExcludesKeylessEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes().ToArray();

		// Assert
		Assert.DoesNotContain(modelValidatingDbContext.Model.FindEntityType(typeof(KeylessClass)), applicationEntityTypes);
	}

	[TestMethod]
	public void ReadOnlyModelExtensions_GetApplicationEntityTypes_IncludeManyToManyEntities_IncludesManyToManyEntity()
	{
		// Arrange
		ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();

		// Act
		IReadOnlyEntityType[] applicationEntityTypes = modelValidatingDbContext.Model.GetApplicationEntityTypes(includeManyToManyEntities: true).ToArray();

		// Assert
		Assert.Contains(modelValidatingDbContext.Model.FindEntityType(typeof(UserRoleMembership)), applicationEntityTypes);
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
		Assert.Contains(modelValidatingDbContext.Model.FindEntityType(typeof(OneCorrectKeyClass)), applicationEntityTypes);
		Assert.DoesNotContain(modelValidatingDbContext.Model.FindEntityType(typeof(UserRoleMembership)), applicationEntityTypes);
	}
}
