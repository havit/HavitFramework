using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;
using Havit.Data.Trace;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Data.Glimpse.SerializationConverters
{
	/// <summary>
	/// Converts DbCommandTraceData.
	/// </summary>
	public class DbCommandTraceDataSerializationConverter : SerializationConverter<IEnumerable<Havit.Data.Trace.DbCommandTraceData>>
	{
		/// <summary>
		/// Converts the specified object.
		/// </summary>
		/// <param name="dbCommandTraceItems">The object to transform.</param>
		/// <returns>The new object representation.</returns>
		public override object Convert(IEnumerable<Havit.Data.Trace.DbCommandTraceData> dbCommandTraceItems)
		{
			var section = new TabSection("Operation", "Command Text", "Parameters", "Tx", "Result", "Duration");

			Dictionary<int, int> transactions = new Dictionary<int, int>();
			foreach (DbCommandTraceData dbCommandTraceData in dbCommandTraceItems)
			{
				string transactionId = String.Empty;
				if (dbCommandTraceData.TransactionHashCode != null)
				{
					int tmp;
					if (!transactions.TryGetValue(dbCommandTraceData.TransactionHashCode.Value, out tmp))
					{
						tmp = transactions.Count + 1;
						transactions.Add(dbCommandTraceData.TransactionHashCode.Value, tmp);
					}
					transactionId = tmp.ToString();
				}
			
				TabSectionRow row = section.AddRow()
					.Column(dbCommandTraceData.Operation)
					.Column(dbCommandTraceData.CommandText)
					.Column(GetParametersSection(dbCommandTraceData))
					.Column(transactionId)
					.Column(dbCommandTraceData.ResultSet ? GetDisplayValue(dbCommandTraceData.Result) : String.Empty)
					.Column(new TimeSpan(dbCommandTraceData.DurationTicks));

				long durationMs = dbCommandTraceData.DurationTicks / TimeSpan.TicksPerMillisecond;
				row.WarnIf(durationMs >= 50);
				row.ErrorIf(durationMs >= 500);
			}

			return section.Build();
		}

		/// <summary>
		/// Converts data for DbCommandTraceData.Parameters.
		/// </summary>
		private object GetParametersSection(Trace.DbCommandTraceData dbCommandTraceData)
		{
			if (dbCommandTraceData.Parameters.Count == 0)
			{
				return String.Empty;
			}

			var section = new TabSection("Name", "Value", "Direction", "DbType");
			foreach (DbParameterTraceData dbParameterTraceData in dbCommandTraceData.Parameters)
			{
				section.AddRow()
					.Column(dbParameterTraceData.ParameterName)
					.Column(GetDisplayValue(dbParameterTraceData.Value))
					.Column(dbParameterTraceData.Direction.ToString())
					.Column(dbParameterTraceData.DbType.ToString());				
			}
			return section;
		}

		/// <summary>
		/// Konvertuje hodnotu null a DBNull.Value na zobrazitelný text.
		/// </summary>
		private object GetDisplayValue(object data)
		{
			if (data == null)
			{
				return "null";
			}
			else if (data == DBNull.Value)
			{
				return "DBNull.Value";
			}
			else
			{
				return data;
			}

		}
	}
}
