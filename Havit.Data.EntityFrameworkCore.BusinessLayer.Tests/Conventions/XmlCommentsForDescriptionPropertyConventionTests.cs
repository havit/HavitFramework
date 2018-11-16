using System;
using System.Collections.Generic;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Havit.Data.EntityFrameworkCore.BusinessLayer.Conventions.XmlCommentsForDescriptionPropertyConvention;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.Conventions
{
	public class XmlCommentsForDescriptionPropertyConventionTests
	{
		[TestClass]
		public class NestedTypeWithXmlComment_EntityHasCorrectMsDescription
		{
			/// <summary>
			/// A comment
			/// </summary>
			private class NestedType
			{
				public int Id { get; set; }
			}

			[TestMethod]
			public void Test()
			{
				var context = new EndToEndDbContext<NestedType>();

				var entityType = context.Model.FindEntityType(typeof(NestedType));

				Assert.AreEqual("A comment", entityType.GetStringExtendedProperty(MsDescriptionExtendedProperty));
			}
		}

		[TestClass]
		public class NonNestedTypeWithXmlComment_EntityHasCorrectMsDescription
		{
			[TestMethod]
			public void Test()
			{
				var context = new EndToEndDbContext<XmlTestEntity>();

				var entityType = context.Model.FindEntityType(typeof(XmlTestEntity));

				Assert.AreEqual("A comment", entityType.GetStringExtendedProperty(MsDescriptionExtendedProperty));
			}
		}

		private class EndToEndDbContext<TEntity> : TestDbContext
			where TEntity : class
		{
			private readonly Action<ModelBuilder> onModelCreating;

			public EndToEndDbContext(Action<ModelBuilder> onModelCreating = default)
			{
				this.onModelCreating = onModelCreating;
			}

			protected override void CustomizeModelCreating(ModelBuilder modelBuilder)
			{
				base.CustomizeModelCreating(modelBuilder);
				modelBuilder.Entity<TEntity>();
				onModelCreating?.Invoke(modelBuilder);
			}

			protected override IEnumerable<IModelConvention> GetModelConventions()
			{
				yield return new XmlCommentsForDescriptionPropertyConvention();
			}
		}
	}
}
