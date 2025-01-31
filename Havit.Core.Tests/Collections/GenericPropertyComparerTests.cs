﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Havit.Collections;

namespace Havit.Tests.Collections;

[TestClass]
public class GenericPropertyComparerTests
{
	[TestMethod]
	public void GenericPropertyComparer_Null()
	{
		var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
		Assert.AreEqual(gpc.Compare(null, null), 0);
		Assert.AreEqual(gpc.Compare(DBNull.Value, DBNull.Value), 0);
		Assert.AreEqual(gpc.Compare(DBNull.Value, null), 0);
	}

	[TestMethod]
	public void GenericPropertyComparer_NotNull()
	{
		var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
		Assert.AreEqual(gpc.Compare(new DataTestClass("A"), new DataTestClass("A")), 0);
		Assert.AreEqual(gpc.Compare(new DataTestClass("A"), new DataTestClass("B")), -1);
		Assert.AreEqual(gpc.Compare(new DataTestClass("B"), new DataTestClass("A")), 1);
	}

	[TestMethod]
	public void GenericPropertyComparer_NullAndNotNull()
	{
		var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
		Assert.AreEqual(gpc.Compare(new DataTestClass("A"), DBNull.Value), 1);
		Assert.AreEqual(gpc.Compare(DBNull.Value, new DataTestClass("B")), -1);
		Assert.AreEqual(gpc.Compare(new DataTestClass(null), DBNull.Value), 0);
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
