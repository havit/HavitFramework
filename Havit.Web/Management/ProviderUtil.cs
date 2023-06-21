using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.Management;

/// <summary>
/// Pomocné metody vykopírované z Microsoft .NET Frameworku.
/// </summary>
internal static class ProviderUtil
{
	/// <summary>
	/// Vyjme z konfigurace (config) hodnotu pro klíč (attrib) a nastaví ji do val.
	/// </summary>
	internal static void GetAndRemoveStringAttribute(NameValueCollection config, string attrib, string providerName, ref string val)
	{
		val = config.Get(attrib);
		config.Remove(attrib);
	}

	/// <summary>
	/// Vyjme z konfigurace (config) hodnotu pro klíč (attrib) a nastaví ji do val jako boolean.
	/// </summary>
	internal static void GetAndRemoveBooleanAttribute(NameValueCollection config, string attrib, string providerName, ref bool? val)
	{
		string value = config.Get(attrib);
		if (!String.IsNullOrEmpty(value))
		{
			val = Boolean.Parse(value);
		}
		config.Remove(attrib);
	}

	/// <summary>
	/// Vyjme z konfigurace (config) hodnotu pro klíč (attrib) a nastaví ji do val jako int.
	/// </summary>
	internal static void GetAndRemoveIntegerAttribute(NameValueCollection config, string attrib, string providerName, ref int? val)
	{
		string value = config.Get(attrib);
		if (!String.IsNullOrEmpty(value))
		{
			val = Int32.Parse(value);
		}
		config.Remove(attrib);
	}


	/// <summary>
	/// Pokud v konfiguraci (config) zbyla nějaká hodnota, je vyhozena výjimka ConfigurationErrorsException.
	/// </summary>
	internal static void CheckUnrecognizedAttributes(NameValueCollection config, string providerName)
	{
		if (config.Count > 0)
		{
			string key = config.GetKey(0);
			if (!string.IsNullOrEmpty(key))
			{
				throw new ConfigurationErrorsException(String.Format("Unexpected_provider_attribute = The attribute '{0}' is unexpected in the configuration of the '{1}' provider.", key, providerName));
			}
		}
	}

}