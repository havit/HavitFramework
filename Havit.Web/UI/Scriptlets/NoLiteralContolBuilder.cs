using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Control builder pro Scriptlet.
    /// Zajišuje omezuje chybné pouití controlu Scriptlet.
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
                throw new ArgumentException("Textovı literál je na nepovoleném místì.");
        }
        
    }
}