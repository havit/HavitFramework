using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using System.Text.RegularExpressions;

[assembly: WebResource("Havit.Web.UI.WebControls.DateTimeValidator.js", "text/javascript")]

namespace Havit.Web.UI.WebControls {
    
	/// <summary>
	/// Kontroluje, jestli je zadany datum/cas v platnem formatu.
	/// </summary>
	public class DateTimeValidator : System.Web.UI.WebControls.BaseValidator {

		/// <summary>
		/// Urcuje, jaky format data/casu se ocekava.
		/// </summary>
		public DateTimePart DateTimePart 
		{
            get { return ViewState["DateTimePart"] == null ? DateTimePart.Date : (DateTimePart)ViewState["DateTimePart"];  }
			set { ViewState["DateTimePart"] = value; }
		}
		
		/// <summary>
		/// Serverova validace - overuje, zda je validovana hodnota v platnem formatu. 
		/// </summary>
		protected override bool EvaluateIsValid() {
			String value = this.GetControlValidationValue(this.ControlToValidate);
			if (value == null || value.Trim().Length == 0) {
				return true;
			}

			try
			{
                string pattern = ServerSidePattern(DateTimePart);

				DateTime dt;

				if (!DateTime.TryParseExact(value, pattern, CultureInfo.CurrentCulture.DateTimeFormat, DateTimeStyles.None, out dt))
				{
					return false;
				}

				if (dt < new DateTime(1900, 1, 1) || (dt >= new DateTime(2100, 1, 1)))
				{
					return false;
				}

				return true;
			} 
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Vrací masku - proti které se ovìøuje platnost data/èasu na stranì serveru.
		/// </summary>
		/// <param name="dateTimePart"></param>
		/// <returns></returns>
		protected string ServerSidePattern(DateTimePart dateTimePart) 
		{
			switch (DateTimePart) 
			{
				case DateTimePart.Date: return "d.M.yyyy";
				case DateTimePart.Time: return "H:mm";
                case DateTimePart.DateTime: return "d.M.yyyy H:mm";
				default: throw new Exception("V metodì ServerSidePattern chybí položka výètu DateTimePart.");
			}
		}

		/// Vrací masku - proti které se ovìøuje platnost data/èasu na stranì klienta.
		protected string ClientSidePattern(DateTimePart dateTimePart) 
		{
			switch (DateTimePart) 
			{
				case DateTimePart.Date: return "%d.%m.%yyyy";
				case DateTimePart.Time: return "%H:%mins";
                case DateTimePart.DateTime: return "%d.%m.%yyyy %H:%mins";
				default: throw new Exception("V metodì ClientSidePattern chybí položka výètu DateTimePart.");
			}
		}

        //protected override void AddAttributesToRender(System.Web.UI.HtmlTextWriter writer) 
        //{
        //    base.AddAttributesToRender(writer);
        //}

		protected override void OnPreRender(System.EventArgs e) 
		{
			base.OnPreRender(e);

            if (this.RenderUplevel && this.EnableClientScript && this.Page.Request.Browser.VBScript)
            {
                this.Attributes.Add("format", ClientSidePattern(DateTimePart));
                this.Attributes.Add("evaluationfunction", "DateTimeValidatorEvaluateIsValid");
            }
            this.RegisterClientScript();
		}

		protected virtual void RegisterClientScript() 
		{
			if (this.RenderUplevel && this.EnableClientScript)
				Page.ClientScript.RegisterClientScriptResource(typeof(DateTimeValidator), "Havit.Web.UI.WebControls.DateTimeValidator.js");			
		}
	}
}