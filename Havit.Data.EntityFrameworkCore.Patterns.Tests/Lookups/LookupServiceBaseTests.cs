using Havit.Data.EntityFrameworkCore.Patterns.Lookups;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.UnitOfWorks;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups
{
	/// <summary>
	/// Test bázové třídy je realizován prostřednictvím potomka UzivatelLookupService.
	/// </summary>
	[TestClass]
	public class LookupServiceBaseTests
	{
		[TestMethod]
		public void LookupServiceBase_GetLookupKeyByEntityKey_ReturnsWhenFound()
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
		public void LookupServiceBase_GetLookupKeyByEntityKey_IgnoresDeletedEntitesWhenNotIncluded()
		{
			// Arrange
			var uzivatel = new Uzivatel { Id = 1, Email = "email1@havit.cz", Deleted = DateTime.Now };

			List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel };
			UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

			// Act + Assert			
			Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail(uzivatel.Email)); // smazaného uživatele nenajdeme
		}

		[TestMethod]
		public void LookupServiceBase_GetLookupKeyByEntityKey_DoesNotIgnoreDeletedEntitesWhenIncluded()
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
		public void LookupServiceBase_GetLookupKeyByEntityKey_Filter()
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
		public void LookupServiceBase_GetLookupKeyByEntityKey_ThrowsExceptionWhenDataContainsDuplicity()
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
		public void LookupServiceBase_GetLookupKeyByEntityKey_ThrowsExceptionWhenNotFound()
		{
			// Arrange
			UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
			uzivatelLookupService.SetThrowExceptionWhenNotFound(true);

			// Act
			uzivatelLookupService.GetUzivatelByEmail("email@havit.cz"); // tento email neevidujeme

			// Assert by method attribute
		}

		[TestMethod]
		public void LookupServiceBase_GetLookupKeyByEntityKey_ReturnNullWhenNotFound()
		{
			// Arrange
			UzivatelLookupService uzivatelLookupService = CreateLookupService(new List<Uzivatel>());
			uzivatelLookupService.SetThrowExceptionWhenNotFound(false);

			// Act + Assert
			Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail("email@havit.cz"));
		}

		[TestMethod]
		public void LookupServiceBase_GetLookupKeyByEntityKey_InvalidateUpdatesLookupData_Insert()
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
			var changes = new Changes
			{
				Inserts = new object[] { uzivatel2 },
				Updates = new object[] { },
				Deletes = new object[] { }
			};
			uzivatelLookupService.Invalidate(changes);

			// po aktualizaci uživatele podle nového emailu najdeme
			Assert.AreSame(uzivatel2, uzivatelLookupService.GetUzivatelByEmail(uzivatel2.Email));
		}

		[TestMethod]
		public void LookupServiceBase_GetLookupKeyByEntityKey_InvalidateUpdatesLookupData_Update()
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
			var changes = new Changes
			{
				Inserts = new object[] { },
				Updates = new object[] { uzivatel1 },
				Deletes = new object[] { }
			};
			uzivatelLookupService.Invalidate(changes);

			// po aktualizaci uživatele podle nového emailu již najdeme
			Assert.AreSame(uzivatel1, uzivatelLookupService.GetUzivatelByEmail(uzivatel1.Email));
		}

		[TestMethod]
		public void LookupServiceBase_GetLookupKeyByEntityKey_InvalidateUpdatesLookupData_Delete()
		{
			// Arrange
			var uzivatel = new Uzivatel { Id = 1, Email = "email1@havit.cz" };

			List<Uzivatel> uzivatele = new List<Uzivatel> { uzivatel };
			UzivatelLookupService uzivatelLookupService = CreateLookupService(uzivatele);

			// Act + Assert			
			Assert.AreSame(uzivatel, uzivatelLookupService.GetUzivatelByEmail(uzivatel.Email)); // potřebný side-effect: sestavení lookup data

			// provedeme aktualizaci uzivatele
			uzivatele.Remove(uzivatel); // pro (ne)dostupnost uživatele Repository
			var changes = new Changes
			{
				Inserts = new object[] { },
				Updates = new object[] { },
				Deletes = new object[] { uzivatel }
			};
			uzivatelLookupService.Invalidate(changes);

			// po aktualizaci uživatele podle nového emailu najdeme
			Assert.IsNull(uzivatelLookupService.GetUzivatelByEmail(uzivatel.Email));
		}

		private static UzivatelLookupService CreateLookupService(List<Uzivatel> uzivatele)
		{
			Mock<IRepository<Uzivatel>> uzivatelRepositoryMock = new Mock<IRepository<Uzivatel>>(MockBehavior.Strict);
			uzivatelRepositoryMock.Setup(m => m.GetObject(It.IsAny<int>())).Returns((int id) => uzivatele.Single(u => u.Id == id));

			Mock<IEntityKeyAccessor> entityKeyAccessorMock = new Mock<IEntityKeyAccessor>(MockBehavior.Strict);
			entityKeyAccessorMock.Setup(m => m.GetEntityKeyPropertyNames(typeof(Uzivatel))).Returns(new string[] { "Id" });
			entityKeyAccessorMock.Setup(m => m.GetEntityKeyValues(It.IsAny<object>())).Returns((object o) => new object[] { ((Uzivatel)o).Id });

			Mock<IDbSet<Uzivatel>> dbSetUzivatelMock = new Mock<IDbSet<Uzivatel>>(MockBehavior.Strict);
			dbSetUzivatelMock.Setup(m => m.AsQueryable(It.IsAny<string>())).Returns(uzivatele.AsQueryable());
			Mock<IDbContext> dbContextMock = new Mock<IDbContext>(MockBehavior.Strict);
			dbContextMock.Setup(m => m.Set<Uzivatel>()).Returns(dbSetUzivatelMock.Object);

			var uzivatelLookupService = new UzivatelLookupService(new EntityLookupDataStorage(), uzivatelRepositoryMock.Object, dbContextMock.Object, entityKeyAccessorMock.Object, new SoftDeleteManager(new ServerTimeService()));
			uzivatelLookupService.SetThrowExceptionWhenNotFound(false);
			return uzivatelLookupService;
		}
	}
}
