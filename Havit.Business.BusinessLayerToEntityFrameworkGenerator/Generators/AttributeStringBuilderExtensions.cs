using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators
{
	public static class AttributeStringBuilderExtensions
	{
		public static AttributeStringBuilder AddExtendedProperty(this AttributeStringBuilder builder, Table table, string propertyPrefix, string propertyName)
		{
			return AddExtendedProperty(builder, table, propertyPrefix, propertyName, value => value);
		}
		public static AttributeStringBuilder AddExtendedProperty(this AttributeStringBuilder builder, Table table, string propertyPrefix, string propertyName, Func<string, string> factory)
		{
			string value = TableHelper.GetStringExtendedProperty(table, $"{propertyPrefix}_{propertyName}");
			if (string.IsNullOrEmpty(value))
			{
				return builder;
			}

			builder.AddParameter(propertyName, factory(value));

			return builder;
		}

		public static AttributeStringBuilder AddBoolExtendedProperty(this AttributeStringBuilder builder, Table table, string propertyPrefix, string propertyName)
		{
			bool? value = TableHelper.GetBoolExtendedProperty(table, $"{propertyPrefix}_{propertyName}");
			if (value == true)
			{
				builder.AddParameter(propertyName, "true");
			}

			return builder;
		}
		public static AttributeStringBuilder AddExtendedProperty(this AttributeStringBuilder builder, Column column, string propertyPrefix, string propertyName)
		{
			string value = ColumnHelper.GetStringExtendedProperty(column, $"{propertyPrefix}_{propertyName}");
			if (string.IsNullOrEmpty(value))
			{
				return builder;
			}

			builder.AddParameter(propertyName, value);

			return builder;
		}

		public static AttributeStringBuilder AddBoolExtendedProperty(this AttributeStringBuilder builder, Column column, string propertyPrefix, string propertyName)
		{
			bool? value = ColumnHelper.GetBoolExtendedProperty(column, $"{propertyPrefix}_{propertyName}");
			if (value == false)
			{
				return builder;
			}

			builder.AddParameter(propertyName, "true");

			return builder;
		}
	}
}