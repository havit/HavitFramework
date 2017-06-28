using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Data.Entity.Tests.Infrastructure.Model;
using Havit.Data.Entity.Tests.Validators.Infrastructure;
using Havit.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Tests
{
	[TestClass]
	public class DbContextTest
	{
		[TestMethod]
		public void DbContext_SaveChanges_CallsRegisteresAfterSaveChangesActionsOnlyOnce()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			int counter = 0;

			// Act + Assert
			dbContext.RegisterAfterSaveChangesAction(() => counter += 1);

			dbContext.SaveChanges();
			Assert.AreEqual(1, counter); // došlo k zaregistrované akci

			dbContext.SaveChanges();
			Assert.AreEqual(1, counter); // nedošlo k zaregistrované akci, registrace zrušena
		}

		[TestMethod]
		public async Task DbContext_SaveChangesAsync_CallsRegisteresAfterSaveChangesActionsOnlyOnce()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			int counter = 0;

			// Act + Assert
			dbContext.RegisterAfterSaveChangesAction(() => counter += 1);

			await dbContext.SaveChangesAsync();
			Assert.AreEqual(1, counter); // došlo k zaregistrované akci

			await dbContext.SaveChangesAsync();
			Assert.AreEqual(1, counter); // nedošlo k zaregistrované akci, registrace zrušena
		}

		[TestMethod]
		public void DbContext_SaveChanges_ThrownDbEntityValidationExceptionIsWrappedWithMessage()
		{
			ValidatingDbContext dbContext = new ValidatingDbContext();
			dbContext.Set<ValidatedEntity>().Add(new ValidatedEntity());
			try
			{
				dbContext.SaveChanges();
			}
			catch (DbEntityValidationException exception)
			{
				if ((exception.InnerException != null) && (exception.InnerException is DbEntityValidationException) && (((DbEntityValidationException)exception.InnerException).FormatErrorMessage() == exception.Message))
				{
					// passed
					return;
				}
			}
			Assert.Fail();
		}

		[TestMethod]
		public async Task DbContext_SaveChangesAsync_ThrownDbEntityValidationExceptionIsWrappedWithMessage()
		{
			ValidatingDbContext dbContext = new ValidatingDbContext();
			dbContext.Set<ValidatedEntity>().Add(new ValidatedEntity());
			try
			{
				await dbContext.SaveChangesAsync();
			}
			catch (DbEntityValidationException exception)
			{
				if ((exception.InnerException != null) && (exception.InnerException is DbEntityValidationException) && (((DbEntityValidationException)exception.InnerException).FormatErrorMessage() == exception.Message))
				{
					// passed
					return;
				}
			}
			Assert.Fail();
		}

		[TestMethod]
		public void DbContext_CollectionsCanHaveOnlyGetter()
		{
			// Arrange + Act
			int masterId;
			using (MasterChildDbContext dbContext = new MasterChildDbContext())
			{
				dbContext.Database.Initialize(true);
				Master master = new Master { Children = { new Child() } };
				dbContext.Set<Master>().Add(master);
				dbContext.SaveChanges();
				masterId = master.Id;
			}

			// Assert
			using (MasterChildDbContext dbContext = new MasterChildDbContext())
			{
				Master master = dbContext.Set<Master>().Include("Children").Single(item => item.Id == masterId);
				Assert.IsNotNull(master.Children);
				Assert.AreEqual(1, master.Children.Count);
			}
		}

		[TestMethod]
		public void DbContext_MigrationsDoNotCreateAnotherDatabase()
		{
			Assert.Inconclusive();

			Contract.Assert(ConfigurationManager.ConnectionStrings["Havit.Data.Entity6.Tests.TwoContructorsDbContext1"] == null, "Pro účely testu nesmí existovat connection string Havit.Data.Entity6.Tests.TwoContructorsDbContext1.");
			Contract.Assert(ConfigurationManager.ConnectionStrings["Havit.Data.Entity6.Tests.TwoContructorsDbContext2"] == null, "Pro účely testu nesmí existovat connection string Havit.Data.Entity6.Tests.TwoContructorsDbContext2.");

			// Arrange
			TwoContructorsDbContext dbContextDefault = new TwoContructorsDbContext();
			TwoContructorsDbContext dbContextSecondary = new TwoContructorsDbContext("Havit.Data.Entity6.Tests.TwoContructorsDbContext2");
			System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<TwoContructorsDbContext, Migrations.Configuration>());
			Contract.Assert(dbContextDefault.Database.Connection.Database == "Havit.Data.Entity6.Tests.TwoContructorsDbContext1", "Výchozí konstruktor má nastavit Havit.Data.Entity6.Tests.TwoContructorsDbContext1");

			// Act
			dbContextDefault.Database.Delete();

			//System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<MasterChildDbContext, Migrations.Configuration>());
			dbContextSecondary.Database.Initialize(true);

			bool defaultCreated = dbContextDefault.Database.Exists();
			bool secondaryCreated = dbContextSecondary.Database.Exists();

			// Clean-up
			dbContextDefault.Database.Delete();
			dbContextSecondary.Database.Delete();

			// Assert
			Assert.IsFalse(defaultCreated, "Havit.Data.Entity6.Tests.TwoContructorsDbContext1 databáze založena.");
			Assert.IsTrue(secondaryCreated, "Havit.Data.Entity6.Tests.TwoContructorsDbContext2 databáze nezaložena.");
		}
	}
}
