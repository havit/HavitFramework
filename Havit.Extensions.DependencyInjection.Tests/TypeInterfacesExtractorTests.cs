using Havit.Extensions.DependencyInjection.Tests.Infrastructure;

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
		Assert.Contains(typeof(IService), interfacesToRegister1, nameof(IService));
		Assert.Contains(typeof(IFirstService), interfacesToRegister1, nameof(IFirstService));
		Assert.DoesNotContain(typeof(ISecondService), interfacesToRegister1, nameof(ISecondService));
		Assert.Contains(typeof(IGenericService<,>), interfacesToRegister2, nameof(IGenericService<object, object>));
		Assert.IsFalse(interfacesToRegister3.Any(), nameof(MyStringService<object>));
	}
}
