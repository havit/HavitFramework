using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// GridView, který automaticky zvýrazòuje položku na základì hodnoty urèité 
	/// property dat. Zvýraznìní je provedeno nastavením hodnoty SelectedIndex.
	/// </summary>
	public abstract class HighlightingGridView : GridView
	{
		#region Properties
		/// <summary>
		/// Vlastnosti pro zvýraznìní øádku.
		/// </summary>
		public Highlighting Hightlighting
		{
			get
			{
				if (hightlighting == null)
					hightlighting = new Highlighting();
				return hightlighting;
			}
		}
		private Highlighting hightlighting;
		#endregion

		#region SaveViewState, LoadViewState
		/// <summary>
		/// Zajistí uložení ViewState. Je pøidáno uložení property Hightlighting.
		/// </summary>
		protected override object SaveViewState()
		{
			// Rozšíøí ViewState o objekt Highlighting.
			Pair viewStateData = new Pair();
			viewStateData.First = base.SaveViewState();
			viewStateData.Second = hightlighting;
			return viewStateData;
		}

		/// <summary>
		/// Zajistí naètení ViewState. Je pøidáno naètení property Hightlighting.
		/// </summary>
		protected override void LoadViewState(object savedState)
		{
			// Naète rozšíøený (viz SaveViewState) ViewState.
			Pair viewStateData = (Pair)savedState;
			base.LoadViewState(viewStateData.First);
			if (viewStateData.Second != null)
				hightlighting = (Highlighting)viewStateData.Second;
		}
		#endregion

		#region Zvýraznìní øádku
		/// <summary>
		/// Zajistí zvýraznìní øádku.
		/// </summary>
		protected override void PerformDataBinding(IEnumerable data)
		{
			// Nastaví SelectionIndex.
			HighlightRow(data);
			base.PerformDataBinding(data);
		}

		/// <summary>
		/// Prohledá data, pokud najde hodnotu rovnou HighlightValue, 
		/// vybere danou položku.
		/// </summary>
		/// <param name="data"></param>
		protected virtual void HighlightRow(IEnumerable data)
		{
			if (Hightlighting.HighlightValue != null && data != null)
			{
				int index = 0;

				foreach (object o in data)
				{
					object value = DataBinder.GetPropertyValue(o, Hightlighting.DataField);
					if (Hightlighting.HighlightValue.Equals(value))
					{
						HighlightIndex(index);
						return;
					}
					index++;
				}
				HighlightIndex(-1);
			}
		}

		/// <summary>
		/// Vybere položku s daným indexem. Je-li hodnota pøíznaku AutoPageChangeEnabled
		/// true, provede pøestránkování, pokud je potøeba a nastaví hodnotu pøíznaku
		/// na false, aby nedocházelo k nežádoucí zmìnì stránky pøi zmìnì stránky z 
		/// uživatelskéro rozhraní a následného databindingu.
		/// Hodnota indexu rovna -1 zruší zvýraznìní položky.
		/// </summary>
		/// <param name="index">Index položky. Poèítáno od nuly.</param>
		protected virtual void HighlightIndex(int index)
		{
			if (index >= 0)
			{
				int TargetPageIndex = (!AllowPaging) ? 0 : index / PageSize;
				int TargetSelectedIndex = index - TargetPageIndex * PageSize;

				if (Hightlighting.AutoPageChangeEnabled && Hightlighting.PageChangeEnabled)
				{
					PageIndex = TargetPageIndex;
					Hightlighting.PageChangeEnabled = false;
				}
				if (PageIndex == TargetPageIndex)
				{
					SelectedIndex = TargetSelectedIndex;
					return;
				}
			}
			SelectedIndex = -1;
		}
		#endregion
	}
}