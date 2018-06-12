using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Writers;
using Havit.Web;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class BusinessObjectStoredProcedures
	{
		#region WriteStoredProcedures
		public static void WriteStoredProcedures(CodeWriter writer, Table table)
		{
			Collection<StoredProcedure> procedures = StoredProcedureHelper.GetStoredProcedures(table);
			if (procedures.Count > 0)
			{
				writer.WriteOpenRegion("Stored procedures wrapper");
				foreach (StoredProcedure procedure in procedures)
				{
					WriteStoredProcedureMethod(writer, table, procedure);
				}
				writer.WriteCloseRegion();
			}
		}
		#endregion

		#region WriteStoredProcedureMethod
		private static void WriteStoredProcedureMethod(CodeWriter writer, Table table, StoredProcedure procedure)
		{
			Table resultTypeTable = StoredProcedureHelper.GetResultTypeTable(procedure);

			WriteStoredProcedureMethodHeader(writer, procedure, false);

			StringBuilder line = new StringBuilder();
			bool wasFirst = false;

			if (StoredProcedureHelper.GetResultType(procedure) != StoreProcedureResultType.None)
			{
				line.Append("return ");
			}
			line.Append(ClassHelper.GetClassName(table));
			line.Append(".");
			line.Append(StoredProcedureHelper.GetMethodName(procedure));
			line.Append("(");

			foreach (StoredProcedureParameter parameter in procedure.Parameters)
			{
				if (wasFirst)
				{
					line.Append(", ");
				}

				if (parameter.IsOutputParameter)
				{
					line.Append("out ");
				}
				line.Append(ConventionsHelper.GetCammelCase(parameter.Name.Substring(1)));

				wasFirst = true;
			}

			if (wasFirst)
			{
				line.Append(", ");
			}

			line.Append("null");
			line.Append(");");
			writer.WriteLine(line.ToString());

			writer.WriteLine("}");
			writer.WriteLine();

			WriteStoredProcedureMethodHeader(writer, procedure, true);

			// hlavicka metody - protected void X(int a) - apod., contract a otevírací závorka

			bool datetrimming = false;
			foreach (StoredProcedureParameter parameter in procedure.Parameters)
			{
				if (TypeHelper.IsDateOnly(parameter.DataType))
				{
					datetrimming = true;
					writer.WriteLine(String.Format("{0} = ({0} == null) ? (DateTime?)null : {0}.Value.Date;", ConventionsHelper.GetCammelCase(parameter.Name.Substring(1))));
				}
			}

			if (datetrimming)
			{
				writer.WriteLine();
			}

			writer.WriteLine(String.Format("DbCommand dbCommand = {0}.ProviderFactory.CreateCommand();", DatabaseHelper.GetDbConnector()));
			writer.WriteLine(String.Format("dbCommand.CommandText = \"[{0}].[{1}]\";", procedure.Schema, procedure.Name));
			writer.WriteLine("dbCommand.CommandType = CommandType.StoredProcedure;");
			writer.WriteLine("dbCommand.Transaction = transaction;");
			writer.WriteLine();
			foreach (StoredProcedureParameter parameter in procedure.Parameters)
			{
				string dbParameterName = ConventionsHelper.GetCammelCase(parameter.Name.Substring(1));
				string parameterValue = null;
				if (!parameter.IsOutputParameter)
				{
					if ((parameter.DataType.SqlDataType == SqlDataType.UserDefinedTableType) && (parameter.DataType.Name == "IntTable"))
					{
						parameterValue = String.Format("IntTable.GetSqlParameterValue({0})", dbParameterName);
					}
					else if ((parameter.DataType.SqlDataType == SqlDataType.UserDefinedType) && (parameter.DataType.Name == "IntArray"))
					{
						parameterValue = String.Format("({0} == null) ? DBNull.Value : (object)(new SqlInt32Array({0}))", dbParameterName);
					}
					else
					{
						parameterValue = String.Format("({0} == null) ? DBNull.Value : (object){0}", dbParameterName);
					}
				}
				writer.WriteLines(BusinessObjectSqlParameter.GetSqlParameterCode(parameter.Name.Substring(1), parameter.DataType, parameterValue, parameter.IsOutputParameter ? ParameterDirection.Output : ParameterDirection.Input));
				writer.WriteLine();
			}

			bool hasResult = true;
			switch (StoredProcedureHelper.GetResultType(procedure))
			{
				case StoreProcedureResultType.None:
					{
						writer.WriteLine(String.Format("{0}.ExecuteNonQuery(dbCommand);", DatabaseHelper.GetDbConnector()));
						hasResult = false;
						break;
					}
				case StoreProcedureResultType.Object:
					{
						string dataLoadPower = StoredProcedureHelper.GetDataLoadPower(procedure);

						if (dataLoadPower == "Ghost")
						{
							writer.WriteLine(String.Format("{0} result = null;", ClassHelper.GetClassFullName(resultTypeTable)));
							writer.WriteLine(String.Format("object scalarValue = {0}.ExecuteScalar(dbCommand);", DatabaseHelper.GetDbConnector()));
							writer.WriteLine("if ((scalarValue != null) && (scalarValue != DBNull.Value))");
							writer.WriteLine("{");
							writer.WriteLine(String.Format("result = {0}.GetObject((int)scalarValue);", ClassHelper.GetClassFullName(resultTypeTable)));
							writer.WriteLine("}");
							if ((StoredProcedureHelper.GetDataLoadPower(procedure) == "Ghost") && (StoredProcedureHelper.GetLoadGhosts(procedure)))
							{
								writer.WriteLine();
								writer.WriteLine("if ((result != null) && (!result.IsLoaded))");
								writer.WriteLine("{");
								writer.WriteLine("result.Load(transaction);");
								writer.WriteLine("}");
							}
						}
						else
						{
							writer.WriteLine(String.Format("{0} result = null;", ClassHelper.GetClassFullName(resultTypeTable)));
							writer.WriteLine(String.Format("using (DbDataReader reader = {0}.ExecuteReader(dbCommand))", DatabaseHelper.GetDbConnector()));
							writer.WriteLine("{");
							writer.WriteLine("if (reader.Read())");
							writer.WriteLine("{");
							writer.WriteLine(
								String.Format("DataRecord dataRecord = new DataRecord(reader, DataLoadPower.{0});",
											  StoredProcedureHelper.GetDataLoadPower(procedure)));
							writer.WriteLine(String.Format("result = {0}.GetObject(dataRecord);", ClassHelper.GetClassFullName(resultTypeTable)));
							writer.WriteLine("}");
							writer.WriteLine("}");
						}
						break;
					}
				case StoreProcedureResultType.Collection:
					{
						writer.WriteLine(String.Format("{0} result = new {0}();", ClassHelper.GetCollectionClassFullName(resultTypeTable)));
						writer.WriteLine(String.Format("using (DbDataReader reader = {0}.ExecuteReader(dbCommand))", DatabaseHelper.GetDbConnector()));
						writer.WriteLine("{");
						writer.WriteLine("while (reader.Read())");
						writer.WriteLine("{");
						writer.WriteLine(String.Format("DataRecord dataRecord = new DataRecord(reader, DataLoadPower.{0});", StoredProcedureHelper.GetDataLoadPower(procedure)));
						writer.WriteLine(String.Format("result.Add({0}.GetObject(dataRecord));", ClassHelper.GetClassFullName(resultTypeTable)));
						writer.WriteLine("}");
						writer.WriteLine("}");
						if ((StoredProcedureHelper.GetDataLoadPower(procedure) == "Ghost") && (StoredProcedureHelper.GetLoadGhosts(procedure)))
						{
							writer.WriteLine("result.LoadAll(transaction);");
						}
						break;
					}
				case StoreProcedureResultType.DataTable:
					{
						writer.WriteLine(String.Format("DataTable result = {0}.ExecuteDataTable(dbCommand);", DatabaseHelper.GetDbConnector()));
						break;
					}
				case StoreProcedureResultType.DataSet:
					{
						writer.WriteLine(String.Format("DataSet result = {0}.ExecuteDataSet(dbCommand);", DatabaseHelper.GetDbConnector()));
						break;
					}
				default:
					break; // StyleCop
			}

			writer.WriteLine();

			// kopie hodnot výstupních parametrů SP do výstupních parametrů metody			
			wasFirst = false;
			foreach (StoredProcedureParameter parameter in procedure.Parameters)
			{
				if (parameter.IsOutputParameter)
				{
					wasFirst = true;
					writer.WriteLine(String.Format("{0} = (dbParameter{1}.Value == DBNull.Value) ? null : ({2})dbParameter{1}.Value;",
						ConventionsHelper.GetCammelCase(parameter.Name.Substring(1)),
						parameter.Name.Substring(1),
						TypeHelper.GetFieldSystemTypeName(parameter.DataType, true)));
				}
			}

			if (wasFirst)
			{
				writer.WriteLine();
			}

			if (hasResult)
			{
				writer.WriteLine("return result;");
			}
			writer.WriteLine("}");
			writer.WriteLine();

		}
		#endregion

		#region WriteStoredProcedureMethodHeader
		private static void WriteStoredProcedureMethodHeader(CodeWriter writer, StoredProcedure procedure, bool withTransactionParameter)
		{
			Table resultTypeTable = StoredProcedureHelper.GetResultTypeTable(procedure);

			writer.WriteCommentSummary(ExtendedPropertiesHelper.GetDescription(ExtendedPropertiesKey.FromStoredProcedure(procedure)));
			if (withTransactionParameter)
			{
				writer.WriteCommentLine("<remarks>");
				writer.WriteCommentLine("<code>");
				StringCollection createScript = procedure.Script();
				foreach (string scriptLine in createScript)
				{
					writer.WriteCommentLine(HttpUtilityExt.HtmlEncode(scriptLine, HtmlEncodeOptions.IgnoreNonASCIICharacters));
				}
				writer.WriteCommentLine("</code>");
				writer.WriteCommentLine("</remarks>");
			}
			// seskládání hlavičky metody
			StringBuilder line = new StringBuilder();
			line.Append(StoredProcedureHelper.GetMethodAccessModifier(procedure));
			line.Append(" static ");

			switch (StoredProcedureHelper.GetResultType(procedure))
			{
				case StoreProcedureResultType.None:
					{
						line.Append("void");
						break;
					}
				case StoreProcedureResultType.Object:
					{
						line.Append(ClassHelper.GetClassFullName(resultTypeTable));
						break;
					}
				case StoreProcedureResultType.Collection:
					{
						line.Append(ClassHelper.GetCollectionClassFullName(resultTypeTable));
						break;
					}
				case StoreProcedureResultType.DataTable:
					{
						line.Append("DataTable");
						break;
					}
				case StoreProcedureResultType.DataSet:
					{
						line.Append("DataSet");
						break;
					}
				default:
					{
						throw new ApplicationException("Neznámá hodnota typu StoreProcedureResultType.");
					}
			}
			line.Append(" ");
			line.Append(StoredProcedureHelper.GetMethodName(procedure));
			line.Append("(");

			bool wasFirst = false;
			foreach (StoredProcedureParameter parameter in procedure.Parameters)
			{
				if (wasFirst)
				{
					line.Append(", ");
				}

				if (parameter.IsOutputParameter)
				{
					line.Append("out ");
				}

				line.Append(TypeHelper.GetFieldSystemTypeName(parameter.DataType, true));
				line.Append(" ");
				line.Append(ConventionsHelper.GetCammelCase(parameter.Name.Substring(1)));

				wasFirst = true;
			}

			if (withTransactionParameter)
			{
				if (wasFirst)
				{
					line.Append(", ");
				}
				line.Append("DbTransaction transaction");
			}

			line.Append(")");
			writer.WriteLine(line.ToString());
			writer.WriteLine("{");

			if (StoredProcedureHelper.GetResultType(procedure) == StoreProcedureResultType.Collection)
			{
				writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetCollectionClassFullName(resultTypeTable)));
				writer.WriteMicrosoftContract("");
			}

		}
		#endregion
	}
}
