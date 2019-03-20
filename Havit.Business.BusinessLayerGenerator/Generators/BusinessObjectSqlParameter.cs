using System;
using System.Collections.Generic;
using System.Data;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Settings;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class BusinessObjectSqlParameter
	{
		/// <summary>
		/// Vygeneruje kód pro přidání parametru.
		/// </summary>
		public static void WriteSqlParameter(CodeWriter writer, Column column)
		{
			WriteSqlParameter(writer, column, ColumnHelper.GetParameterValue(column));
		}

		/// <summary>
		/// Vygeneruje kód pro přidání parametru se zadanou hodnotou (kód v C# pro získání hodnoty).
		/// </summary>
		public static void WriteSqlParameter(CodeWriter writer, Column column, string parameterValue)
		{
			writer.WriteLines(GetSqlParameterCode(column.Name, column.DataType, parameterValue, ParameterDirection.Input));
		}

		/// <summary>
		/// Vrátí kód pro vytvoření instance sql parametru.
		/// </summary>
		public static string[] GetSqlParameterCode(string parameterName, DataType parameterDataType, string parameterValue, ParameterDirection parameterDirection)
		{
			List<String> lines = new List<string>();

			string dbParameterName = String.Format("dbParameter{0}", parameterName);

			SqlDbType sqlDbType = TypeHelper.ToSqlDbType(parameterDataType);

			bool userDefinedType = (sqlDbType == SqlDbType.Udt) /* IntArray, Geography, Geometry */
								   || (sqlDbType == SqlDbType.Structured) /* TableValueParameter */;

			if (!userDefinedType)
			{
				lines.Add(String.Format("DbParameter {0} = {1}.ProviderFactory.CreateParameter();", dbParameterName,
										DatabaseHelper.GetDbConnector()));

				if (TypeHelper.ToDbType(parameterDataType) == DbType.Time)
				{
					lines.Add(String.Format("// {0}.DbType = DbType.{1}; // Workaround: http://connect.microsoft.com/VisualStudio/feedback/details/381934", dbParameterName, TypeHelper.ToDbType(parameterDataType).ToString()));
				}
				else
				{
					lines.Add(String.Format("{0}.DbType = DbType.{1};", dbParameterName, TypeHelper.ToDbType(parameterDataType).ToString()));
				}

				switch (parameterDataType.SqlDataType)
				{
					case SqlDataType.NVarCharMax:
					case SqlDataType.VarCharMax:
					case SqlDataType.NVarChar:
					case SqlDataType.VarChar:
						lines.Add(String.Format("{0}.Size = {1};", dbParameterName, parameterDataType.MaximumLength));
						break;
					default:
						break; // StyleCop
				}
			}
			else
			{
				lines.Add(String.Format("SqlParameter {0} = new SqlParameter();", dbParameterName));
				switch (sqlDbType)
				{
					case SqlDbType.Udt:
						lines.Add(String.Format("{0}.SqlDbType = SqlDbType.Udt;", dbParameterName));
						lines.Add(String.Format("{0}.UdtTypeName = \"{1}\";", dbParameterName, parameterDataType.Name));
						break;

					case SqlDbType.Structured:
						lines.Add(String.Format("{0}.SqlDbType = SqlDbType.Structured;", dbParameterName));
						lines.Add(String.Format("{0}.TypeName = \"{1}\";", dbParameterName, parameterDataType.Name));
						break;

					default: throw new ApplicationException("Chyba v aplikaci - nenalezena sekce pro user defined type.");
				}
			}

			lines.Add(String.Format("{0}.Direction = ParameterDirection.{1};", dbParameterName, parameterDirection.ToString()));
			lines.Add(String.Format("{0}.ParameterName = \"{1}\";", dbParameterName, parameterName));
			if (!String.IsNullOrEmpty(parameterValue))
			{
				lines.Add(String.Format("{0}.Value = {1};", dbParameterName, parameterValue));
			}
			lines.Add(String.Format("dbCommand.Parameters.Add({0});", dbParameterName));

			return lines.ToArray();
		}

		/// <summary>
		/// Vygeneruje kód pro vytvoření parametru typu kolekce.
		/// </summary>
		public static void WriteCollectionSqlParameter(CodeWriter writer, CollectionProperty collectionProperty)
		{
			WriteCollectionSqlParameter(writer, collectionProperty, String.Format("this.{0}.Value.GetIDs()", PropertyHelper.GetPropertyHolderName(collectionProperty.PropertyName)));
		}

		/// <summary>
		/// Vygeneruje kód pro vytvoření parametru typu kolekce.
		/// </summary>
		public static void WriteCollectionSqlParameter(CodeWriter writer, CollectionProperty collectionProperty, string value)
		{
			string sqlParameterName = String.Format("dbParameter{0}", collectionProperty.PropertyName);

			if (GeneratorSettings.TargetPlatform >= TargetPlatform.SqlServer2008)
			{
				writer.WriteLine(String.Format("SqlParameter {0} = new SqlParameter(\"{1}\", SqlDbType.Structured);", sqlParameterName, collectionProperty.PropertyName));
				writer.WriteLine(String.Format("{0}.TypeName = \"dbo.IntTable\";", sqlParameterName));
				writer.WriteLine(String.Format("{0}.Value = IntTable.GetSqlParameterValue({1});", sqlParameterName, value));
				writer.WriteLine(String.Format("dbCommand.Parameters.Add({0});", sqlParameterName));				
			}
			else
			{
				writer.WriteLine(String.Format("SqlParameter {0} = new SqlParameter(\"{1}\", SqlDbType.Udt);", sqlParameterName, collectionProperty.PropertyName));
				writer.WriteLine(String.Format("{0}.UdtTypeName = \"IntArray\";", sqlParameterName));
				writer.WriteLine(String.Format("{0}.Value = new SqlInt32Array({1});", sqlParameterName, value));
				writer.WriteLine(String.Format("dbCommand.Parameters.Add({0});", sqlParameterName));
			}
		}

		public static void WriteDeletedDateTimeSqlParameter(CodeWriter writer)
		{
			writer.WriteLines(BusinessObjectSqlParameter.GetSqlParameterCode("DeletedDateTime", DataType.DateTime, ApplicationTimeHelper.GetApplicationCurrentTimeExpression(), ParameterDirection.Input));
		}
	}
}
