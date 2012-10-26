﻿using System;
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
	/// GridView, který automaticky zvýrazňuje položku na základě hodnoty určité 
	/// property dat. Zvýraznění je provedeno nastavením hodnoty SelectedIndex.
	/// </summary>
	public abstract class HighlightingGridView : GridView
	{
		#region Properties
		/// <summary>
		/// Vlastnosti pro zvýraznění řádku.
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
		/// Zajistí uložení ViewState. Je přidáno uložení property Hightlighting.
		/// </summary>
		protected override object SaveViewState()
		{
			// Rozšíří ViewState o objekt Highlighting.
			Pair viewStateData = new Pair();
			viewStateData.First = base.SaveViewState();
			viewStateData.Second = hightlighting;
			return viewStateData;
		}

		/// <summary>
		/// Zajistí načtení ViewState. Je přidáno načtení property Hightlighting.
		/// </summary>
		protected override void LoadViewState(object savedState)
		{
			// Načte rozšířený (viz SaveViewState) ViewState.
			Pair viewStateData = (Pair)savedState;
			base.LoadViewState(viewStateData.First);
			if (viewStateData.Second != null)
				hightlighting = (Highlighting)viewStateData.Second;
		}
		#endregion

		#region Zvýraznění řádku
		/// <summary>
		/// Zajistí zvýraznění řádku.
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
		/// Vybere položku s daným indexem. Je-li hodnota příznaku AutoPageChangeEnabled
		/// true, provede přestránkování, pokud je potřeba a nastaví hodnotu příznaku
		/// na false, aby nedocházelo k nežádoucí změně stránky při změně stránky z 
		/// uživatelskéro rozhraní a následného databindingu.
		/// Hodnota indexu rovna -1 zruší zvýraznění položky.
		/// </summary>
		/// <param name="index">Index položky. Počítáno od nuly.</param>
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