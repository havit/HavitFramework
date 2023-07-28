using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.ManyToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.ManyToManyAsTwoOneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure.Model.OneToOne;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal;

[TestClass]
public class NavigationTargetServiceTests
{
	[TestMethod]
	public void NavigationTargetService_GetNavigationTarget()
	{
		// Arrange
		var navigationTargetTypeStorage = new NavigationTargetStorage();
		var dbContext = new CachingTestDbContext();

		var navigationTargetService = new NavigationTargetService(navigationTargetTypeStorage, dbContext);

		// Act & Assert

		// OneToMany
		Assert.AreEqual(
			new NavigationTarget { Type = typeof(Master), IsCollection = false },
			navigationTargetService.GetNavigationTarget(typeof(Child), nameof(Child.Parent)));

		Assert.AreEqual(
			new NavigationTarget { Type = typeof(Child), IsCollection = true },
			navigationTargetService.GetNavigationTarget(typeof(Master), nameof(Master.Children)));

		// ManyToMany pomocí dekompozice do dvou OneToMany
		Assert.AreEqual(
			new NavigationTarget { Type = typeof(Membership), IsCollection = true },
			navigationTargetService.GetNavigationTarget(typeof(LoginAccount), nameof(LoginAccount.Memberships)));

		Assert.AreEqual(
			new NavigationTarget { Type = typeof(LoginAccount), IsCollection = false },
			navigationTargetService.GetNavigationTarget(typeof(Membership), nameof(Membership.LoginAccount)));

		Assert.AreEqual(
			new NavigationTarget { Type = typeof(Role), IsCollection = false },
			navigationTargetService.GetNavigationTarget(typeof(Membership), nameof(Membership.Role)));

		// ManyToMany
		Assert.AreEqual(
			new NavigationTarget { Type = typeof(ClassManyToManyB), IsCollection = true },
			navigationTargetService.GetNavigationTarget(typeof(ClassManyToManyA), nameof(ClassManyToManyA.Items)));

		// OneToOne
		Assert.AreEqual(
			new NavigationTarget { Type = typeof(ClassOneToOneB), IsCollection = false },
			navigationTargetService.GetNavigationTarget(typeof(ClassOneToOneA), nameof(ClassOneToOneA.ClassB)));

		Assert.AreEqual(
			new NavigationTarget { Type = typeof(ClassOneToOneA), IsCollection = false },
			navigationTargetService.GetNavigationTarget(typeof(ClassOneToOneB), nameof(ClassOneToOneB.ClassA)));

		// A nic víc!
		Assert.AreEqual(8, navigationTargetTypeStorage.Value.Count());
	}
}
