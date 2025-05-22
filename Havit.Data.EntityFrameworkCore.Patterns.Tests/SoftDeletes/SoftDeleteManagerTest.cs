using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Services.TimeServices;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.SoftDeletes;

[TestClass]
public class SoftDeleteManagerTest
{
	[TestMethod]
	public void SoftDeleteManager_NullableDateTimeIsSupported()
	{
		// Arrange
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);

		// Act
		bool softDeleteSupported = softDeleteManager.IsSoftDeleteSupported<NullableDateTimeDeleted>();

		// Assert
		Assert.IsTrue(softDeleteSupported);
	}

	[TestMethod]
	public void SoftDeleteManager_DateTimeIsNotSupported()
	{
		// Arrange
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);

		// Act
		bool softDeleteSupported = softDeleteManager.IsSoftDeleteSupported<DateTimeDeleted>();

		// Assert
		Assert.IsFalse(softDeleteSupported);
	}

	[TestMethod]
	public void SoftDeleteManager_BooleanIsNotSupported()
	{
		// Arrange
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);

		// Act
		bool softDeleteSupported = softDeleteManager.IsSoftDeleteSupported<BooleanDeleted>();

		// Assert
		Assert.IsFalse(softDeleteSupported);
	}

	[TestMethod]
	public void SoftDeleteManager_SetDeleted_SetsDateTime()
	{
		// Arrange
		DateTime currentDateTime = new DateTime(2015, 1, 1, 23, 59, 59); // prostě nějaký čas, hodnota je náhodná
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		mockTimeSevice.Setup(m => m.GetCurrentTime()).Returns(currentDateTime);
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);
		NullableDateTimeDeleted entity = new NullableDateTimeDeleted();

		// Act
		softDeleteManager.SetDeleted(entity);

		// Assert
		Assert.AreEqual(currentDateTime, entity.Deleted);
	}

	[TestMethod]
	public void SoftDeleteManager_SetDeleted_DoesNotUpdateDateTime()
	{
		// Arrange
		DateTime currentDateTime = new DateTime(2015, 1, 1, 23, 59, 59); // prostě nějaký čas, hodnota je náhodná			
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		mockTimeSevice.Setup(m => m.GetCurrentTime()).Returns(currentDateTime);

		DateTime deletedDateTime = new DateTime(2001, 1, 1);
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);
		NullableDateTimeDeleted entity = new NullableDateTimeDeleted();
		entity.Deleted = deletedDateTime;

		// Act
		softDeleteManager.SetDeleted(entity);

		// Assert
		Assert.AreEqual(deletedDateTime, entity.Deleted);
	}

	[TestMethod]
	public void SoftDeleteManager_SetNotDeleted_ClearsDeleted()
	{
		// Arrange
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);
		NullableDateTimeDeleted entity = new NullableDateTimeDeleted();
		entity.Deleted = new DateTime(2001, 1, 1);

		// Act
		softDeleteManager.SetNotDeleted(entity);

		// Assert
		Assert.IsNull(entity.Deleted);
	}

	[TestMethod]
	[ExpectedException(typeof(NotSupportedException))]
	public void SoftDeleteManager_SetDeleted_ThrowsExceptionOnNotSopportedType()
	{
		// Arrange
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);
		object unsupportedType = new object();

		// Act
		softDeleteManager.SetDeleted(unsupportedType);

		// Assert by method attribute 
	}

	[TestMethod]
	[ExpectedException(typeof(NotSupportedException))]
	public void SoftDeleteManager_SetNotDeleted_ThrowsExceptionOnNotSopportedType()
	{
		// Arrange
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);
		object unsupportedType = new object();

		// Act
		softDeleteManager.SetNotDeleted(unsupportedType);

		// Assert by method attribute 
	}

	/// <summary>
	/// See https://github.com/dotnet/efcore/issues/35059
	/// </summary>
	[TestMethod]
	public void SoftDeleteManager_ContainsWorkaroundForIssue35059()
	{
		// Arrange
		Mock<ITimeService> mockTimeSevice = new Mock<ITimeService>();
		SoftDeleteManager softDeleteManager = new SoftDeleteManager(mockTimeSevice.Object);

		// UseSqlServer: Potřebujeme nastavit database provider pro tvorbu sql dotazů.
		using var dbContext = new SoftDeleteManagerDbContext(new DbContextOptionsBuilder().UseSqlServer("Data Source=FAKE").Options);

		// Precondition
		// Testujeme, zda máme správně tento unit test. Tedy ověřujeme, jak se do query projeví,
		// pokud máme v dotazu nesplnitelnou podmínku (což je project chyby dle issue 35059).		
		bool ContainsImpossibleCondition(string query) => query.Contains("WHERE 0 = 1");

		string queryPrecondition = dbContext.NullableDateTimeDeletedDbSet.Where(item => false).ToQueryString();
		Assert.IsTrue(ContainsImpossibleCondition(queryPrecondition));

		// Act
		string query = dbContext.NullableDateTimeDeletedDbSet
			.WhereNotDeleted(softDeleteManager) // zde doplníme podmínku na Deleted
			.Where(x => x.Deleted == null) // a zde ji doplníme ještě jednou, což je předmět dané issue
			.ToQueryString();

		// Assert
		Assert.IsFalse(ContainsImpossibleCondition(query));
	}

	public class SoftDeleteManagerDbContext : DbContext
	{
		public DbSet<NullableDateTimeDeleted> NullableDateTimeDeletedDbSet { get; set; }

		public SoftDeleteManagerDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
		{
			// noop
		}
	}

	public class NullableDateTimeDeleted
	{
		public int Id { get; set; }
		public DateTime? Deleted { get; set; }
	}

	public class DateTimeDeleted
	{
		public DateTime Deleted { get; set; }
	}

	public class BooleanDeleted
	{
		public bool Deleted { get; set; }
	}
}