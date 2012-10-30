﻿// The following code was generated by Microsoft Visual Studio 2005.
// The test owner should check each test for validity.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using Havit.Collections;
namespace HavitTest
{
	/// <summary>
	/// Test třídy GenericPropertyComparer.
	/// </summary>
	[TestClass]
	public class GenericPropertyComparerTest
	{
		#region GenericPropertyComparer_Null
		[TestMethod]
		public void GenericPropertyComparer_Null()
		{
			var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
			Assert.AreEqual(gpc.Compare(null, null), 0);
			Assert.AreEqual(gpc.Compare(DBNull.Value, DBNull.Value), 0);
			Assert.AreEqual(gpc.Compare(DBNull.Value, null), 0);
		}
		#endregion

		#region GenericPropertyComparer_NotNull
		[TestMethod]
		public void GenericPropertyComparer_NotNull()
		{
			var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
			Assert.AreEqual(gpc.Compare(new DataTestClass("A"), new DataTestClass("A")), 0);
			Assert.AreEqual(gpc.Compare(new DataTestClass("A"), new DataTestClass("B")), -1);
			Assert.AreEqual(gpc.Compare(new DataTestClass("B"), new DataTestClass("A")), 1);
		}
		#endregion

		#region GenericPropertyComparer_NotNull
		[TestMethod]
		public void GenericPropertyComparer_NullAndNotNull()
		{
			var gpc = new GenericPropertyComparer<object>(new SortItem("Nazev", SortDirection.Ascending)) as IComparer<object>;
			Assert.AreEqual(gpc.Compare(new DataTestClass("A"), DBNull.Value), 1);
			Assert.AreEqual(gpc.Compare(DBNull.Value, new DataTestClass("B")), -1);
			Assert.AreEqual(gpc.Compare(new DataTestClass(null), DBNull.Value), 0);
		}
		#endregion

		private class DataTestClass
		{
			public DataTestClass(string nazev)
			{
				this.Nazev = nazev;
			}
			public string Nazev { get; set; }
		}
	}
}
