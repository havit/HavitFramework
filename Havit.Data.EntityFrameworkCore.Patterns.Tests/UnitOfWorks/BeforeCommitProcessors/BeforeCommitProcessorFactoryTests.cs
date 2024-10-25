using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks.BeforeCommitProcessors;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.UnitOfWorks.BeforeCommitProcessors;

[TestClass]
public class BeforeCommitProcessorsFactoryTests
{
	[TestMethod]
	public void BeforeCommitProcessorsFactory_Create_ResolvesProcessors()
	{
		// Arrange
		IServiceCollection services = new ServiceCollection();
		services.AddTransient<IBeforeCommitProcessor<Entity>>(sp => Mock.Of<IBeforeCommitProcessor<Entity>>(MockBehavior.Strict));
		services.AddTransient<IBeforeCommitProcessor<Entity>>(sp => Mock.Of<IBeforeCommitProcessor<Entity>>(MockBehavior.Strict));
		var serviceProvider = services.BuildServiceProvider();

		BeforeCommitProcessorsFactory factory = new BeforeCommitProcessorsFactory(serviceProvider);

		// Act
		var beforeCommitProcessors = factory.Create(typeof(Entity)).ToList();

		// Assert
		Assert.AreEqual(beforeCommitProcessors.Count, 2);
		Assert.IsTrue(beforeCommitProcessors.All(beforeCommitProcessor => beforeCommitProcessor.GetType().ImplementsInterface(typeof(IBeforeCommitProcessor<Entity>))));
	}

	[TestMethod]
	public void BeforeCommitProcessorsFactory_Create_ResolvesProcessorsForParentTypes()
	{
		// Arrange
		IServiceCollection services = new ServiceCollection();
		services.AddTransient<IBeforeCommitProcessor<object>>(sp => Mock.Of<IBeforeCommitProcessor<object>>(MockBehavior.Strict));
		var serviceProvider = services.BuildServiceProvider();

		BeforeCommitProcessorsFactory factory = new BeforeCommitProcessorsFactory(serviceProvider);

		// Act
		var beforeCommitProcessors = factory.Create(typeof(Entity)).ToList();

		// Assert
		Assert.AreEqual(beforeCommitProcessors.Count, 1);
		Assert.IsTrue(beforeCommitProcessors.Single().GetType().ImplementsInterface(typeof(IBeforeCommitProcessor<object>)));
	}

	[TestMethod]
	public void BeforeCommitProcessorsFactory_Create_ReturnsProcessorsFromParentsToSpecificType()
	{
		// Arrange
		IServiceCollection services = new ServiceCollection();
		services.AddTransient<IBeforeCommitProcessor<Entity>>(sp => Mock.Of<IBeforeCommitProcessor<Entity>>(MockBehavior.Strict));
		services.AddTransient<IBeforeCommitProcessor<object>>(sp => Mock.Of<IBeforeCommitProcessor<object>>(MockBehavior.Strict));
		var serviceProvider = services.BuildServiceProvider();

		BeforeCommitProcessorsFactory factory = new BeforeCommitProcessorsFactory(serviceProvider);

		// Act
		var beforeCommitProcessors = factory.Create(typeof(Entity)).ToList();

		// Assert
		Assert.AreEqual(beforeCommitProcessors.Count, 2);
		Assert.IsTrue(beforeCommitProcessors[0].GetType().ImplementsInterface(typeof(IBeforeCommitProcessor<object>)));
		Assert.IsTrue(beforeCommitProcessors[1].GetType().ImplementsInterface(typeof(IBeforeCommitProcessor<Entity>)));
	}

	public class Entity
	{

	}
}
