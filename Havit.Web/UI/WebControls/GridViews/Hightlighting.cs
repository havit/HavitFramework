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
				AutoPageChangeEnabled = true;
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
		/// Pøíznak je automaticky nastaven pøi nastavení hodnoty HighlightValue
		/// a je po databindingu automaticky vypnut.
		/// </summary>
		internal bool AutoPageChangeEnabled
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
		bool autoPageChangeEnabled = false;

	}
}
