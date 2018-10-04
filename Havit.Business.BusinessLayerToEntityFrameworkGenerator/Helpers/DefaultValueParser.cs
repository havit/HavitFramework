using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Helpers
{
	public class DefaultValueParser
	{
		private static readonly Dictionary<SqlDataType, Func<DefaultConstraint, string>> defaultValueParserFuncs = new Dictionary<SqlDataType, Func<DefaultConstraint, string>>
		{
			{ SqlDataType.DateTime, DateTimeParser },
			{ SqlDataType.DateTime2, DateTimeParser },
			{ SqlDataType.SmallDateTime, DateTimeParser }
		};

		public string GetDefaultValue(Column column)
		{
			if (defaultValueParserFuncs.TryGetValue(column.DataType.SqlDataType, out var provider))
			{
				return provider(column.DefaultConstraint);
			}

			return column.DefaultConstraint.Text;
		}

		private static string DateTimeParser(DefaultConstraint constraint)
		{
			// (((2010)-(1))-(1))

			Match match = Regex.Match(constraint.Text, @"\((\d+)\)-\((\d+)\)\)-\((\d+)\)");
			if (match.Success)
			{
				return $"({match.Groups[1].Value}-{match.Groups[2].Value}-{match.Groups[3].Value})";
			}

			return constraint.Text;
		}
	}
}