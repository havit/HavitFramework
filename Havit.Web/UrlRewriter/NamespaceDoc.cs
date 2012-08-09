#if NDOC
namespace Havit.Web.UrlRewriter
{
	/// <summary>
	/// UrlRewriter zajišuje url-rewriting na základì RegEx pravidel definovanıch ve web.config.<br/>
	/// Vychází z MSDN èlánku <a href="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnaspp/html/urlrewriting.asp" target="_blank">URL Rewriting in ASP.NET</a>,
	/// kde je podrobnì rozvedena metodika, moné zpùsoby pouití i jejich vıhody/nevıhody.<br/>
	/// <br/>
	/// Veškeré nasazení UrlRewriteru probíhá pøes web.config, viz pøíklad.
	/// Nasazení mùe vypadat napøíklad takto:
	/// The structure in the Web.config file is as follows:
	/// <code>
	/// &lt;configuration&gt;
	/// 	&lt;configSections&gt;
	/// 		&lt;section name="UrlRewriterConfig" 
	/// 		            type="Havit.Web.UrlRewriter.Config.RewriterConfigSerializerSectionHandler, Havit.Web" /&gt;
	///		&lt;/configSections&gt;
	///		
	///		&lt;UrlRewriterConfig&gt;
	///			&lt;Rules&gt;
	///				&lt;RewriterRule&gt;
	///					&lt;LookFor&gt;~/(\d{4})/(\d{2})/(\d{2})\.aspx&lt;/LookFor&gt;
	///					&lt;SendTo&gt;>~/ShowBlogContent.aspx?year=$1&amp;amp;month=$2&amp;amp;day=$3&lt;/SendTo&gt;
	///				&lt;/RewriterRule&gt;
	///				&lt;RewriterRule&gt;
	///					&lt;LookFor&gt;~/(\d{4})/(\d{2})/Default\.aspx&lt;/LookFor&gt;
	///					&lt;SendTo&gt;&lt;![CDATA[~/ShowBlogContent.aspx?year=$1&amp;month=$2]]&gt;&lt;/SendTo&gt;
	///				&lt;/RewriterRule&gt;
	///				...
	///				&lt;RewriterRule&gt;
	///					&lt;LookFor&gt;<i>pattern</i>&lt;/LookFor&gt;
	///					&lt;SendTo&gt;<i>replace with</i>&lt;/SendTo&gt;
	///				&lt;/RewriterRule&gt;
	///			&lt;/Rules&gt;
	///		&lt;/UrlRewriterConfig&gt;
	///		
	///		&lt;system.web&gt;
	///			&lt;!--*******************************************************************************
	///				Comment either the &lt;httpModules&gt; or &lt;httpHandlers&gt; section to use
	///				URL rewriting...  Refer to the article for a discussion on the pros and
	///				cons of each approach...
	///				*******************************************************************************--&gt;
	///			&lt;httpModules&gt;
	///				&lt;add type="Havit.Web.UrlRewriter.ModuleRewriter, Havit.Web" name="ModuleRewriter" /&gt;
	///			&lt;/httpModules&gt;
	///			
	///			&lt;!--&lt;httpHandlers&gt;
	///				&lt;add verb="*" path="*.aspx" type="Havit.Web.UrlRewriter.RewriterFactoryHandler, Havit.web" /&gt;
	///			&lt;/httpHandlers&gt;--&gt;
	///			...
	///		&lt;/system.web&gt;
	///	&lt;/configuration&gt;
	/// </code>
	/// </summary>
	public class NamespaceDoc
	{
		// speciální tøída pro NDoc tvoøící help k namespace, a která se díky #if nekompiluje
	}
}
#endif
