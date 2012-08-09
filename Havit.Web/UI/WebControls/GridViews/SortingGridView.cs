using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Collections;
using Havit.Collections;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// SortingGridView zajišuje øazení poloek.
	/// Ukládá nastavení øazení dle uivatele a pøípadnì zajišuje automatické øazení pomocí GenericPropertyCompareru.
	/// </summary>
	public abstract class SortingGridView: HighlightingGridView
	{
		#region Properties

		/// <summary>
		/// Nastavuje, zda má pøi databindingu dojít k seøazení poloek podle nastavení.
		/// </summary>
		public bool AutoSort
		{
			get { return (bool)(ViewState["AutoSort"] ?? false); }
			set { ViewState["AutoSort"] = value; }				
		}

		/// <summary>
		/// Vıchozí øazení, které se pouije, pokud je povoleno automatické øazení nastavením vlastnosti AutoSort 
		/// a uivatel dosu ádné øazení nezvolil.
		/// </summary>
		public string DefaultSortExpression
		{
			get { return (string)(ViewState["DefaultSortExpression"] ?? String.Empty); }
			set { ViewState["DefaultSortExpression"] = value; }
		}

		/// <summary>
		/// Zajišuje práci se senzamem poloek, podle kterıch se øadí.
		/// </summary>
		public new Sorting Sorting
		{
			get
			{
				return sorting;
			}
		}
		private Sorting sorting = new Sorting();
		#endregion

		#region	OnSorting
		/// <summary>
		/// Pøi poadavku na øazení si zapamatujeme, jak chtìl uivatel øadit a nastavíme RequiresDataBinding na true.
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnSorting(GridViewSortEventArgs e)
		{
			sorting.AddSortExpression(e.SortExpression);
			base.RequiresDataBinding = true;
		}
		#endregion

		#region OnSorted
		/// <summary>
		/// Po setøídìní podle sloupce zajistí u vícestránkovıch gridù návrat na první stránku
		/// </summary>
		/// <param name="e">argumenty události</param>
		protected override void OnSorted(EventArgs e)
		{
			base.OnSorted(e);
			PageIndex = 0;
		}
		#endregion

		#region Naèítání a ukládání ViewState
		/// <summary>
		/// Zajistí uloení ViewState. Je pøidáno uloení property Sorting.
		/// </summary>
		protected override object SaveViewState()
		{
			Pair viewStateData = new Pair();
			viewStateData.First = base.SaveViewState();
			viewStateData.Second = sorting;
			return viewStateData;
		}

		/// <summary>
		/// Zajistí naètení ViewState. Je pøidáno naètení property Sorting.
		/// </summary>
		protected override void LoadViewState(object savedState)
		{
			Pair viewStateData = (Pair)savedState;
			base.LoadViewState(viewStateData.First);
			if (viewStateData.Second != null)
				sorting = (Sorting)viewStateData.Second;
		}
		#endregion

		#region PerformDataBinding
		/// <summary>
		/// Provede databinding dat.
		/// Pokud data nejsou null a AutoSort je true, automaticky seøadí data pomocí GenericPropertyCompareru.
		/// Pokud uivatel dosud nenastavil ádné øazení, pouije se øazení dle DefaultSortExpression.
		/// </summary>
		/// <param name="data"></param>
		protected override void PerformDataBinding(System.Collections.IEnumerable data)
		{
			if ((data != null) && AutoSort)
			{
				if ((Sorting.SortItems.Count == 0) && !String.IsNullOrEmpty(DefaultSortExpression))
				{
					// sorting je nutné zmìnit na základì DefaultSortExpression,
					// kdybychom jej nezmìnili, tak první kliknutí shodné s DefaultSortExpression nic neudìlá
					sorting.AddSortExpression(DefaultSortExpression);
				}

				IEnumerable sortedData = SortHelper.PropertySort(data, Sorting.SortItems);
				base.PerformDataBinding(sortedData);
			}
			else
			{
				base.PerformDataBinding(data);
			}
		}
		#endregion
	}
}
