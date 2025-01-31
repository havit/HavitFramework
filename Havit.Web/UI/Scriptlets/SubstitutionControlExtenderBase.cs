﻿using System.Diagnostics.CodeAnalysis;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// ControlExtender určený ke tvorbě potomků, kteří provádějí substituci za jiný control.
/// Např. pokud je potřeba substituovat UserControl za některý vnořený control.
/// </summary>
public abstract class SubstitutionControlExtenderBase : IControlExtender
{
	/// <summary>
	/// Vrátí hodnotu priority ControlExtenderu, která se použije, pokud je 
	/// ControlExtender použitelný pro zpracování controlu.
	/// </summary>
	protected virtual int GetPriorityValue
	{
		get
		{
			return 100;
		}
	}

	/// <summary>
	/// Vrací priotitu vhodnosti extenderu pro zpracování controlu.
	/// Pokud extender není vhodný pro zpracování controlu, vrací null.
	/// </summary>
	/// <param name="control">Ověřovaný control.</param>
	/// <returns>Priorita.</returns>
	public int? GetPriority(Control control)
	{
		return (GetSupportedControlType().IsInstanceOfType(control)) ? (int?)this.GetPriorityValue : null;
	}

	/// <summary>
	/// Vytvoří klientský parametr pro předaný control.
	/// </summary>
	/// <param name="parameterPrefix">Název objektu na klientské straně.</param>
	/// <param name="parameter">Parametr předávající řízení extenderu.</param>
	/// <param name="control">Control ke zpracování.</param>
	/// <param name="scriptBuilder">Script builder.</param>
	public void GetInitializeClientSideValueScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, ScriptBuilder scriptBuilder)
	{
		Control substitutedControl = GetSubstitutedControl(control);
		parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetInitializeClientSideValueScript(parameterPrefix, parameter, substitutedControl, scriptBuilder);
	}

	/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetAttachEventsScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public void GetAttachEventsScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
	{
		Control substitutedControl = GetSubstitutedControl(control);
		parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetAttachEventsScript(parameterPrefix, parameter, substitutedControl, scriptletFunctionCallDelegate, scriptBuilder);
	}

	/// <include file='IControlExtender.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlExtender.GetDetachEventsScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public void GetDetachEventsScript(string parameterPrefix, IScriptletParameter parameter, System.Web.UI.Control control, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
	{
		Control substitutedControl = GetSubstitutedControl(control);
		parameter.Scriptlet.ControlExtenderRepository.FindControlExtender(substitutedControl).GetDetachEventsScript(parameterPrefix, parameter, substitutedControl, scriptletFunctionCallDelegate, scriptBuilder);
	}

	/// <summary>
	/// Vrací substituovaný control.
	/// </summary>
	protected abstract Control GetSubstitutedControl(Control control);

	/// <summary>
	/// Vrací typ, který je třídou podporován k substituci.
	/// </summary>
	protected abstract Type GetSupportedControlType();
}
