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
#warning Comment
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

		#region ControlName
		/// <summary>
		/// Název Controlu, kterı je zdrojem pro vytvoøení klientského parametru.
		/// Pro vyhledávání ve vnoøeném naming containeru lze názvy controlù oddìlit teèkou.
		/// </summary>
		public string ControlName
		{
			get { return (string)ViewState["ControlName"]; }
			set { ViewState["ControlName"] = value; }
		}
		#endregion

		#region Name
		/// <summary>
		/// Název parametru, pod kterım bude parametr pøístupnı v klienském skriptu.
		/// Pokud není hodnota nastavena, pouije se hodnota <see cref="ControlName">ControlName</see>.
		/// Obsahuje-li <see cref="ControlName">ControlName</see> teèku, je nahrazena podtrítkem.
		/// </summary>
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

		#region CheckProperties (overriden)
		/// <summary>
		/// Zkontroluje, zda je parametr správnì inicializován.
		/// </summary>
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

		#region GetInitializeClientSideValueScript
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetInitializeClientSideValueScript")]/*' />        
		public override void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder builder)
		{
			// najdeme control
			Control control = GetControl(parentControl);
			DoJobOnExtender(control, delegate(IControlExtender extender)
			{
				extender.GetInitializeClientSideValueScript(parameterPrefix, this, control, builder);
			});
		}
		#endregion

		#region GetAttachEventsScript
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetAttachEventsScript")]/*' />
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
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetDetachEventsScript")]/*' />
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

			Control result = parentControl.FindControl(controlName);

			if (result == null)
			{
				throw new HttpException(String.Format("Control {0} nebyl nalezen.", ControlName));
			}

			return result;
		}
		#endregion        
		
    }
}
