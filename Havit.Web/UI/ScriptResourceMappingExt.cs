using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		#region EnsureScriptRegistration
		/// <summary>
		/// Zajistí registraci ClientScriptResource.
		/// </summary>
		/// <param name="scriptResourceMapping">ScriptResourceMapping, ze kterého se právádí registrace scriptu.</param>
		/// <param name="page">Stránka, do které se registrace skriptu provádí.</param>
		/// <param name="resourceName">Jméno registrovaného scriptu.</param>
		public static void EnsureScriptRegistration(this ScriptResourceMapping scriptResourceMapping, Page page, string resourceName)
		{
			Contract.Requires(scriptResourceMapping != null);
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
		#endregion

		#region EnsureScriptRegistrationForEmbeddedResource
		/// <summary>
		/// Zajistí registraci ClientScriptResource pomocí ScriptResourceMappingu pro embedded resource.
		/// </summary>
		/// <param name="scriptResourceMapping">ScriptResourceMapping, ze kterého se právádí registrace scriptu.</param>
		/// <param name="page">Stránka, do které se registrace skriptu provádí.</param>
		/// <param name="type">Jméno typu pro určení, ve které assembly se embedded resource nachází.</param>
		/// <param name="embeddedResourceName">Embedded resource name.</param>
		public static void EnsureScriptRegistrationForEmbeddedResource(this ScriptResourceMapping scriptResourceMapping, Page page, Type type, string embeddedResourceName)
		{
			Contract.Requires(scriptResourceMapping != null);
			Contract.Requires(page != null);
			Contract.Requires(type != null);
			Contract.Requires(!String.IsNullOrEmpty(embeddedResourceName));

			string resourceFullName = type.Assembly.FullName + "|" + embeddedResourceName;

			if ((scriptResourceMapping != null)
				&& (scriptResourceMapping.GetDefinition(resourceFullName, typeof(Page).Assembly) == null)
				&& (scriptResourceMapping.GetDefinition(resourceFullName) == null))
			{
				ScriptManager.ScriptResourceMapping.AddDefinition(resourceFullName, new ScriptResourceDefinition { Path = page.ClientScript.GetWebResourceUrl(type, embeddedResourceName) });
			}
			ScriptManager.RegisterNamedClientScriptResource(page, resourceFullName);
		}
		#endregion

	}
}
