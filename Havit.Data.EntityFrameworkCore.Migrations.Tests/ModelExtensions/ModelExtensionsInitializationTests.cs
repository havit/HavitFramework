using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.StoredProcedures;
using Havit.Data.EntityFrameworkCore.Migrations.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.ModelExtensions;

/// <summary>
/// Tests for initializing Model Extensions in <see cref="DbContext"/>.
/// </summary>
[TestClass]
public class ModelExtensionsInitializationTests
{
	/// <summary>
	/// Verifies, that created Model Extensions creates correct annotations
	/// that can be converted back to <see cref="StoredProcedureModelExtension"/>s with correct values.
	/// </summary>
	[TestMethod]
	public void DbDbContext_ModelExtensionsInitializedThroughPredefinedModelExtender_AnnotationsAreCorrect()
	{
		IModelExtensionAnnotationProvider annotationProvider = new StoredProcedureAnnotationProvider();
		using (var dbContext = new TestDbContext(DefineModelExtenders(typeof(DummyStoredProcedures))))
		{
			var modelExtensions = annotationProvider.GetModelExtensions(dbContext.Model.GetAnnotations().ToList());

			Assert.AreEqual(1, modelExtensions.Count);
			Assert.IsInstanceOfType(modelExtensions[0], typeof(StoredProcedureModelExtension));
			Assert.AreEqual(nameof(DummyStoredProcedures.GetTables), ((StoredProcedureModelExtension)modelExtensions[0]).ProcedureName);
		}
	}

	/// <summary>
	/// Verify, that initializing Model Extensions do not modify model for IHistoryRepository.
	///
	/// Uses simple test, that create script does not contain CREATE OR ALTER PROCEDURE statement.
	/// </summary>
	[TestMethod]
	public void DbDbContext_InitializingModelExtensions_HistoryRepositoryModelIsNotAffected()
	{
		const string createStatement = "CREATE OR ALTER PROCEDURE";

		using (var dbContext = new TestDbContext(DefineModelExtenders(typeof(DummyStoredProcedures))))
		{
			var actualCreateScript = dbContext.GetService<IHistoryRepository>().GetCreateScript();

			StringAssert.DoesNotMatch(actualCreateScript, new Regex(createStatement),
				"Create script for __EFMigrationsHistory should not contain statements for Model Extensions");
		}
	}

	private static Action<DbContextOptionsBuilder> DefineModelExtenders(params Type[] modelExtenderTypes)
	{
		return builder =>
		{
			builder.SetModelExtenderTypes(modelExtenderTypes?.Select(t => t.GetTypeInfo()) ?? new TypeInfo[0]);
		};
	}

	private class DummyEntity
	{
		public int Id { get; set; }
	}

	private class DummyStoredProcedures : StoredProcedureModelExtender
	{
		public StoredProcedureModelExtension GetTables()
		{
			string procedure = $"CREATE OR ALTER PROCEDURE [dbo].[{nameof(GetTables)}]() AS BEGIN SELECT * FROM [sys].[tables] END";

			return new StoredProcedureModelExtension { CreateSql = procedure, ProcedureName = nameof(GetTables) };
		}
	}

	private class TestDbContext : DbContext
	{
		private readonly Action<DbContextOptionsBuilder> initializeAction;

		public TestDbContext(Action<DbContextOptionsBuilder> initializeAction = null)
			: base(new DbContextOptionsBuilder<TestDbContext>().UseSqlServer("Server=Dummy").Options)
		{
			this.initializeAction = initializeAction;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseModelExtensions(builder => builder.UseStoredProcedures());

			initializeAction?.Invoke(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DummyEntity>();
		}
	}
}
