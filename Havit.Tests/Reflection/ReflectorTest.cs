using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Tests.Reflection
{
	/// <summary>
	/// Test třídy Havit.Reflection.Reflector.
	/// </summary>
	[TestClass]
	public class ReflectorTest
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

		private class ReflectorTestClass
		{
			public object Value { get; set; }
		}
	}
}
