using System;
using Havit.Business.BusinessLayerGenerator.Csproj;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class MoneyBaseClass
{
	public static void Generate(Table currencyTable, CsprojFile csprojFile)
	{
		string fileName = FileHelper.GetFilename(NamespaceHelper.GetNamespaceName(currencyTable, false), "Money", "Base.cs", FileHelper.GeneratedFolder);

		if (csprojFile != null)
		{
			csprojFile.Ensures(fileName);
		}

		CodeWriter writer = new CodeWriter(FileHelper.ResolvePath(fileName));

		BusinessObjectUsings.WriteUsings(writer);

		writer.WriteLine("namespace " + NamespaceHelper.GetNamespaceName(currencyTable));
		writer.WriteLine("{");

		writer.WriteCommentSummary("Třída reprezentující peněžní částky s měnou.");
		writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
		writer.WriteGeneratedCodeAttribute();
		writer.WriteLine("[System.Diagnostics.DebuggerDisplay(\"{GetType().FullName,nq} (Amount={Amount, nq}, Currency={Currency == null ? \\\"null\\\" : Currency.ID.ToString(), nq})\")]");
		writer.WriteLine("public class MoneyBase : Havit.Business.MoneyImplementationBase<Currency, Money>");
		writer.WriteLine("{");
		writer.WriteLine();
		WriteConstructors(writer, currencyTable, "MoneyBase", true);
		writer.WriteLine("}");

		writer.WriteLine("}");

		writer.Save();
	}

	public static void WriteConstructors(CodeWriter writer, Table table, string className, bool baseClass)
	{
		writer.WriteOpenRegion("Constructors");
		writer.WriteCommentSummary("Inicializuje třídu money s prázdními hodnotami (Amount i Currency jsou null).");

		if (!baseClass)
		{
			writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
			writer.WriteGeneratedCodeAttribute();
		}

		writer.WriteLine(String.Format("public {0}() : base()", className));
		writer.WriteLine("{");
		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteCommentSummary("Inicializuje třídu money zadanými hodnotami.");

		if (!baseClass)
		{
			writer.WriteMicrosoftContract(ContractHelper.GetContractVerificationAttribute(false));
			writer.WriteGeneratedCodeAttribute();
		}

		writer.WriteLine(String.Format("public {0}(decimal? amount, Currency currency) : base(amount, currency)", className));
		writer.WriteLine("{");
		writer.WriteLine("}");

		writer.WriteCloseRegion();

	}
}
