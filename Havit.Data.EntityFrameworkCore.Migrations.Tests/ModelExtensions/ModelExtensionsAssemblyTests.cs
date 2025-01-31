﻿using System.Reflection;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions;
using Havit.Data.EntityFrameworkCore.Migrations.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.Migrations.Tests.ModelExtensions;

[TestClass]
public class ModelExtensionsAssemblyTests
{
	private class TestDbContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseSqlServer(new Microsoft.Data.SqlClient.SqlConnection("Database=Dummy"));
		}
	}

	[TestMethod]
	public void ModelExtensionsAssembly_Assembly_NoCustomAssemblyDefined_AssemblyOfDbContextTypeIsUsed()
	{
		// Arrange
		var modelExtensionsAssembly = new ModelExtensionsAssembly(new CurrentDbContext(new TestDbContext()), new DbContextOptionsBuilder<TestDbContext>().Options);

		// Act + Assert
		Assert.AreSame(typeof(TestDbContext).Assembly, modelExtensionsAssembly.Assembly);
	}

	[TestMethod]
	public void ModelExtensionsAssembly_Assembly_CustomAssemblyDefined_SpecifiedAssemblyIsUsed()
	{
		// Arrange
		Assembly extensionsAssembly = typeof(DbContextOptionsBuilderExtensions).Assembly;
#pragma warning disable CS0618 // Type or member is obsolete
		var modelExtensionsAssembly = new ModelExtensionsAssembly(new CurrentDbContext(new TestDbContext()),
			new DbContextOptionsBuilder<TestDbContext>()
				.UseModelExtensions(b => b.ModelExtensionsAssembly(extensionsAssembly))
				.Options);
#pragma warning restore CS0618 // Type or member is obsolete

		// Act + Assert
		Assert.AreSame(extensionsAssembly, modelExtensionsAssembly.Assembly);
	}

	[TestMethod]
	public void ModelExtensionsAssembly_ModelExtenders_ReturnsExpectedModelExtender()
	{
		// Arrange
#pragma warning disable CS0618 // Type or member is obsolete
		var modelExtensionsAssembly = new ModelExtensionsAssembly(new CurrentDbContext(new TestDbContext()),
			new DbContextOptionsBuilder<TestDbContext>()
				.UseModelExtensions()
				.Options);
#pragma warning restore CS0618 // Type or member is obsolete

		TypeInfo testModelExtenderType = typeof(TestModelExtender).GetTypeInfo();

		// Act + Assert
		CollectionAssert.Contains(modelExtensionsAssembly.ModelExtenders.ToArray(), testModelExtenderType);
	}

	[TestMethod]
	public void ModelExtensionsAssembly_CreateModelExtender_ReturnsInstanceOfExpectedModelExtender()
	{
		// Arrange
#pragma warning disable CS0618 // Type or member is obsolete
		var modelExtensionsAssembly = new ModelExtensionsAssembly(new CurrentDbContext(new TestDbContext()),
			new DbContextOptionsBuilder<TestDbContext>()
				.UseModelExtensions()
				.Options);
#pragma warning restore CS0618 // Type or member is obsolete

		TypeInfo testModelExtenderType = typeof(TestModelExtender).GetTypeInfo();

		// Act
		var modelExtender = modelExtensionsAssembly.CreateModelExtender(testModelExtenderType);

		// Assert
		Assert.AreSame(testModelExtenderType, modelExtender.GetType().GetTypeInfo());
	}
}