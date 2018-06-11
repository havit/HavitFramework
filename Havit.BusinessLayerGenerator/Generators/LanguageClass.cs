using System;
using Havit.Business.BusinessLayerGenerator.Helpers;
using Havit.Business.BusinessLayerGenerator.Helpers.NamingConventions;
using Havit.Business.BusinessLayerGenerator.Writers;
using Microsoft.SqlServer.Management.Smo;

namespace Havit.Business.BusinessLayerGenerator.Generators
{
	public static class LanguageClass
	{
		#region WriteLanguage
		public static void WriteLanguage(CodeWriter writer, Table table)
		{
			if (LanguageHelper.IsLanguageTable(table))
			{
				writer.WriteOpenRegion("Localizations");
				WriteGetByCulture(writer, table);
				WriteCurrentProperty(writer, table);
				writer.WriteCloseRegion();
			}
		}
		#endregion

		#region WriteGetByCulture
		private static void WriteGetByCulture(CodeWriter writer, Table table)
		{
			Column uiCultureColumn = LanguageHelper.GetUICultureColumn();

			writer.WriteCommentSummary(String.Format("Dohledá objekt {0} podle zadaného CultureName.", ClassHelper.GetClassName(table)));
			writer.WriteCommentLine("<param name=\"cultureName\">CultureName v podobě cs-CZ, en-US, atp.</param>");
			writer.WriteCommentLine("<remarks>Dohledává dle logiky resources, tedy od nejspecifičtějšího (en-US) k obecnějšímu (en) až po invariant.</remarks>");
			writer.WriteCommentLine("<returns>Nalezený objekt v případě úspěchu; jinak <c>null</c>.</returns>");
			writer.WriteLine(String.Format("public static {0} GetByUICulture(string cultureName)", ClassHelper.GetClassName(table)));
			writer.WriteLine("{");
			writer.WriteHavitContract("global::Havit.Diagnostics.Contracts.Contract.Requires(cultureName != null, \"cultureName != null\");");
			writer.WriteHavitContract("");
			writer.WriteMicrosoftContract("global::System.Diagnostics.Contracts.Contract.Requires(cultureName != null);");
			writer.WriteMicrosoftContract("");
			writer.WriteLine("if (culturesDictionary == null)");
			writer.WriteLine("{");
			writer.WriteLine("lock (culturesDictionaryLock)");
			writer.WriteLine("{");
			writer.WriteLine("if (culturesDictionary == null)");
			writer.WriteLine("{");
			writer.WriteLine(String.Format("{0} languages = {1}.GetAll();", ClassHelper.GetCollectionClassName(table), ClassHelper.GetClassName(table)));
			writer.WriteLine("culturesDictionary = new Dictionary<string, int>();");
			writer.WriteLine(String.Format("foreach ({0} language in languages)", ClassHelper.GetClassName(table)));
			writer.WriteLine("{");
			if (uiCultureColumn != null)
			{
				writer.WriteLine("culturesDictionary.Add(language.UICulture, language.ID);");
				writer.WriteLine();
				writer.WriteLine("// pokud není nastaveno UiCulture, jedná se o výchozí jazyk (invariant)");
				writer.WriteLine(String.Format("if (String.IsNullOrEmpty(language.{0}))", PropertyHelper.GetPropertyName(uiCultureColumn)));
				writer.WriteLine("{");
				writer.WriteLine("_defaultUILanguageID = language.ID;");
				writer.WriteLine("}");
			}
			else
			{
				writer.WriteLine("culturesDictionary.Add(language.Culture, language);");
			}
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine("// nejprve zkusíme hledat podle plného názvu");
			writer.WriteLine("int? resultID = null;");
			writer.WriteLine("int tmp;");
			writer.WriteLine("if (culturesDictionary.TryGetValue(cultureName, out tmp))");
			writer.WriteLine("{");
			writer.WriteLine("resultID = tmp;");
			writer.WriteLine("}");
			writer.WriteLine();

			writer.WriteLine("// pokud není nalezeno, hledáme podle samotného jazyka");
			writer.WriteLine("if ((resultID == null) && (cultureName.Length > 2))");
			writer.WriteLine("{");
			writer.WriteLine("if (culturesDictionary.TryGetValue(cultureName.Substring(0, 2), out tmp))");
			writer.WriteLine("{");
			writer.WriteLine("resultID = tmp;");
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine();

			if (uiCultureColumn != null)
			{
				writer.WriteLine("// pokud není nalezeno, použijeme výchozí jazyk (je-li stanoven).");
				writer.WriteLine("if (resultID == null)");
				writer.WriteLine("{");
				writer.WriteLine("resultID = _defaultUILanguageID;");
				writer.WriteLine("}");
			}
			writer.WriteLine(String.Format("return (resultID == null) ? null : {0}.GetObject(resultID.Value);", ClassHelper.GetClassName(table)));
			writer.WriteLine();

			writer.WriteLine("}");
			writer.WriteLine("private static Dictionary<string, int> culturesDictionary;");
			writer.WriteLine("private static object culturesDictionaryLock = new object();");
			if (uiCultureColumn != null)
			{
				writer.WriteLine("private static int? _defaultUILanguageID;");
			}
			writer.WriteLine();

			writer.WriteCommentSummary(String.Format("Dohledá objekt {0} podle zadaného Culture.", ClassHelper.GetClassName(table)));
			writer.WriteCommentLine("<param name=\"culture\">CultureName v podobě cs-CZ, en-US, atp.</param>");
			writer.WriteCommentLine("<remarks>Dohledává dle logiky resources, tedy od nejspecifičtějšího (en-US) k obecnějšímu (en) až po invariant.</remarks>");
			writer.WriteCommentLine("<returns>Nalezený objekt v případě úspěchu; jinak <c>null</c>.</returns>");
			writer.WriteLine(String.Format("public static {0} GetByUICulture(CultureInfo culture)", ClassHelper.GetClassName(table)));
			writer.WriteLine("{");
			writer.WriteLine("if (culture == null)");
			writer.WriteLine("{");
			writer.WriteLine("throw new ArgumentNullException(\"culture\");");
			writer.WriteLine("}");
			writer.WriteLine();
			writer.WriteLine(String.Format("return {0}.GetByUICulture(culture.Name);", ClassHelper.GetClassName(table)));
			writer.WriteLine("}");
			writer.WriteLine();
		}
		#endregion

		#region WriteCurrentProperty
		private static void WriteCurrentProperty(CodeWriter writer, Table table)
		{
			writer.WriteCommentLine("<summary>");
			writer.WriteCommentLine("Aktuální jazyk určený z System.Threading.Thread.CurrentThread.CurrentUICulture.");
			writer.WriteCommentLine("</summary>");
			writer.WriteLine(String.Format("public static {0} Current", ClassHelper.GetClassName(table)));
			writer.WriteLine("{");
			writer.WriteLine("get");
			writer.WriteLine("{");
			writer.WriteLine("// pokud se ptáme na stále stejnou cultureName, v rámci stejné identity mapy...");
			writer.WriteLine("// raději si zapamatuju IdentityMap, než si pamatovat ID objektu a použít Language.GetObject,");
			writer.WriteLine("// protože to stejně použije identitymapu a navíc bude hledat ve slovníku");
			writer.WriteLine("if ((Thread.CurrentThread.CurrentUICulture.Name == _currentCultureName)");
			writer.WriteLine("\t&& (IdentityMapScope.Current == _currentIdentityMap))");
			writer.WriteLine("{");
			writer.WriteLine(String.Format("return _current{0};", ClassHelper.GetClassName(table)));
			writer.WriteLine("}");
			writer.WriteLine();
			writer.WriteLine("_currentCultureName = Thread.CurrentThread.CurrentUICulture.Name;");
			writer.WriteLine("_currentIdentityMap = IdentityMapScope.Current;");
			writer.WriteLine(String.Format("_current{0} = Language.GetByUICulture(Thread.CurrentThread.CurrentUICulture.Name);", ClassHelper.GetClassName(table)));
			writer.WriteLine(String.Format("return _current{0};", ClassHelper.GetClassName(table)));
			writer.WriteLine("}");
			writer.WriteLine("}");
			writer.WriteLine("[ThreadStatic]");
			writer.WriteLine("private static string _currentCultureName;");
			writer.WriteLine("[ThreadStatic]");
			writer.WriteLine("private static IdentityMap _currentIdentityMap;");
			writer.WriteLine("[ThreadStatic]");
			writer.WriteLine(String.Format("private static {0} _current{0};", ClassHelper.GetClassName(table)));
			writer.WriteLine();
		}
		#endregion

	}
}
