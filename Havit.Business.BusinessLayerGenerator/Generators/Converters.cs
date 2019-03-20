using System;
using System.Collections.Generic;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class Converters
	{
		public static void WriteConverters(CodeWriter writer, Table table)
		{
			List<Column> list = new List<Column>();
			foreach (Column column in TableHelper.GetNotIgnoredColumns(table))
			{
				if (TypeHelper.IsNonstandardType(column))
				{
					string converter = TypeHelper.GetNonstandarPropertyTypeConverterName(column);
					if (!String.IsNullOrEmpty(converter))
					{
						list.Add(column);
					}
				}
			}

			if (list.Count > 0)
			{
				writer.WriteOpenRegion("Type converters (static)");

				foreach (Column column in list)
				{
					writer.WriteLine(String.Format("private static {0} {1} = new {0}();", TypeHelper.GetNonstandarPropertyTypeConverterName(column), ConverterHelper.GetFieldConvertorName(column)));
				}
				writer.WriteCloseRegion();
			}
		}
	}
}
