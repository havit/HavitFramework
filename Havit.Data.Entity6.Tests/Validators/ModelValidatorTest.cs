using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.MappingAPI;
using EntityFramework.MappingAPI.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;
using Havit.Data.Entity.Validators;
using Havit.Data.Entity.Tests.Validators.Infrastructure;
using Havit.Data.Entity.Tests.Validators.Infrastructure.Model;

namespace Havit.Data.Entity.Tests.Validators
{
	[TestClass]
	public class DbContextConventionsValidatorTest
	{			
		[TestMethod]
		public void DbContextConventionsValidator_CheckPrimaryKey_ReportsMorePrimaryKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKey(modelValidatingDbContext.Db(typeof(MoreInvalidKeysClass))).ToArray();
			
			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("only one is expected")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckPrimaryKey_ReportsNonIntKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckPrimaryKey(modelValidatingDbContext.Db(typeof(StringIdClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("type int (System.Int32) is expected")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckPrimaryKey_DoesNotReportOnePrimaryKeys()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsOneCorrectKeyClass = modelValidator.CheckPrimaryKey(modelValidatingDbContext.Db(typeof(OneCorrectKeyClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsOneCorrectKeyClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckIdNamingConvention_ReportsCapitalId()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdNamingConvention(modelValidatingDbContext.Db(typeof(CapitalIDClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("which ends with")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckIdNamingConvention_DoesNotReportPascalCaseId()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckIdNamingConvention(modelValidatingDbContext.Db(typeof(OneCorrectKeyClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckStringMaxLengthConvention_ReportsMissingMaxLengthAttribute()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringMaxLengthConvention(modelValidatingDbContext.Db(typeof(NoMaxLengthAttributeClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("MaxLengthAttribute on property is expected")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckStringMaxLengthConvention_DoesNotReportMissingAttributeWithPositiveValue()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckStringMaxLengthConvention(modelValidatingDbContext.Db(typeof(MaxLengthAttributeWithPositiveValueClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckNestedMembers_ReportsNestedClass()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNestedMembers(modelValidatingDbContext.Db(typeof(WithNestedClassClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("unsupported nested type")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckNestedMembers_ReportsNonEntryEnum()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNestedMembers(modelValidatingDbContext.Db(typeof(WithNestedEnumOtherClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("unsupported nested type")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckNestedMembers_DoesNotReportEntryEnum()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckNestedMembers(modelValidatingDbContext.Db(typeof(WithNestedEnumEntryClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckForeignKeyForNavigationProperties_ReportsNavigationPropertyWithoutForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckForeignKeyForNavigationProperties(modelValidatingDbContext.Db(typeof(NavigationPropertyWithoutForeignKeyClass))).ToArray();

			// Assert			
			Assert.IsTrue(errorsMoreInvalidKeysClass.Any(item => item.Contains("no foreign key")));
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckForeignKeyForNavigationProperties_DoesNotReportNavigationPropertyWithForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass = modelValidator.CheckForeignKeyForNavigationProperties(modelValidatingDbContext.Db(typeof(NavigationPropertyWithForeignKeyClass))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass.Any());
		}

		[TestMethod]
		public void DbContextConventionsValidator_CheckSymbolVsPrimaryKeyForEntries_DoesNotReportNavigationPropertyWithForeignKey()
		{
			// Arrange
			ModelValidatingDbContext modelValidatingDbContext = new ModelValidatingDbContext();
			ModelValidator modelValidator = new ModelValidator();

			// Act
			string[] errorsMoreInvalidKeysClass1 = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Db(typeof(EntryWithGeneratedPrimaryKeyAndNoSymbol))).ToArray();
			string[] errorsMoreInvalidKeysClass2 = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Db(typeof(EntryWithGeneratedPrimaryKeyAndWithSymbol))).ToArray();
			string[] errorsMoreInvalidKeysClass3 = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Db(typeof(EntryWithPrimaryKeyAndNoSymbol))).ToArray();
			string[] errorsMoreInvalidKeysClass4 = modelValidator.CheckSymbolVsPrimaryKeyForEntries(modelValidatingDbContext.Db(typeof(EntryWithPrimaryKeyAndWithSymbol))).ToArray();

			// Assert			
			Assert.IsFalse(errorsMoreInvalidKeysClass1.Any());
			Assert.IsTrue(errorsMoreInvalidKeysClass2.Any());
			Assert.IsTrue(errorsMoreInvalidKeysClass3.Any());
			Assert.IsFalse(errorsMoreInvalidKeysClass4.Any());

		}
	}
}

