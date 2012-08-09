using System;
using System.Text.RegularExpressions;
using System.Configuration;
using Havit.Web.UrlRewriter.Config;

namespace Havit.Web.UrlRewriter
{
	/// <summary>
	/// Provides a rewriting HttpModule.
	/// </summary>
	public class ModuleRewriter : BaseModuleRewriter
	{
		/// <summary>
		/// Metoda řešící vlastní UrlRewriting zdrojové adresy na cílovou.
		/// </summary>
		/// <param name="requestedPath">The Path being requested.</param>
		/// <param name="app">The HttpApplication instance.</param>
		protected override void Rewrite(string requestedPath, System.Web.HttpApplication app)
		{
			// log information to the Trace object.
			app.Context.Trace.Write("ModuleRewriter", "Entering ModuleRewriter");

			// get the configuration rules
			RewriterRuleCollection rules = RewriterConfiguration.GetConfig().Rules;

			// iterate through each rule...
			for(int i = 0; i < rules.Count; i++)
			{
				// get the pattern to look for, and Resolve the Url (convert ~ into the appropriate directory)
				string lookFor = "^" + HttpServerUtilityExt.ResolveUrl(app.Context.Request.ApplicationPath, rules[i].LookFor) + "$";

				// Create a regex (note that IgnoreCase is set...)
				Regex re = new Regex(lookFor, RegexOptions.IgnoreCase);

				// See if a match is found
				if (re.IsMatch(requestedPath))
				{
					// match found - do any replacement needed
					string sendToUrl = HttpServerUtilityExt.ResolveUrl(app.Context.Request.ApplicationPath, re.Replace(requestedPath, rules[i].SendTo));

					// log rewriting information to the Trace object
					app.Context.Trace.Write("ModuleRewriter", "Rewriting URL to " + sendToUrl);

					// Rewrite the URL
					RewriterUtils.RewriteUrl(app.Context, sendToUrl);
					break;		// exit the for loop
				}
			}

			// Log information to the Trace object
			app.Context.Trace.Write("ModuleRewriter", "Exiting ModuleRewriter");
		}
	}
}
