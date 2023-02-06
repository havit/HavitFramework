using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;
using Havit.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure
{
	[TestClass]
	public class DbEntityKeyAccessorTests
	{
		[TestMethod]
		public void GetEntityKeyPropertyName_GetEntityKeyPropertyName()
		{
			// Arrange
			DbEntityKeyAccessor dbEntityKeyAccessor = new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), new TestDbContext());

			// Act + Assert
			Assert.AreEqual(nameof(Language.Id), dbEntityKeyAccessor.GetEntityKeyPropertyNames(typeof(Language)).Single());
		}

		[TestMethod]
		public void GetEntityKeyPropertyName_GetEntityKey()
		{
			// Arrange
			DbEntityKeyAccessor dbEntityKeyAccessor = new DbEntityKeyAccessor(new DbEntityKeyAccessorStorage(), new TestDbContext());

			Language language = new Language() { Id = 999 };

			// Act + Assert
			Assert.AreEqual(language.Id, dbEntityKeyAccessor.GetEntityKeyValues(language).Single());
		}

	}
}
