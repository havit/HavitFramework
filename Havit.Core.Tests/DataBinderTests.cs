namespace Havit.Core.Tests;

[TestClass]
public class DataBinderTests
{
	[TestMethod]
	public void DataBinderExt_SetValue_PublicProperty()
	{
		// Arrange
		var obj = new MyClass();

		// Act
		DataBinderExt.SetValue(obj, "MyPublicProperty", 1);

		// Assert
		Assert.AreEqual(1, obj.MyPublicProperty);
	}

	[TestMethod]
	public void DataBinderExt_SetValue_ProtectedSetterProperty()
	{
		// Arrange
		var obj = new MyClass();

		// Assert
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			// Act
			DataBinderExt.SetValue(obj, "MyPropertyWithProtectedSetter", 1);
		});
	}

	[TestMethod]
	public void DataBinderExt_GetValue_NullDataItemReturnsNull()
	{
		Assert.IsNull(DataBinderExt.GetValue(null, "MyPublicProperty"));
	}

	[TestMethod]
	public void DataBinderExt_GetValue_NullDataFieldThrowsArgumentNullException()
	{
		Assert.ThrowsExactly<ArgumentNullException>(() => DataBinderExt.GetValue(new MyClass(), null));
	}

	private class MyClass
	{
		public int MyPublicProperty { get; set; }
		public int MyPropertyWithProtectedSetter { get; protected set; }
	}

}
