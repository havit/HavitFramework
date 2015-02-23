using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web.UI;

using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;

namespace Havit.Entity.Glimpse.DbCommandInterception
{
	/// <summary>
	/// DbCommandInterceptor oznamující Glimpse (prostřednictvím MessageBrokeru) provedené dotazy, jejich trvání a výsledky.
	/// </summary>
	internal class DbCommandLoggingInterceptor : IDbCommandInterceptor
	{
		#region Private fields
		private IMessageBroker messageBroker;
		#endregion

		#region Constructor
		public DbCommandLoggingInterceptor(IMessageBroker messageBroker)
		{
			this.messageBroker = messageBroker;
		}
		#endregion

		#region NonQueryExecuting, NonQueryExecuted
		public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
		{
			StoreUserData(interceptionContext);
		}

		public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
		{
			InterceptionUserData interceptionUserData = RestoreUserData(interceptionContext);
			LogOperation("ExecuteNonQuery", command, interceptionContext.IsAsync, interceptionContext.Exception, interceptionContext.Result, interceptionUserData.Stopwatch.ElapsedTicks);
        }
		#endregion

		#region ReaderExecuting, ReaderExecuted
		public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
		{
			StoreUserData(interceptionContext);
		}

		public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
		{
			InterceptionUserData interceptionUserData = RestoreUserData(interceptionContext);

			object result = interceptionContext.OriginalResult;
			if (result != null)
			{
				DbDataReader dataReader = (DbDataReader)result;
				DbDataReaderResult dataReaderResult = new DbDataReaderResult();
				interceptionContext.Result = new RecordCountingDbDataReader(dataReader, dataReaderResult);

				result = dataReaderResult;
			}

			LogOperation("ExecuteReader", command, interceptionContext.IsAsync, interceptionContext.Exception, result, interceptionUserData.Stopwatch.ElapsedTicks);
		}
		#endregion

		#region ScalarExecuting, ScalarExecuted
		public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
		{
			StoreUserData(interceptionContext);
		}

		public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
		{
			InterceptionUserData interceptionUserData = RestoreUserData(interceptionContext);
			LogOperation("ScalarExecuted", command, interceptionContext.IsAsync, interceptionContext.Exception, interceptionContext.Result, interceptionUserData.Stopwatch.ElapsedTicks);
		}
		#endregion

		#region StoreUserData, RestoreUserData
		/// <summary>
		/// Uloží do UserData interceptionContextu stopky a zajistí (spolu s RestoreUserData), aby dosavadní hodnota ostatních interceptorů v UserState byla zachována.
		/// </summary>
		private void StoreUserData<T>(DbCommandInterceptionContext<T> interceptionContext)
		{
			interceptionContext.UserState = new InterceptionUserData { OriginalUserData = interceptionContext.UserState, Stopwatch = Stopwatch.StartNew() };
		}

		/// <summary>
		/// Vyzvedne z UserData interceptionContextu stopky, zastaví je. Zajistí, aby dosavadní hodnota ostatních interceptorů v UserState byla zachována.
		/// </summary>
		private InterceptionUserData RestoreUserData<T>(DbCommandInterceptionContext<T> interceptionContext)
		{
			InterceptionUserData interceptionUserData = (InterceptionUserData)interceptionContext.UserState;
			interceptionUserData.Stopwatch.Stop();

			interceptionContext.UserState = interceptionUserData.OriginalUserData;

			return interceptionUserData;
		}
		#endregion

		#region LogOperation
		/// <summary>
		/// Zapíše zprávu o provedeném databázovém dotazu do MessageBrokeru.
		/// </summary>
		private void LogOperation(string operation, DbCommand command, bool isAsync, Exception exception, object result, long durationTicks)
		{
			DbCommandLogItem logItem = new DbCommandLogItem
			{
				Operation = operation,
				Command = command,
				IsAsync = isAsync,
				Exception = exception,
                Result = result,
				DurationTicks = durationTicks
			};

			lock (messageBroker)
			{
				messageBroker.Publish(logItem);
			}
		}
		#endregion LogDbCommand
	}
}
