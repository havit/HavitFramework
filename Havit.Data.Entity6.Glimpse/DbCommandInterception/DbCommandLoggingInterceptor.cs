using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web.UI;

using Glimpse.Core.Extensibility;
using Glimpse.Core.Framework;

namespace Havit.Data.Entity.Glimpse.DbCommandInterception
{
	/// <summary>
	/// DbCommandInterceptor oznamující Glimpse (prostřednictvím MessageBrokeru) provedené dotazy, jejich trvání a výsledky.
	/// </summary>
	internal class DbCommandLoggingInterceptor : IDbCommandInterceptor
	{
		private const string InterceptorUserStateKey = nameof(DbCommandLoggingInterceptor);

		private readonly IMessageBroker messageBroker;

		public DbCommandLoggingInterceptor(IMessageBroker messageBroker)
		{
			this.messageBroker = messageBroker;
		}

		public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
		{
			StoreUserData(interceptionContext);
		}

		public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
		{
			InterceptionUserData interceptionUserData = RestoreUserData(interceptionContext);
			LogOperation("ExecuteNonQuery", command, interceptionContext.IsAsync, interceptionContext.Exception, interceptionContext.Result, interceptionUserData.Stopwatch.ElapsedTicks);
        }

		public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
		{
			StoreUserData(interceptionContext);
		}

		public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
		{
			InterceptionUserData interceptionUserData = RestoreUserData(interceptionContext);

			// kvůli chybě v EF, který se jinak chová k SqlDataReaderu a jinak k ostatním readerům
			// nemůžeme DbDataReader vyměnit za náš měřící interceptor, neboť pak dojde při některých dotazech k pádu aplikace
			// bohužel tak nemůžeme měřit počet záznamů vrácených SELECt dotazy

			LogOperation("ExecuteReader", command, interceptionContext.IsAsync, interceptionContext.Exception, interceptionContext.OriginalResult, interceptionUserData.Stopwatch.ElapsedTicks);
		}

		public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
		{
			StoreUserData(interceptionContext);
		}

		public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
		{
			InterceptionUserData interceptionUserData = RestoreUserData(interceptionContext);
			LogOperation("ScalarExecuted", command, interceptionContext.IsAsync, interceptionContext.Exception, interceptionContext.Result, interceptionUserData.Stopwatch.ElapsedTicks);
		}

		/// <summary>
		/// Uloží do UserData interceptionContextu stopky a zajistí (spolu s RestoreUserData), aby dosavadní hodnota ostatních interceptorů v UserState byla zachována.
		/// </summary>
		private void StoreUserData<T>(DbCommandInterceptionContext<T> interceptionContext)
		{
			interceptionContext.SetUserState(InterceptorUserStateKey, new InterceptionUserData { Stopwatch = Stopwatch.StartNew() });
		}

		/// <summary>
		/// Vyzvedne z UserData interceptionContextu stopky, zastaví je. Zajistí, aby dosavadní hodnota ostatních interceptorů v UserState byla zachována.
		/// </summary>
		private InterceptionUserData RestoreUserData<T>(DbCommandInterceptionContext<T> interceptionContext)
		{
			InterceptionUserData interceptionUserData = (InterceptionUserData)interceptionContext.FindUserState(InterceptorUserStateKey);
			interceptionUserData.Stopwatch.Stop();

			return interceptionUserData;
		}

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
	}
}
