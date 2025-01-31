﻿using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// Parametr Skriptletu reprezentující renderovaný control Control.
/// </summary>
public class ControlParameter : ParameterBase
{

	/* Parametry ControlParametru *************** */

	/// <summary>
	/// Controlu, který je zdrojem pro vytvoření klientského parametru.
	/// Nesmí být zadáno současně s ControlName.
	/// Hodnota nepřežívá postback.
	/// </summary>
	public Control Control
	{
		get
		{
			return _control;
		}
		set
		{
			_control = value;
		}
	}
	private Control _control;

	/// <summary>
	/// Název controlu, který je zdrojem pro vytvoření klientského parametru.
	/// Pro vyhledávání ve vnořeném naming containeru lze názvy controlů oddělit tečkou.
	/// Nesmí být zadáno současně s Control.
	/// </summary>
	public string ControlName
	{
		get { return (string)ViewState["ControlName"]; }
		set { ViewState["ControlName"] = value; }
	}

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"P:Havit.Web.UI.Scriptlets.IScriptletParameter.Name")]/*' />        
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public override string Name
	{
		get { return base.Name ?? ControlName.Replace(".", "_"); }
		set { base.Name = value; }
	}

	/// <summary>
	/// Udává, zda v případě změny hodnoty prvku (zaškrtnutí, změna textu, apod.)
	/// dojde ke spuštění skriptu.
	/// Výchozí hodnota je <c>false</c>.
	/// </summary>
	public bool StartOnChange
	{
		get { return (bool)(ViewState["StartOnChange"] ?? false); }
		set { ViewState["StartOnChange"] = value; }
	}

	/* Kontrola platnosti parametrů *************** */

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.CheckProperties")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public override void CheckProperties()
	{
		base.CheckProperties();
		// navíc zkontrolujeme nastavení ControlName
		CheckControlAndControlNameProperty();
	}

	/// <summary>
	/// Testuje nastavení hodnoty property Name.
	/// Přepisuje chování předka tím způsobem, že zde není property Name povinná
	/// (takže se ani netestuje).
	/// </summary>
	protected override void CheckNameProperty()
	{
		// narozdíl od zde definujeme jméno jako nepovinné
		// nebudeme zde tedy jméno kontrolovat
	}

	/// <summary>
	/// Zkontroluje nastavení property <see cref="Control">Control</see> a <see cref="ControlName">ControlName</see>.
	/// Pokud není nastavena hodnota právě jedné vlastnosti, vyhodí výjimku.
	/// </summary>
	protected virtual void CheckControlAndControlNameProperty()
	{
		if ((_control == null) && String.IsNullOrEmpty(ControlName))
		{
			throw new HttpException("Není určen control, nastavte vlastnost Control nebo ControlName.");
		}
		if ((_control != null) && !String.IsNullOrEmpty(ControlName))
		{
			throw new HttpException("Není možné určit control vlastnostmi Control a ControlName zároveň.");
		}
	}

	/* Parametry IScriptletParameter *************** */

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetInitializeClientSideValueScript")]/*' />        
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public override void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder)
	{
		// najdeme control
		Control control = GetControl(parentControl);
		DoJobOnExtender(control, delegate (IControlExtender extender)
		{
			extender.GetInitializeClientSideValueScript(parameterPrefix, this, control, scriptBuilder);
		});
	}

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetAttachEventsScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public override void GetAttachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
	{
		// najdeme control
		Control control = GetControl(parentControl);
		DoJobOnExtender(control, delegate (IControlExtender extender)
		{
			extender.GetAttachEventsScript(parameterPrefix, this, control, scriptletFunctionCallDelegate, scriptBuilder);
		});
	}

	/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetDetachEventsScript")]/*' />
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1604:ElementDocumentationMustHaveSummary", Justification = "Bráno z externího souboru.")]
	public override void GetDetachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
	{
		// najdeme control
		Control control = GetControl(parentControl);
		DoJobOnExtender(control, delegate (IControlExtender extender)
		{
			extender.GetDetachEventsScript(parameterPrefix, this, control, scriptletFunctionCallDelegate, scriptBuilder);
		});
	}

	private void DoJobOnExtender(Control control, ExtenderJobEventHandler job)
	{
		// ak když je viditelný
		if (control.Visible)
		{
			// najdeme extender, který tento control bude řešit
			IControlExtender extender = Scriptlet.ControlExtenderRepository.FindControlExtender(control);
			// a řekneme, ať ho vyřeší
			job(extender);
		}
	}
	private delegate void ExtenderJobEventHandler(IControlExtender extender);

	/* *************** */

	/// <summary>
	/// Nalezne Control, který má být zpracován.
	/// Pokud není Control nalezen, vyhodí výjimku HttpException.
	/// </summary>
	/// <param name="parentControl">Control v rámci něhož se hledá (NamingContainer).</param>
	/// <returns>Control.</returns>
	protected virtual Control GetControl(Control parentControl)
	{
		if (_control != null)
		{
			return Control;
		}

		string controlName = ControlName.Replace(".", "$");

		Control result;
		if (controlName.StartsWith("Page$"))
		{
			result = this.Page.FindControl(controlName.Substring(5)); // 5 .. přeskočíme "Page."
		}
		else
		{
			result = parentControl.FindControl(controlName);
		}

		if (result == null)
		{
			throw new HttpException(String.Format("Control {0} nebyl nalezen.", ControlName));
		}

		return result;
	}
}
