using Havit.Extensions.DependencyInjection.Tests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Extensions.DependencyInjection.Tests;

[TestClass]
public class TypeInterfacesExtractorTests
{
	[TestMethod]
	public void TypeInterfacesExtractor_GetInterfacesToRegister()
	{
		// Act
		List<Type> interfacesToRegister1 = TypeInterfacesExtractor.GetInterfacesToRegister(typeof(MyFirstService)).ToList();
		List<Type> interfacesToRegister2 = TypeInterfacesExtractor.GetInterfacesToRegister(typeof(MyGenericService<,>)).ToList();
		List<Type> interfacesToRegister3 = TypeInterfacesExtractor.GetInterfacesToRegister(typeof(MyStringService<>)).ToList();

		// Assert
		Assert.IsTrue(interfacesToRegister1.Contains(typeof(IService)), nameof(IService));
		Assert.IsTrue(interfacesToRegister1.Contains(typeof(IFirstService)), nameof(IFirstService));
		Assert.IsFalse(interfacesToRegister1.Contains(typeof(ISecondService)), nameof(ISecondService));
		Assert.IsTrue(interfacesToRegister2.Contains(typeof(IGenericService<,>)), nameof(IGenericService<object, object>));
		Assert.IsFalse(interfacesToRegister3.Any(), nameof(MyStringService<object>));
	}
}
