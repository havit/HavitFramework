using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Tests
{
    [TestClass]
	public class DataBinderTests
    {
		[TestMethod]
		public void DataBinderExt_SetValue_PublicProperty()
		{
			// arrange
			var obj = new MyClass();

			// act
			DataBinderExt.SetValue(obj, "MyPublicProperty", 1);

			// assert
			Assert.AreEqual(1, obj.MyPublicProperty);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataBinderExt_SetValue_ProtectedSetterProperty() 
		{
			// arrange
			var obj = new MyClass();

			// act
			DataBinderExt.SetValue(obj, "MyPropertyWithProtectedSetter", 1);

			// assert
			Assert.AreEqual(1, obj.MyPropertyWithProtectedSetter);
		}

		private class MyClass
		{
			public int MyPublicProperty { get; set; }
			public int MyPropertyWithProtectedSetter { get; protected set; }
		}
        
    }
}
