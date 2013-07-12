using Havit.Business;
using Havit.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.UI.WebControls
{
	/// <summary>
	/// EnterprisGridView poskytuje:
	/// - hledání klíče řádku, ve kterém došlo k události
	/// - hledání sloupce (IEnterpriseField) na základě ID sloupce
	/// - stránkování
	/// - zveřejňuje vlastnost RequiresDataBinding
	/// - automatický databinding při prvním načtení stránky nebo nastavení RequiresDataBinding na true (podmíněno vlastností AutoDataBind)
	/// - přechod na stránku 0 při změně řazení
	/// </summary>
	public class EnterpriseGridView : GridViewExt
	{
		#region Constructor
		/// <summary>
		/// Vytvoří instanci EnterpriseGridView. Nastavuje defaultní DataKeyNames na ID.
		/// </summary>
		public EnterpriseGridView()
		{
			this.DataKeyNames = new string[] { "ID" };
		}
		#endregion

		#region dataItemTypes (private field)
		/// <summary>
		/// Slouží pro uložení datový typu bindovaných objektu.
		/// Řešeno s optimalizací, abychom neukládali pro každý řádek stejnou hodnotu.
		/// Ukládáno do ViewState metodami LoadViewState/SaveViewState.
		/// </summary>
		private List<DataItemTypeEntry> dataItemTypes;
		#endregion

		#region AutoCrudOperations
		/// <summary>
		/// Nastavuje, zda Grid automaticky provádí CRUD operace na řádku. Hodnoty jsou z UI extrahovány pomocí two-way databindingu.
		/// Pokud je zapnuto, potom:
		/// <ol>
		///		<li>Získá business objekt, do kterého má nastavit hodnoty z UI. Pokud jde o business objekt, jehož ID je NoID, potom si o objekt řekne delegátem/událostí GetInsertRowDataItem.</li>
		///		<li>Vyzvedne hodnoty z UI pomocí two-way databindingu.</li>
		///		<li>Nastaví hodnoty business objektu.</li>
		///		<li>Uloží business objekt.</li>
		///	</ol>
		///	AutoCrudOperations je povoleno jen pro business objekty (potomky BusinessObjectBase), což jsou jediné objekty, které by měly být s EnterpriseGridView používány.
		/// </summary>
		public bool AutoCrudOperations
		{
			get { return (bool)(ViewState["AutoCrudOperations"] ?? false); }
			set { ViewState["AutoCrudOperations"] = value; }
		}
		#endregion

		#region GetRowID - Hledání klíče položky
		/// <summary>
		/// Nalezne hodnotu ID klíče položky, ve kterém se nachází control.
		/// </summary>
		/// <param name="control">Control. Hledá se řádek, ve kterém se GridViewRow nalézá a DataKey řádku.</param>
		/// <returns>Vrací hodnotu klíče.</returns>
		public int GetRowID(Control control)
		{
			return (int)GetRowKey(control).Value;
		}

		/// <summary>
		/// Nalezne hodnotu ID klíče položky na základě události.
		/// </summary>
		/// <param name="e">Událost, ke které v gridu došlo.</param>
		/// <returns>Vrací hodnotu klíče daného řádku.</returns>
		public int GetRowID(GridViewCommandEventArgs e)
		{
			return (int)GetRowKey(e).Value;
		}

		/// <summary>
		/// Nalezne hodnotu ID klíče položky na základě indexu řádku v gridu.
		/// </summary>
		/// <param name="rowIndex">index řádku</param>
		/// <returns>Vrací hodnotu klíče daného řádku.</returns>
		public int GetRowID(int rowIndex)
		{
			return (int)GetRowKey(rowIndex).Value;
		}
		#endregion

		#region GetRowBusinessObject
		/// <summary>
		/// Vrátí business object nabindovaný na daný řádek.
		/// Pokud jde o nový záznam, pak jej získá udáslostí GetInsertRowDataItem.
		/// </summary>
		private BusinessObjectBase GetRowBusinessObject(GridViewRow row)
		{
			Contract.Requires(row != null);

			if ((row.RowState & DataControlRowState.Insert) == DataControlRowState.Insert)
			{
				object insertRowDataItem = GetInsertRowDataItem();
				return (BusinessObjectBase)insertRowDataItem;
			}
			else
			{
				Type type = dataItemTypes.OrderByDescending(item => item.StartingRowIndex).First(item => item.StartingRowIndex <= row.RowIndex).Type;
				MethodInfo mi = type.GetMethod("GetObject", new Type[] { typeof(int) });
				BusinessObjectBase businessObject = (BusinessObjectBase)mi.Invoke(null, new object[] { this.GetRowID(row.RowIndex) });
				return businessObject;
			}
		}
		#endregion

		#region ExtractRowValues
		/// <summary>
		/// Extrahuje hodnoty z daného řádku do business objektu, který byl na řádek nabidnován.
		/// Vrátí tento business objekt.
		/// </summary>
		public T ExtractRowValues<T>(GridViewRow row)
			where T : BusinessObjectBase
		{
			Contract.Requires(row != null);

			T businessObject = (T)GetRowBusinessObject(row); // vyzvedneme business objekt
			ExtractRowValues(row, businessObject); // naplníme ho hodnotami
			return businessObject; // vrátíme jej
		}
		#endregion

		#region OnDataBinding
		/// <summary>
		/// Raises the System.Web.UI.Control.DataBinding event.
		/// </summary>
		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);
			dataItemTypes = AutoCrudOperations ? new List<DataItemTypeEntry>() : null; // vyčistíme hodnotu dataItemTypes
		}
		#endregion

		#region OnRowDataBound
		/// <summary>
		/// Raises the System.Web.UI.WebControls.GridView.RowDataBound event.
		/// </summary>		
		protected override void OnRowDataBound(GridViewRowEventArgs e)
		{
			base.OnRowDataBound(e);

			// Zkontrolujeme, zda jde o business objekty
			if ((e.Row.RowType == DataControlRowType.DataRow) && !(e.Row.DataItem is BusinessObjectBase))
			{
				throw new InvalidOperationException("EnterpriseGridView vyžaduje jako datové prvky potomky BusinessObjectBase.");
			}

			// zapamatujeme si typy bindovaných objektů
			// pro možnost heterogenních seznamů si pamatujeme hodnotu pro každý řádek
			// ale uložení optimalizujeme - pokud se hodnoty neliší, nepřidáváme více položek do dataItemTypes
			if (AutoCrudOperations && (e.Row.RowType == DataControlRowType.DataRow))
			{
				Type type = e.Row.DataItem.GetType();
				if (dataItemTypes.Count == 0 || dataItemTypes[dataItemTypes.Count - 1].Type != type)
				{
					dataItemTypes.Add(new DataItemTypeEntry { Type = type, StartingRowIndex = e.Row.RowIndex });
				}
			}
		}
		#endregion

		#region OnRowUpdating
		/// <summary>
		/// Výchozí chování RowUpdating - pokud není zvoleno e.Cancel, pak vypne editaci řádku.
		/// V režimu AutoCrudOperations zajišťuje extrakci hodnot a jejich uložení.
		/// </summary>
		protected override void OnRowUpdating(GridViewUpdateEventArgs e)
		{
			base.OnRowUpdating(e);

			if (!e.Cancel && AutoCrudOperations)
			{
				GridViewRow row = Rows[e.RowIndex];
				Contract.Assert(row != null);
				BusinessObjectBase updatingBusinessObject = ExtractRowValues<BusinessObjectBase>(row);
				updatingBusinessObject.Save();
			}
		}
		#endregion

		#region OnRowInserting
		/// <summary>
		/// Spouští událost RowInserting.
		/// V režimu AutoCrudOperations zajišťuje extrakci hodnot a jejich uložení.
		/// </summary>
		protected override void OnRowInserting(GridViewInsertEventArgs e)
		{
			base.OnRowInserting(e);

			if (!e.Cancel && AutoCrudOperations)
			{
				GridViewRow row = Rows[e.RowIndex];
				Contract.Assert(row != null);
				BusinessObjectBase insertingBusinessObject = ExtractRowValues<BusinessObjectBase>(row);
				insertingBusinessObject.Save();			
			}
		}
		#endregion

		#region OnRowDeleting
		/// <summary>
		/// Spouští událost RowDeleting.
		/// V režimu AutoCrudOperations zajišťuje smazání objektu.
		/// </summary>
		protected override void OnRowDeleting(GridViewDeleteEventArgs e)
		{
			base.OnRowDeleting(e);

			if (!e.Cancel && AutoCrudOperations)
			{
				GridViewRow row = Rows[e.RowIndex];
				Contract.Assert(row != null);
				BusinessObjectBase deletingBusinessObject = GetRowBusinessObject(row);
				deletingBusinessObject.Delete();
			}
		}
		#endregion

		#region SaveViewState, LoadViewState
		/// <summary>
		/// Zajišťuje uložení vlastních hodnot objektu (dataItemTypes) do ViewState.
		/// </summary>
		protected override object SaveViewState()
		{
			return new object[] { base.SaveViewState(), dataItemTypes };
		}

		/// <summary>
		/// Zajišťuje načtení vlastních hodnot objektu (dataItemTypes) z ViewState.
		/// </summary>
		protected override void LoadViewState(object savedState)
		{
			object[] savedStateArray = (object[])savedState;
			base.LoadViewState(savedStateArray[0]);
			dataItemTypes = (List<DataItemTypeEntry>)savedStateArray[1];
		}
		#endregion

		#region DataItemTypeEntry (nested class)
		/// <summary>
		/// Slouží pro uložení typu datového objektu k řádkům gridu.
		/// </summary>
		[Serializable]		
		internal class DataItemTypeEntry
		{
			public Type Type { get; set; }
			public int StartingRowIndex { get; set; }
		}
		#endregion
	}
}
