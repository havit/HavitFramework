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
    /// Repository extenderů. 
    /// </summary>
    public interface IControlExtenderRepository
    {
	    /// <summary>
        /// Vrací extender, který má Control zpracovávat.
        /// </summary>
        /// <param name="control">Control, který bude zpracováván.</param>
        /// <returns>Nalezený extender.</returns>
        IControlExtender FindControlExtender(Control control);
    }
}