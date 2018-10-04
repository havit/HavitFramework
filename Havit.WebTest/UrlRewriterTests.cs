using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Havit.Web.UrlRewriter.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.WebTest
{
	[TestClass]
	public class UrlRewriterTests
	{
		/// <summary>
		/// Metoda testuje schopnost načíst konfiguraci UrlRewriteru, v širším slova smyslu zkouší, zda funguje správně XML serializace/deserializace.
		/// 
		/// Čas od času se po nějaké úpravě v knihovně stane, že načtení konfigurace přestane fungovat, nepodařilo se určit přesnou příčinu.
		/// Projev je takový, že XML serializer začne ignorovat attribut XmlRootAttribute u dané třídy, což lze vidět zobrazením
		/// assembly Microsoft.GeneratedCode.dll v .NET Reflectoru.
		/// Výskyt problému nenačtení konfigurace vypadá velmi nedeterministicky,
		/// ale s určitou verzí/podobou zdrojových kódů se každý build chová vždy stejně.
		/// 
		/// Pomáhá doplnit nebo odebrat k problematické třídě (nebo i jinam, například k úplně nové třídě) nějaký atribut.
		/// Celé to vypadá na problém v SGEN, resp. v System.Xml.Serialization.XmlSerializerFactory či podobné třídě zajišťující serializaci.
		/// </summary>
		[TestMethod]		
		public void UrlRewriter_RewriterConfigSerializerSectionHandler()
		{
			string configurationString = @"
				<UrlRewriterConfig>
					<Rules>
						<RewriterRule>
							<LookFor>~/business/sablony/e(\d+).*\.aspx</LookFor>
							<SendTo>~/business/sablony/sekce.aspx?id=$1</SendTo>
						</RewriterRule>
					</Rules>
				</UrlRewriterConfig>";

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(configurationString);

#pragma warning disable 618
			RewriterConfigSerializerSectionHandler configSectionHandler = new RewriterConfigSerializerSectionHandler();
			object config = configSectionHandler.Create(null, null, doc);
			Assert.IsInstanceOfType(config, typeof(RewriterConfiguration));
#pragma warning restore 618
		}
	}
}
