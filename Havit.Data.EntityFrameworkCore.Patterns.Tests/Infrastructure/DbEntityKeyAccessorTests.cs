﻿using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.TestsInfrastructure;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure;

[TestClass]
public class DbEntityKeyAccessorTests
{
	[TestMethod]
	public void GetEntityKeyPropertyName_GetEntityKeyPropertyName()
	{
		// Arrange
		DbEntityKeyAccessor dbEntityKeyAccessor = new DbEntityKeyAccessor(new DbEntityKeyAccessorStorageBuilder(new TestDbContext()).Build());

		// Act + Assert
		Assert.AreEqual(nameof(Language.Id), dbEntityKeyAccessor.GetEntityKeyPropertyNames(typeof(Language)).Single());
	}

	[TestMethod]
	public void GetEntityKeyPropertyName_GetEntityKey()
	{
		// Arrange
		DbEntityKeyAccessor dbEntityKeyAccessor = new DbEntityKeyAccessor(new DbEntityKeyAccessorStorageBuilder(new TestDbContext()).Build());

		Language language = new Language() { Id = 999 };

		// Act + Assert
		Assert.AreEqual(language.Id, dbEntityKeyAccessor.GetEntityKeyValues(language).Single());
	}

}
