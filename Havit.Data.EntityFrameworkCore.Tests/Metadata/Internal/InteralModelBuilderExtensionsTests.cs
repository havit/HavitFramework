using Havit.Data.Entity.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Tests.Metadata.Internal
{
	[TestClass]
	public class InteralModelBuilderExtensionsTests
	{
		[TestMethod]
		public void ModelBuilderExtensions_GetConventionSet()
		{
			// Arrange
			ConventionSet conventionSet = new ConventionSet();
			InternalModelBuilder modelBuilder = new ModelBuilder(conventionSet).GetInfrastructure();

			// Act
			ConventionSet result = modelBuilder.GetConventionSet();
			
			// Assert
			Assert.AreSame(conventionSet, result);
		}
	}
}
