using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.Scriptlets;

    /// <summary>
    /// Předek pro tvorbu klientských parametrů.
    /// </summary>
[ControlBuilder(typeof(NoLiteralContolBuilder))]
public abstract class ParameterBase : ScriptletNestedControl, IScriptletParameter
{
	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"P:Havit.Web.UI.Scriptlets.IScriptletParameter.Name")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public virtual string Name
	{
		get { return (string)ViewState["Name"]; }
		set { ViewState["Name"] = value; }
	}

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.CheckProperties")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public virtual void CheckProperties()
	{
		// zkontrolujeme property Name
		CheckNameProperty();
	}

	/// <summary>
	/// Testuje nastavení hodnoty property Name.
	/// Pokud není hodnota nastavena, je vyhozena výjimka.
	/// </summary>
	protected virtual void CheckNameProperty()
	{
		if (String.IsNullOrEmpty(Name))
		{
			throw new ArgumentException("Property Name není nastavena.");
		}
	}

	/// <summary>
	/// Zavoláno, když je do kolekce Controls přidán Control.
	/// Zajišťuje, aby nebyl přidán control neimplementující 
	/// IScriptletParameter.
	/// </summary>
	/// <param name="control">Přidávaný control.</param>
	/// <param name="index">Pozice v kolekci controlů, kam je control přidáván.</param>
	protected override void AddedControl(Control control, int index)
	{
		base.AddedControl(control, index);

		if (!(control is IScriptletParameter))
		{
			throw new ArgumentException(String.Format("Do parametru scriptletu je vkládán nepodporovaný control {0}.", control));
		}
	}

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetInitializeClientSideValueScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public abstract void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetAttachEventsScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public abstract void GetAttachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder);

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetDetachEventsScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public abstract void GetDetachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder);
}
