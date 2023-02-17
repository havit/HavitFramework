using System;
using System.Text;

// Soubor je použit (přilinkován) k Havit.Business.BusinessLayerGeneratoru, který využívá metodu HtmlEncode.
// Důstojnou alternativu k této metodě jsem v .NET světě nenašel (ani System.Net, ani AntiXSS nuget balíček, či jiné).
// Většinou je v těchto knihovnách encodována čeština, někdy i konce řádek.

namespace Havit.Web
{
	/// <summary>
	/// Poskytuje další pomocné metody pro kódování a dekódování textu pro použití na webu.
	/// </summary>
	public static partial class HttpUtilityExt
	{
		/// <summary>
		/// Zkonvertuje string do HTML-encoded podoby.
		/// Oproti standardnímu <see cref="System.Web.HttpUtility.HtmlEncode(string)"/> může encodovat všechny non-ASCII znaky
		/// a hlavně umožňuje pomocí options řídit požadovanou výslednou podobu. Lze například použít rozšířenou sadu HTML-entit,
		/// popřípadě úplně vyloučit převod ne-ASCII znaků na podobu &amp;#1234;.
		/// </summary>
		/// <param name="unicodeText">převáděný string v Unicode</param>
		/// <param name="options">options volby konverze</param>
		/// <returns>HTML-encoded string dle options</returns>
		public static string HtmlEncode(string unicodeText, HtmlEncodeOptions options)
		{
			int unicodeValue;
			StringBuilder result = new StringBuilder();

			bool opIgnoreNonASCIICharacters = ((options & HtmlEncodeOptions.IgnoreNonASCIICharacters) == HtmlEncodeOptions.IgnoreNonASCIICharacters);
			bool opExtendedHtmlEntities = ((options & HtmlEncodeOptions.ExtendedHtmlEntities) == HtmlEncodeOptions.ExtendedHtmlEntities);
			bool opXmlApostropheEntity = ((options & HtmlEncodeOptions.XmlApostropheEntity) == HtmlEncodeOptions.XmlApostropheEntity);

			int length = unicodeText.Length;
			for (int i = 0; i < length; i++)
			{
				unicodeValue = unicodeText[i];
				switch (unicodeValue) 
				{
					case '&':
						result.Append("&amp;");
						break;
					case '<':
						result.Append("&lt;");
						break;
					case '>':
						result.Append("&gt;");
						break;
					case '"':
						result.Append("&quot;");
						break;
					case '\'':
						if (opXmlApostropheEntity)
						{
							result.Append("&apos;");
							break;
						}
						else
						{
							goto default;
						}
					case 0xA0: // no-break space
						if (opExtendedHtmlEntities)
						{
							result.Append("&nbsp;");
							break;
						}
						else
						{
						goto default;
						}
					case '€':
						if (opExtendedHtmlEntities)
						{
							result.Append("&euro;");
							break;
						}
						else
						{
							goto default;
						}
					case '©':
						if (opExtendedHtmlEntities)
						{
							result.Append("&copy;");
							break;
						}
						else
						{
							goto default;
						}
					case '®':
						if (opExtendedHtmlEntities)
						{
							result.Append("&reg;");
							break;
						}
						else
						{
							goto default;
						}
					case '™': // trade-mark
						if (opExtendedHtmlEntities)
						{
							result.Append("&trade;");
							break;
						}
						else
						{
							goto default;
						}
					default:
						if (((unicodeText[i] >= ' ') && (unicodeText[i] <= 0x007E)) 
							|| opIgnoreNonASCIICharacters)
						{ 
							result.Append(unicodeText[i]);
						} 
						else 
						{
							result.Append("&#");
							result.Append(unicodeValue.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
							result.Append(";");
						}
						break;
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// Zkonvertuje string do HTML-encoded podoby s použitím výchozích options.
		/// Oproti standardnímu <see cref="System.Web.HttpUtility.HtmlEncode(string)"/> encoduje všechny non-ASCII znaky.
		/// </summary>
		/// <remarks>
		/// Pro podrobné řízení voleb konverze je nutno použít overload s options, takto je použito <see cref="HtmlEncodeOptions.None"/>,
		/// tj. pouze pět standardních XML entit (&amp;gt;; &amp;lt;, &amp;amp;, &amp;quot;, &amp;apos;)
		/// </remarks>
		/// <param name="unicodeText">převáděný string v Unicode</param>
		/// <returns>HTML-encoded string</returns>
		public static string HtmlEncode(string unicodeText)
		{
			return HtmlEncode(unicodeText, HtmlEncodeOptions.None);
		}
	}

	/// <summary>
	/// Poskytuje množinu hodnot k nastavení voleb metody <see cref="Havit.Web.HttpUtilityExt.HtmlEncode(string, HtmlEncodeOptions)"/>
	/// </summary>
	[Flags]
	public enum HtmlEncodeOptions
	{
		/// <summary>
		/// Označuje, že nemají být nastaveny žádné options, použije se default postup.
		/// Default postup převede pouze čtyři základní entity
		/// <list type="bullet">
		///		<item>&lt; --- &amp;lt;</item>
		///		<item>&gt; --- &amp;gt;</item>
		///		<item>&amp; --- &amp;amp;</item>
		///		<item>&quot; --- &amp;quot;</item>
		/// </list>
		/// </summary>
		None = 0,

		/// <summary>
		/// Při konverzi budou ignorovány znaky mimo ASCII hodnoty, nebudou tedy tvořeny číselné entity typu &amp;#123;.
		/// </summary>
		IgnoreNonASCIICharacters = 1,

		/// <summary>
		/// Při konverzi bude použita rozšířená sada HTML-entit, které by se jinak převedly na číselné entity.
		/// Např. bude použito &amp;copy;, &amp;nbsp;, &amp;sect;, atp. 
		/// </summary>
		ExtendedHtmlEntities = 2,

		/// <summary>
		/// Při konverzi převede apostrofy na &amp;apos; entitu.
		/// POZOR! &amp;apos; není standardní HTML entita a třeba IE ji v HTML režimu nepozná!!!
		/// </summary>
		/// <remarks>
		/// V kombinaci se základním <see cref="HtmlEncodeOptions.None"/> dostaneme sadu pěti built-in XML entit:
		/// <list type="bullet">
		///		<item>&lt; --- &amp;lt;</item>
		///		<item>&gt; --- &amp;gt;</item>
		///		<item>&amp; --- &amp;amp;</item>
		///		<item>&quot; --- &amp;quot;</item>
		///		<item>&apos; --- &amp;apos;</item>
		/// </list>
		/// </remarks>
		XmlApostropheEntity = 4
	}
}
