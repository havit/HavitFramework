using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Havit.Web.UI;

/// <summary>
/// Extension methods k ScriptResourceMapping.
/// </summary>
public static class ScriptResourceMappingExt
{
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
				break;

			default:
				break;
		}
	}

	/// <summary>
	/// Pokusí se zaregistrovat ClientScriptResource. Pokud neuspěje, vrací false.
	/// </summary>
	/// <param name="scriptResourceMapping">ScriptResourceMapping, ze kterého se právádí registrace scriptu.</param>
	/// <param name="page">Stránka, do které se registrace skriptu provádí.</param>
	/// <param name="resourceName">Jméno registrovaného scriptu.</param>
	public static TryEnsureScriptRegistrationResult TryEnsureScriptRegistration(this ScriptResourceMapping scriptResourceMapping, Page page, string resourceName)
	{
		Contract.Requires<ArgumentNullException>(scriptResourceMapping != null, nameof(scriptResourceMapping));
		Contract.Requires<ArgumentNullException>(page != null, nameof(page));
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(resourceName), nameof(resourceName));

		ScriptManager currentScriptManager = ScriptManager.GetCurrent(page);
		bool scriptResourceExists = (currentScriptManager != null) && currentScriptManager.Scripts.Any(script => script.Name.Equals(resourceName));

		if ((scriptResourceMapping != null)
			&& (scriptResourceMapping.GetDefinition(resourceName, typeof(Page).Assembly) == null)
			&& (scriptResourceMapping.GetDefinition(resourceName) == null))
		{
			return TryEnsureScriptRegistrationResult.MissingScriptResourceMapping;
		}
		//else if ((currentScriptManager != null) && currentScriptManager.IsInAsyncPostBack && !scriptResourceExists)
		//{
		//	return TryEnsureScriptRegistrationResult.ScriptResourceMappingWhileAsyncPostback;
		//}
		else
		{
			if (!scriptResourceExists)
			{
				ScriptManager.RegisterNamedClientScriptResource(page, resourceName);
			}
			return TryEnsureScriptRegistrationResult.OK;
		}
	}

	/// <summary>
	/// Zajistí registraci ClientScriptResource pomocí ScriptResourceMappingu pro embedded resource.
	/// </summary>
	private static void EnsureScriptRegistrationForEmbeddedResource(this ScriptResourceMapping scriptResourceMapping, Page page, Control control, Type type, string embeddedResourceName)
	{
		string resourceFullName = type.Assembly.FullName + "|" + embeddedResourceName;

		//ScriptManager currentScriptManager = ScriptManager.GetCurrent(page);
		//bool scriptResourceExists = (currentScriptManager != null) && currentScriptManager.Scripts.Any(script => script.Name.Equals(resourceFullName));

		if ((scriptResourceMapping != null)
			&& (scriptResourceMapping.GetDefinition(resourceFullName, typeof(Page).Assembly) == null)
			&& (scriptResourceMapping.GetDefinition(resourceFullName) == null))
		{
			ScriptManager.ScriptResourceMapping.AddDefinition(resourceFullName, new ScriptResourceDefinition { Path = page.ClientScript.GetWebResourceUrl(type, embeddedResourceName) });
		}
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
		Contract.Requires<ArgumentNullException>(scriptResourceMapping != null, nameof(scriptResourceMapping));
		Contract.Requires<ArgumentNullException>(page != null, nameof(page));
		Contract.Requires<ArgumentNullException>(type != null, nameof(type));
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(embeddedResourceName), nameof(embeddedResourceName));

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
		Contract.Requires<ArgumentNullException>(scriptResourceMapping != null, nameof(scriptResourceMapping));
		Contract.Requires<ArgumentNullException>(control != null, nameof(control));
		Contract.Requires<ArgumentException>(control.Page != null, nameof(control) + "." + nameof(control.Page));
		Contract.Requires<ArgumentNullException>(type != null, nameof(type));
		Contract.Requires<ArgumentException>(!String.IsNullOrEmpty(embeddedResourceName), nameof(embeddedResourceName));

		EnsureScriptRegistrationForEmbeddedResource(scriptResourceMapping, control.Page, control, type, embeddedResourceName);
	}
}
