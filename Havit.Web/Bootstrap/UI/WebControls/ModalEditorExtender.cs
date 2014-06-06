using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.Adapters;

using Havit.Web.Bootstrap.UI.WebControls;
using Havit.Web.UI;
using Havit.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls
{
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
	public class ModalEditorExtender : DataBoundControlWithHiddenPublicMembersFromIntellisence, IEditorExtender, INamingContainer
	{
		#region Private fields
		private ModalDialog modalDialog;
		private FormViewExt headerFormView;
		private FormViewExt contentFormView;
		#endregion

		#region HeaderText
		/// <summary>
		/// Editor dialog header text.
		/// </summary>
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
		#endregion

		#region HeaderTemplate
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
		#endregion

		#region ContentTemplate
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
		#endregion

		#region FooterTemplate
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
		#endregion

		#region TargetControlID
		/// <summary>
		/// Control to which is editor attached.
		/// </summary>
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
		#endregion

		#region TargetControl
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
		#endregion

		#region Triggers
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
		#endregion

		#region Width
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
		#endregion

		#region UseAnimations
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
		#endregion

		#region CssClass
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
		#endregion

		#region HeaderCssClass
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
		#endregion

		#region ContentCssClass
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
		#endregion

		#region FooterCssClass
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
		#endregion

		#region ShowCloseButton
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
		#endregion

		#region CloseOnEscapeKey
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
		#endregion

		#region DragMode
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
		#endregion

		#region Constructor
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
			contentFormView.DataBound += ContentFormViewDataBound;

			modalDialog = new ModalDialog();
			modalDialog.DialogHidden += ModalDialogDialogHidden;
		}
		#endregion

		#region OnInit
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
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Creates child controls.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();

			if (headerFormView.ItemTemplate != null)
			{
				//if (headerFormView.ItemTemplate is IBindableTemplate)
				//{
				//	throw new HttpException("Two-way databinding is not supported in Modal Editor header.");
				//}
				modalDialog.HeaderTemplateContainer.Controls.Add(headerFormView);
			}

			modalDialog.ContentTemplateContainer.Controls.Add(contentFormView);

			this.Controls.Add(modalDialog);
		}
		#endregion

		#region OnBubbleEvent
		/// <summary>
		/// Handles events Save, OK, Cancel from children.
		/// </summary>
		protected override bool OnBubbleEvent(object source, EventArgs args)
		{
			if (args is CommandEventArgs)
			{
				CommandEventArgs commandEventArgs = (CommandEventArgs)args;

				switch (commandEventArgs.CommandName)
				{
					case CommandNames.Save:
						HandleItemSave();
						return true;

					case CommandNames.OK:
						if (HandleItemSave())
						{
							CloseEditor();
						}
						return true;

					case CommandNames.Cancel:
						CloseEditor();
						return true;
				}
			}

			return base.OnBubbleEvent(source, args);
		}
		#endregion

		#region StartEditor
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
		#endregion

		#region OnDataBinding
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
		#endregion

		#region PerformDataBinding
		/// <summary>
		/// Binds data to dialog header and content.
		/// </summary>
		protected override void PerformDataBinding(IEnumerable data)
		{
			base.PerformDataBinding(data);

			headerFormView.DataSource = data;
			headerFormView.DataBind();

			contentFormView.DataSource = data;
			contentFormView.DataBind();
		}
		#endregion

		#region ExtractValues
		/// <summary>
		/// Extract editor values to dataObject.
		/// Uses two-way databinding.
		/// </summary>
		public void ExtractValues(object dataObject)
		{
			contentFormView.ExtractValues(dataObject);
		}
		#endregion

		#region Save
		/// <summary>
		/// Handles save (ItemSaving, ItemSaved events).
		/// </summary>
		public void Save()
		{
			HandleItemSave();
		}
		#endregion

		#region CloseEditor
		/// <summary>
		/// Closes editor (with EditClosed event).
		/// </summary>
		public void CloseEditor()
		{
			modalDialog.Hide();
		}
		#endregion

		#region ModalDialogDialogHidden
		/// <summary>
		/// When dialog hidden, notify ebout editor close.
		/// </summary>
		private void ModalDialogDialogHidden(object sender, EventArgs e)
		{
			OnEditClosed(EventArgs.Empty);
		}
		#endregion

		#region ContentFormViewItemCreated
		/// <summary>
		/// Propagates Item Created event from nested form view.
		/// </summary>
		private void ContentFormViewItemCreated(object sender, EventArgs e)
		{
			OnItemCreated(e);
		}
		#endregion

		#region ContentFormViewDataBound
		/// <summary>
		/// Propagates Item DataBound event from nested form view.
		/// </summary>
		private void ContentFormViewDataBound(object sender, EventArgs e)
		{
			OnItemDataBound(e);
		}
		#endregion

		#region HandleItemSave
		/// <summary>
		/// Processes item save. Returns true if item successfully saved (meaning item is not canceled).
		/// </summary>
		private bool HandleItemSave()
		{
			CancelEventArgs itemSaving = new CancelEventArgs();
			OnItemSaving(itemSaving);
			if (!itemSaving.Cancel)
			{
				OnItemSaved(EventArgs.Empty);
				return true;
			}

			return false;
		}
		#endregion

		#region (On)GetEditedObject
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
		#endregion

		#region (On)EditClosed
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
		#endregion

		#region (On)ItemSaving
		/// <summary>
		/// Notifies beginning of item save.
		/// Event handler ItemSaving must be handled.
		/// </summary>
		protected void OnItemSaving(CancelEventArgs eventArgs)
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
		public event CancelEventHandler ItemSaving;
		#endregion

		#region (On)ItemSaved
		/// <summary>
		/// Notifies item save completion.
		/// </summary>
		protected void OnItemSaved(EventArgs eventArgs)
		{
			if (ItemSaved != null)
			{
				ItemSaved(this, eventArgs);
			}
		}

		/// <summary>
		/// Notifies item save completion.
		/// </summary>
		public event EventHandler ItemSaved;
		#endregion

		#region (On)ItemCreated
		/// <summary>
		/// Notifies item creation from nested FormView.
		/// </summary>
		protected void OnItemCreated(EventArgs eventArgs)
		{
			if (ItemCreated != null)
			{
				ItemCreated(this, eventArgs);
			}
		}

		/// <summary>
		/// Notifies item creation from nested FormView.
		/// </summary>
		public event EventHandler ItemCreated;
		#endregion

		#region (On)ItemDataBound
		/// <summary>
		/// Notifies item databound event from nested FormView.
		/// </summary>
		protected void OnItemDataBound(EventArgs eventArgs)
		{
			if (ItemDataBound != null)
			{
				ItemDataBound(this, eventArgs);
			}
		}

		/// <summary>
		/// Notifies item databound event from nested FormView.
		/// </summary>
		public event EventHandler ItemDataBound;
		#endregion

		#region Render
		/// <summary>
		/// Ensures rendering dialog only.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			modalDialog.RenderControl(writer);
		}
		#endregion

	}
}
