using Havit.Extensions.DependencyInjection.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Extensions.DependencyInjection.Tests
{
	[TestClass]
	public class TypeInterfacesExtractorTests
	{
		[TestMethod]
		public void TypeInterfacesExtractor_GetInterfacesToRegister()
		{
			// Act
			List<Type> interfacesToRegister = TypeInterfacesExtractor.GetInterfacesToRegister(typeof(MyFirstService)).ToList();

			// Assert
			Assert.IsTrue(interfacesToRegister.Contains(typeof(IService)));
			Assert.IsTrue(interfacesToRegister.Contains(typeof(IFirstService)));
			Assert.IsFalse(interfacesToRegister.Contains(typeof(ISecondService)));
		}
	}
}
