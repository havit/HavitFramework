using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Havit.Web.Bootstrap.Tutorial;

public static class ScriptManagerConfig
{
	public static void RegisterScriptResourceMappings()
	{
		// není nutno registrovat jquery - zajišťuje AspNet.Scriptmanager.jquery
		// není nutno registrovat bootstrap - zajišťuje Havit.Web.Bootstrap
	}
}