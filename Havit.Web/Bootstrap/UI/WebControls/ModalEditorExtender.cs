using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
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
	[ParseChildren(true)]
	[PersistChildren(false)]
	[Themeable(true)]
	public class ModalEditorExtender : Control, IEditorExtender
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
		[TemplateContainer(typeof(FormView), BindingDirection.TwoWay)]
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

		#region ItemType
		/// <summary>
		/// Item type. Pro strong type databinding.
		/// </summary>
		[DefaultValue("")]
		[Themeable(false)]
		public virtual string ItemType
		{
			get
			{
				return contentFormView.ItemType;
			}
			set
			{
				headerFormView.ItemType = value;
				contentFormView.ItemType = value;
			}
		}
		#endregion

		#region For
		/// <summary>
		/// Control to which is editor attached.
		/// </summary>
		public string For
		{
			get
			{
				return (string)(ViewState["For"] ?? default(string));
			}
			set
			{
				if (initPerformed)
				{
					throw new InvalidOperationException("Cannot set For after init was performed.");
				}
				ViewState["For"] = value;
			}
		}
		#endregion

		#region ForControl
		/// <summary>
		/// For control as IEditorExtensible.
		/// </summary>
		protected IEditorExtensible ForControl
		{
			get
			{
				if (String.IsNullOrEmpty(For))
				{
					throw new InvalidOperationException("'For' property not set.");
				}

				Control forControl = this.NamingContainer.FindControl(For);
				if (forControl == null)
				{
					throw new InvalidOperationException(String.Format("Control with ID '{0}' set in 'For' property was not found.", For));
				}

				if (!(forControl is IEditorExtensible))
				{
					throw new InvalidOperationException(String.Format("Control with ID '{0}' set in 'For' property does not implement IEditorExtensible.", For));
				}

				return (IEditorExtensible)forControl;
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
		/// Ensures child controls and registers editor to a "For" control.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			initPerformed = true;
			base.OnInit(e);
			EnsureChildControls();

			ForControl.RegisterEditor(this);
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

			headerFormView.DataSource = new object[] { dataEventArgs.Data };
			headerFormView.DataBind();

			contentFormView.DataSource = new object[] { dataEventArgs.Data };
			contentFormView.DataBind();

			modalDialog.Show();
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
	}
}
