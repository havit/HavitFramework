using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.Scriptlets
{
    /// <summary>
    /// Parametr Skriptletu reprezentující renderovanı control Control.
    /// </summary>
    public class ControlParameter : ParameterBase
    {

		/* Parametry ControlParametru *************** */

		#region Control
		/// <summary>
		/// Controlu, kterı je zdrojem pro vytvoøení klientského parametru.
		/// Nesmí bıt zadáno souèasnì s ControlName.
		/// Hodnota nepøeívá postback.
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
		#endregion

		#region ControlName
		/// <summary>
		/// Název controlu, kterı je zdrojem pro vytvoøení klientského parametru.
		/// Pro vyhledávání ve vnoøeném naming containeru lze názvy controlù oddìlit teèkou.
		/// Nesmí bıt zadáno souèasnì s Control.
		/// </summary>
		public string ControlName
		{
			get { return (string)ViewState["ControlName"]; }
			set { ViewState["ControlName"] = value; }
		}
		#endregion

		#region Name
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"P:Havit.Web.UI.Scriptlets.IScriptletParameter.Name")]/*' />        
		public override string Name
		{
			get { return base.Name ?? ControlName.Replace(".", "_"); }
			set { base.Name = value; }
		}		
		#endregion

		#region StartOnChange
		/// <summary>
		/// Udává, zda v pøípadì zmìny hodnoty prvku (zaškrtnutí, zmìna textu, apod.)
		/// dojde ke spuštìní skriptu.
		/// Vıchozí hodnota je <c>false</c>.
		/// </summary>
		public bool StartOnChange
		{
			get { return (bool)(ViewState["StartOnChange"] ?? false); }
			set { ViewState["StartOnChange"] = value; }
		}
		#endregion

		/* Kontrola platnosti parametrù *************** */

		#region CheckProperties (overriden)
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.CheckProperties")]/*' />
		public override void CheckProperties()
		{
			base.CheckProperties();
			// navíc zkontrolujeme nastavení ControlName
			CheckControlAndControlNameProperty();
		}		
		#endregion

		#region CheckNameProperty
		/// <summary>
		/// Testuje nastavení hodnoty property Name.
		/// Pøepisuje chování pøedka tím zpùsobem, e zde není property Name povinná
		/// (take se ani netestuje).
		/// </summary>
		protected override void CheckNameProperty()
		{
			// narozdíl od zde definujeme jméno jako nepovinné
			// nebudeme zde tedy jméno kontrolovat
		}
		#endregion

		#region CheckControlAndControlNameProperty
		/// <summary>
		/// Zkontroluje nastavení property <see cref="Control">Control</see> a <see cref="ControlName">ControlName</see>.
		/// Pokud není nastavena hodnota právì jedné vlastnosti, vyhodí vıjimku.
		/// </summary>
		protected virtual void CheckControlAndControlNameProperty()
		{
			if ((_control == null) && String.IsNullOrEmpty(ControlName))
			{
				throw new HttpException("Není urèen control, nastavte vlastnost Control nebo ControlName.");
			}
			if ((_control != null) && !String.IsNullOrEmpty(ControlName))
			{
				throw new HttpException("Není moné urèit control vlastnostmi Control a ControlName zároveò.");
			}
		}
		#endregion

		/* Parametry IScriptletParameter *************** */

		#region GetInitializeClientSideValueScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetInitializeClientSideValueScript")]/*' />        
		public override void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder)
		{
			// najdeme control
			Control control = GetControl(parentControl);
			DoJobOnExtender(control, delegate(IControlExtender extender)
			{
				extender.GetInitializeClientSideValueScript(parameterPrefix, this, control, scriptBuilder);
			});
		}
		#endregion

		#region GetAttachEventsScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetAttachEventsScript")]/*' />
		public override void GetAttachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			// najdeme control
			Control control = GetControl(parentControl);
			DoJobOnExtender(control, delegate(IControlExtender extender)
			{
				extender.GetAttachEventsScript(parameterPrefix, this, control, scriptletFunctionCallDelegate, scriptBuilder);
			});
		}
		#endregion

		#region GetDetachEventsScript
		/// <include file='IScriptletParameter.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IScriptletParameter.GetDetachEventsScript")]/*' />
		public override void GetDetachEventsScript(string parameterPrefix, Control parentControl, string scriptletFunctionCallDelegate, ScriptBuilder scriptBuilder)
		{
			// najdeme control
			Control control = GetControl(parentControl);
			DoJobOnExtender(control, delegate(IControlExtender extender)
			{
				extender.GetDetachEventsScript(parameterPrefix, this, control, scriptletFunctionCallDelegate, scriptBuilder);
			});
		}
		#endregion

		#region DoJobOnExtender (ExtenderJobEventHandler)
		private void DoJobOnExtender(Control control, ExtenderJobEventHandler job)
		{
			// ak kdy je viditelnı
			if (control.Visible)
			{
				// najdeme extender, kterı tento control bude øešit
				IControlExtender extender = Scriptlet.ControlExtenderRepository.FindControlExtender(control);
				// a øekneme, a ho vyøeší
				job(extender);
			}
		}
		private delegate void ExtenderJobEventHandler(IControlExtender extender);
		
		#endregion		

		/* *************** */

		#region GetControl
		/// <summary>
		/// Nalezne Control, kterı má bıt zpracován.
		/// Pokud není Control nalezen, vyhodí vıjimku HttpException.
		/// </summary>
		/// <param name="parentControl">Control v rámci nìho se hledá (NamingContainer).</param>
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
				result = this.Page.FindControl(controlName.Substring(5)); // 5 .. pøeskoèíme "Page."
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
		#endregion        
		
    }
}
