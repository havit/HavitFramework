using Havit.Data.EntityFrameworkCore.Tests.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Tests;

[TestClass]
public class DbContextTests
{
	public TestContext TestContext { get; set; }

	/// <summary>
	/// Ověřuje počet volání metody AfterSaveChanges po SaveChanges.
	/// Cílem je ověřit, zda je správně ošetřeno volání SaveChanges(bool acceptAllChangesOnSuccess) z SaveChanges().
	/// </summary>
	[TestMethod]
	public void DbContext_SaveChanges_CallsAfterSaveChangesOnlyOnce()
	{
		// Arrange
		Mock<EmptyDbContext> dbContextMock1 = new Mock<EmptyDbContext>();
		dbContextMock1.CallBase = true;

		Mock<EmptyDbContext> dbContextMock2 = new Mock<EmptyDbContext>();
		dbContextMock2.CallBase = true;

		// Act
		dbContextMock1.Object.SaveChanges();
		dbContextMock2.Object.SaveChanges(true);

		// Assert
		dbContextMock1.Verify(m => m.AfterSaveChanges(), Times.Once, "dbContextMock1");
		dbContextMock2.Verify(m => m.AfterSaveChanges(), Times.Once, "dbContextMock2");
	}

	/// <summary>
	/// Ověřuje počet volání metody AfterSaveChanges po SaveChangesAsync.
	/// Cílem je ověřit, zda je správně ošetřeno volání SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken) z SaveChangesAsync(CancellationToken cancellationToken).
	/// </summary>
	[TestMethod]
	public async Task DbContext_SaveChangesAsync_CallsAfterSaveChangesOnlyOnce()
	{
		// Arrange
		Mock<EmptyDbContext> dbContextMock1 = new Mock<EmptyDbContext>();
		dbContextMock1.CallBase = true;

		Mock<EmptyDbContext> dbContextMock2 = new Mock<EmptyDbContext>();
		dbContextMock2.CallBase = true;

		// Act
		await dbContextMock1.Object.SaveChangesAsync(TestContext.CancellationToken);
		await dbContextMock2.Object.SaveChangesAsync(true, TestContext.CancellationToken);

		// Assert
		dbContextMock1.Verify(m => m.AfterSaveChanges(), Times.Once, "dbContextMock1");
		dbContextMock2.Verify(m => m.AfterSaveChanges(), Times.Once, "dbContextMock2");
	}

	/// <summary>
	/// Ověřuje počet volání registrované akce po SaveChanges, cílem je, aby nebyla registrovaná akce spuštěna opakovaně (z více volání SaveChanges).
	/// </summary>
	[TestMethod]
	public void DbContext_AfterSaveChanges_CallsRegisteresAfterSaveChangesActionsOnlyOnce()
	{
		// Arrange
		EmptyDbContext dbContext = new EmptyDbContext();
		int counter = 0;

		// Act + Assert
		dbContext.RegisterAfterSaveChangesAction(() => counter += 1);

		dbContext.AfterSaveChanges();
		Assert.AreEqual(1, counter); // došlo k zaregistrované akci

		dbContext.AfterSaveChanges();
		Assert.AreEqual(1, counter); // nedošlo k zaregistrované akci, registrace zrušena
	}

	/// <summary>
	/// Ověřuje, že akce, která vyhodí výjimku, není při dalším volání AfterSaveChanges spuštěna znovu.
	/// </summary>
	[TestMethod]
	public void DbContext_AfterSaveChanges_DoesNotRepeatActionWhenActionThrows()
	{
		// Arrange
		EmptyDbContext dbContext = new EmptyDbContext();
		int counter = 0;
		dbContext.RegisterAfterSaveChangesAction(() =>
		{
			counter += 1;
			throw new InvalidOperationException();
		});

		// Act + Assert
		Assert.ThrowsExactly<InvalidOperationException>(() => dbContext.AfterSaveChanges());
		Assert.AreEqual(1, counter); // došlo k zaregistrované akci

		dbContext.AfterSaveChanges();
		Assert.AreEqual(1, counter); // nedošlo k opakovanému spuštění akce, registrace zrušena (přestože akce skončila výjimkou)
	}

	/// <summary>
	/// Ověřuje, že akce může zaregistrovat další akci - ta je spuštěna až po dalším SaveChanges (resp. AfterSaveChanges).
	/// </summary>
	[TestMethod]
	public void DbContext_AfterSaveChanges_SupportsRegisteringActionFromAction()
	{
		// Arrange
		EmptyDbContext dbContext = new EmptyDbContext();
		int counter = 0;
		dbContext.RegisterAfterSaveChangesAction(() =>
		{
			dbContext.RegisterAfterSaveChangesAction(() => counter += 10);
			counter += 1;
		});

		// Act + Assert
		dbContext.AfterSaveChanges();
		Assert.AreEqual(1, counter); // proběhla jen vnější akce, vnořeně registrovaná akce ještě ne

		dbContext.AfterSaveChanges();
		Assert.AreEqual(11, counter); // vnořeně registrovaná akce proběhla po dalším (After)SaveChanges
	}

	/// <summary>
	/// Ověřuje, že při selhání SaveChanges zůstávají registrované akce zachovány a provedou se (jednorázově) po nejbližším úspěšném SaveChanges.
	/// </summary>
	[TestMethod]
	public void DbContext_SaveChanges_KeepsRegisteredActionsWhenSaveChangesFails()
	{
		// Arrange
		ToggleThrowOnSaveDbContext dbContext = new ToggleThrowOnSaveDbContext();
		int counter = 0;
		dbContext.RegisterAfterSaveChangesAction(() => counter += 1);

		// Act + Assert
		dbContext.ThrowOnSave = true;
		Assert.ThrowsExactly<DbUpdateException>(() => dbContext.SaveChanges());
		Assert.AreEqual(0, counter); // uložení selhalo, akce neproběhla

		dbContext.ThrowOnSave = false;
		dbContext.SaveChanges();
		Assert.AreEqual(1, counter); // akce proběhla po úspěšném uložení

		dbContext.SaveChanges();
		Assert.AreEqual(1, counter); // akce neproběhla opakovaně
	}

	/// <summary>
	/// Ověřuje, že dispose DbContextu zahodí neprovedené registrované akce.
	/// Chrání použití poolovaného DbContextu - neprovedené akce nesmí přežít vrácení instance do poolu (EF custom stav odvozeného DbContextu neresetuje).
	/// </summary>
	[TestMethod]
	public void DbContext_Dispose_DiscardsRegisteredAfterSaveChangesActions()
	{
		// Arrange
		EmptyDbContext dbContext = new EmptyDbContext();
		int counter = 0;
		dbContext.RegisterAfterSaveChangesAction(() => counter += 1);

		// Act
		dbContext.Dispose();
		dbContext.AfterSaveChanges();

		// Assert
		Assert.AreEqual(0, counter); // akce byla dispose zahozena, neproběhla
	}
}
