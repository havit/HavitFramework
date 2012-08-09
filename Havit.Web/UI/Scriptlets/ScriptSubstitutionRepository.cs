using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// Repository substitucí.
	/// </summary>
    public class ScriptSubstitutionRepository : List<IScriptSubstitution>, IScriptSubstitution
    {
		#region Default (static)
		/// <summary>
		/// Výchozí substituce. Použita, pokud není scriptletu nastaveno jinak.
		/// </summary>
		public static ScriptSubstitutionRepository Default
		{
			get
			{
				lock (typeof(ScriptSubstitutionRepository))
				{
					if (_default == null)
					{
						_default = new ScriptSubstitutionRepository();
					}
				}
				return _default;
			}
		}
		private static ScriptSubstitutionRepository _default = null;
		#endregion

		#region Substitute
		/// <summary>
		/// Provede substituci tím zpùsobem, že zavolá postupnì substituce
		/// na všech instancích v repository.
		/// </summary>
		/// <param name="script">Skript, ve kterém má dojít k substituci.</param>
		/// <returns>Substituovaný skript.</returns>
		public string Substitute(string script)
        {
            string result = script;
			foreach (IScriptSubstitution scriptSubstitution in this)
			{
				result = scriptSubstitution.Substitute(result);
			}
            return result;                
        }
		#endregion
	}
}