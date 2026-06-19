namespace Havit.Core.Tests.Reflection;

[TestClass]
public class ReflectorTests
{
	[TestMethod]
	public void Reflector_GetPropertyValue()
	{
		ReflectorTestClass data = new ReflectorTestClass();
		object value = new object();
		data.Value = value;
		object valueReflection = Havit.Reflection.Reflector.GetPropertyValue(data, "Value");
		Assert.AreEqual(value, valueReflection);
	}

	[TestMethod]
	public void Reflector_SetPropertyValue()
	{
		ReflectorTestClass data = new ReflectorTestClass();
		object value = new object();
		Havit.Reflection.Reflector.SetPropertyValue(data, "Value", value);
		Assert.AreEqual(value, data.Value);
	}

	[TestMethod]
	public void Reflector_GetPropertyValue_FindsPrivatePropertyOnBaseType()
	{
		ReflectorDerivedClass data = new ReflectorDerivedClass();
		data.SetBasePrivateValue(42);

		object value = Havit.Reflection.Reflector.GetPropertyValue(data, "BasePrivateValue");

		Assert.AreEqual(42, value);
	}

	[TestMethod]
	public void Reflector_SetPropertyValue_SetsPrivatePropertyOnBaseType()
	{
		ReflectorDerivedClass data = new ReflectorDerivedClass();

		Havit.Reflection.Reflector.SetPropertyValue(data, "BasePrivateValue", 99);

		Assert.AreEqual(99, data.GetBasePrivateValue());
	}

	[TestMethod]
	public void Reflector_GetPropertyValue_FindsPublicPropertyOnBaseType()
	{
		ReflectorDerivedClass data = new ReflectorDerivedClass();
		data.BasePublicValue = "abc";

		object value = Havit.Reflection.Reflector.GetPropertyValue(data, "BasePublicValue");

		Assert.AreEqual("abc", value);
	}

	[TestMethod]
	public void Reflector_GetPropertyValue_DerivedNewPropertyWinsOverBase()
	{
		ReflectorShadowingDerivedClass data = new ReflectorShadowingDerivedClass();
		data.Shadowed = "derived";

		// most derived declaration must win (no AmbiguousMatchException)
		object value = Havit.Reflection.Reflector.GetPropertyValue(data, "Shadowed");

		Assert.AreEqual("derived", value);
	}

	[TestMethod]
	public void Reflector_GetPropertyValue_ReturnsNullForUnknownProperty()
	{
		ReflectorTestClass data = new ReflectorTestClass();

		object value = Havit.Reflection.Reflector.GetPropertyValue(data, "DoesNotExist");

		Assert.IsNull(value);
	}

	[TestMethod]
	public void Reflector_GetPropertyValue_WithTargetType()
	{
		ReflectorTestClass data = new ReflectorTestClass();
		object value = new object();
		data.Value = value;

		object valueReflection = Havit.Reflection.Reflector.GetPropertyValue(data, typeof(ReflectorTestClass), "Value");

		Assert.AreEqual(value, valueReflection);
	}

	[TestMethod]
	public void Reflector_GetPropertyValue_WithTargetType_FindsPrivatePropertyOnSpecifiedBaseType()
	{
		ReflectorDerivedClass data = new ReflectorDerivedClass();
		data.SetBasePrivateValue(42);

		// the targetType overload searches the specified type - private member is found on the base type passed explicitly
		object value = Havit.Reflection.Reflector.GetPropertyValue(data, typeof(ReflectorBaseClass), "BasePrivateValue");

		Assert.AreEqual(42, value);
	}

	[TestMethod]
	public void Reflector_GetPropertyValue_WithTargetType_ReturnsNullForUnknownProperty()
	{
		ReflectorTestClass data = new ReflectorTestClass();

		// exercises caching of a "not found" (null) lookup result on the targetType overload
		object value = Havit.Reflection.Reflector.GetPropertyValue(data, typeof(ReflectorTestClass), "DoesNotExist");

		Assert.IsNull(value);
	}

	[TestMethod]
	public void Reflector_SetPropertyValue_WithTargetType()
	{
		ReflectorTestClass data = new ReflectorTestClass();
		object value = new object();

		Havit.Reflection.Reflector.SetPropertyValue(data, typeof(ReflectorTestClass), "Value", value);

		Assert.AreEqual(value, data.Value);
	}

	[TestMethod]
	public void Reflector_SetPropertyValue_WithTargetType_ThrowsForUnknownProperty()
	{
		ReflectorTestClass data = new ReflectorTestClass();

		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			Havit.Reflection.Reflector.SetPropertyValue(data, typeof(ReflectorTestClass), "DoesNotExist", null);
		});
	}

	[TestMethod]
	public void Reflector_GetPropertyValue_RepeatedCallsReturnConsistentValue()
	{
		// the cached PropertyInfo must be reused correctly across calls (and across different instances of the same type)
		ReflectorTestClass data1 = new ReflectorTestClass { Value = "first" };
		ReflectorTestClass data2 = new ReflectorTestClass { Value = "second" };

		Assert.AreEqual("first", Havit.Reflection.Reflector.GetPropertyValue(data1, "Value"));
		Assert.AreEqual("second", Havit.Reflection.Reflector.GetPropertyValue(data2, "Value"));
		Assert.AreEqual("first", Havit.Reflection.Reflector.GetPropertyValue(data1, typeof(ReflectorTestClass), "Value"));
		Assert.AreEqual("second", Havit.Reflection.Reflector.GetPropertyValue(data2, typeof(ReflectorTestClass), "Value"));
	}

	private class ReflectorTestClass
	{
		public object Value { get; set; }
	}

	private class ReflectorBaseClass
	{
		private int BasePrivateValue { get; set; }
		public string BasePublicValue { get; set; }

		public void SetBasePrivateValue(int value) => BasePrivateValue = value;
		public int GetBasePrivateValue() => BasePrivateValue;
	}

	private class ReflectorDerivedClass : ReflectorBaseClass
	{
	}

	private class ReflectorShadowingBaseClass
	{
		public string Shadowed { get; set; }
	}

	private class ReflectorShadowingDerivedClass : ReflectorShadowingBaseClass
	{
		public new string Shadowed { get; set; }
	}
}
