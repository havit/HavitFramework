using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Caching.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Havit.Services.TimeServices;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups;

/// <summary>
/// Test bázové třídy je realizován prostřednictvím potomka UzivatelLookupService.
/// </summary>
[TestClass]
public class LookupServiceBaseTests
{
	[TestMethod]
	public void LookupServiceBase_GetEntityByLookupKey_ReturnsWhenFound()
	{
		// Arrange
		var uzivatel1 = new Uzivatel { Id = 1, Email = "email1@havit.cz" };
		var uzivatel2 = new Uzivatel { Id = 2, Email = "email2@havit.cz" };
		var uzivatel3 = new Uzivatel { Id = 3, Email = "email3@havit.cz" };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel1, uzivatel2, uzivatel3 };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act
		Uzivatel uzivatel = uzivatelLookupService.GetUzivatelByEmail(uzivatel2.Email);

		// Assert
		Assert.AreSame(uzivatel2, uzivatel);
	}

	[TestMethod]
	public void LookupServiceBase_GetEntityByLookupKey_IgnoresDeletedEntitesWhenNotIncluded()
	{
		// Arrange
		var uzivatel = new Uzivatel { Id = 1, Email = "email1@havit.cz", Deleted = DateTime.Now };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act + Assert			
		Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail(uzivatel.Email)); // smazaného uživatele nenajdeme
	}

	[TestMethod]
	public void LookupServiceBase_GetEntityByLookupKey_DoesNotIgnoreDeletedEntitesWhenIncluded()
	{
		// Arrange
		var uzivatel = new Uzivatel { Id = 1, Email = "email1@havit.cz", Deleted = DateTime.Now };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);
		uzivatelLookupService.SetIncludeDeleted(true); // chceme i smazané uživatele

		// Act + Assert
		Assert.AreSame(uzivatel, uzivatelLookupService.GetUzivatelByEmail(uzivatel.Email)); // smazaného uživatele najdeme, řekli jsme si i o smazané
	}

	[TestMethod]
	public void LookupServiceBase_GetEntityByLookupKey_Filter()
	{
		// Arrange
		var uzivatel1 = new Uzivatel { Id = 1, Email = "email1@havit.cz" };
		var uzivatel2 = new Uzivatel { Id = 2, Email = "email2@havit.cz" };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel1, uzivatel2 };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);
		uzivatelLookupService.SetFilter(u => u.Id != 1); // zafiltrujeme uživatele

		// Act + Assert
		Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail(uzivatel1.Email)); // uzivatel1 neprošel filtrem
		Assert.AreSame(uzivatel2, uzivatelLookupService.GetUzivatelByEmail(uzivatel2.Email)); // uživatel2 prošel filtrem
	}

	[TestMethod]
	[ExpectedException(typeof(InvalidOperationException))]
	public void LookupServiceBase_GetEntityByLookupKey_ThrowsExceptionWhenDataContainsDuplicity()
	{
		// Arrange
		var uzivatel1 = new Uzivatel { Id = 1, Email = "email@havit.cz" }; // duplicitní email
		var uzivatel2 = new Uzivatel { Id = 2, Email = "email@havit.cz" }; // duplicitní email

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel1, uzivatel2 };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act
		uzivatelLookupService.GetUzivatelByEmail(uzivatel2.Email);

		// Assert by method attribute
	}

	[TestMethod]
	[ExpectedException(typeof(Data.Patterns.Exceptions.ObjectNotFoundException))]
	public void LookupServiceBase_GetEntityByLookupKey_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
		uzivatelLookupService.SetThrowExceptionWhenNotFound(true);

		// Act
		uzivatelLookupService.GetUzivatelByEmail("email@havit.cz"); // tento email neevidujeme

		// Assert by method attribute
	}

	[TestMethod]
	public void LookupServiceBase_GetEntityByLookupKey_ReturnNullWhenNotFound()
	{
		// Arrange
		UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
		uzivatelLookupService.SetThrowExceptionWhenNotFound(false);

		// Act + Assert
		Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail("email@havit.cz"));
	}

	[TestMethod]
	public void LookupServiceBase_GetEntityByLookupKey_InvalidateUpdatesLookupData_Insert()
	{
		// Arrange
		var uzivatel1 = new Uzivatel { Id = 1, Email = "email1@havit.cz" };
		var uzivatel2 = new Uzivatel { Id = 2, Email = "email2@havit.cz" };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel1 }; // uzivatel2 zde není
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act + Assert						
		Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail(uzivatel2.Email)); // potřebný side-effect: sestavení lookup data

		// provedeme aktualizaci uzivatele
		uzivatele.Add(uzivatel2); // pro dostupnost uživatele v Repository

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange
			{
				ChangeType = ChangeType.Insert,
				ClrType = typeof(Uzivatel),
				EntityType = null, // pro účely testu není třeba
				Entity = uzivatel2
			}
		});

		uzivatelLookupService.Invalidate(changes);

		// po aktualizaci uživatele podle nového emailu najdeme
		Assert.AreSame(uzivatel2, uzivatelLookupService.GetUzivatelByEmail(uzivatel2.Email));
	}

	[TestMethod]
	public void LookupServiceBase_GetEntityByLookupKey_InvalidateUpdatesLookupData_Update()
	{
		// Arrange
		var uzivatel1 = new Uzivatel { Id = 1, Email = "email@havit.cz" };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel1 };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act + Assert			
		Assert.AreSame(uzivatel1, uzivatelLookupService.GetUzivatelByEmail(uzivatel1.Email));

		// změníme uživateli email
		uzivatel1.Email = "another-email@havit.cz"; // změní

		// podle nového emalu uživatele nenajdeme
		Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail(uzivatel1.Email));

		// provedeme aktualizaci uzivatele
		Changes changes = new Changes(new List<Change>
		{
			new FakeChange
			{
				ChangeType = ChangeType.Update,
				ClrType = typeof(Uzivatel),
				EntityType = null, // pro účely testu není třeba
				Entity = uzivatel1
			}
		});
		uzivatelLookupService.Invalidate(changes);

		// po aktualizaci uživatele podle nového emailu již najdeme
		Assert.AreSame(uzivatel1, uzivatelLookupService.GetUzivatelByEmail(uzivatel1.Email));
	}

	[TestMethod]
	public void LookupServiceBase_GetEntityByLookupKey_InvalidateUpdatesLookupData_Delete()
	{
		// Arrange
		var uzivatel = new Uzivatel { Id = 1, Email = "email1@havit.cz" };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act + Assert			
		Assert.AreSame(uzivatel, uzivatelLookupService.GetUzivatelByEmail(uzivatel.Email)); // potřebný side-effect: sestavení lookup data

		// provedeme aktualizaci uzivatele
		uzivatele.Remove(uzivatel); // pro (ne)dostupnost uživatele Repository

		Changes changes = new Changes(new List<Change>
		{
			new FakeChange
			{
				ChangeType = ChangeType.Delete,
				ClrType = typeof(Uzivatel),
				EntityType = null, // pro účely testu není třeba
				Entity = uzivatel
			}
		});

		uzivatelLookupService.Invalidate(changes);

		// po aktualizaci uživatele podle nového emailu najdeme
		Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail(uzivatel.Email));
	}

	[TestMethod]
	public async Task LookupServiceBase_GetEntityByLookupKeyAsync_ReturnsWhenFound()
	{
		// Arrange
		var uzivatel1 = new Uzivatel { Id = 1, Email = "email1@havit.cz" };
		var uzivatel2 = new Uzivatel { Id = 2, Email = "email2@havit.cz" };
		var uzivatel3 = new Uzivatel { Id = 3, Email = "email3@havit.cz" };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel1, uzivatel2, uzivatel3 };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act
		Uzivatel uzivatel = await uzivatelLookupService.GetUzivatelByEmailAsync(uzivatel2.Email);

		// Assert
		Assert.AreSame(uzivatel2, uzivatel);
	}

	[TestMethod]
	[ExpectedException(typeof(Data.Patterns.Exceptions.ObjectNotFoundException))]
	public async Task LookupServiceBase_GetEntityByLookupKeyAsync_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
		uzivatelLookupService.SetThrowExceptionWhenNotFound(true);

		// Act
		await uzivatelLookupService.GetUzivatelByEmailAsync("email@havit.cz"); // tento email neevidujeme

		// Assert by method attribute
	}

	[TestMethod]
	public async Task LookupServiceBase_GetEntityByLookupKeyAsync_ReturnNullWhenNotFound()
	{
		// Arrange
		UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
		uzivatelLookupService.SetThrowExceptionWhenNotFound(false);

		// Act + Assert
		Assert.IsNull(await uzivatelLookupService.GetUzivatelByEmailAsync("email@havit.cz"));
	}

	[TestMethod]
	public void LookupServiceBase_GetEntitiesByLookupKeys_ReturnsWhenFound()
	{
		// Arrange
		var uzivatel1 = new Uzivatel { Id = 1, Email = "email1@havit.cz" };
		var uzivatel2 = new Uzivatel { Id = 2, Email = "email2@havit.cz" };
		var uzivatel3 = new Uzivatel { Id = 3, Email = "email3@havit.cz" };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel1, uzivatel2, uzivatel3 };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act
		List<Uzivatel> result = uzivatelLookupService.GetUzivateleByEmails(new[] { uzivatel2.Email });

		// Assert
		Assert.AreEqual(1, result.Count);
		Assert.AreSame(uzivatel2, result[0]);
	}

	[TestMethod]
	[ExpectedException(typeof(Data.Patterns.Exceptions.ObjectNotFoundException))]
	public void LookupServiceBase_GetEntitiesByLookupKeys_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
		uzivatelLookupService.SetThrowExceptionWhenNotFound(true);

		// Act
		uzivatelLookupService.GetUzivateleByEmails(new[] { "email1@havit.cz" });

		// Assert
	}

	[TestMethod]
	public void LookupServiceBase_GetEntitiesByLookupKeys_SkipWhenNotFound()
	{
		// Arrange
		UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
		uzivatelLookupService.SetThrowExceptionWhenNotFound(false);

		// Act
		var result = uzivatelLookupService.GetUzivateleByEmails(new[] { "email1@havit.cz" });

		// Assert
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task LookupServiceBase_GetEntitiesByLookupKeysAsync_ReturnsWhenFound()
	{
		// Arrange
		var uzivatel1 = new Uzivatel { Id = 1, Email = "email1@havit.cz" };
		var uzivatel2 = new Uzivatel { Id = 2, Email = "email2@havit.cz" };
		var uzivatel3 = new Uzivatel { Id = 3, Email = "email3@havit.cz" };

		List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel1, uzivatel2, uzivatel3 };
		UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

		// Act
		List<Uzivatel> result = await uzivatelLookupService.GetUzivateleByEmailsAsync(new[] { uzivatel2.Email });

		// Assert
		Assert.AreEqual(1, result.Count);
		Assert.AreSame(uzivatel2, result[0]);
	}

	[TestMethod]
	[ExpectedException(typeof(Data.Patterns.Exceptions.ObjectNotFoundException))]
	public async Task LookupServiceBase_GetEntitiesByLookupKeysAsync_ThrowsExceptionWhenNotFound()
	{
		// Arrange
		UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
		uzivatelLookupService.SetThrowExceptionWhenNotFound(true);

		// Act
		await uzivatelLookupService.GetUzivateleByEmailsAsync(new[] { "email1@havit.cz" });

		// Assert
	}

	[TestMethod]
	public async Task LookupServiceBase_GetEntitiesByLookupKeysAsync_SkipWhenNotFound()
	{
		// Arrange
		UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
		uzivatelLookupService.SetThrowExceptionWhenNotFound(false);

		// Act
		var result = await uzivatelLookupService.GetUzivateleByEmailsAsync(new[] { "email1@havit.cz" });

		// Assert
		Assert.AreEqual(0, result.Count);
	}

	private static UzivatelLookupService CreateLookupService(List<Uzivatel> uzivatele)
	{
		Mock<IRepository<Uzivatel>> uzivatelRepositoryMock = new Mock<IRepository<Uzivatel>>(MockBehavior.Strict);
		uzivatelRepositoryMock.Setup(m => m.GetObject(It.IsAny<int>())).Returns((int id) => uzivatele.Single(u => u.Id == id));
		uzivatelRepositoryMock.Setup(m => m.GetObjectAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((int id, CancellationToken _) => uzivatele.Single(u => u.Id == id));
		uzivatelRepositoryMock.Setup(m => m.GetObjects(It.IsAny<int[]>())).Returns((int[] ids) => uzivatele.Where(u => ids.Contains(u.Id)).ToList());
		uzivatelRepositoryMock.Setup(m => m.GetObjectsAsync(It.IsAny<int[]>(), It.IsAny<CancellationToken>())).ReturnsAsync((int[] ids, CancellationToken _) => uzivatele.Where(u => ids.Contains(u.Id)).ToList());

		Mock<IEntityKeyAccessor> entityKeyAccessorMock = new Mock<IEntityKeyAccessor>(MockBehavior.Strict);
		entityKeyAccessorMock.Setup(m => m.GetEntityKeyPropertyNames(typeof(Uzivatel))).Returns(new string[] { "Id" });
		entityKeyAccessorMock.Setup(m => m.GetEntityKeyValues(It.IsAny<object>())).Returns((object o) => new object[] { ((Uzivatel)o).Id });

		Mock<IDbSet<Uzivatel>> dbSetUzivatelMock = new Mock<IDbSet<Uzivatel>>(MockBehavior.Strict);
		dbSetUzivatelMock.Setup(m => m.AsQueryable(It.IsAny<string>())).Returns(uzivatele.AsQueryable());
		Mock<IDbContext> dbContextMock = new Mock<IDbContext>(MockBehavior.Strict);
		dbContextMock.Setup(m => m.Set<Uzivatel>()).Returns(dbSetUzivatelMock.Object);

		var uzivatelLookupService = new UzivatelLookupService(new DictionaryEntityLookupDataStorage(), uzivatelRepositoryMock.Object, dbContextMock.Object, entityKeyAccessorMock.Object, new SoftDeleteManager(new ServerTimeService()));
		uzivatelLookupService.SetThrowExceptionWhenNotFound(false);
		return uzivatelLookupService;
	}
}
