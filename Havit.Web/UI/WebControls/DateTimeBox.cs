using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using Havit.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
    /// <summary>
    /// Editaèní box na datumy
    /// </summary>
    public class DateTimeBox : TextBox
    {
        private const string clientScriptBlockName = "Havit.Web.UI.WebControls.DateTimeBox_KeyPressScript";
        private bool registerJavaScript = true;

		//private static string editFormatPattern;
		///// <summary>
		///// Formát, v jakém se edituje datum.
		///// </summary>
		//public static string EditFormatPattern
		//{
		//    get { return editFormatPattern; }
		//    set { editFormatPattern = value; }
		//}


		/// <summary>
		/// Udává, jaký typ vstupu se oèekává.
		/// </summary>
		public DateTimePart DateTimePart
        {
            get { return (DateTimePart)(ViewState["DateTimePart"] ?? DateTimePart.Date); }
			set { ViewState["DateTimePart"] = value; }
        }

		/// <summary>
		/// Udává, zda se má použít DateTimePicker.
		/// Není-li nastaveno, vrací true, pokud je DateTimePart nastaveno na Date nebo DateTime.
		/// </summary>
		public bool DisplayDatePicker
		{
			get { return (bool)(ViewState["DisplayDatePicker"] ?? ((DateTimePart == DateTimePart.Date || DateTimePart == DateTimePart.DateTime) && Enabled && Visible)); }
			set { ViewState["DisplayDatePicker"] = value; }
		}

		//public TimeSpan ValueAsTimeSpan
		//{
		//    get
		//    {
		//        string text = Text.Trim();
		//        if (String.IsNullOrEmpty(text))
		//            return TimeSpan.Zero;

		//        return TimeSpan.Parse(text);
		//    }
		//    set { Text = Convert.ToString(value); }
		//}

		public bool ValueIsNull
		{
			get
			{
				return Value == null;
			}
		}

		public DateTime SelectedDateTime
		{
			get
			{
				return Value.Value;
			}
		}

        public DateTime? Value
        {
            get
            {
                string text = Text.Trim();
                if (String.IsNullOrEmpty(text))
                    return null;

                return DateTime.Parse(text);
            }
			set
			{
				if (value == null || value == DateTime.MinValue)
				{
					Text = string.Empty;
					return;
				}

				switch (DateTimePart)
				{
					case DateTimePart.Date:
						Text = value.Value.ToShortDateString();
						return;
					case DateTimePart.Time:
						Text = value.Value.ToShortTimeString();
						return;
					case DateTimePart.DateTime:
						Text = value.Value.ToString();
						return;
				}
				//Text = Convert.ToString(value);
			}
        }

		/*
        public override string Text
        {
            get { return base.Text; }
            set
            {                
                string s = value;

                if (!String.IsNullOrEmpty(s))
                {
                    if (EditFormatPattern != null && (DateTimePart == DateTimePart.Date || DateTimePart == DateTimePart.DateTime))
                    {
                        DateTime dt;
                        if (DateTime.TryParse(s, out dt))
                            s = dt.ToString(EditFormatPattern);
                    }

                    s = s.Trim();

                    System.Globalization.DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
                    switch (DateTimePart)
                    {
                        case DateTimePart.Date:
                            {
                                // pokud je to editace pouze datumu a nekdo nam tam napcal datum i cas, tak cas odrizmene
                                if (s.Contains(" ") && s.Contains(dateTimeFormatInfo.TimeSeparator))
                                    s = s.Remove(s.IndexOf(" "));
                                break;
                            }
                        case DateTimePart.TimeOffset: goto case DateTimePart.Time;
                        case DateTimePart.Time:
                            // pokud je to editace pouze casu a nekdo nam tam napcal cas i datum, tak datum odrizmene                            
                            if (s.Contains(" ") && s.Contains(dateTimeFormatInfo.DateSeparator))
                                s = s.Remove(0, s.IndexOf(" ") - 1);
                            if (!ShowSeconds)
                            {
                                int first = s.IndexOf(":");
                                int last = s.LastIndexOf(":");
                                if (first != last)
                                    s = s.Substring(0, last);
                            }
                            break;
                    }
                }
                base.Text = s;
            }
        }
        */

        private LiteralControl literal;
        private DatePicker datePicker;

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			// pøipojení DatePickeru

			literal = new LiteralControl("&nbsp;");

			datePicker = new DatePicker();
			datePicker.AssociatedControl = this;
			datePicker.ShowImage = true;

			Controls.Add(literal);
			Controls.Add(datePicker);
		}

        protected override void OnLoad(EventArgs e)
        {
            if (registerJavaScript)
            {
                if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(DateTimeBox), clientScriptBlockName))
                {
                    // ziskat ASCII kody oddelovacu datumu a casu
                    System.Globalization.DateTimeFormatInfo dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
                    string timeSeparatorCode = System.Text.Encoding.ASCII.GetBytes(dateTimeFormatInfo.TimeSeparator)[0].ToString();
                    string dateSeparatorCode = System.Text.Encoding.ASCII.GetBytes(dateTimeFormatInfo.DateSeparator)[0].ToString();

                    string javaScript = @"
<script language=""JavaScript"">
	function HavitDateTimeBox_KeyPress(allowDate, allowTime) {
		event.returnValue = (event.keyCode >= 48 && event.keyCode <= 57)
			|| (allowDate && event.keyCode == " + dateSeparatorCode + @")
			|| (allowDate && allowTime && event.keyCode == 32)			
			|| (allowTime && event.keyCode == " + timeSeparatorCode + @");
	}
</script>
					";                    
                    Page.ClientScript.RegisterClientScriptBlock(typeof(DateTimeBox), clientScriptBlockName, javaScript);
                }

				Attributes.Add("onKeyPress", "HavitDateTimeBox_KeyPress("
					+ (DateTimePart == DateTimePart.Date || DateTimePart == DateTimePart.DateTime).ToString().ToLower()
					+ ", "
					+ (DateTimePart == DateTimePart.Time || DateTimePart == DateTimePart.DateTime).ToString().ToLower()
					+ ")");
            }

            // neni-li nastavena MaxLength, omezime si ji podle zvoleneho rezimu
            if (MaxLength == 0)
            {
                switch (DateTimePart)
                {
                    case DateTimePart.Date:
                        MaxLength = 10;
                        break;
                    case DateTimePart.Time:
                        MaxLength = 8;
                        break;
                    case DateTimePart.DateTime:
                        MaxLength = 19;
                        break;
                }
            }

            base.OnLoad(e);
        }

		protected override void  OnPreRender(EventArgs e)
		{
 			base.OnPreRender(e);

			datePicker.Visible = DisplayDatePicker;
			if (DisplayDatePicker)
			{
				datePicker.Enabled = this.Enabled;
			}

		}

        protected override void Render(HtmlTextWriter writer)
        {
            Style.Add("text-align", "right");
            base.Render(writer);
            RenderChildren(writer);
        }

    }
}
