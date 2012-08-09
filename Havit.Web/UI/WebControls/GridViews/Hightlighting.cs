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
	/// Tøída Highlighting drží data pro zvýraznìní vybrané položky podle hodnoty klíèe.
	/// </summary>
	[Serializable]
	public class Highlighting
	{
		/// <summary>
		/// Hodnota "klíèe" položky, která má být zvýraznìna.
		/// Nastavuje pøíznak AutoPageChangeEnabled.
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

		/// <summary>
		/// Pøíznak, zda mùže dojít ke zmìnì stránky pro zvýraznìní položky.
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
		bool autoPageChangeEnabled = true;

		/// <summary>
		/// Pøíznak, zda mùže dojít je možná zmìna stránky.
		/// Pøíznak je automaticky nastaven pøi nastavení hodnoty HighlightValue
		/// a je po databindingu automaticky vypnut. Tím je zajištìno pøepnutí stránky pouze pøi prvním zobrazení stránky
		/// po nastavení HiglightValue. Dále se stránka nepøepíná a uživatel mùže v klidu stránkovat.
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
		bool pageChangeEnabled = false;

	}
}
