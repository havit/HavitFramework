using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// Čistě formální třída zpřístupňující Scriptlet pro prvky vkládané do Scritletu.
/// </summary>
public abstract class ScriptletNestedControl : Control
{
	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"P:Havit.Web.UI.Scriptlets.IScriptletParameter.Scriptlet")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public Scriptlet Scriptlet
	{
		get
		{
			if (Parent is IScriptletParameter)
			{
				return ((IScriptletParameter)Parent).Scriptlet;
			}
			
			if (!(Parent is Scriptlet))
			{
				throw new InvalidOperationException("Scriptlet nebyl nalezen - control se nachází na neznámém místě ve stromu controlů.");
			}
			
			return (Scriptlet)Parent;
		}
	}
}
