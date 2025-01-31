﻿using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Havit.Web.UI;
using Havit.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls;

/// <summary>
/// Editor extender as bootstrap modal dialog.
/// Dialog header and content are nested into form view to enable (one-way) databinding for dialog header and two-way databinding for dialog content.
/// </summary>
/// <remarks>
/// Control would like to inherit from Control class. But when we want do have intellisence in Visual Studio, we have to inherit from DataBoundControl. There is no other use of DataBoundControl (and it ancesors).
/// Unused members are hidden for intellisence by EditorBrowsableAttributes and DesignerSerializationVisibilityAttributes.
/// </remarks>
[ParseChildren(true)]
[PersistChildren(false)]
[Themeable(true)]
public class ModalEditorExtender : DataBoundControlWithHiddenPublicMembersFromIntellisence, IEditorExtenderWithPreviousNextNavigation, INamingContainer
{
	private readonly ModalDialog modalDialog;
	private readonly FormViewExt headerFormView;
	private readonly FormViewExt contentFormView;

	/// <summary>
	/// Editor dialog header text.
	/// </summary>
	[DefaultValue("")]
	public string HeaderText
	{
		get
		{
			return modalDialog.HeaderText;
		}
		set
		{
			modalDialog.HeaderText = value;
		}
	}

	/// <summary>
	/// Editor dialog header template.
	/// </summary>
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[TemplateContainer(typeof(FormView), BindingDirection.OneWay)]
	public ITemplate HeaderTemplate
	{
		get
		{
			return headerFormView.ItemTemplate;
		}
		set
		{
			headerFormView.ItemTemplate = value;
		}
	}

	/// <summary>
	/// Editor dialog content template.
	/// </summary>
	[PersistenceMode(PersistenceMode.InnerProperty)]
	[TemplateContainer(typeof(FormViewExt), BindingDirection.TwoWay)]
	public ITemplate ContentTemplate
	{
		get
		{
			return contentFormView.EditItemTemplate;
		}
		set
		{
			contentFormView.EditItemTemplate = value;
		}
	}

	/// <summary>
	/// Editor dialog footer template.
	/// </summary>
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public ITemplate FooterTemplate
	{
		get
		{
			return modalDialog.FooterTemplate;
		}
		set
		{
			modalDialog.FooterTemplate = value;
		}
	}

	/// <summary>
	/// Control to which is editor attached.
	/// </summary>
	[DefaultValue("")]
	public string TargetControlID
	{
		get
		{
			return (string)(ViewState["TargetControlID"] ?? default(string));
		}
		set
		{
			if (initPerformed)
			{
				throw new InvalidOperationException("Cannot set TargetControlID after init was performed.");
			}
			ViewState["TargetControlID"] = value;
		}
	}

	/// <summary>
	/// TargetControlID control as IEditorExtensible.
	/// </summary>
	private IEditorExtensible TargetControl
	{
		get
		{
			if (String.IsNullOrEmpty(TargetControlID))
			{
				throw new InvalidOperationException("TargetControlID property not set.");
			}

			Control forControl = this.NamingContainer.FindControl(TargetControlID);
			if (forControl == null)
			{
				throw new InvalidOperationException(String.Format("Control with ID '{0}' set in TargetControlID property was not found.", TargetControlID));
			}

			if (!(forControl is IEditorExtensible))
			{
				throw new InvalidOperationException(String.Format("Control with ID '{0}' set in TargetControlID property does not implement IEditorExtensible.", TargetControlID));
			}

			return (IEditorExtensible)forControl;
		}
	}

	/// <summary>
	/// Nested UpdatePanel's triggers.
	/// </summary>
	[PersistenceMode(PersistenceMode.InnerProperty)]
	public UpdatePanelTriggerCollection Triggers
	{
		get
		{
			return modalDialog.Triggers;
		}
	}

	/// <summary>
	/// Šířka dialogu v pixelech.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Always)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public override Unit Width
	{
		get
		{
			return modalDialog.Width;
		}
		set
		{
			modalDialog.Width = value;
		}
	}

	/// <summary>
	/// Get or sets whether use animations when showing and hiding dialog.
	/// </summary>
	public bool UseAnimations
	{
		get
		{
			return modalDialog.UseAnimations;
		}
		set
		{
			modalDialog.UseAnimations = value;
		}
	}

	/// <summary>
	/// Css class to be used for dialog - used in element with modal-dialog class.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Always)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public override string CssClass
	{
		get
		{
			return modalDialog.CssClass;
		}
		set
		{
			modalDialog.CssClass = value;
		}
	}

	/// <summary>
	/// Css class to be used for header - used in element with modal-header class.
	/// </summary>
	public string HeaderCssClass
	{
		get
		{
			return modalDialog.HeaderCssClass;
		}
		set
		{
			modalDialog.HeaderCssClass = value;
		}
	}

	/// <summary>
	/// Css class to be used for content - used in element with modal-body class.
	/// </summary>
	public string ContentCssClass
	{
		get
		{
			return modalDialog.ContentCssClass;
		}
		set
		{
			modalDialog.ContentCssClass = value;
		}
	}

	/// <summary>
	/// Css class to be used for footer - used in element with modal-footer class.
	/// </summary>
	public string FooterCssClass
	{
		get
		{
			return modalDialog.FooterCssClass;
		}
		set
		{
			modalDialog.FooterCssClass = value;
		}
	}

	/// <summary>
	/// Indicates whether to show close button.
	/// Default value is true.
	/// </summary>
	public bool ShowCloseButton
	{
		get
		{
			return modalDialog.ShowCloseButton;
		}
		set
		{
			modalDialog.ShowCloseButton = value;
		}
	}

	/// <summary>
	/// Indicates whether to close dialog on escape key pressed.
	/// </summary>
	public bool CloseOnEscapeKey
	{
		get
		{
			return modalDialog.CloseOnEscapeKey;
		}
		set
		{
			modalDialog.CloseOnEscapeKey = value;
		}
	}

	/// <summary>
	/// Modal dialog drag mode.
	/// Default value is ModalDialogDragMode.IfAvailable.
	/// </summary>
	public ModalDialogDragMode DragMode
	{
		get
		{
			return modalDialog.DragMode;
		}
		set
		{
			modalDialog.DragMode = value;
		}
	}

	/// <summary>
	/// Validation Group to be automaticaly set to buttons with CommandName Previous, Next, New, OK or Save.
	/// If property value not set (neither empty string), validation group is not set to buttons.
	/// Default value is null.
	/// </summary>
	public string ValidationGroup
	{
		get
		{
			return (string)(ViewState["ValidationGroup"]);
		}
		set
		{
			ViewState["ValidationGroup"] = value;
		}
	}

	/// <summary>
	/// Constructor.
	/// </summary>
	public ModalEditorExtender()
	{
		headerFormView = new FormViewExt();
		headerFormView.DefaultMode = FormViewMode.ReadOnly;
		headerFormView.RenderOuterTable = false;

		contentFormView = new FormViewExt();
		contentFormView.DefaultMode = FormViewMode.Edit;
		contentFormView.RenderOuterTable = false;
		contentFormView.ItemCreated += ContentFormViewItemCreated;

		modalDialog = new ModalDialog();
		modalDialog.DialogHidden += ModalDialogDialogHidden;
	}

	/// <summary>
	/// Ensures child controls and registers editor to a TargetControlID control.
	/// </summary>
	protected override void OnInit(EventArgs e)
	{
		initPerformed = true;
		base.OnInit(e);
		EnsureChildControls();

		TargetControl.RegisterEditor(this);
	}
	private bool initPerformed = false;

	/// <summary>
	/// Creates child controls.
	/// </summary>
	protected override void CreateChildControls()
	{
		base.CreateChildControls();

		if (headerFormView.ItemTemplate != null)
		{
			modalDialog.HeaderTemplateContainer.Controls.Add(headerFormView);
		}

		modalDialog.ContentTemplateContainer.Controls.Add(contentFormView);

		this.Controls.Add(modalDialog);
	}

	/// <summary>
	/// Handles events Save, OK, Cancel from children.
	/// </summary>
	protected override bool OnBubbleEvent(object source, EventArgs args)
	{
		if (args is CommandEventArgs)
		{
			bool causesValidation = false;
			IButtonControl control = source as IButtonControl;
			if (control != null)
			{
				causesValidation = control.CausesValidation;
			}

			CommandEventArgs commandEventArgs = (CommandEventArgs)args;

			switch (commandEventArgs.CommandName)
			{
				case CommandNames.Save:
					HandleSaveCommand(causesValidation);
					return true;

				case CommandNames.OK:
					HandleOKCommand(causesValidation);
					return true;

				case CommandNames.Cancel:
					HandleCancelCommand(causesValidation);
					return true;

				case CommandNames.Previous:
					HandlePreviousCommand(causesValidation);
					return true;

				case CommandNames.Next:
					HandleNextCommand(causesValidation);
					return true;

				case CommandNames.New:
					HandleNewCommand(causesValidation);
					return true;

				default:
					break;
			}
		}

		return base.OnBubbleEvent(source, args);
	}

	/// <summary>
	/// Shows editor and databinds it (header and content section).
	/// </summary>
	public void StartEditor()
	{
		DataEventArgs<object> dataEventArgs = new DataEventArgs<object>();
		OnGetEditedObject(dataEventArgs);

		DataBind();
		modalDialog.Show();
	}

	/// <summary>
	/// In DataSource not set, gets the edited object via OnGetEditedObject.
	/// </summary>
	protected override void OnDataBinding(EventArgs e)
	{
		base.OnDataBinding(e);
		if (this.DataSource == null)
		{
			DataEventArgs<object> dataEventArgs = new DataEventArgs<object>();
			OnGetEditedObject(dataEventArgs);
			this.DataSource = new object[] { dataEventArgs.Data };
		}
	}

	/// <summary>
	/// Binds data to dialog header and content.
	/// </summary>
	protected override void PerformDataBinding(IEnumerable data)
	{
		object[] dataArray = (data == null) ? new object[0] : data.Cast<object>().ToArray();

		base.PerformDataBinding(dataArray);

		headerFormView.DataSource = dataArray;
		headerFormView.DataBind();

		contentFormView.DataSource = dataArray;
		contentFormView.DataBind();

		SetValidationGroup();
		SetNewButtons();
		SetPreviousNextButtons();

		object dataItem = dataArray.FirstOrDefault();
		EditorExtenderItemDataBoundEventArgs args = new EditorExtenderItemDataBoundEventArgs(dataItem);
		OnItemDataBound(args);
	}

	/// <summary>
	/// Nastaví validační skupinu tlačítkům OK a Save.
	/// </summary>
	private void SetValidationGroup()
	{
		if (this.ValidationGroup != null)
		{
			Predicate<Control> isValidationButtonPredicate = control => ((control is IButtonControl) &&
				((((IButtonControl)control).CommandName == CommandNames.OK)
					|| (((IButtonControl)control).CommandName == CommandNames.Save)
					|| (((IButtonControl)control).CommandName == CommandNames.New)
					|| (((IButtonControl)control).CommandName == CommandNames.Previous)
					|| (((IButtonControl)control).CommandName == CommandNames.Next)));

			List<IButtonControl> validationButtons = headerFormView.FindControls(isValidationButtonPredicate, false)
				.Concat(contentFormView.FindControls(isValidationButtonPredicate, false))
				.Concat(modalDialog.FooterTemplateContainer.FindControls(isValidationButtonPredicate, false))
				.OfType<IButtonControl>()
				.ToList();
			validationButtons.ForEach(button => button.ValidationGroup = this.ValidationGroup);
		}
	}

	/// <summary>
	/// Povolí zakáže tlačítka pro navigaci na další záznam.
	/// </summary>
	private void SetPreviousNextButtons()
	{
		List<WebControl> previousButtons = this.modalDialog.FooterTemplateContainer.FindControls(control => (control is IButtonControl) && ((IButtonControl)control).CommandName == CommandNames.Previous, false).OfType<WebControl>().ToList();
		List<WebControl> nextButtons = this.modalDialog.FooterTemplateContainer.FindControls(control => (control is IButtonControl) && ((IButtonControl)control).CommandName == CommandNames.Next, false).OfType<WebControl>().ToList();

		if (previousButtons.Count > 0)
		{
			DataEventArgs<bool> dataEventArgs = new DataEventArgs<bool>(false);
			this.OnGetCanNavigatePrevious(dataEventArgs);
			bool canNavigatePrevious = dataEventArgs.Data;
			previousButtons.ForEach(button => button.Enabled = canNavigatePrevious);
		}

		if (nextButtons.Count > 0)
		{
			DataEventArgs<bool> dataEventArgs = new DataEventArgs<bool>(false);
			this.OnGetCanNavigateNext(dataEventArgs);
			bool canNavigateNext = dataEventArgs.Data;
			nextButtons.ForEach(button => button.Enabled = canNavigateNext);
		}
	}

	/// <summary>
	/// Povolí zakáže tlačítka pro založení nové položky.
	/// </summary>
	private void SetNewButtons()
	{
		List<WebControl> newButtons = this.modalDialog.FooterTemplateContainer.FindControls(control => (control is IButtonControl) && ((IButtonControl)control).CommandName == CommandNames.New, false).OfType<WebControl>().ToList();

		if (newButtons.Count > 0)
		{
			DataEventArgs<bool> dataEventArgs = new DataEventArgs<bool>(false);
			this.OnGetCanCreateNew(dataEventArgs);
			bool canCreateNewItem = dataEventArgs.Data;
			newButtons.ForEach(button => button.Enabled = canCreateNewItem);
		}
	}

	/// <summary>
	/// Extract editor values to dataObject.
	/// Uses two-way databinding.
	/// </summary>
	public void ExtractValues(object dataObject)
	{
		contentFormView.ExtractValues(dataObject);
	}

	/// <summary>
	/// Handles save (ItemSaving, ItemSaved events).
	/// </summary>
	public bool Save()
	{
		DataEventArgs<object> dataEventArgs = new DataEventArgs<object>();
		OnGetEditedObject(dataEventArgs);

		EditorExtenderItemSavingEventArgs extenderItemSaving = new EditorExtenderItemSavingEventArgs(dataEventArgs.Data);
		OnItemSaving(extenderItemSaving);
		if (!extenderItemSaving.Cancel)
		{
			OnItemSaved(new EditorExtenderItemSavedEventArgs(extenderItemSaving.SavedObject ?? extenderItemSaving.EditedObject));
			return true;
		}
		return false;
	}

	/// <summary>
	/// Closes editor (with EditClosed event).
	/// </summary>
	public void CloseEditor()
	{
		modalDialog.Hide();
	}

	/// <summary>
	/// Navigates to previous record.
	/// </summary>
	public void NavigatePrevious()
	{
		CancelEventArgs previousNavigating = new CancelEventArgs();
		OnPreviousNavigating(previousNavigating);
		if (!previousNavigating.Cancel)
		{
			DataBind();
			OnPreviousNavigated(EventArgs.Empty);
		}
	}

	/// <summary>
	/// Navigates to next record.
	/// </summary>
	public void NavigateNext()
	{
		CancelEventArgs nextNavigating = new CancelEventArgs();
		OnNextNavigating(nextNavigating);
		if (!nextNavigating.Cancel)
		{
			DataBind();
			OnNextNavigated(EventArgs.Empty);
		}
	}

	/// <summary>
	/// Starts editing new record.
	/// </summary>
	public void EditNew()
	{
		CancelEventArgs newProcessingEventArgs = new CancelEventArgs();
		OnNewProcessing(newProcessingEventArgs);
		if (!newProcessingEventArgs.Cancel)
		{
			DataBind();
			OnNewProcessed(EventArgs.Empty);
		}
	}

	/// <summary>
	/// When dialog hidden, notify ebout editor close.
	/// </summary>
	private void ModalDialogDialogHidden(object sender, EventArgs e)
	{
		OnEditClosed(EventArgs.Empty);
	}

	/// <summary>
	/// Propagates Item Created event from nested form view.
	/// </summary>
	private void ContentFormViewItemCreated(object sender, EventArgs e)
	{
		OnItemCreated(e);
	}

	/// <summary>
	/// Handles Save command, processes item save. Returns true if item successfully saved (meaning item is not canceled).
	/// Handles command only if if Page is valid (or not validated).
	/// </summary>
	private void HandleSaveCommand(bool causesValidation)
	{
		if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
		{
			if (Save())
			{
				// potřebujeme aktualizovat zobrazená data, minimálně tlačítka Previous/Next
				DataBind();
			}
		}
	}

	/// <summary>
	/// Handles OK command.
	/// Handles command only if if Page is valid (or not validated).
	/// </summary>
	private void HandleOKCommand(bool causesValidation)
	{
		if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
		{
			if (Save())
			{
				this.CloseEditor();
			}
		}
	}

	/// <summary>
	/// Handle Cancel command.
	/// Handles command only if if Page is valid (or not validated).
	/// </summary>
	private void HandleCancelCommand(bool causesValidation)
	{
		if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
		{
			this.CloseEditor();
		}
	}

	/// <summary>
	/// Handles Previous command.
	/// Handles command only if if Page is valid (or not validated).
	/// </summary>
	private void HandlePreviousCommand(bool causesValidation)
	{
		if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
		{
			if (Save())
			{
				NavigatePrevious();
			}
		}
	}

	/// <summary>
	/// Handles Next command.
	/// Handles command only if if Page is valid (or not validated).
	/// </summary>
	private void HandleNextCommand(bool causesValidation)
	{
		if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
		{
			if (Save())
			{
				NavigateNext();
			}
		}
	}

	private void HandleNewCommand(bool causesValidation)
	{
		if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
		{
			if (Save())
			{
				EditNew();
			}
		}
	}

	/// <summary>
	/// Searches the for a control with the specified id.
	/// If not found by stardard method, tries search control in modal dialog content control and header control.
	/// </summary>
	public override Control FindControl(string id)
	{
		return base.FindControl(id) ?? FindContentControl(id) ?? FindHeaderControl(id);
	}

	/// <summary>
	/// Finds control by id in modal dialog header.
	/// Returns null if not found.
	/// </summary>
	public Control FindHeaderControl(string id)
	{
		EnsureChildControls();
		return headerFormView.FindControl(id);
	}

	/// <summary>
	/// Finds control by id in modal dialog content.
	/// Returns null if not found.
	/// </summary>
	public Control FindContentControl(string id)
	{
		EnsureChildControls();
		return contentFormView.FindControl(id);
	}

	/// <summary>
	/// Notifies request for edited object. 
	/// Event handler GetEditedObject must be hadnled.
	/// </summary>
	protected void OnGetEditedObject(DataEventArgs<object> eventArgs)
	{
		if (GetEditedObject == null)
		{
			throw new InvalidOperationException("GetEditedObject event is not handled.");
		}

		GetEditedObject(this, eventArgs);
	}

	/// <summary>
	/// Notifies request for edited object. 
	/// Event handler GetEditedObject must be hadnled.
	/// </summary>
	public event DataEventHandler<object> GetEditedObject;

	/// <summary>
	/// Notifies edit mode close (whatever reason).
	/// </summary>
	protected void OnEditClosed(EventArgs eventArgs)
	{
		if (EditClosed != null)
		{
			EditClosed(this, eventArgs);
		}
	}

	/// <summary>
	/// Notifies edit mode close (whatever reason).
	/// </summary>
	public event EventHandler EditClosed;

	/// <summary>
	/// Notifies beginning of item save.
	/// Event handler ItemSaving must be handled.
	/// </summary>
	protected void OnItemSaving(EditorExtenderItemSavingEventArgs eventArgs)
	{
		if (ItemSaving == null)
		{
			throw new HttpException("ItemSaving event must be handled. Event handler must save edited object.");
		}
		ItemSaving(this, eventArgs);
	}

	/// <summary>
	/// Notifies item save start.
	/// </summary>
	public event EditorExtenderItemSavingEventHandler ItemSaving;

	/// <summary>
	/// Notifies item save completion.
	/// </summary>
	protected void OnItemSaved(EditorExtenderItemSavedEventArgs eventArgs)
	{
		if (ItemSaved != null)
		{
			ItemSaved(this, eventArgs);
		}
	}

	/// <summary>
	/// Notifies item save completion.
	/// </summary>
	public event EditorExtenderItemSavedEventHandler ItemSaved;

	/// <summary>
	/// Notifies item creation.
	/// </summary>
	protected void OnItemCreated(EventArgs eventArgs)
	{
		if (ItemCreated != null)
		{
			ItemCreated(this, eventArgs);
		}
	}

	/// <summary>
	/// Notifies item creation.
	/// </summary>
	public event EventHandler ItemCreated;

	/// <summary>
	/// Notifies item databound event.
	/// </summary>
	protected void OnItemDataBound(EditorExtenderItemDataBoundEventArgs eventArgs)
	{
		if (ItemDataBound != null)
		{
			ItemDataBound(this, eventArgs);
		}
	}

	/// <summary>
	/// Notifies item databound event.
	/// </summary>
	public event EditorExtenderItemDataBoundEventHandler ItemDataBound;

	/// <summary>
	/// Notifies request to start editing a new record.
	/// </summary>
	protected void OnNewProcessing(CancelEventArgs eventArgs)
	{
		if (NewProcessing == null)
		{
			throw new HttpException("NewProcessing event must be handled.");
		}
		NewProcessing(this, eventArgs);
	}

	/// <summary>
	/// Notifies request to start editing a new record.
	/// </summary>
	public event CancelEventHandler NewProcessing;

	/// <summary>
	/// Notifies completion of the start editiong a new record.
	/// </summary>
	protected void OnNewProcessed(EventArgs eventArgs)
	{
		if (NewProcessed != null)
		{
			NewProcessed(this, eventArgs);
		}
	}

	/// <summary>
	/// Notifies about completion of the new record editing start.
	/// </summary>
	public event EventHandler NewProcessed;

	/// <summary>
	/// Notifies request to navigate to previous record.
	/// </summary>
	protected void OnPreviousNavigating(CancelEventArgs eventArgs)
	{
		if (PreviousNavigating == null)
		{
			throw new HttpException("PreviousNavigating event must be handled.");
		}
		PreviousNavigating(this, eventArgs);
	}

	/// <summary>
	/// Notifies request to navigate to previous record.
	/// </summary>
	public event CancelEventHandler PreviousNavigating;

	/// <summary>
	/// Notifies about navigation to previous record.
	/// </summary>
	protected void OnPreviousNavigated(EventArgs eventArgs)
	{
		if (PreviousNavigated != null)
		{
			PreviousNavigated(this, eventArgs);
		}
	}

	/// <summary>
	/// Notifies about navigation to previous record.
	/// </summary>
	public event EventHandler PreviousNavigated;

	/// <summary>
	/// Notifies request to navigate to next record.
	/// </summary>
	protected void OnNextNavigating(CancelEventArgs eventArgs)
	{
		if (NextNavigating == null)
		{
			throw new HttpException("NextNavigating event must be handled.");
		}
		NextNavigating(this, eventArgs);
	}

	/// <summary>
	/// Notifies request to navigate to next record.
	/// </summary>
	public event CancelEventHandler NextNavigating;

	/// <summary>
	/// Notifies about navigation to next record.
	/// </summary>
	protected void OnNextNavigated(EventArgs eventArgs)
	{
		if (NextNavigated != null)
		{
			NextNavigated(this, eventArgs);
		}
	}

	/// <summary>
	/// Notifies about navigation to next record.
	/// </summary>
	public event EventHandler NextNavigated;

	/// <summary>
	/// Ensures rendering dialog only.
	/// </summary>
	protected override void Render(HtmlTextWriter writer)
	{
		modalDialog.RenderControl(writer);
	}

	/// <summary>
	/// Asks if it is possible to create a new record.
	/// </summary>
	protected void OnGetCanCreateNew(DataEventArgs<bool> eventArgs)
	{
		if (GetCanCreateNew == null)
		{
			throw new HttpException("OnGetCanCreateNew event must be handled.");
		}
		GetCanCreateNew(this, eventArgs);
	}

	/// <summary>
	/// Asks if it is possible to create a new record.
	/// </summary>
	public event DataEventHandler<bool> GetCanCreateNew;

	/// <summary>
	/// Asks if it is possible to navigate to previous item.
	/// </summary>
	protected void OnGetCanNavigatePrevious(DataEventArgs<bool> eventArgs)
	{
		if (GetCanNavigatePrevious == null)
		{
			throw new HttpException("GetCanNavigatePrevious event must be handled.");
		}
		GetCanNavigatePrevious(this, eventArgs);
	}

	/// <summary>
	/// Asks if it is possible to navigate to previous item.
	/// </summary>
	public event DataEventHandler<bool> GetCanNavigatePrevious;

	/// <summary>
	/// Asks if it is possible to navigate to next item.
	/// </summary>
	protected void OnGetCanNavigateNext(DataEventArgs<bool> eventArgs)
	{
		if (GetCanNavigateNext == null)
		{
			throw new HttpException("GetCanNavigateNext event must be handled.");
		}
		GetCanNavigateNext(this, eventArgs);
	}

	/// <summary>
	/// Asks if it is possible to navigate to next item.
	/// </summary>
	public event DataEventHandler<bool> GetCanNavigateNext;
}
