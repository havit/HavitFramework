using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators;

public static class LocalizationCollectionGenerator
{
	public static void WriteLocalizations(CodeWriter writer, Table table)
	{
		if (LocalizationHelper.IsLocalizationTable(table))
		{
			writer.WriteOpenRegion("Localizations");
			WriteIndexer(writer, table);
			WriteCurrentProperty(writer, table);
			WriteInterfaceMethods(writer, table);
			writer.WriteCloseRegion();
		}
	}

	private static void WriteIndexer(CodeWriter writer, Table table)
	{
		writer.WriteCommentSummary("Vrací objekt s lokalizovanými daty na základě jazyka, který je předán.");
		writer.WriteLine(
			String.Format("public {0} this[{1} language]",
			ClassHelper.GetClassName(table),
			ClassHelper.GetClassFullName(LanguageHelper.GetLanguageTable())));
		writer.WriteLine("{");
		writer.WriteLine("get");
		writer.WriteLine("{");
		writer.WriteLine(String.Format("return this.Find(delegate({0} item)", ClassHelper.GetClassName(table)));
		writer.Indent();
		writer.WriteLine("{");
		writer.WriteLine("return (item.Language == language);");
		writer.Unindent();
		writer.WriteLine("});");
		writer.Unindent();
		writer.WriteLine("}");
		writer.WriteLine("}");
		writer.WriteLine();
	}

	private static void WriteCurrentProperty(CodeWriter writer, Table table)
	{
		writer.WriteCommentSummary("Vrací objekt s lokalizovanými daty na základě aktuálního jazyka (aktuální jazyk se hledá na základě CurrentUICulture).");
		writer.WriteLine(String.Format("public virtual {0} Current", ClassHelper.GetClassName(table)));
		writer.WriteLine("{");
		writer.WriteLine("get");
		writer.WriteLine("{");
		writer.WriteLine(String.Format(
			"return this[{0}.Current];",
			ClassHelper.GetClassFullName(LanguageHelper.GetLanguageTable())));
		writer.WriteLine("}");
		writer.WriteLine("}");
		writer.WriteLine("");
	}

	private static void WriteInterfaceMethods(CodeWriter writer, Table table)
	{
		//writer.WriteOpenRegion("ILocalizationCollection interface implementation");
		writer.WriteCommentSummary("Vrací objekt s lokalizovanými daty na základě jazyka, který je předán.");
		writer.WriteLine("BusinessObjectBase ILocalizationCollection.this[ILanguage language]");
		writer.WriteLine("{");
		writer.WriteLine("get");
		writer.WriteLine("{");
		writer.WriteLine(String.Format("return this[({0})language];", ClassHelper.GetClassFullName(LanguageHelper.GetLanguageTable())));
		writer.WriteLine("}");
		writer.WriteLine("}");
		writer.WriteLine();
		writer.WriteCommentSummary("Vrací objekt s lokalizovanými daty na základě aktuálního jazyka (aktuální jazyk se hledá na základě CurrentUICulture).");
		writer.WriteLine("BusinessObjectBase ILocalizationCollection.Current");
		writer.WriteLine("{");
		writer.WriteLine("get");
		writer.WriteLine("{");
		writer.WriteLine("return this.Current;");
		writer.WriteLine("}");
		writer.WriteLine("}");
		//writer.WriteCloseRegion();

	}
}
