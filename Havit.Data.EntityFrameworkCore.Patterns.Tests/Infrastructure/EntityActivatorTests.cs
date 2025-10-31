using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure;

[TestClass]
public class EntityActivatorTests
{
	[TestMethod]
	public void EntityActivator_CreateInstance_PublicConstructor()
	{
		// Act + Assert
		Assert.IsNotNull(EntityActivator.CreateInstance<ClassWithPublicConstuctor>());
	}

	[TestMethod]
	public void EntityActivator_CreateInstance_ParametirezedPublicConstructor()
	{
		// Assert
		Assert.ThrowsExactly<MissingMemberException>(() =>
		{
			// Act
			Assert.IsNotNull(EntityActivator.CreateInstance<ClassWithParametirezedPublicConstuctor>());
		});
	}

	[TestMethod]
	public void EntityActivator_CreateInstance_PrivateConstructor()
	{
		// Act + Assert
		Assert.IsNotNull(EntityActivator.CreateInstance<ClassWithPrivateConstuctor>());
	}

	private class ClassWithPublicConstuctor
	{
		// Default parameterless constructor
		// NOOP
	}

	private class ClassWithParametirezedPublicConstuctor
	{
		public ClassWithParametirezedPublicConstuctor(object _)
		{
			// NOOP
		}
	}

	private class ClassWithPrivateConstuctor
	{
		private ClassWithPrivateConstuctor()
		{
			// NOOP
		}
	}
}
