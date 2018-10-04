using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

[assembly: WebResource("WebApplicationTest.a.js", "text/javascript")]
[assembly: WebResource("WebApplicationTest.b.js", "text/javascript")]

namespace Havit.WebApplicationTest.HavitWebTests
{
	public partial class ScriptManagerTest : System.Web.UI.Page
	{
		#region OnInit
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			ScriptManager.ScriptResourceMapping.RemoveDefinition("test1");
			ScriptManager.ScriptResourceMapping.RemoveDefinition("test2");

			ScriptManager.ScriptResourceMapping.AddDefinition("test1", new ScriptResourceDefinition { Path = "~/SystemWebTests/a.js" });
			ScriptManager.ScriptResourceMapping.AddDefinition("test2", new ScriptResourceDefinition { Path = "~/SystemWebTests/b.js" });

			ScriptManager.ScriptResourceMapping.AddDefinition("WebApplicationTest.a.js", new ScriptResourceDefinition { Path = ClientScript.GetWebResourceUrl(typeof(ScriptManagerTest), "WebApplicationTest.a.js") });
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			ScriptManager.RegisterClientScriptBlock(this, typeof(ScriptManagerTest), "ClientScriptBlock-A", "//ClientScriptBlock-A", true);
			ScriptManager.RegisterClientScriptInclude(this, typeof(ScriptManagerTest), "Key", "~/a.js");
			//ScriptManager.RegisterClientScriptBlock(this, typeof(ScriptManagerTest), "ClientScriptBlock-B", "//ClientScriptBlock-B", true);
			//ScriptManager.RegisterClientScriptBlock(this, typeof(ScriptManagerTest), "ClientScriptBlock-C", "//ClientScriptBlock-C", true);
			//ScriptManager.RegisterClientScriptBlock(this, typeof(ScriptManagerTest), "ClientScriptBlock-D", "//ClientScriptBlock-D", true);
			//ScriptManager.RegisterClientScriptBlock(this, typeof(ScriptManagerTest), "ClientScriptBlock-E", "//ClientScriptBlock-E", true);
			//ScriptManager.RegisterClientScriptBlock(this, typeof(ScriptManagerTest), "ClientScriptBlock-F", "//ClientScriptBlock-F", true);

			ScriptManager.RegisterStartupScript(this, typeof(ScriptManagerTest), "StartupScript-B", "//StartupScript-B", true);

			//ScriptManager.RegisterClientScriptResource(this, typeof(ScriptManagerTest), "WebApplicationTest.a.js");

			//ScriptManager.RegisterNamedClientScriptResource(this, "test1");
			//ScriptManager.RegisterClientScriptResource(this, typeof(ScriptManagerTest), "WebApplicationTest.b.js");

			ScriptManager.RegisterNamedClientScriptResource(this, "test1");
			//ScriptManager.RegisterNamedClientScriptResource(this, "test2");
			//ScriptManager.RegisterNamedClientScriptResource(this, "WebApplicationTest.a.js");
			//ScriptManager.GetCurrent(this).Scripts.Add(new ScriptReference("WebApplicationTest.a.js", "WebApplicationTest"));
		}
		#endregion
	}
}