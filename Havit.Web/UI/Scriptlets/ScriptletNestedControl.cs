using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets
{
	/// <summary>
	/// Èistì formální tøída zpøístupòující Scriptlet pro prvky vkládané do Scritletu.
	/// </summary>
	public abstract class ScriptletNestedControl: Control
	{
		/// <summary>
		/// Zpøístupòuje Scriplet, ve kterém je tento ClientScript obsažen.
		/// </summary>
		public Scriptlet Scriptlet
		{
			get {
                if (Parent is IScriptletParameter)
                    return ((IScriptletParameter)Parent).Scriptlet;
                return (Scriptlet)Parent; 
            }
		}

	}
}
