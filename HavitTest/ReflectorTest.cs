using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HavitTest
{
	/// <summary>
	/// Summary description for ReflectorTest
	/// </summary>
	[TestClass]
	public class ReflectorTest
	{
		[TestMethod]
		public void TestGetPropertyValue()
		{
			ReflectorTestClass data = new ReflectorTestClass();
			object value = new object();
			data.Value = value;
			object valueReflection = Havit.Reflection.Reflector.GetPropertyValue(data, "Value");
			Assert.AreEqual(value, valueReflection);
		}

		[TestMethod]
		public void TestSetPropertyValue()
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
