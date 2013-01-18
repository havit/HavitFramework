using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HavitTest
{
	/// <summary>
	/// Test třídy Havit.Reflection.Reflector.
	/// </summary>
	[TestClass]
	public class ReflectorTest
	{
		#region TestGetPropertyValue
		[TestMethod]
		public void TestGetPropertyValue()
		{
			ReflectorTestClass data = new ReflectorTestClass();
			object value = new object();
			data.Value = value;
			object valueReflection = Havit.Reflection.Reflector.GetPropertyValue(data, "Value");
			Assert.AreEqual(value, valueReflection);
		}
		#endregion

		#region TestSetPropertyValue
		[TestMethod]
		public void TestSetPropertyValue()
		{
			ReflectorTestClass data = new ReflectorTestClass();
			object value = new object();
			Havit.Reflection.Reflector.SetPropertyValue(data, "Value", value);
			Assert.AreEqual(value, data.Value);
		}
		#endregion

		private class ReflectorTestClass
		{
			#region Value
			public object Value { get; set; }
			#endregion
		}
	}
}
