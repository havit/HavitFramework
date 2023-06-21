using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Havit.Data.Entity.CodeGenerator.Services;

public class CammelCaseNamingStrategy
{
	/// <summary>
	/// Zdroj: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
	/// </summary>
	private readonly HashSet<string> csharpKeywords = new HashSet<string>
	{
		"abstract",
		"as",
		"base",
		"bool",
		"break",
		"byte",
		"case",
		"catch",
		"char",
		"checked",
		"class",
		"const",
		"continue",
		"decimal",
		"default",
		"delegate",
		"do",
		"double",
		"else",
		"enum",
		"event",
		"explicit",
		"extern",
		"false",
		"finally",
		"fixed",
		"float",
		"for",
		"foreach",
		"goto",
		"if",
		"implicit",
		"in",
		"int",
		"interface",
		"internal",
		"is",
		"lock",
		"long",
		"namespace",
		"new",
		"null",
		"object",
		"operator",
		"out",
		"override",
		"params",
		"private",
		"protected",
		"public",
		"readonly",
		"ref",
		"return",
		"sbyte",
		"sealed",
		"short",
		"sizeof",
		"stackalloc",
		"static",
		"string",
		"struct",
		"switch",
		"this",
		"throw",
		"true",
		"try",
		"typeof",
		"uint",
		"ulong",
		"unchecked",
		"unsafe",
		"ushort",
		"using",
		"using",
		"static",
		"virtual",
		"void",
		"volatile",
		"while",
		// Contextual Keywords
		"add",
		"alias",
		"ascending",
		"async",
		"await",
		"descending",
		"dynamic",
		"from",
		"get",
		"global",
		"group",
		"into",
		"join",
		"let",
		"nameof",
		"orderby",
		"partial",
		"remove",
		"select",
		"set",
		"value",
		"var",
		"when",
		"where",
		"where",
		"yield"
	};

public string GetCammelCase(string name)
	{
		string result = name[0].ToString().ToLower() + name.Substring(1);
		return csharpKeywords.Contains(result)
			? "@" + result
			: result;
	}
}
