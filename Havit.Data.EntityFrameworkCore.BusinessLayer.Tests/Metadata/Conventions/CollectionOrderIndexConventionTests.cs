using System.Collections.Generic;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.Metadata.Conventions
{
	/// <summary>
	/// Integration tests of conventions focused on testing <see cref="CollectionOrderIndexConvention"/>.
	/// </summary>
	[TestClass]
	public class CollectionOrderIndexConventionTests
	{
		private class Master<T>
		{
			public int Id { get; set; }

			[Collection(Sorting = "[Order]")]
			public List<T> Children { get; set; } = new List<T>();
		}

		private class ChildNoDeleted
		{
			public int Id { get; set; }

			public Master<ChildNoDeleted> Parent { get; set; }

			public int ParentId { get; set; }

			public int Order { get; set; }
		}

		private class ChildDeleted
		{
			public int Id { get; set; }

			public Master<ChildDeleted> Parent { get; set; }

			public int ParentId { get; set; }

			public int Order { get; set; }

			public bool Deleted { get; set; }
		}

		[TestMethod]
		public void CollectionOrderIndexConvention_OrderPropertyWithoutDeletedColumn_IndexIsDefined()
		{
			using (var dbContext = new TestDbContext<Master<ChildNoDeleted>>())
			{
				var entity = dbContext.Model.FindEntityType(typeof(ChildNoDeleted));

				IIndex index = entity.FindIndex(new[] { entity.FindProperty(nameof(ChildNoDeleted.ParentId)), entity.FindProperty(nameof(ChildNoDeleted.Order)) });

				Assert.IsNotNull(index);
			}
		}

		[TestMethod]
		public void CollectionOrderIndexConvention_OrderPropertyAndDeletedColumn_IndexIsDefined()
		{
			using (var dbContext = new TestDbContext<Master<ChildDeleted>>())
			{
				var entity = dbContext.Model.FindEntityType(typeof(ChildDeleted));

				IIndex index = entity.FindIndex(new[] { entity.FindProperty(nameof(ChildDeleted.ParentId)), entity.FindProperty(nameof(ChildDeleted.Order)), entity.FindProperty(nameof(ChildDeleted.Deleted)) });

				Assert.IsNotNull(index);
			}
		}

		private class TestDbContext<T> : Tests.TestDbContext<T>
			where T : class
		{
			protected override BusinessLayerDbContextSettings CreateDbContextSettings()
			{
				var settings = base.CreateDbContextSettings();
				settings.UseCollectionOrderIndexConvention = true;
				return settings;
			}
		}
	}
}