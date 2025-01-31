using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Business;
using Havit.Diagnostics.Contracts;

namespace Havit.Web.UI.WebControls;

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
	/// <summary>
	/// Vytvoří instanci EnterpriseGridView. Nastavuje defaultní DataKeyNames na ID.
	/// </summary>
	public EnterpriseGridView()
	{
		DataKeyNames = new string[] { "ID" };
	}

	/// <summary>
	/// Slouží pro uložení datový typu bindovaných objektu.
	/// Řešeno s optimalizací, abychom neukládali pro každý řádek stejnou hodnotu.
	/// Ukládáno do ViewState metodami LoadViewState/SaveViewState.
	/// </summary>
	private List<DataItemTypeEntry> dataItemTypes;

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

	/// <summary>
	/// Zpráva zobrazená v messengeru při uložení objektu. Zobrazuje se při aktualizaci objektu pomocí AutoCrud operace.
	/// </summary>
	public string MessengerUpdatedMessage
	{
		get
		{
			return (string)(ViewState["MessengerUpdatedMessage"] ?? String.Empty);
		}
		set
		{
			ViewState["MessengerUpdatedMessage"] = value;
		}
	}

	/// <summary>
	/// Zpráva zobrazená v messengeru při založení objektu. Zobrazuje se při založení objektu pomocí AutoCrud operace.
	/// </summary>
	public string MessengerInsertedMessage
	{
		get
		{
			return (string)(ViewState["MessengerInsertedMessage"] ?? String.Empty);
		}
		set
		{
			ViewState["MessengerInsertedMessage"] = value;
		}
	}

	/// <summary>
	/// Zpráva zobrazená v messengeru při smazání objektu. Zobrazuje se při smazání objektu pomocí AutoCrud operace.
	/// </summary>
	public string MessengerDeletedMessage
	{
		get
		{
			return (string)(ViewState["MessengerDeletedMessage"] ?? String.Empty);
		}
		set
		{
			ViewState["MessengerDeletedMessage"] = value;
		}
	}

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

	/// <summary>
	/// Vrátí business object nabindovaný na daný řádek.
	/// Pokud jde o nový záznam, pak jej získá udáslostí GetInsertRowDataItem.
	/// </summary>
	private BusinessObjectBase GetRowBusinessObject(GridViewRow row)
	{
		Contract.Requires(row != null);

		if (dataItemTypes == null)
		{
			throw new InvalidOperationException("Na EntepriseGridView ještě neproběhl DataBind nebo ještě nebyl načten ViewState.");
		}

		if ((row.RowState & DataControlRowState.Insert) == DataControlRowState.Insert)
		{
			return GetInsertRowBusinessObject();
		}
		else
		{
			Type type = dataItemTypes.OrderByDescending(item => item.StartingRowIndex).First(item => item.StartingRowIndex <= row.RowIndex).Type;

			if (!type.IsSubclassOf(typeof(BusinessObjectBase)))
			{
				throw new ApplicationException(String.Format("GridViewRow není nabindován business objektem (potomkem BusinessObjectBase), ale typem '{0}'.", type.FullName));
			}

			MethodInfo mi = type.GetMethod("GetObject", new Type[] { typeof(int) });
			BusinessObjectBase businessObject = (BusinessObjectBase)mi.Invoke(null, new object[] { GetRowID(row.RowIndex) });
			return businessObject;
		}
	}

	/// <summary>
	/// Vrací instanci pro založení/uložení nového záznamu jako business object.
	/// </summary>
	private BusinessObjectBase GetInsertRowBusinessObject()
	{
		object insertRowDataItem = GetInsertRowDataItem();

		if (insertRowDataItem == null)
		{
			throw new ApplicationException("Událost GetInsertRowDataItem vrátila null.");
		}

		if (!(insertRowDataItem is BusinessObjectBase))
		{
			throw new ApplicationException(
				String.Format(
					"GetInsertRowDataItem nevrátila business objekt (potomek BusinessObjectBase), ale typ '{0}'.",
					insertRowDataItem.GetType().FullName));
		}

		return (BusinessObjectBase)insertRowDataItem;
	}

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

	/// <summary>
	/// Extrahuje hodnoty z řádku dle rowIndexu do business objektu, který byl na řádek nabidnován.
	/// Vrátí tento business objekt.
	/// </summary>
	public T ExtractRowValues<T>(int rowIndex)
		where T : BusinessObjectBase
	{
		if ((rowIndex < 0) || (rowIndex >= Rows.Count))
		{
			throw new ArgumentOutOfRangeException("rowIndex");
		}

		GridViewRow row = Rows[rowIndex];
		return ExtractRowValues<T>(row);
	}

	/// <summary>
	/// Zajišťuje data-binding dat na GridView.
	/// </summary>
	protected override void PerformDataBinding(System.Collections.IEnumerable data)
	{
		dataItemTypes = new List<DataItemTypeEntry>(); // vyčistíme hodnotu dataItemTypes
		base.PerformDataBinding(data);
	}

	/// <summary>
	/// Raises the System.Web.UI.WebControls.GridView.RowDataBound event.
	/// </summary>		
	protected override void OnRowDataBound(GridViewRowEventArgs e)
	{
		base.OnRowDataBound(e);

		// zapamatujeme si typy bindovaných objektů
		// pro možnost heterogenních seznamů si pamatujeme hodnotu pro každý řádek
		// ale uložení optimalizujeme - pokud se hodnoty neliší, nepřidáváme více položek do dataItemTypes
		if (e.Row.RowType == DataControlRowType.DataRow)
		{
			Type type = e.Row.DataItem.GetType();
			if (dataItemTypes.Count == 0 || dataItemTypes[dataItemTypes.Count - 1].Type != type)
			{
				dataItemTypes.Add(new DataItemTypeEntry { Type = type, StartingRowIndex = e.Row.RowIndex });
			}
		}
	}

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

			ShowMessengerMessage(MessengerUpdatedMessage);
		}
	}

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

			ShowMessengerMessage(MessengerInsertedMessage);
		}
	}

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

			ShowMessengerMessage(MessengerDeletedMessage);
		}
	}

	private void ShowMessengerMessage(string messengerMessage)
	{
		string message = HttpUtilityExt.GetResourceString(messengerMessage);
		if (!String.IsNullOrEmpty(message))
		{
			Messenger.Default.AddMessage(message);
		}
	}

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

	/// <summary>
	/// Slouží pro uložení typu datového objektu k řádkům gridu.
	/// </summary>
	[Serializable]
	internal class DataItemTypeEntry
	{
		public Type Type { get; set; }
		public int StartingRowIndex { get; set; }
	}

	/// <summary>
	/// Registruje editor pro použití s GridView.
	/// </summary>
	public override void RegisterEditor(IEditorExtender editorExtender)
	{
		base.RegisterEditor(editorExtender);

		editorExtender.GetEditedObject += EditorExtenderGetEditedObject;
		if (AutoCrudOperations)
		{
			editorExtender.ItemSaving += EditorExtenderItemSavingAutoCrudOperations;
		}
	}

	/// <summary>
	/// Vrací (nastavuje) externí editovaný objekt pro editor.
	/// </summary>
	private void EditorExtenderGetEditedObject(object sender, DataEventArgs<object> e)
	{
		e.Data = GetEditorExtenderEditedObject();
	}

	/// <summary>
	/// Vrací editovaný business objekt pro externí editor.
	/// </summary>
	public BusinessObjectBase GetEditorExtenderEditedObject()
	{
		return (this.EditorExtenderMode == WebControls.EditorExtenderMode.Insert) ? GetInsertRowBusinessObject() : GetRowBusinessObject(Rows[EditorExtenderEditIndex]);
	}

	/// <summary>
	/// Obsluha Save operace pokud je povoleno AutoCrudOperations.
	/// </summary>
	private void EditorExtenderItemSavingAutoCrudOperations(object sender, EditorExtenderItemSavingEventArgs args)
	{
		if (!args.Cancel)
		{
			BusinessObjectBase dataObject = (BusinessObjectBase)args.EditedObject;
			EditorExtender.ExtractValues(dataObject);
			dataObject.Save();

			ShowMessengerMessage((this.EditorExtenderMode == WebControls.EditorExtenderMode.Insert) ? MessengerInsertedMessage : MessengerUpdatedMessage);
		}
	}
}
