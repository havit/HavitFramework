using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EFCore.Tests
{
	[TestClass]
	public class DbUpdateExceptionExtensionsTests
	{
		[TestMethod]
		public void DbUpdateExceptionExtensions_FormatErrorMessage_WithInnerException()
		{
			// Act
			string result = DbUpdateExceptionExtensions.FormatErrorMessage(new DbUpdateException("A", new ApplicationException("B")));
			
			// Assert
			Assert.AreEqual("A B", result);
		}

		[TestMethod]
		public void DbUpdateExceptionExtensions_FormatErrorMessage_WithoutInnerException()
		{
			// Act
			string result = DbUpdateExceptionExtensions.FormatErrorMessage(new DbUpdateException("A", (Exception)null));
			
			// Assert
			Assert.AreEqual("A", result);
		}
	}
}
