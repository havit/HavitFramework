﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Havit.Data.EntityFrameworkCore.Patterns.DataSeeds;
using Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.DataSeeds
{
	[TestClass]
	public class DbDataSeedPersisterTests
	{		
		[TestMethod]
		public void DbDataSeedPersister_GetPropertiesForInserting_EntityWithAutogeneratedKey()
		{
			// Arrange
			var dbContext = new DbDataSeedPersisterTestsDbContext();
			var persister = new DbDataSeedPersister(dbContext);
			
			// Act
			var result = persister.GetPropertiesForInserting(dbContext.Model.FindEntityType(typeof(ItemWithAutogeneratedKey)));

			//Assert
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.Any(item => item.Name == nameof(ItemWithAutogeneratedKey.Symbol)));
			Assert.IsFalse(result.Any(item => item.Name == nameof(ItemWithAutogeneratedKey.Id))); // automaticky generované, nebudeme hodnotu nastavovat
		}

		[TestMethod]
		public void DbDataSeedPersister_GetPropertiesForInserting_EntityWithoutAutogeneratedKey()
		{
			// Arrange
			var dbContext = new DbDataSeedPersisterTestsDbContext();
			var persister = new DbDataSeedPersister(dbContext);

			// Act
			var result = persister.GetPropertiesForInserting(dbContext.Model.FindEntityType(typeof(ItemWithoutAutogeneratedKey)));

			//Assert
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result.Any(item => item.Name == nameof(ItemWithoutAutogeneratedKey.Value)));
			Assert.IsTrue(result.Any(item => item.Name == nameof(ItemWithoutAutogeneratedKey.Id))); // není automaticky generované, budeme hodnotu nastavovat
		}

		[TestMethod]
		public void DbDataSeedPersister_GetPropertiesForUpdating_EntityWithAutogeneratedKey()
		{
			// Arrange
			var dbContext = new DbDataSeedPersisterTestsDbContext();
			var persister = new DbDataSeedPersister(dbContext);

			// Act
			var result = persister.GetPropertiesForUpdating<ItemWithAutogeneratedKey>(dbContext.Model.FindEntityType(typeof(ItemWithAutogeneratedKey)), null);

			//Assert
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.Any(item => item.Name == nameof(ItemWithAutogeneratedKey.Symbol)));
			Assert.IsFalse(result.Any(item => item.Name == nameof(ItemWithAutogeneratedKey.Id))); // automaticky generované, nebudeme hodnotu nastavovat
		}

		[TestMethod]
		public void DbDataSeedPersister_GetPropertiesForUpdating_EntityWithoutAutogeneratedKey_WithoutPairing()
		{
			// Arrange
			var dbContext = new DbDataSeedPersisterTestsDbContext();
			var persister = new DbDataSeedPersister(dbContext);

			// Act
			var result = persister.GetPropertiesForUpdating<ItemWithoutAutogeneratedKey>(dbContext.Model.FindEntityType(typeof(ItemWithoutAutogeneratedKey)), null);

			//Assert
			Assert.AreEqual(2, result.Count);
			Assert.IsTrue(result.Any(item => item.Name == nameof(ItemWithoutAutogeneratedKey.Value)));
			Assert.IsTrue(result.Any(item => item.Name == nameof(ItemWithoutAutogeneratedKey.Id))); // není automaticky generované a není součástí párovacího klíče -> budeme hodnotu nastavovat
		}

		[TestMethod]
		public void DbDataSeedPersister_GetPropertiesForUpdating_EntityWithoutAutogeneratedKey_WithPairing()
		{
			// Arrange
			var dbContext = new DbDataSeedPersisterTestsDbContext();
			var persister = new DbDataSeedPersister(dbContext);

			// Act
			var result = persister.GetPropertiesForUpdating<ItemWithoutAutogeneratedKey>(
				dbContext.Model.FindEntityType(typeof(ItemWithoutAutogeneratedKey)),
				new List<Expression<Func<ItemWithoutAutogeneratedKey, object>>> { item => item.Id });

			//Assert
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result.Any(item => item.Name == nameof(ItemWithoutAutogeneratedKey.Value)));
			Assert.IsFalse(result.Any(item => item.Name == nameof(ItemWithoutAutogeneratedKey.Id))); // není automaticky generované a je součástí párovacího klíče -> nebudeme hodnotu nastavovat
		}


		internal class DbDataSeedPersisterTestsDbContext : Havit.Data.EntityFrameworkCore.DbContext
		{
			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				base.OnConfiguring(optionsBuilder);

				optionsBuilder.UseInMemoryDatabase(nameof(DbDataSeedPersisterTests));
			}

			protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
			{
				base.CustomizeModelCreating(modelBuilder);

				modelBuilder.Entity<ItemWithAutogeneratedKey>();
				modelBuilder.Entity<ItemWithoutAutogeneratedKey>();
			}
		}

		internal class ItemWithAutogeneratedKey
		{
			public int Id { get; set; }

			public string Symbol { get; set; }
		}

		internal class ItemWithoutAutogeneratedKey
		{
			[DatabaseGenerated(DatabaseGeneratedOption.None)]
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}