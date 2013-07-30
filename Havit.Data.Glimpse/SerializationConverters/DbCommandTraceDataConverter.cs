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
		#region Convert
		/// <summary>
		/// Converts the specified object.
		/// </summary>
		/// <param name="dbCommandTraceItems">The object to transform.</param>
		/// <returns>The new object representation.</returns>
		public override object Convert(IEnumerable<Havit.Data.Trace.DbCommandTraceData> dbCommandTraceItems)
		{
			var section = new TabSection("Operation", "Command Text", "Parameters", "Duration");

			foreach (DbCommandTraceData dbCommandTraceData in dbCommandTraceItems)
			{
				section.AddRow()
					.Column(dbCommandTraceData.Operation)
					.Column(dbCommandTraceData.CommandText)
					.Column(GetParametersSection(dbCommandTraceData))
					.Column(dbCommandTraceData.Duration);
			}

			return section.Build();
		}
		#endregion

		#region GetParametersSection
		/// <summary>
		/// Converts data for DbCommandTraceData.Parameters.
		/// </summary>
		private object GetParametersSection(Trace.DbCommandTraceData dbCommandTraceData)
		{
			if (dbCommandTraceData.Parameters.Count == 0)
			{
				return null;
			}

			var section = new TabSection("Name", "Value", "Direction", "DbType");
			foreach (DbParameterTraceData dbParameterTraceData in dbCommandTraceData.Parameters)
			{
				object value;
				bool valueSpecialCase;

				if (dbParameterTraceData.Value == null)
				{
					value = "null";
					valueSpecialCase = true;
				}
				else if (dbParameterTraceData.Value == DBNull.Value)
				{
					value = "DBNull.Value";
					valueSpecialCase = true;
				}
				else
				{
					value = dbParameterTraceData.Value;
					valueSpecialCase = false;
				}

				section.AddRow()
					.Column(dbParameterTraceData.ParameterName)
					.Column(value).EmphasisIf(valueSpecialCase)
					.Column(dbParameterTraceData.Direction.ToString())
					.Column(dbParameterTraceData.DbType.ToString());
			}
			return section;

		}
		#endregion
	}
}
