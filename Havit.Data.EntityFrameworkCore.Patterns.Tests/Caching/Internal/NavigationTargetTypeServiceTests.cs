using Havit.Data.EntityFrameworkCore.Patterns.Caching.Internal;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.OneToOne;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.OneToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.ManyToMany;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal.Infrastructure.Model.ManyToManyAsTwoOneToMany;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Internal;

[TestClass]
public class NavigationTargetTypeServiceTests
{
	[TestMethod]
	public void NavigationTargetTypeService_GetNavigationTargetType()
	{
		// Arrange
		var navigationTargetTypeStorage = new NavigationTargetTypeStorage();
		var dbContext = new ReferencingNavigationsTestDbContext();

		var navigationTargetTypeService = new NavigationTargetTypeService(navigationTargetTypeStorage, dbContext);

		// Act & Assert		
		Assert.AreSame(typeof(Master), navigationTargetTypeService.GetNavigationTargetType(typeof(Child), nameof(Child.Parent)));
		Assert.AreSame(typeof(Child), navigationTargetTypeService.GetNavigationTargetType(typeof(Master), nameof(Master.Children)));

		Assert.AreSame(typeof(Membership), navigationTargetTypeService.GetNavigationTargetType(typeof(LoginAccount), nameof(LoginAccount.Memberships)));
		Assert.AreSame(typeof(LoginAccount), navigationTargetTypeService.GetNavigationTargetType(typeof(Membership), nameof(Membership.LoginAccount)));
		Assert.AreSame(typeof(Role), navigationTargetTypeService.GetNavigationTargetType(typeof(Membership), nameof(Membership.Role)));

		Assert.AreSame(typeof(ClassManyToManyB), navigationTargetTypeService.GetNavigationTargetType(typeof(ClassManyToManyA), nameof(ClassManyToManyA.Items)));

		Assert.AreSame(typeof(ClassOneToOneB), navigationTargetTypeService.GetNavigationTargetType(typeof(ClassOneToOneA), nameof(ClassOneToOneA.ClassB)));
		Assert.AreSame(typeof(ClassOneToOneA), navigationTargetTypeService.GetNavigationTargetType(typeof(ClassOneToOneB), nameof(ClassOneToOneB.ClassA)));

		Assert.AreEqual(8, navigationTargetTypeStorage.Value.Count());
	}
}
