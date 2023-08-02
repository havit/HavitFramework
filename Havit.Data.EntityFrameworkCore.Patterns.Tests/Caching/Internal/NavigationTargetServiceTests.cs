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

		NavigationTarget navigationTarget;

		// OneToMany

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(Child), nameof(Child.Parent));
		Assert.AreEqual(typeof(Master), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.Reference, navigationTarget.NavigationType);

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(Master), nameof(Master.Children));
		Assert.AreEqual(typeof(Child), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.OneToMany, navigationTarget.NavigationType);

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(Category), nameof(Category.Parent));
		Assert.AreEqual(typeof(Category), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.Reference, navigationTarget.NavigationType);

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(Category), nameof(Category.Children));
		Assert.AreEqual(typeof(Category), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.OneToMany, navigationTarget.NavigationType);

		// ManyToMany pomocí dekompozice do dvou OneToMany
		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(LoginAccount), nameof(LoginAccount.Memberships));
		Assert.AreEqual(typeof(Membership), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.ManyToManyDecomposedToOneToMany, navigationTarget.NavigationType);

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(Membership), nameof(Membership.LoginAccount));
		Assert.AreEqual(typeof(LoginAccount), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.Reference, navigationTarget.NavigationType);

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(Membership), nameof(Membership.Role));
		Assert.AreEqual(typeof(Role), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.Reference, navigationTarget.NavigationType);

		// ManyToMany
		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(ClassManyToManyA), nameof(ClassManyToManyA.Items));
		Assert.AreEqual(typeof(ClassManyToManyB), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.ManyToMany, navigationTarget.NavigationType);

		// OneToOne
		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(ClassOneToOneA), nameof(ClassOneToOneA.ClassB));
		Assert.AreEqual(typeof(ClassOneToOneB), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.OneToOne, navigationTarget.NavigationType);

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(ClassOneToOneB), nameof(ClassOneToOneB.ClassA));
		Assert.AreEqual(typeof(ClassOneToOneA), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.Reference, navigationTarget.NavigationType);

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(ClassOneToOneC), nameof(ClassOneToOneC.Indirect));
		Assert.AreEqual(typeof(ClassOneToOneC), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.OneToOne, navigationTarget.NavigationType);

		navigationTarget = navigationTargetService.GetNavigationTarget(typeof(ClassOneToOneC), nameof(ClassOneToOneC.Direct));
		Assert.AreEqual(typeof(ClassOneToOneC), navigationTarget.TargetClrType);
		Assert.AreEqual(NavigationType.Reference, navigationTarget.NavigationType);

		// A nic víc!
		Assert.AreEqual(12, navigationTargetTypeStorage.Value.Count());
	}
}
