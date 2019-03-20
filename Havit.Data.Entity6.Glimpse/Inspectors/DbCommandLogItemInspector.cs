using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;
using Glimpse.Core.SerializationConverter;

using Havit.Data.Entity.Glimpse.DbCommandInterception;
using Havit.Data.Entity.Glimpse.Model;

namespace Havit.Data.Entity.Glimpse.Inspectors
{
	/// <summary>
	/// Zpracovává položky DbCommandLogItem do DbCommandMessage.
	/// </summary>
	public class DbCommandLogItemInspector : IInspector, IDisposable
	{
		private IMessageBroker messageBroker;
		private Func<IExecutionTimer> timerStrategy;
		private DbCommandLoggingInterceptor dbCommandLoggingInterceptor;

		public void Setup(IInspectorContext context)
		{
			messageBroker = context.MessageBroker;
			timerStrategy = context.TimerStrategy;

			context.MessageBroker.Subscribe<DbCommandLogItem>(ProcessMessage);

			dbCommandLoggingInterceptor = new DbCommandLoggingInterceptor(messageBroker);
            DbInterception.Add(dbCommandLoggingInterceptor);
		}

		private void ProcessMessage(DbCommandLogItem dbCommandLogItem)
		{
			DateTime now = DateTime.Now;
			DbCommandMessage message = new DbCommandMessage();

			IExecutionTimer timer = null;
			if (timerStrategy != null)
			{
				timer = timerStrategy();
				if (timer != null)
				{
					TimerResult timePoint = timer.Point();
					message.Offset = new TimeSpan(timePoint.Offset.Ticks - dbCommandLogItem.DurationTicks);
				}

				message.Operation = dbCommandLogItem.Operation;
				message.CommandText = dbCommandLogItem.Command.CommandText;
				foreach (DbParameter dbParameter in dbCommandLogItem.Command.Parameters)
				{
					message.CommandParameters.Add(DbParameterData.Create(dbParameter));
				}
				message.Result = dbCommandLogItem.Result;
				message.IsAsync = dbCommandLogItem.IsAsync;

                message.StartTime = now - message.Duration;
				message.Duration = new TimeSpan(dbCommandLogItem.DurationTicks);

				messageBroker.Publish(message);
			}
		}

		public void Dispose()
		{
			if (dbCommandLoggingInterceptor != null)
			{
				DbInterception.Remove(dbCommandLoggingInterceptor);
				dbCommandLoggingInterceptor = null;
			}
		}
	}
}
