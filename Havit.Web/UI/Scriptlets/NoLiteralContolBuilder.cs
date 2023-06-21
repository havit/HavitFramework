using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Havit.Web.UI.Scriptlets;

    /// <summary>
/// Control builder pro <see cref="Scriptlet">Scriptlet</see>.
/// Omezuje chybné použití controlu <see cref="Scriptlet">Scriptlet</see>.
    /// </summary>
    internal class NoLiteralContolBuilder : ControlBuilder
    {
    public override bool AllowWhitespaceLiterals()
	{
		return false;
	}

    public override void AppendLiteralString(string s)
	{
		if (s.Trim().Length > 0)
		{
			throw new HttpException("Textový literál je na nepovoleném místě.");
		}
	}
    }