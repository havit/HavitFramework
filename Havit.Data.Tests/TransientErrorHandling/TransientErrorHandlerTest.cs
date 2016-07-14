﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.TransientErrorHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Tests.TransientErrorHandling
{
	[TestClass]
	public class TransientErrorHandlerTest
	{
		[TestMethod]
		public void TransientErrorHandler_ExecuteAction_RetriesWithDelaysUpToMaxAttempts()
		{
			// Arrange
			SqlException sqlException = SqlExceptionHelper.CreateSqlException(10928);
			Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(sqlException)); // prerequisite

			// Act
			int maxAttempts = 3;
			int delayMs = 500;
			TransientErrorRetryPolicy policy = new TransientErrorRetryPolicy(maxAttempts, new int[] { delayMs });
			int i = 0;

			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				TransientErrorHandler.ExecuteAction<object>(
					() =>
					{
						i += 1;
						throw sqlException;
					},
					() => true,
					policy);
			}
			catch (Exception e)
			{
				if (e != sqlException)
				{
					throw;
				}
			}
			stopwatch.Stop();

			// Assert
			Assert.AreEqual(maxAttempts, 3, "Max attempts");
			Assert.IsTrue(stopwatch.ElapsedMilliseconds >= ((maxAttempts - 1) * delayMs), "Delay - lower limit");
			Assert.IsTrue(stopwatch.ElapsedMilliseconds < (maxAttempts * delayMs), "Delay - upper limit");
		}

		[TestMethod]
		public void TransientErrorHandler_ExecuteAction_DoesNotRetryWhenNotPossible()
		{
			// Arrange
			SqlException sqlException = SqlExceptionHelper.CreateSqlException(10928);
			Assert.IsTrue(SqlDatabaseTransientErrorDetectionStrategy.IsTransient(sqlException)); // prerequisite

			// Act
			TransientErrorRetryPolicy policy = new TransientErrorRetryPolicy(3, new int[] { 0 });

			int i = 0;
			try
			{
				TransientErrorHandler.ExecuteAction<object>(
					() =>
					{
						i += 1;
						throw sqlException;
					},
					() => false, // nepovolujeme opakování
					policy);
			}
			catch (Exception e)
			{
				if (e != sqlException)
				{
					throw;
				}
			}

			// Assert
			Assert.AreEqual(1, i, "Attempts"); // nedošlo k opakování, přestože jde o transientní výjimku
		}
	}
}