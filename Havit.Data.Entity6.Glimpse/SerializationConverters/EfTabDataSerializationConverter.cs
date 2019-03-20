using System;
using System.Collections.Generic;
using System.Data.Common;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Tab.Assist;

using Havit.Data.Entity.Glimpse.Model;

namespace Havit.Data.Entity.Glimpse.SerializationConverters
{
	/// <summary>
	/// Formátuje DbCommandTraceData k zobrazení.
	/// </summary>
	public class EfTabDataSerializationConverter : SerializationConverter<EntityFrameworkTabData>
	{
		public override object Convert(EntityFrameworkTabData data)
		{
			var section = new TabSection("Operation", "Command Text", "Parameters", "Async", "Result", "Duration"); // záhlaví sloupců

			foreach (DbCommandMessage message in data.Commands) // řádky s daty
			{		
				TabSectionRow row = section.AddRow()
					.Column(message.Operation)
					.Column(message.CommandText)
					.Column(GetParametersSection(message.CommandParameters))
					.Column(message.IsAsync ? "yes" : "no")
					.Column(GetDisplayValue(message.Exception ?? message.Result))
					.Column(message.Duration);

				row.ErrorIf(message.Exception != null); // pokud je výjimka, zobrazíme řádek jako chybu
			}
			
			return section.Build();
		}

		/// <summary>
		/// Converts data for DbCommandTraceData.Parameters.
		/// </summary>
		private object GetParametersSection(List<DbParameterData> parameters)
		{
			if (parameters.Count == 0)
			{
				return String.Empty;
			}

			var section = new TabSection("Name", "Value", "Direction", "DbType");
			foreach (DbParameterData dbParameterTraceData in parameters)
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
			else if (data is DbDataReader)
			{
				DbDataReader dbDataReader = ((DbDataReader)data);
				return (dbDataReader.IsClosed && (dbDataReader.RecordsAffected >= 0)) ? String.Format("{0} record(s)", dbDataReader.RecordsAffected) : "unknown";
			}
			else
			{
				return data;
			}
		}
	}
}
