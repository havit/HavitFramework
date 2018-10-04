#pragma warning disable 1591
using System;
using System.Text;
using System.Web;

namespace Havit.Web.UI.WebControls
{
	/// <summary>Data structure for menu items in suggestion div</summary>
	public class AutoSuggestMenuItem
	{
		#region Private fields
		private string _label;
		private string _value;
		private bool _isSelectable;
		private string _cssClass;
		#endregion

		#region Class Properties

		public string Label
		{
			get { return _label; }
			set { _label = value; }
		}

		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public bool IsSelectable
		{
			get { return _isSelectable; }
			set { _isSelectable = value; }
		}

		public string CSSClass
		{
			get { return _cssClass; }
			set { _cssClass = value; }
		}
		#endregion

		#region Constructors
		public AutoSuggestMenuItem()
		{
			_cssClass = null; //By default overridable by AutoSuggestMenu.MenuItemCssClass
			_isSelectable = true;
		}

		public AutoSuggestMenuItem(string label, string value)
			: this()
		{
			_label = label;
			_value = value;
		}
		#endregion

		#region GetJSON
		public string GetJSON()
		{
			XJsonWriter writer = new XJsonWriter();

			writer.WriteNameValue("label", _label);
			writer.WriteNameValue("value", _value);
			writer.WriteNameValue("isSelectable", _isSelectable);
			writer.WriteNameValue("cssClass", _cssClass, true);

			return writer.ToString();
		}
		#endregion
	}

}
#pragma warning restore 1591
