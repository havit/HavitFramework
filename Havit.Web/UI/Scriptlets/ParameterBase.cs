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
    /// Pøedek pro tvorbu klientskıch parametrù.
    /// </summary>
    [ControlBuilder(typeof(NoLiteralContolBuilder))]
    public abstract class ParameterBase : ScriptletNestedControl, IScriptletParameter
    {
		#region Name
		/// <summary>
		/// Název parametru, pod kterım bude parametr pøístupnı v klienském skriptu.
		/// </summary>
		public virtual string Name
		{
			get { return (string)ViewState["Name"]; }
			set { ViewState["Name"] = value; }
		}
		#endregion

		#region CheckProperties
		/// <summary>
		/// Zkontroluje, zda je parametr správnì inicializován.
		/// </summary>
		public virtual void CheckProperties()
		{
			// zkontrolujeme property Name
			CheckNameProperty();
		}
		#endregion

		#region CheckNameProperty
		/// <summary>
		/// Testuje nastavení hodnoty property Name.
		/// Pokud není hodnota nastavena, je vyhozena vıjimka.
		/// </summary>
		protected virtual void CheckNameProperty()
		{
			if (String.IsNullOrEmpty(Name))
			{
				throw new ArgumentException("Property Name není nastavena.");
			}
		}
		#endregion

		#region AddedControl
		/// <summary>
		/// Zavoláno, kdy je do kolekce Controls pøidán Control.
		/// Zajišuje, aby nebyl pøidán control neimplementující 
		/// IScriptletParameter.
		/// </summary>
		/// <param name="control">Pøidávanı control.</param>
		/// <param name="index">Pozice v kolekci controlù, kam je control pøidáván.</param>
		protected override void AddedControl(Control control, int index)
		{
			base.AddedControl(control, index);

			if (!(control is IScriptletParameter))
			{
				throw new ArgumentException(String.Format("Do parametru scriptletu je vkládán nepodporovanı control {0}.", control));
			}
		}
		#endregion

		#region GetInitializeClientSideValueScript (abstract, IScriptletParameter)
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetInitializeClientSideValueScript")]/*' />
		public abstract void GetInitializeClientSideValueScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
		#endregion

		#region GetAttachEventsScript (abstract, IScriptletParameter)
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetAttachEventsScript")]/*' />
		public abstract void GetAttachEventsScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
		#endregion

		#region GetDetachEventsScript (abstract, IScriptletParameter)
		/// <include file='..\\Dotfuscated\\Havit.Web.xml' path='doc/members/member[starts-with(@name,"M:Havit.Web.UI.Scriptlets.IControlParameter.GetDetachEventsScript")]/*' />
		public abstract void GetDetachEventsScript(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
		#endregion
	}
}
