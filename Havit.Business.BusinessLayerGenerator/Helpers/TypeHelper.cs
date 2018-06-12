using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Settings;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class TypeHelper
	{
		#region GetFieldSystemTypeName
		/// <summary>
		/// Získá systémový typ na základě typu sloupce;
		/// </summary>
		public static string GetFieldSystemTypeName(Column column)
		{
			return GetFieldSystemTypeName(column.DataType, column.Nullable);
		}
		#endregion

		#region GetFieldSystemTypeName
		/// <summary>
		/// Získá systémový typ na základě SqlDbType (string) a příznaku, zda je povoleno ukládání hodnoty null;
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1107:CodeMustNotContainMultipleStatementsOnOneLine", Justification = "Pro lepší čitelnost zde chceme v switch/case přiřazení a break na jednom řádku.")]
		public static string GetFieldSystemTypeName(DataType dataType, bool nullable)
		{
			string result = null;

			if (dataType.SqlDataType == SqlDataType.UserDefinedDataType)
			{
				//Havit.BusinessLayerGenerator.Program.Database.UserDefinedDataTypes[column.DataType.Name];
				string systemType = DatabaseHelper.Database.UserDefinedDataTypes[dataType.Name].SystemType;

				switch (systemType)
				{
					case "int":
						result = "Int32?";
						break;

					case "numeric":
						result = "Decimal?";
						break;

					case "datetime":
						result = "DateTime?";
						break;

					case "smalldatetime":
						result = "DateTime?";
						break;

					case "uniqueidentifier":
						result = "Guid?";
						break;

					case "varbinary":
						result = "byte[]";
						break;

					case "nvarchar":
						result = "string";
						break;

					default:
						throw new ApplicationException(String.Format("Nepodařilo se přeložit SqlDataType.UserDefinedDataType \"{0}\" na systémový typ.", systemType));
				}
			}
			else if (dataType.SqlDataType == SqlDataType.UserDefinedType)
			{
				switch (dataType.Name)
				{
					case "IntArray":
						result = "int[]";
						break;

					default:
						throw new ApplicationException(String.Format("Nepodařilo se přeložit UserDefinedType \"{0}\" na systémový typ.", dataType.Name));
				}
			}
			else if (dataType.SqlDataType == SqlDataType.UserDefinedTableType)
			{
				switch (dataType.Name)
				{
					case "IntTable":
						result = "int[]";
						break;

					default:
						throw new ApplicationException(String.Format("Nepodařilo se přeložit UserDefinedTableType \"{0}\" na systémový typ.", dataType.Name));
				}
			}
			else
			{
				switch (dataType.SqlDataType)
				{
					case SqlDataType.BigInt:
						result = "Int64?";
						break;

					case SqlDataType.Binary:
						result = "byte[]";
						break;

					case SqlDataType.Bit:
						result = "bool?";
						break;

					case SqlDataType.Char:
						result = "string";
						break; // poznámka: Pro Char, NChar  délky jedna  by to  byl "char?", obecně je string...

					case SqlDataType.Date:
						result = "DateTime?";
						break;

					case SqlDataType.DateTime:
						result = "DateTime?";
						break;

					case SqlDataType.DateTime2:
						result = "DateTime?";
						break;

					case SqlDataType.DateTimeOffset:
						result = "DateTimeOffset?";
						break;

					case SqlDataType.Decimal:
						result = "Decimal?";
						break;

					case SqlDataType.Float:
						result = "Double?";
						break;

					case SqlDataType.Geography:
						result = "Microsoft.SqlServer.Types.SqlGeography";
						break;

					case SqlDataType.Geometry:
						result = "Microsoft.SqlServer.Types.SqlGeometry";
						break;

					case SqlDataType.Image:
						result = "byte[]";
						break;

					case SqlDataType.Int:
						result = "int?";
						break;

					case SqlDataType.Money:
						result = "Decimal?";
						break;

					//case SqlDataType.None:
					//	result = "string";
					//	break;

					case SqlDataType.NChar:
						result = "char?";
						break;

					case SqlDataType.NText:
						result = "string";
						break;

					case SqlDataType.Numeric:
						result = "Decimal?";
						break;

					case SqlDataType.NVarChar:
						result = "string";
						break;

					case SqlDataType.NVarCharMax:
						result = "string";
						break;

					case SqlDataType.Real:
						result = "single?";
						break;

					case SqlDataType.SmallDateTime:
						result = "DateTime?";
						break;

					case SqlDataType.SmallInt:
						result = "Int16?";
						break;

					case SqlDataType.SmallMoney:
						result = "Decimal?";
						break;

					case SqlDataType.Text:
						result = "string";
						break;

					case SqlDataType.Time:
						result = "TimeSpan?";
						break;

					case SqlDataType.Timestamp:
						result = "byte[]";
						break;

					case SqlDataType.TinyInt:
						result = "byte?";
						break;

					case SqlDataType.UniqueIdentifier:
						result = "Guid?";
						break;

					//case SqlDataType.UserDefinedDataType:
					//	result = "";
					//	break;
					//case SqlDataType.UserDefinedTableType:
					//	result = "";
					//	break;
					//case SqlDataType.UserDefinedType:
					//	result = "";
					//	break;

					case SqlDataType.VarBinary:
						result = "byte[]";
						break;

					case SqlDataType.VarBinaryMax:
						result = "byte[]";
						break;

					case SqlDataType.VarChar:
						result = "string";
						break;

					case SqlDataType.VarCharMax:
						result = "string";
						break;

					case SqlDataType.Variant:
						result = "object";
						break;

					case SqlDataType.Xml:
						result = "XmlDocument";
						break;

					//case SqlDataType.HierarchyId:
					//case SqlDataType.Numeric
					//case SqlDataType.SysName:				

					default:
						break; // StyleCop
				}
			}

			if (result == null)
			{
				throw new ApplicationException(String.Format("Nepodařilo se přeložit SqlDataType \"{0}\" na systémový typ.", dataType.SqlDataType));
			}
			return nullable ? result : result.Replace("?", "");
		}
		#endregion

		#region ToSqlDbType
		/// <summary>
		/// Převede SqlDataType na SqlDbType.
		/// </summary>
		public static SqlDbType ToSqlDbType(DataType dataType)
		{

			if (dataType.SqlDataType == SqlDataType.UserDefinedTableType)
			{
				switch (dataType.Name)
				{
					case "IntTable":
						return SqlDbType.Structured;

					default:
						throw new ApplicationException(String.Format("Nepodařilo se přeložit SqlDataType.UserDefinedTableType \"{0}\" na SqlDbType.", dataType.Name));
				}
			}

			if (dataType.SqlDataType == SqlDataType.UserDefinedDataType)
			{
				string systemType = DatabaseHelper.Database.UserDefinedDataTypes[dataType.Name].SystemType;

				switch (systemType)
				{
					case "int":
						return SqlDbType.Int;

					case "numeric":
						return SqlDbType.Decimal;

					case "datetime":
						return SqlDbType.DateTime;

					case "smalldatetime":
						return SqlDbType.SmallDateTime;

					case "uniqueidentifier":
						return SqlDbType.UniqueIdentifier;

					case "varbinary":
						return SqlDbType.VarBinary;

					case "nvarchar":
						return SqlDbType.NVarChar;

					default:
						throw new ApplicationException(String.Format("Nepodařilo se přeložit SqlDataType.UserDefinedDataType \"{0}\" na SqlDbType.", systemType));
				}
			}

			switch (dataType.SqlDataType)
			{
				case SqlDataType.BigInt: return SqlDbType.BigInt;
				case SqlDataType.Binary: return SqlDbType.Binary;
				case SqlDataType.Bit: return SqlDbType.Bit;
				case SqlDataType.Char: return SqlDbType.Char;
				case SqlDataType.Date: return SqlDbType.Date;
				case SqlDataType.DateTime: return SqlDbType.DateTime;
				case SqlDataType.DateTime2: return SqlDbType.DateTime2;
				case SqlDataType.DateTimeOffset: return SqlDbType.DateTimeOffset;
				case SqlDataType.Decimal: return SqlDbType.Decimal;
				case SqlDataType.Float: return SqlDbType.Float;
				case SqlDataType.Image: return SqlDbType.Image;
				case SqlDataType.Int: return SqlDbType.Int;
				case SqlDataType.Money: return SqlDbType.Money;
				//case SqlDataType.None: result = "string"; break;
				case SqlDataType.NChar: return SqlDbType.NChar;
				case SqlDataType.NText: return SqlDbType.NText;
				case SqlDataType.Numeric: return SqlDbType.Decimal;
				case SqlDataType.NVarChar: return SqlDbType.NVarChar;
				case SqlDataType.NVarCharMax: return SqlDbType.NVarChar;
				case SqlDataType.Real: return SqlDbType.Real;
				case SqlDataType.SmallDateTime: return SqlDbType.SmallDateTime;
				case SqlDataType.SmallInt: return SqlDbType.SmallInt;
				case SqlDataType.SmallMoney: return SqlDbType.SmallMoney;
				case SqlDataType.Text: return SqlDbType.Text;
				case SqlDataType.Time: return SqlDbType.Time;
				case SqlDataType.Timestamp: return SqlDbType.Timestamp;
				case SqlDataType.TinyInt: return SqlDbType.TinyInt;
				case SqlDataType.UniqueIdentifier: return SqlDbType.UniqueIdentifier;
				case SqlDataType.UserDefinedDataType: return SqlDbType.Udt;
				case SqlDataType.UserDefinedType: return SqlDbType.Udt;
				case SqlDataType.VarBinary: return SqlDbType.VarBinary;
				case SqlDataType.VarBinaryMax: return SqlDbType.VarBinary;
				case SqlDataType.VarChar: return SqlDbType.VarChar;
				case SqlDataType.VarCharMax: return SqlDbType.VarChar;
				case SqlDataType.Variant: return SqlDbType.Variant;
				case SqlDataType.Xml: return SqlDbType.Xml;
				case SqlDataType.Geography: return SqlDbType.Udt;
				case SqlDataType.Geometry: return SqlDbType.Udt;
				default: throw new ArgumentException(String.Format("Nepodařilo se přeložit SqlDataType \"{0}\" na SqlDbType.", dataType.SqlDataType.ToString()));
			}
		}
		#endregion

		#region ToDbType
		/// <summary>
		/// Převede SqlDataType na DbType.
		/// </summary>
		public static DbType ToDbType(DataType dataType)
		{
			if (dataType.SqlDataType == SqlDataType.UserDefinedDataType)
			{
				string systemType = DatabaseHelper.Database.UserDefinedDataTypes[dataType.Name].SystemType;

				switch (systemType)
				{
					case "int": return DbType.Int32;
					case "numeric": return DbType.Decimal;
					case "datetime": return DbType.DateTime;
					case "smalldatetime": return DbType.DateTime;
					case "uniqueidentifier": return DbType.Guid;
					case "varbinary": return DbType.Binary;
					case "nvarchar": return DbType.String;
					case "geo": return DbType.String;
					default:
						{
							throw new ApplicationException(String.Format("Nepodařilo se přeložit SqlDataType.UserDefinedDataType \"{0}\" na SqlDbType.", systemType));
						}
				}
			}

			switch (dataType.SqlDataType)
			{
				case SqlDataType.BigInt: return DbType.Int64;
				case SqlDataType.Binary: return DbType.Binary;
				case SqlDataType.Bit: return DbType.Boolean;
				case SqlDataType.Char: return DbType.AnsiStringFixedLength;
				case SqlDataType.Date: return DbType.Date;
				case SqlDataType.DateTime: return DbType.DateTime;
				case SqlDataType.DateTime2: return DbType.DateTime2;
				case SqlDataType.DateTimeOffset: return DbType.DateTimeOffset;
				case SqlDataType.Decimal: return DbType.Decimal;
				case SqlDataType.Float: return DbType.Double;
				case SqlDataType.Image: return DbType.Object;
				case SqlDataType.Int: return DbType.Int32;
				case SqlDataType.Money: return DbType.Decimal;
				//case SqlDataType.None: result = "string"; break;
				case SqlDataType.NChar: return DbType.StringFixedLength;
				case SqlDataType.NText: return DbType.String;
				case SqlDataType.Numeric: return DbType.Decimal;
				case SqlDataType.NVarChar: return DbType.String;
				case SqlDataType.NVarCharMax: return DbType.String;
				case SqlDataType.Real: return DbType.Single;
				case SqlDataType.SmallDateTime: return DbType.DateTime;
				case SqlDataType.SmallInt: return DbType.Int16;
				case SqlDataType.SmallMoney: return DbType.Decimal;
				case SqlDataType.Text: return DbType.String;
				case SqlDataType.Time: return DbType.Time;
				case SqlDataType.Timestamp: return DbType.Binary;
				case SqlDataType.TinyInt: return DbType.Byte;
				case SqlDataType.UniqueIdentifier: return DbType.Guid;
				case SqlDataType.UserDefinedDataType: return DbType.Object;
				case SqlDataType.UserDefinedType: return DbType.Object;
				case SqlDataType.VarBinary: return DbType.Binary;
				case SqlDataType.VarBinaryMax: return DbType.Binary;
				case SqlDataType.VarChar: return DbType.AnsiString;
				case SqlDataType.VarCharMax: return DbType.AnsiString;
				case SqlDataType.Variant: return DbType.Object;
				case SqlDataType.Xml: return DbType.Xml;
				//case SqlDataType.Geography: return DbType.Object;
				default: throw new ArgumentException(String.Format("Nepodařilo se přeložit SqlDataType \"{0}\" na DbType.", dataType.SqlDataType.ToString()));
			}
		}
		#endregion

		#region IsDateTime
		/// <summary>
		/// Vrátí true, pokud je typ sloupce DateTime nebo SmallDateTime.
		/// </summary>
		public static bool IsDateTime(Column column)
		{
			return GetFieldSystemTypeName(column).StartsWith("DateTime");
		}
		#endregion

		#region IsNonstandardType
		/// <summary>
		/// Vrací true, pokud má být sloupec reprezentován jako datový typ Enum.
		/// </summary>
		public static bool IsNonstandardType(Column column)
		{
			return !String.IsNullOrEmpty(GetNonstandardPropertyTypeName(column));
		}
		#endregion

		#region GetNonstandardPropertyTypeName
		/// <summary>
		/// Vrací název datového typu Enum, kterým je reprezentována hodnota daného sloupce v C#.
		/// </summary>
		private static string GetNonstandardPropertyTypeName(Column column)
		{
			return ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "PropertyType");
		}
		#endregion

		#region GetNonstandarPropertyTypeConverterName
		/// <summary>
		/// Vrací název konvertoru přiřazeného k danému sloupci.
		/// </summary>
		public static string GetNonstandarPropertyTypeConverterName(Column column)
		{
			return ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromColumn(column), "PropertyTypeConverter");
		}
		#endregion

		#region GetPropertyTypeName
		/// <summary>
		/// Vrací typ property na základě sloupce.
		/// Je-li sloupec cizím klíčem, použije se referencovaná tabulka na určení třídy.
		/// </summary>
		public static string GetPropertyTypeName(Column column)
		{
			if (TypeHelper.IsNonstandardType(column))
			{
				return TypeHelper.GetNonstandardPropertyTypeName(column);
			}

			if (TypeHelper.IsBusinessObjectReference(column))
			{
				Table referencedTable = ColumnHelper.GetReferencedTable(column);
				if (referencedTable == null)
				{
					throw new ApplicationException(String.Format("Sloupec {0}: Obsahuje referenci na tabulku, která nebyla nalezena (Ignored?).", column.Name));
				}
				return ClassHelper.GetClassFullName(referencedTable);
			}

			return TypeHelper.GetFieldSystemTypeName(column);
		}
		#endregion

		#region IsBusinessObjectReference
		/// <summary>
		/// Vrací true, pokud sloupec referencuje business object.
		/// </summary>
		public static bool IsBusinessObjectReference(Column column)
		{
			return column.IsForeignKey && String.IsNullOrEmpty(GetNonstandardPropertyTypeName(column));
		}
		#endregion

		#region IsNumeric
		/// <summary>
		/// Vrací true, pokud je být sloupec reprezentován jako číselný datový typ.
		/// </summary>
		public static bool IsNumeric(Column column)
		{
			switch (ToSqlDbType(column.DataType))
			{
				case SqlDbType.BigInt:
				case SqlDbType.Decimal:
				case SqlDbType.Float:
				case SqlDbType.Int:
				case SqlDbType.Real:
				case SqlDbType.SmallInt:
				case SqlDbType.SmallMoney:
				case SqlDbType.TinyInt:
				case SqlDbType.Money:
					return true;
				default:
					return false;
			}
		}
		#endregion

		#region IsDateOnly
		/// <summary>
		/// Vrací true, pokud jde o typ, který ořezává hodnotu datetime na datum.
		/// </summary>
		public static bool IsDateOnly(DataType dataType)
		{
			if ((GeneratorSettings.Strategy != GeneratorStrategy.Havit) && (GeneratorSettings.Strategy != GeneratorStrategy.WikiReality))
			{
				return false;
			}

			if (dataType.SqlDataType == SqlDataType.Date)
			{
				return true;
			}

			if (dataType.SqlDataType == SqlDataType.UserDefinedDataType)
			{
				if (dataType.Name == "TDate")
				{
					return true;
				}
			}
			return false;
		}
		#endregion

		#region IsXml
		/// <summary>
		/// Vrací true, pokud jde o typ ukládající XML hodnotu (dokument/fragmenty).
		/// </summary>
		public static bool IsXml(Column column)
		{
			return (column.DataType.SqlDataType == SqlDataType.Xml) && (GetFieldSystemTypeName(column) == "XmlDocument");
		}
		#endregion
     
	}
}
