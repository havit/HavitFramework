using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Havit.Data;
namespace Havit.DataTest
{
	/// <summary>
	/// This is a test class for Havit.Data.DataRecord and is intended
	/// to contain all Havit.Data.DataRecord Unit Tests
	/// </summary>
	[TestClass]
	public class DataRecordTest
	{
		#region TryGet
		/// <summary>
		/// A test for TryGet&lt;&gt; (string, out T)
		/// </summary>
		[TestMethod]
		public void TryGetTest_NacteniInt()
		{
			DataTable table = new DataTable();
			table.Columns.Add("ColumnName", typeof(int));
			table.Rows.Add(10);

#pragma warning disable 612,618
			DataRecord record = new DataRecord(table.Rows[0]);
#pragma warning restore 612,618

			string fieldName = "ColumnName";

			int TryGet_target;
			int target_expected = 10;

			bool expected = true;
			bool actual;

			actual = record.TryGet(fieldName, out TryGet_target);

			Assert.AreEqual(target_expected, TryGet_target, "target_TryGet_expected was not set correctly.");
			Assert.AreEqual(expected, actual, "Havit.Data.DataRecord.TryGet<T> did not return the expected value.");
		}

		/// <summary>
		/// A test for TryGet&lt;&gt; (string, out T)
		/// </summary>
		[TestMethod]
		public void TryGetTest_NullOK()
		{
			DataTable table = new DataTable();
			table.Columns.Add("ColumnName", typeof(object));
			table.Rows.Add(DBNull.Value);

#pragma warning disable 612,618
			DataRecord record = new DataRecord(table.Rows[0]);
#pragma warning restore 612,618

			string fieldName = "ColumnName";

			object TryGet_target = 10; // fake value
			object target_expected = null;

			bool expected = true;
			bool actual;

			actual = record.TryGet(fieldName, out TryGet_target);

			Assert.AreEqual(target_expected, TryGet_target, "target_TryGet_expected was not set correctly.");
			Assert.AreEqual(expected, actual, "Havit.Data.DataRecord.TryGet<T> did not return the expected value.");
		}

		/// <summary>
		/// A test for TryGet&lt;&gt; (string, out T)
		/// </summary>
		[TestMethod]
		public void TryGetTest_PretypovaniIntNaNullableInt()
		{
			DataTable table = new DataTable();
			table.Columns.Add("ColumnName", typeof(int));
			table.Rows.Add(10);

#pragma warning disable 612,618
			DataRecord record = new DataRecord(table.Rows[0]);
#pragma warning restore 612,618

			string fieldName = "ColumnName";

			int? TryGet_target;
			int? target_expected = 10;

			bool expected = true;
			bool actual;

			actual = record.TryGet(fieldName, out TryGet_target);

			Assert.AreEqual(target_expected, TryGet_target, "target_TryGet_expected was not set correctly.");
			Assert.AreEqual(expected, actual, "Havit.Data.DataRecord.TryGet<T> did not return the expected value.");
		}

		/// <summary>
		/// A test for TryGet&lt;&gt; (string, out T)
		/// </summary>
		[TestMethod]
		public void TryGetTest_PretypovaniDecimalNaDouble()
		{
			DataTable table = new DataTable();
			table.Columns.Add("ColumnName", typeof(decimal));
			table.Rows.Add(10.1M);

#pragma warning disable 612,618
			DataRecord record = new DataRecord(table.Rows[0]);
#pragma warning restore 612,618

			string fieldName = "ColumnName";

			double TryGet_target;
			double target_expected = 10.1;

			bool expected = true;
			bool actual;

			actual = record.TryGet(fieldName, out TryGet_target);

			Assert.AreEqual(target_expected, TryGet_target, "target_TryGet_expected was not set correctly.");
			Assert.AreEqual(expected, actual, "Havit.Data.DataRecord.TryGet<T> did not return the expected value.");
		}

		/// <summary>
		/// A test for TryGet&lt;&gt; (string, out T)
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void TryGetTest_InvalidCast()
		{
			DataTable table = new DataTable();
			table.Columns.Add("ColumnName", typeof(int));
			table.Rows.Add(10);

#pragma warning disable 612,618
			DataRecord record = new DataRecord(table.Rows[0]);
#pragma warning restore 612,618

			string fieldName = "ColumnName";

			Exception TryGet_target; // nekompatibilní typ

			bool actual;

			actual = record.TryGet(fieldName, out TryGet_target);
		}

		/// <summary>
		/// A test for TryGet&lt;&gt; (string, out T)
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TryGetTest_NenalezenoFullLoad()
		{
			DataTable table = new DataTable();
			table.Columns.Add("ColumnName", typeof(int));
			table.Rows.Add(10);

#pragma warning disable 612,618
			DataRecord record = new DataRecord(table.Rows[0]);
#pragma warning restore 612,618

			string fieldName = "ColumnName_Jiny";

			int? TryGet_target;

			bool actual;

			actual = record.TryGet(fieldName, out TryGet_target);
		}

		/// <summary>
		/// A test for TryGet&lt;&gt; (string, out T)
		/// </summary>
		[TestMethod]
		public void TryGetTest_NenalezenoNotFullLoad()
		{
			DataTable table = new DataTable();
			table.Columns.Add("ColumnName", typeof(int));
			table.Rows.Add(10);

#pragma warning disable 612,618
			DataRecord record = new DataRecord(table.Rows[0], false);
#pragma warning restore 612,618
//			record.FullLoad = false;

			string fieldName = "ColumnName_Jiny";

			int? TryGet_target;
			int? target_expected = null;

			bool expected = false;
			bool actual;

			actual = record.TryGet(fieldName, out TryGet_target);

			Assert.AreEqual(target_expected, TryGet_target, "target_TryGet_expected was not set correctly.");
			Assert.AreEqual(expected, actual, "Havit.Data.DataRecord.TryGet<T> did not return the expected value.");
		}
		#endregion
	}

}
