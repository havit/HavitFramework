using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Infrastructure
{
	[TestClass]
	public class EntityActivatorTests
	{
		[TestMethod]
		public void EntityActivator_CreateInstance_PublicConstructor()
		{
			// Act + Assert
			Assert.IsNotNull(EntityActivator.CreateInstance<ClassWithPublicConstuctor>());
		}

		[TestMethod]
		[ExpectedException(typeof(MissingMemberException))]
		public void EntityActivator_CreateInstance_ParametirezedPublicConstructor()
		{
			// Act
			Assert.IsNotNull(EntityActivator.CreateInstance<ClassWithParametirezedPublicConstuctor>());

			// Assert by method attribute
		}

		[TestMethod]
		public void EntityActivator_CreateInstance_PrivateConstructor()
		{
			// Act + Assert
			Assert.IsNotNull(EntityActivator.CreateInstance<ClassWithPrivateConstuctor>());
		}

		private class ClassWithPublicConstuctor
		{
			// Default parameterless constructor
			// NOOP
		}

		private class ClassWithParametirezedPublicConstuctor
		{
			public ClassWithParametirezedPublicConstuctor(object o)
			{
				// NOOP
			}
		}

		private class ClassWithPrivateConstuctor
		{
			private ClassWithPrivateConstuctor()
			{
				// NOOP
			}
		}
	}
}
