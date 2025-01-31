﻿using System.Text;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Helpers.Types;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class BusinessObjectPartialClass
{
	public static void Generate(Table table, CsprojFile csprojFile)
	{
		string fileName = FileHelper.GetFilename(table, ".partial.cs", FileHelper.GeneratedFolder);

		if (csprojFile != null)
		{
			csprojFile.Ensures(fileName);
		}

		CodeWriter writer = new CodeWriter(FileHelper.ResolvePath(fileName));

		Autogenerated.WriteAutogeneratedNoCodeHere(writer);
		BusinessObjectUsings.WriteUsings(writer);
		WriteNamespaceClassBegin(writer, table, true);
		BusinessObjectConstructors.WriteConstructors(writer, ClassHelper.GetClassName(table), TableHelper.GetPrimaryKey(table).Name, TableHelper.IsReadOnly(table), false);
		WriteCreateObject(writer, table);
		WriteGetObject(writer, table);
		WriteGetObjectOrDefault(writer, table);
		WriteGetObjects(writer, table);
		WriteCreateDisconnectedObject(writer, table);
		Autogenerated.WriteAutogeneratedNoCodeHere(writer);
		WriteNamespaceClassEnd(writer);

		writer.Save();
	}

	public static void WriteNamespaceClassBegin(CodeWriter writer, Table table, bool includeAttributes)
	{
		writer.WriteLine("namespace " + NamespaceHelper.GetNamespaceName(table));
		writer.WriteLine("{");

		string comment = TableHelper.GetDescription(table);
		writer.WriteCommentSummary(comment);

		//			if (includeAttributes)
		//				writer.WriteLine("[Serializable]");

		writer.WriteLine(String.Format("{0} partial class {1} : {2}",
			TableHelper.GetAccessModifier(table),
			ClassHelper.GetClassName(table),
			ClassHelper.GetBaseClassName(table)));
		writer.WriteLine("{");
	}

	private static void WriteCreateObject(CodeWriter writer, Table table)
	{
		if (!TableHelper.IsReadOnly(table) && !TableHelper.OmitCreateObjectMethod(table))
		{
			string modifier = TableHelper.GetCreateObjectAccessModifier(table);

			List<Column> ownerColumns = TableHelper.GetOwnerColumns(table);

			StringBuilder parametersBuilder = new StringBuilder();
			foreach (Column ownerColumn in ownerColumns)
			{
				if (parametersBuilder.Length > 0)
				{
					parametersBuilder.Append(", ");
				}
				parametersBuilder.AppendFormat("{0} {1}", TypeHelper.GetPropertyTypeName(ownerColumn), ConventionsHelper.GetCammelCase(PropertyHelper.GetPropertyName(ownerColumn)));
			}

			writer.WriteOpenRegion("CreateObject (static)");
			writer.WriteCommentSummary("Vrátí nový objekt.");
			writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
			writer.WriteGeneratedCodeAttribute();
			writer.WriteLine(String.Format("{2} static {0} CreateObject({1})", ClassHelper.GetClassName(table), parametersBuilder.ToString(), modifier));
			writer.WriteLine("{");
			foreach (Column ownerColumn in ownerColumns)
			{
				writer.WriteMicrosoftContract(String.Format("global::System.Diagnostics.Contracts.Contract.Requires({0} != null);", ConventionsHelper.GetCammelCase(PropertyHelper.GetPropertyName(ownerColumn))));
			}
			writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetClassName(table)));
			writer.WriteMicrosoftContract("");
			writer.WriteLine(String.Format("{0} result = new {0}();", ClassHelper.GetClassName(table)));

			foreach (Column ownerColumn in ownerColumns)
			{
				writer.WriteLine(String.Format("result.{0} = {1};", PropertyHelper.GetPropertyName(ownerColumn), ConventionsHelper.GetCammelCase(PropertyHelper.GetPropertyName(ownerColumn))));

				Table ownerTable = ColumnHelper.GetReferencedTable(ownerColumn);
				List<CollectionProperty> collectionProperties = TableHelper.GetCollectionColumns(ownerTable);
				foreach (CollectionProperty collectionProperty in collectionProperties)
				{
					if (collectionProperty.ReferenceColumn == ownerColumn)
					{
						writer.WriteLine(String.Format("{0}.{1}.Add(result);", ConventionsHelper.GetCammelCase(PropertyHelper.GetPropertyName(ownerColumn)), collectionProperty.PropertyName));
					}
				}
			}
			writer.WriteLine("return result;");
			writer.WriteLine("}");
			writer.WriteCloseRegion();
		}
	}

	private static void WriteGetObject(CodeWriter writer, Table table)
	{
		bool cachableInstances = TableHelper.CanCacheBusinessObjectInstances(table);

		writer.WriteOpenRegion("GetObject (static)");
		writer.WriteLine();

		if (cachableInstances)
		{
			writer.WriteLine("private static object lockGetObjectCacheAccess = new object();");
			writer.WriteLine();
		}

		writer.WriteCommentSummary("Vrátí existující objekt s daným ID.");
		writer.WriteCommentLine(String.Format("<param name=\"id\">{0} (PK).</param>", TableHelper.GetPrimaryKey(table).Name));
		writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
		writer.WriteGeneratedCodeAttribute();
		writer.WriteLine(String.Format("public static {0} GetObject(int id)", ClassHelper.GetClassName(table)));
		writer.WriteLine("{");
		writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetClassName(table)));
		writer.WriteMicrosoftContract("");
		writer.WriteLine(String.Format("{0} result;", ClassHelper.GetClassName(table)));
		writer.WriteLine();

		writer.WriteLine("IdentityMap currentIdentityMap = IdentityMapScope.Current;");
		writer.WriteHavitContract("global::Havit.Diagnostics.Contracts.Contract.Assert(currentIdentityMap != null, \"currentIdentityMap != null\");");
		writer.WriteMicrosoftContract("global::System.Diagnostics.Contracts.Contract.Assume(currentIdentityMap != null);");
		writer.WriteLine(String.Format("if (currentIdentityMap.TryGet<{0}>(id, out result))", ClassHelper.GetClassName(table)));
		writer.WriteLine("{");
		writer.WriteMicrosoftContract("global::System.Diagnostics.Contracts.Contract.Assume(result != null);");
		writer.WriteLine("return result;");
		writer.WriteLine("}");
		writer.WriteLine();

		if (cachableInstances)
		{
			// fromCache - zjišťujeme, zda byl do IdentityMap přidán konstruktorem
			writer.WriteLine("bool fromCache = true;");
			writer.WriteLine();

			writer.WriteLine(String.Format("result = ({0})GetBusinessObjectFromCache(id);", ClassHelper.GetClassName(table)));
			writer.WriteLine("if (result == null)");
			writer.WriteLine("{");
			writer.WriteLine("lock (lockGetObjectCacheAccess)");
			writer.WriteLine("{");
			writer.WriteLine(String.Format("result = ({0})GetBusinessObjectFromCache(id);", ClassHelper.GetClassName(table)));
			writer.WriteLine("if (result == null)");
			writer.WriteLine("{");
			writer.WriteLine("fromCache = false;");
			writer.WriteLine(String.Format("result = new {0}(id);", ClassHelper.GetClassName(table)));
			writer.WriteLine();
			writer.WriteLine("AddBusinessObjectToCache(result);");
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine("}");

			writer.WriteLine();
			writer.WriteLine("if (fromCache && (currentIdentityMap != null))");
			writer.WriteLine("{");
			writer.WriteLine("currentIdentityMap.Store(result);");
			writer.WriteLine("}");
		}
		else
		{
			writer.WriteLine(String.Format("result = new {0}(id);", ClassHelper.GetClassName(table)));
		}

		writer.WriteLine();

		writer.WriteLine("return result;");
		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteCommentSummary("Vrátí existující objekt inicializovaný daty z DataReaderu.");
		writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
		writer.WriteGeneratedCodeAttribute();
		writer.WriteLine(String.Format("internal static {0} GetObject(DataRecord dataRecord)", ClassHelper.GetClassName(table)));
		writer.WriteLine("{");
		writer.WriteHavitContract("global::Havit.Diagnostics.Contracts.Contract.Requires(dataRecord != null);");
		writer.WriteHavitContract("");
		writer.WriteMicrosoftContract("global::System.Diagnostics.Contracts.Contract.Requires(dataRecord != null);");
		writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetClassName(table)));
		writer.WriteMicrosoftContract("");

		writer.WriteLine(String.Format("{0} result = null;", ClassHelper.GetClassName(table)));
		writer.WriteLine();
		writer.WriteLine(String.Format("int id = dataRecord.Get<int>({0}.Properties.ID.FieldName);", ClassHelper.GetClassName(table)));
		writer.WriteLine("");
		writer.WriteLine("if ((dataRecord.DataLoadPower == DataLoadPower.Ghost) || (dataRecord.DataLoadPower == DataLoadPower.FullLoad))");
		writer.WriteLine("{");

		writer.WriteLine(String.Format("result = {0}.GetObject(id);", ClassHelper.GetClassName(table)));
		writer.WriteLine("if (!result.IsLoaded && (dataRecord.DataLoadPower == DataLoadPower.FullLoad))");
		writer.WriteLine("{");
		writer.WriteLine("result.Load(dataRecord);");
		writer.WriteLine("}");
		writer.WriteLine("}");
		writer.WriteLine("else");
		writer.WriteLine("{");
		writer.WriteLine(String.Format("result = new {0}(id, dataRecord);", ClassHelper.GetClassName(table)));
		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteLine("return result;");

		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteCloseRegion();
	}

	private static void WriteGetObjectOrDefault(CodeWriter writer, Table table)
	{
		writer.WriteOpenRegion("GetObjectOrDefault (static)");
		writer.WriteLine();
		writer.WriteCommentSummary("Pokud je zadáno ID objektu (not-null), vrátí existující objekt s daným ID. Jinak vrací výchozí hodnotu (není-li zadána, pak vrací null).");
		writer.WriteCommentLine("<param name=\"id\">ID objektu.</param>");
		writer.WriteCommentLine("<param name=\"defaultValue\">Výchozí hodnota.</param>");
		writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
		writer.WriteGeneratedCodeAttribute();
		writer.WriteLine(String.Format("public static {0} GetObjectOrDefault(int? id, {0} defaultValue = null)", ClassHelper.GetClassName(table)));
		writer.WriteLine("{");
		writer.WriteLine("return (id != null) ? GetObject(id.Value) : defaultValue;");
		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteCloseRegion();
	}

	private static void WriteGetObjects(CodeWriter writer, Table table)
	{
		writer.WriteOpenRegion("GetObjects (static)");
		writer.WriteLine();
		writer.WriteCommentSummary("Vrátí kolekci obsahující objekty danými ID.");
		writer.WriteCommentLine("<param name=\"ids\">Identifikátory objektů.</param>");
		writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
		writer.WriteGeneratedCodeAttribute();
		writer.WriteLine(String.Format("public static {0} GetObjects(params int[] ids)", ClassHelper.GetCollectionClassName(table)));
		writer.WriteLine("{");
		writer.WriteHavitContract("global::Havit.Diagnostics.Contracts.Contract.Requires(ids != null, \"ids != null\");");
		writer.WriteHavitContract("");
		writer.WriteMicrosoftContract("global::System.Diagnostics.Contracts.Contract.Requires(ids != null);");
		writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetCollectionClassName(table)));
		writer.WriteMicrosoftContract("");
		writer.WriteLine(String.Format("return new {1}(Array.ConvertAll<int, {0}>(ids, id => {0}.GetObject(id)));", ClassHelper.GetClassName(table), ClassHelper.GetCollectionClassName(table)));
		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteCloseRegion();
	}

	private static void WriteCreateDisconnectedObject(CodeWriter writer, Table table)
	{
		writer.WriteOpenRegion("CreateDisconnectedObject (static)");

		writer.WriteCommentSummary("Vrátí nový disconnected objekt. Určeno výhradně pro účely testů.");
		writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
		writer.WriteGeneratedCodeAttribute();
		writer.WriteLine(String.Format("public static {0} CreateDisconnectedObject()", ClassHelper.GetClassName(table)));
		writer.WriteLine("{");
		writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetClassName(table)));
		writer.WriteMicrosoftContract("");
		writer.WriteLine(String.Format("return new {0}(ConnectionMode.Disconnected);", ClassHelper.GetClassName(table)));
		writer.WriteLine("}");

		writer.WriteLine();

		writer.WriteCommentSummary("Vrátí nový disconnected objekt s daným Id. Určeno výhradně pro účely testů.");
		writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
		writer.WriteGeneratedCodeAttribute();
		writer.WriteLine(String.Format("public static {0} CreateDisconnectedObject(int id)", ClassHelper.GetClassName(table)));
		writer.WriteLine("{");
		writer.WriteMicrosoftContract(ContractHelper.GetEnsuresResultNotNull(ClassHelper.GetClassName(table)));
		writer.WriteMicrosoftContract("");
		writer.WriteLine(String.Format("return new {0}(id, ConnectionMode.Disconnected);", ClassHelper.GetClassName(table)));
		writer.WriteLine("}");

		writer.WriteCloseRegion();
	}

	public static void WriteNamespaceClassEnd(CodeWriter writer)
	{
		writer.WriteLine("}");
		writer.WriteLine("}");
	}
}
