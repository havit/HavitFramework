using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.Web.UI
{
	/// <summary>
	/// Extension methods k ScriptResourceMapping.
	/// </summary>
	internal static class ScriptResourceMappingExt
	{
		/// <summary>
		/// Zajistí registraci ClietScriptResource.
		/// </summary>
		/// <param name="scriptResourceMapping">ScriptResourceMapping, ze kterého se právádí registrace scriptu.</param>
		/// <param name="page">Stránka, do které se registrace skriptu provádí.</param>
		/// <param name="resourceName">Jméno scriptu</param>
		public static void EnsureScriptRegistration(this ScriptResourceMapping scriptResourceMapping, Page page, string resourceName)
		{
			Contract.Requires(page != null);
			Contract.Requires(!String.IsNullOrEmpty(resourceName));
			
			if ((scriptResourceMapping != null)
				&& (scriptResourceMapping.GetDefinition(resourceName, typeof(Page).Assembly) == null)
				&& (scriptResourceMapping.GetDefinition(resourceName) == null))
			{
				throw new InvalidOperationException(String.Format("Missing script resource mapping '{0}'. Please add a ScriptResourceMapping named '{0}' (case-sensitive).", resourceName));
			}
			else
			{
				ScriptManager.RegisterNamedClientScriptResource(page, resourceName);
			}
		}
	}
}
