using System.Diagnostics.CodeAnalysis;
using System.Web;
using Havit.Web.Bootstrap.UI.ClientScripts;
using Havit.Web.UI.ClientScripts;

[assembly: PreApplicationStartMethod(typeof(StartUp), "Start")]

/// <summary>
/// Performs application startup tasks.
/// </summary>
[SuppressMessage("SonarLint", "S3903", Justification = "Je to špatně, ale z důvodu zpětné kompatibility neměním.")]
public static class StartUp
{
	/// <summary>
	/// Called on application startup. Run by PreApplicationStartMethodAttribute.
	/// Registers script resources.
	/// </summary>
	public static void Start()
	{
		HavitFrameworkClientScriptHelper.RegisterScriptResourceMappings();
		BootstrapClientScriptHelper.RegisterScriptResourceMappings();
	}
}