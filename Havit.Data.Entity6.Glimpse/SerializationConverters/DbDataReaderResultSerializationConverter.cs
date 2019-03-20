using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glimpse.Core.Extensibility;

using Havit.Data.Entity.Glimpse.DbCommandInterception;

namespace Havit.Data.Entity.Glimpse.SerializationConverters
{
	/// <summary>
	/// Formátuje DbDataReaderResult k zobrazení.
	/// </summary>
	internal class DbDataReaderResultSerializationConverter : SerializationConverter<DbDataReaderResult>
	{
		public override object Convert(DbDataReaderResult dbDataReaderResult)
		{
			return (dbDataReaderResult.RecordsCount == null)
				? String.Empty
				: String.Format("{0} record{1}", dbDataReaderResult.RecordsCount.Value, (dbDataReaderResult.RecordsCount.Value == 1) ? "" : "s");
		}
	}
}
