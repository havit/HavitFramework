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
        /// <summary>
        /// Název Controlu, kterı je zdrojem pro vytvoøení klientského parametru.
        /// </summary>
		public string ControlName
		{
			get { return (string)ViewState["ControlName"]; }
			set { ViewState["ControlName"] = value; }		
		}

		/// <summary>
		/// Název parametru, pod kterım bude parametr pøístupnı v klienském skriptu.
		/// Pokud není hodnota nastavena, pouije se ControlName.
		/// </summary>
		public override string Name
        {
            get { return base.Name ?? ControlName; }
            set { base.Name = value; }
        }

        /// <summary>
        /// Udává, zda v pøípadì zmìny hodnoty prvku (zaškrtnutí, zmìna textu, apod.)
        /// dojde ke spuštìní skriptu.
        /// Vıchozí hodnota je false.
        /// </summary>
        public bool StartOnChange
        {
            get { return (bool)(ViewState["StartOnChange"] ?? false); }
            set { ViewState["StartOnChange"] = value; }
        }

		/// <summary>
		/// Zkontroluje, zda je parametr správnì inicializován.
		/// </summary>
		public override void CheckProperties()
		{
			base.CheckProperties();
            // navíc zkontrolujeme nastavení ControlName
            CheckControlNameProperty();
		}

		/// <summary>
		/// Testuje nastavení hodnoty property Name.
		/// Pøepisuje chování pøedka tím zpùsobem, e zde není property Name povinná
		/// (take se ani netestuje).
		/// </summary>
		protected override void CheckNameProperty()
        {
            // nebudeme zde jméno kontrolovat
        }

        /// <summary>
        /// Zkontroluje nastavení property ControlName.
        /// Pokud není hodnota nastavena, vyhodí vıjimku.
        /// </summary>
        protected virtual void CheckControlNameProperty()
        {
            if (String.IsNullOrEmpty(ControlName))
                throw new ArgumentException("Property ControlName nemá hodnotu.");
        }

        /// <summary>
        /// Vytvoøí klientskı skript pro parametr.
        /// </summary>
        /// <param name="parameterPrefix">Prefix pro název parametru. Controly mohou bıt sloené (napø. TextBox v Repeateru).</param>
        /// <param name="parentControl">Rodièovskı prvek, pro kterı je parametr renderován.</param>
        /// <param name="builder">Script builder.</param>
		public override void CreateParameter(string parameterPrefix, Control parentControl, ScriptBuilder builder)
		{
            // najdeme control
            Control control = GetControl(parentControl);

            // ak kdy je viditelnı
            if (control.Visible)
            {
                // najdeme extender, kterı tento control bude øešit
                IControlExtender extender = Scriptlet.ControlExtenderRepository.FindControlExtender(control);
                // a øekneme, a ho vyøeší
                extender.CreateParameter(parameterPrefix, this, control, builder);
            }
		}

        /// <summary>
        /// Nalezne Control, kterı má bıt zpracován.
        /// Pokud není Control nalezen, vyhodí vıjimku.
        /// </summary>
        /// <param name="parentControl">Control v rámci nìho se hledá (NamingContainer).</param>
        /// <returns>Control.</returns>
        protected virtual Control GetControl(Control parentControl)
        {            
            Control result = parentControl.FindControl(ControlName);
            
            if (result == null)
                throw new ArgumentException(String.Format("Control {0} nebyl nalezen.", ControlName));

            return result;
        }
    }
}
