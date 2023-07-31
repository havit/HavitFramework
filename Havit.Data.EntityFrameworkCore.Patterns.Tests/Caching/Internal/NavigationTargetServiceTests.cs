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
			new NavigationTarget { TargetClrType = typeof(Master), NavigationType = NavigationType.Reference },
			navigationTargetService.GetNavigationTarget(typeof(Child), nameof(Child.Parent)));

		Assert.AreEqual(
			new NavigationTarget { TargetClrType = typeof(Child), NavigationType = NavigationType.OneToMany },
			navigationTargetService.GetNavigationTarget(typeof(Master), nameof(Master.Children)));

		// ManyToMany pomocí dekompozice do dvou OneToMany
		Assert.AreEqual(
			new NavigationTarget { TargetClrType = typeof(Membership), NavigationType = NavigationType.ManyToManyDecomposedToOneToMany },
			navigationTargetService.GetNavigationTarget(typeof(LoginAccount), nameof(LoginAccount.Memberships)));

		Assert.AreEqual(
			new NavigationTarget { TargetClrType = typeof(LoginAccount), NavigationType = NavigationType.Reference },
			navigationTargetService.GetNavigationTarget(typeof(Membership), nameof(Membership.LoginAccount)));

		Assert.AreEqual(
			new NavigationTarget { TargetClrType = typeof(Role), NavigationType = NavigationType.Reference },
			navigationTargetService.GetNavigationTarget(typeof(Membership), nameof(Membership.Role)));

		// ManyToMany
		Assert.AreEqual(
			new NavigationTarget { TargetClrType = typeof(ClassManyToManyB), NavigationType = NavigationType.ManyToMany },
			navigationTargetService.GetNavigationTarget(typeof(ClassManyToManyA), nameof(ClassManyToManyA.Items)));

		// OneToOne
		Assert.AreEqual(
			new NavigationTarget { TargetClrType = typeof(ClassOneToOneB), NavigationType = NavigationType.OneToOne },
			navigationTargetService.GetNavigationTarget(typeof(ClassOneToOneA), nameof(ClassOneToOneA.ClassB)));

		Assert.AreEqual(
			new NavigationTarget { TargetClrType = typeof(ClassOneToOneA), NavigationType = NavigationType.Reference },
			navigationTargetService.GetNavigationTarget(typeof(ClassOneToOneB), nameof(ClassOneToOneB.ClassA)));

		// A nic víc!
		Assert.AreEqual(8, navigationTargetTypeStorage.Value.Count());
	}
}
