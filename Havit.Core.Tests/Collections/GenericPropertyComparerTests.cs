using Havit.Collections;

namespace Havit.Tests.Collections;

[TestClass]
public class GenericPropertyComparerTests
{
	[TestMethod]
	public void GenericPropertyComparer_Null()
	{
		var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
		Assert.AreEqual(0, gpc.Compare(null, null));
		Assert.AreEqual(0, gpc.Compare(DBNull.Value, DBNull.Value));
		Assert.AreEqual(0, gpc.Compare(DBNull.Value, null));
	}

	[TestMethod]
	public void GenericPropertyComparer_NotNull()
	{
		var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
		Assert.AreEqual(0, gpc.Compare(new DataTestClass("A"), new DataTestClass("A")));
		Assert.AreEqual(-1, gpc.Compare(new DataTestClass("A"), new DataTestClass("B")));
		Assert.AreEqual(1, gpc.Compare(new DataTestClass("B"), new DataTestClass("A")));
	}

	[TestMethod]
	public void GenericPropertyComparer_NullAndNotNull()
	{
		var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
		Assert.AreEqual(1, gpc.Compare(new DataTestClass("A"), DBNull.Value));
		Assert.AreEqual(-1, gpc.Compare(DBNull.Value, new DataTestClass("B")));
		Assert.AreEqual(0, gpc.Compare(new DataTestClass(null), DBNull.Value));
	}

	private class DataTestClass
	{
		public string Nazev { get; set; }

		public DataTestClass(string nazev)
		{
			this.Nazev = nazev;
		}
	}
}
