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
        private List<IScriptletParameter> scriptletParameters = new List<IScriptletParameter>();
        /// <summary>
        /// Název parametru, pod kterım bude parametr pøístupnı v klienském skriptu.
        /// </summary>
		public virtual string Name
		{
			get { return (string)ViewState["Name"]; }
			set { ViewState["Name"] = value; }
		}

		/// <summary>
		/// Vytvoøí klientskı skript pro parametr.
		/// </summary>
		/// <param name="parameterPrefix">Prefix parametru.</param>
		/// <param name="parentControl">Control, v rámci kterého je tento parametr.</param>
		/// <param name="scriptBuilder">Script builder.</param>
		public abstract void CreateParameter(string parameterPrefix, Control parentControl, ScriptBuilder scriptBuilder);
        
        /// <summary>
        /// Zkontroluje, zda je parametr správnì inicializován.
        /// </summary>
		public virtual void CheckProperties()
		{
            // zkontrolujeme property Name
            CheckNameProperty();
		}

        /// <summary>
        /// Testuje nastavení hodnoty property Name.
        /// Pokud není hodnota nastavena, je vyhozena vıjimka.
        /// </summary>
        protected virtual void CheckNameProperty()
        {
            if (String.IsNullOrEmpty(Name))
                throw new ArgumentException("Property Name není nastavena.");
        }

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
                throw new ArgumentException(String.Format("Do parametru scriptletu je vkládán nepodporovanı control {0}.", control));

            scriptletParameters.Add((IScriptletParameter)control);
        }

    }
}
