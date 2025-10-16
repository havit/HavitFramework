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
	[ExpectedException(typeof(MissingMemberException))]
	public void EntityActivator_CreateInstance_ParametirezedPublicConstructor()
	{
		// Act
		Assert.IsNotNull(EntityActivator.CreateInstance<ClassWithParametirezedPublicConstuctor>());

		// Assert by method attribute
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
