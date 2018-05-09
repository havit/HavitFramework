using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// Třída Highlighting drží data pro zvýraznění vybrané položky podle hodnoty klíče.
	/// </summary>
	[Serializable]
	public class Highlighting
	{
		#region HighlightValue
		/// <summary>
		/// Hodnota "klíče" položky, která má být zvýrazněna.
		/// Nastavuje příznak AutoPageChangeEnabled.
		/// </summary>
		public object HighlightValue
		{
			get
			{
				return highlightValue;
			}
			set
			{
				highlightValue = value;
				PageChangeEnabled = true;
			}
		}
		private object highlightValue;
		#endregion

		#region DataField
		/// <summary>
		/// Položka dat, jejíž hodnota se porovnává s HighlightValue.
		/// </summary>
		public string DataField
		{
			get
			{
				return dataField;
			}
			set
			{
				dataField = value;
			}
		}
		private string dataField;
		#endregion

		#region AutoPageChangeEnabled
		/// <summary>
		/// Příznak, zda může dojít ke změně stránky pro zvýraznění položky.
		/// </summary>
		public bool AutoPageChangeEnabled
		{
			get
			{
				return autoPageChangeEnabled;
			}
			set
			{
				autoPageChangeEnabled = value;
			}
		}
		private bool autoPageChangeEnabled = true;
		#endregion

		#region PageChangeEnabled
		/// <summary>
		/// Příznak, zda může dojít je možná změna stránky.
		/// Příznak je automaticky nastaven při nastavení hodnoty HighlightValue
		/// a je po databindingu automaticky vypnut. Tím je zajištěno přepnutí stránky pouze při prvním zobrazení stránky
		/// po nastavení HiglightValue. Dále se stránka nepřepíná a uživatel může v klidu stránkovat.
		/// </summary>
		internal bool PageChangeEnabled
		{
			get
			{
				return pageChangeEnabled;
			}
			set
			{
				pageChangeEnabled = value;
			}
		}
		private bool pageChangeEnabled = false;
		#endregion

	}
}
