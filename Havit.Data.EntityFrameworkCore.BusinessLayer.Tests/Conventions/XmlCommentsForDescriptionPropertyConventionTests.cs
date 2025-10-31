using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.Conventions;

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
		public void XmlCommentsForDescriptionPropertyConvention_NestedTypeWithXmlComment_EntityHasCorrectMsDescription()
		{
			var context = new EndToEndDbContext<NestedType>();

			var entityType = context.Model.FindEntityType(typeof(NestedType));

			Assert.AreEqual("A comment", entityType.GetStringExtendedProperty(XmlCommentsForDescriptionPropertyConvention.MsDescriptionExtendedProperty));
		}
	}

	[TestClass]
	public class NonNestedTypeWithXmlComment_EntityHasCorrectMsDescription
	{
		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_NonNestedTypeWithXmlComment_EntityHasCorrectMsDescription()
		{
			var context = new EndToEndDbContext<XmlTestEntity>();

			var entityType = context.Model.FindEntityType(typeof(XmlTestEntity));

			Assert.AreEqual("A comment", entityType.GetStringExtendedProperty(XmlCommentsForDescriptionPropertyConvention.MsDescriptionExtendedProperty));
		}
	}

	[TestClass]
	public class PrimaryKeyPropertyWithXmlComment_PropertyHasCorrectMsDescription
	{
		/// <summary>
		/// A comment
		/// </summary>
		private class AClass
		{
			/// <summary>
			/// Primary key
			/// </summary>
			public int Id { get; set; }
		}

		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_PrimaryKeyPropertyWithXmlComment_PropertyHasCorrectMsDescription()
		{
			var context = new EndToEndDbContext<AClass>();

			var entityType = context.Model.FindEntityType(typeof(AClass));

			Assert.AreEqual("Primary key", entityType.FindProperty(nameof(AClass.Id)).GetStringExtendedProperty(XmlCommentsForDescriptionPropertyConvention.MsDescriptionExtendedProperty));
		}
	}

	[TestClass]
	public class RegularPropertyWithXmlComment_PropertyHasCorrectMsDescription
	{
		/// <summary>
		/// A comment
		/// </summary>
		private class AClass
		{
			public int Id { get; set; }

			/// <summary>
			/// Some comment
			/// </summary>
			public string Name { get; set; }
		}

		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_RegularPropertyWithXmlComment_PropertyHasCorrectMsDescription()
		{
			var context = new EndToEndDbContext<AClass>();

			var entityType = context.Model.FindEntityType(typeof(AClass));

			Assert.AreEqual("Some comment", entityType.FindProperty(nameof(AClass.Name)).GetStringExtendedProperty(XmlCommentsForDescriptionPropertyConvention.MsDescriptionExtendedProperty));
		}
	}

	[TestClass]
	public class NavigationPropertyWithXmlComment_CollectionHasCorrectMsDescription
	{
		/// <summary>
		/// A comment
		/// </summary>
		private class Parent
		{
			public int Id { get; set; }

			/// <summary>
			/// Some comment for collection
			/// </summary>
			public List<Child> Children { get; set; }
		}

		private class Child
		{
			public int Id { get; set; }

			public Parent Parent { get; set; }
			public int ParentId { get; set; }
		}

		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_NavigationPropertyWithXmlComment_CollectionHasCorrectMsDescription()
		{
			var context = new EndToEndDbContext<Parent>();

			var entityType = context.Model.FindEntityType(typeof(Parent));

			Assert.AreEqual("Some comment for collection", entityType.GetStringExtendedProperty($"Collection_{nameof(Parent.Children)}_Description"));
		}
	}

	[TestClass]
	public class NavigationPropertyWithXmlComment_ForeignKeyPropertyHasCorrectMsDescription
	{
		/// <summary>
		/// A comment
		/// </summary>
		private class Parent
		{
			public int Id { get; set; }

			public List<Child> Children { get; set; }
		}

		/// <summary>
		/// Child class comment
		/// </summary>
		private class Child
		{
			public int Id { get; set; }

			/// <summary>
			/// Parent comment
			/// </summary>
			public Parent Parent { get; set; }
			public int ParentId { get; set; }
		}

		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_NavigationPropertyWithXmlComment_ForeignKeyPropertyHasCorrectMsDescription()
		{
			var context = new EndToEndDbContext<Parent>();

			var entityType = context.Model.FindEntityType(typeof(Child));

			Assert.AreEqual("Parent comment", entityType.FindProperty(nameof(Child.ParentId)).GetStringExtendedProperty(XmlCommentsForDescriptionPropertyConvention.MsDescriptionExtendedProperty));
		}
	}

	[TestClass]
	public class ForeignKeyPropertyWithXmlComment_ForeignKeyPropertyHasCorrectMsDescription
	{
		/// <summary>
		/// A comment
		/// </summary>
		private class Parent
		{
			public int Id { get; set; }

			public List<Child> Children { get; set; }
		}

		/// <summary>
		/// Child class comment
		/// </summary>
		private class Child
		{
			public int Id { get; set; }

			public Parent Parent { get; set; }

			/// <summary>
			/// Parent comment FK
			/// </summary>
			public int ParentId { get; set; }
		}

		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_ForeignKeyPropertyWithXmlComment_ForeignKeyPropertyHasCorrectMsDescription()
		{
			var context = new EndToEndDbContext<Parent>();

			var entityType = context.Model.FindEntityType(typeof(Child));

			Assert.AreEqual("Parent comment FK", entityType.FindProperty(nameof(Child.ParentId)).GetStringExtendedProperty(XmlCommentsForDescriptionPropertyConvention.MsDescriptionExtendedProperty));
		}
	}

	[TestClass]
	public class LocalizationsNavigationPropertyWithXmlComment_NoMsDescriptionDefined
	{
		/// <summary>
		/// A comment
		/// </summary>
		private class Parent
		{
			public int Id { get; set; }

			/// <summary>
			/// Localizations of Parent entity
			/// </summary>
			public List<ParentLocalization> Localizations { get; set; }
		}

		/// <summary>
		/// Child class comment
		/// </summary>
		private class ParentLocalization
		{
			public int Id { get; set; }

			public Parent Parent { get; set; }
			public int ParentId { get; set; }
		}

		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_LocalizationsNavigationPropertyWithXmlComment_NoMsDescriptionDefined()
		{
			var context = new EndToEndDbContext<Parent>();

			var entityType = context.Model.FindEntityType(typeof(Parent));

			Assert.IsNull(entityType.GetStringExtendedProperty($"Collection_{nameof(Parent.Localizations)}_Description"));
		}
	}

	/// <summary>
	/// Scenario with parent class having no XML comment (Bug 41564)
	/// </summary>
	[TestClass]
	public class ParentClassWithoutXmlComment_RegularPropertyWithXmlComment_PropertyHasCorrectMsDescription
	{
		private class AClass
		{
			public int Id { get; set; }

			/// <summary>
			/// Short comment
			/// </summary>
			public string Name { get; set; }
		}

		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_ParentClassWithoutXmlComment_RegularPropertyWithXmlComment_PropertyHasCorrectMsDescription()
		{
			var context = new EndToEndDbContext<AClass>();

			var entityType = context.Model.FindEntityType(typeof(AClass));

			Assert.AreEqual("Short comment", entityType.FindProperty(nameof(AClass.Name)).GetStringExtendedProperty(XmlCommentsForDescriptionPropertyConvention.MsDescriptionExtendedProperty));
		}
	}

	/// <summary>
	/// Scenario with parent class having no XML comment (Bug 41564)
	/// </summary>
	[TestClass]
	public class ParentClassWithoutXmlComment_RegularPropertyWithXmlComment_ParentClassHasNoMsDescription
	{
		private class AClass
		{
			public int Id { get; set; }

			/// <summary>
			/// Short comment
			/// </summary>
			public string Name { get; set; }
		}

		[TestMethod]
		public void XmlCommentsForDescriptionPropertyConvention_ParentClassWithoutXmlComment_RegularPropertyWithXmlComment_ParentClassHasNoMsDescription()
		{
			var context = new EndToEndDbContext<AClass>();

			var entityType = context.Model.FindEntityType(typeof(AClass));

			Assert.IsNull(entityType.GetStringExtendedProperty(XmlCommentsForDescriptionPropertyConvention.MsDescriptionExtendedProperty));
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
	}
}
