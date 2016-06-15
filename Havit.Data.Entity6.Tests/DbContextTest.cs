﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Data.Entity.Tests.Infrastructure.Model;
using Havit.Data.Entity.Tests.Validators.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Havit.Data.Entity.Tests
{
	[TestClass]
	public class DbContextTest
	{
		[TestMethod]
		public void DbContext_SaveChanges_CallsRegisteresAfterSaveChangesActions()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			bool called = false;

			// Act
			dbContext.RegisterAfterSaveChangesAction(() => called = true);
			dbContext.SaveChanges();

			// Assert
			Assert.IsTrue(called);
		}

		[TestMethod]
		public async Task DbContext_SaveChangesAsync_CallsRegisteresAfterSaveChangesActions()
		{
			// Arrange
			EmptyDbContext dbContext = new EmptyDbContext();
			bool called = false;

			// Act
			dbContext.RegisterAfterSaveChangesAction(() => called = true);
			await dbContext.SaveChangesAsync();

			// Assert
			Assert.IsTrue(called);
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

	}
}
