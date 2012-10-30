using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.ComponentModel;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Třída pro generování wrapujícího html okolo svého obsahu (například vnořené DIVy za účelem kulatých rohů, atp.).
	/// </summary>
	[Themeable(true)]
	public sealed class Wrapper : System.Web.UI.Control, IAttributeAccessor
	{
		#region BeginHtml
		/// <summary>
		/// Html, které je renderováno před obsahem wrapperu.
		/// </summary>
		public string BeginHtml
		{
			get
			{
				return (string)(ViewState["BeginHtml"] ?? String.Empty);
			}
			set
			{
				ViewState["BeginHtml"] = value;
			}
		}
		#endregion

		#region EndHtml
		/// <summary>
		/// Html, které je renderováno za obsahem wrapperu.
		/// </summary>
		public string EndHtml
		{
			get
			{
				return (string)(ViewState["EndHtml"] ?? String.Empty);
			}
			set
			{
				ViewState["EndHtml"] = value;
			}
		}
		#endregion

		#region Render
		/// <summary>
		/// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"/> object, which writes the content to be rendered on the client.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> object that receives the server control content. </param>
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			writer.Write(DoAttributeReplacements(BeginHtml));
			base.Render(writer);
			writer.Write(DoAttributeReplacements(EndHtml));
		}
		#endregion

		#region DoAttributeReplacements
		/// <summary>
		/// Provede v textu náhrady dle hodnot v attributech dle IAttributeAccessoru.
		/// </summary>
		/// <param name="value">BeginHtml nebo EndHtml. Obsažené značky #ZNACKA# jsou nahrazeny hodnotami attributů, pokud hodnoty existuje. Značka musí být vždy upper-case.</param>
		private string DoAttributeReplacements(string value)
		{
			if (_attributesValues == null)
			{
				return value;
			}

			string result = value;

			foreach (string attributeKey in _attributesValues.Keys)
			{
				result = result.Replace("#" + attributeKey + "#", (string)_attributesValues[attributeKey]);
			}
			return result;
		}
		#endregion

		#region IAttributeAccessor: GetAttribute, SetAttribute
		private StateBag _attributesValues;

		string IAttributeAccessor.GetAttribute(string key)
		{
			if (_attributesValues == null)
			{
				return null;
			}
			return (string)_attributesValues[key.ToUpper()];
		}

		void IAttributeAccessor.SetAttribute(string key, string value)
		{
			if (_attributesValues == null)
			{
				_attributesValues = new StateBag();
			}
			_attributesValues[key.ToUpper()] = value;
		}
		#endregion
	}
}
