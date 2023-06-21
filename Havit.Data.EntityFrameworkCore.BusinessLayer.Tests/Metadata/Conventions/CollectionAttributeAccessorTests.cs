using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.Metadata.Conventions;

[TestClass]
public class CollectionAttributeAccessorTests
{
	private class Master
	{
		public int Id { get; set; }

		public List<Child> Children { get; set; } = new List<Child>();
	}

	private class Child
	{
		public int Id { get; set; }

		public int MasterId { get; set; }
	}

	[TestMethod]
	public void CollectionAttributeAccessor_SortingProperty_NoExtendedProperty_EmptyString()
	{
		var model = CreateModel(builder =>
		{
			builder.Entity<Master>();
			builder.Entity<Child>();
		});

		INavigation navigation = model.FindEntityType(typeof(Master)).FindNavigation(nameof(Master.Children));

		var sorting = new CollectionAttributeAccessor(navigation).Sorting;

		Assert.IsNull(sorting);
	}

	[TestMethod]
	public void CollectionAttributeAccessor_SortingProperty_CollectionExtendedPropertyDefined_CorrectString()
	{
		var model = CreateModel(builder =>
		{
			builder.Entity<Master>();
			builder.Entity<Child>();
			builder.Entity<Master>()
				.AddExtendedProperties(new CollectionAttribute { Sorting = "[Count]" }
					.GetExtendedProperties(typeof(Master).GetProperty(nameof(Master.Children))));
		});

		INavigation navigation = model.FindEntityType(typeof(Master)).FindNavigation(nameof(Master.Children));

		var sorting = new CollectionAttributeAccessor(navigation).Sorting;

		Assert.AreEqual("[Count]", sorting);
	}

	[TestMethod]
	public void CollectionAttributeAccessor_ParseSortingProperties_CollectionExtendedPropertyDefined_CorrectProperty()
	{
		var model = CreateModel(builder =>
		{
			builder.Entity<Master>();
			builder.Entity<Child>();
			builder.Entity<Master>()
				.AddExtendedProperties(new CollectionAttribute { Sorting = "[Count]" }
					.GetExtendedProperties(typeof(Master).GetProperty(nameof(Master.Children))));
		});

		INavigation navigation = model.FindEntityType(typeof(Master)).FindNavigation(nameof(Master.Children));

		List<string> properties = new CollectionAttributeAccessor(navigation).ParseSortingProperties();

		Assert.AreEqual(1, properties.Count);

		// "Count" is property on Child (which defines sorting of the collection), not List<T>.Count!
		Assert.AreEqual("Count", properties[0]);
	}

	private IModel CreateModel(Action<ModelBuilder> onModelCreating)
	{
		return new TestDbContext(onModelCreating).Model;
	}
}