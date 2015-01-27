using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Havit.Web.UI
{
	/// <summary>
	/// Extension methods k ScriptResourceMapping.
	/// </summary>
	public static class ScriptResourceMappingExt
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
			TryEnsureScriptRegistrationResult registrationScriptResult = TryEnsureScriptRegistration(scriptResourceMapping, page, resourceName);

			switch (registrationScriptResult)
			{
				case TryEnsureScriptRegistrationResult.MissingScriptResourceMapping:
					throw new InvalidOperationException(String.Format("Missing script resource mapping '{0}'. Please add a ScriptResourceMapping named '{0}' (case-sensitive).", resourceName));

				case TryEnsureScriptRegistrationResult.ScriptResourceMappingWhileAsyncPostback:
					// TODO: lépe udělat persistence skriptů - vlastní script manager?
					//if (HttpContext.Current.IsDebuggingEnabled)
					//{
					//	throw new InvalidOperationException(String.Format("Attempting to register '{0}' during asynchronous postback, which is not possible. Register script in the page or in master page.", resourceName));
					//}
					break;
			}
		}
		#endregion

		#region TryEnsureScriptRegistration
		/// <summary>
		/// Pokusí se zaregistrovat ClientScriptResource. Pokud neuspěje, vrací false.
		/// </summary>
		/// <param name="scriptResourceMapping">ScriptResourceMapping, ze kterého se právádí registrace scriptu.</param>
		/// <param name="page">Stránka, do které se registrace skriptu provádí.</param>
		/// <param name="resourceName">Jméno registrovaného scriptu.</param>
		public static TryEnsureScriptRegistrationResult TryEnsureScriptRegistration(this ScriptResourceMapping scriptResourceMapping, Page page, string resourceName)
		{
			Contract.Requires<ArgumentNullException>(scriptResourceMapping != null, "scriptResourceMapping");
			Contract.Requires<ArgumentNullException>(page != null, "page");
			Contract.Requires(!String.IsNullOrEmpty(resourceName));

			ScriptManager currentScriptManager = ScriptManager.GetCurrent(page);
			bool scriptResourceExists = currentScriptManager.Scripts.Any(script => script.Name.Equals(resourceName));

			if ((scriptResourceMapping != null)
				&& (scriptResourceMapping.GetDefinition(resourceName, typeof(Page).Assembly) == null)
				&& (scriptResourceMapping.GetDefinition(resourceName) == null))
			{
				return TryEnsureScriptRegistrationResult.MissingScriptResourceMapping;
			}
			else if (currentScriptManager.IsInAsyncPostBack && !scriptResourceExists)
			{
				return TryEnsureScriptRegistrationResult.ScriptResourceMappingWhileAsyncPostback;
			}
			else
			{
				if (!scriptResourceExists)
				{
					ScriptManager.RegisterNamedClientScriptResource(page, resourceName);
				}
				return TryEnsureScriptRegistrationResult.OK;
			}
		}
		#endregion

		#region EnsureScriptRegistrationForEmbeddedResource
		/// <summary>
		/// Zajistí registraci ClientScriptResource pomocí ScriptResourceMappingu pro embedded resource.
		/// </summary>
		private static void EnsureScriptRegistrationForEmbeddedResource(this ScriptResourceMapping scriptResourceMapping, Page page, Control control, Type type, string embeddedResourceName)
		{
			string resourceFullName = type.Assembly.FullName + "|" + embeddedResourceName;

			ScriptManager currentScriptManager = ScriptManager.GetCurrent(page);
			bool scriptResourceExists = currentScriptManager.Scripts.Any(script => script.Name.Equals(resourceFullName));

			if ((scriptResourceMapping != null)
				&& (scriptResourceMapping.GetDefinition(resourceFullName, typeof(Page).Assembly) == null)
				&& (scriptResourceMapping.GetDefinition(resourceFullName) == null))
			{
				ScriptManager.ScriptResourceMapping.AddDefinition(resourceFullName, new ScriptResourceDefinition { Path = page.ClientScript.GetWebResourceUrl(type, embeddedResourceName) });
			}
			// TODO: lépe udělat persistence skriptů - vlastní script manager?
			//else if (currentScriptManager.IsInAsyncPostBack && !scriptResourceExists && HttpContext.Current.IsDebuggingEnabled)
			//{
			//	throw new InvalidOperationException(String.Format("Attempting to register embedded resource script '{0}' during asynchronous postback, which is not possible. Register script in the page or in master page.", embeddedResourceName));
			//}
			else
			{
				if (control == null)
				{
					ScriptManager.RegisterNamedClientScriptResource(page, resourceFullName);
				}
				else
				{
					ScriptManager.RegisterNamedClientScriptResource(control, resourceFullName);
				}
			}
		}

		/// <summary>
		/// Zajistí registraci ClientScriptResource pomocí ScriptResourceMappingu pro embedded resource.
		/// </summary>
		/// <param name="scriptResourceMapping">ScriptResourceMapping, ze kterého se právádí registrace scriptu.</param>
		/// <param name="page">Stránka, ke které se registrují scripty.</param>
		/// <param name="type">Jméno typu pro určení, ve které assembly se embedded resource nachází.</param>
		/// <param name="embeddedResourceName">Embedded resource name.</param>
		public static void EnsureScriptRegistrationForEmbeddedResource(this ScriptResourceMapping scriptResourceMapping, Page page, Type type, string embeddedResourceName)
		{
			Contract.Requires<ArgumentNullException>(scriptResourceMapping != null, "scriptResourceMapping");
			Contract.Requires<ArgumentNullException>(page != null, "page");
			Contract.Requires<ArgumentNullException>(type != null, "type");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(embeddedResourceName), "Parameter embeddedResourceName cannot be an empty string.");

			EnsureScriptRegistrationForEmbeddedResource(scriptResourceMapping, page, null, type, embeddedResourceName);
		}

		/// <summary>
		/// Zajistí registraci ClientScriptResource pomocí ScriptResourceMappingu pro embedded resource.
		/// </summary>
		/// <param name="scriptResourceMapping">ScriptResourceMapping, ze kterého se právádí registrace scriptu.</param>
		/// <param name="control">Control, ke kterému se registrují scripty.</param>
		/// <param name="type">Jméno typu pro určení, ve které assembly se embedded resource nachází.</param>
		/// <param name="embeddedResourceName">Embedded resource name.</param>
		public static void EnsureScriptRegistrationForEmbeddedResource(this ScriptResourceMapping scriptResourceMapping, Control control, Type type, string embeddedResourceName)
		{
			Contract.Requires<ArgumentNullException>(scriptResourceMapping != null, "scriptResourceMapping");
			Contract.Requires<ArgumentNullException>(control != null, "control");
			Contract.Requires<ArgumentException>(control.Page != null, "Parameter control.Page cannot be null.");
			Contract.Requires<ArgumentNullException>(type != null, "type");
			Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(embeddedResourceName), "Parameter embeddedResourceName cannot be an empty string.");

			EnsureScriptRegistrationForEmbeddedResource(scriptResourceMapping, control.Page, control, type, embeddedResourceName);
		}
		#endregion

	}
}
