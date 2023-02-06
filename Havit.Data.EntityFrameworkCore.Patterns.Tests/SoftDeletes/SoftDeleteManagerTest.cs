using System;
using Havit.Data.EntityFrameworkCore.Patterns.SoftDeletes;
using Havit.Services.TimeServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.SoftDeletes
{
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

		public class NullableDateTimeDeleted
		{
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
}
