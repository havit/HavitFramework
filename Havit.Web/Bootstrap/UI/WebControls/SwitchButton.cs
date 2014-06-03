﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using Havit.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Control  with touchable selection of Yes/No value.
	/// By default, the "No" choice is selected (Value is false).
	/// </summary>
	[Themeable(true)]
	public class SwitchButton : Control
	{
		#region Private fields
		private RadioButtonList radioButtonList;
		private ListItem yesItem;
		private ListItem noItem;
		#endregion

		#region Value
		/// <summary>
		/// Returns true if "yes" choice is selected.
		/// </summary>
		public virtual bool Value
		{
			get
			{
				return yesItem.Selected;
			}
			set
			{
				yesItem.Selected = value;
				noItem.Selected = !value;
			}
		}
		#endregion

		#region ValueChanged, OnValueChanged
		/// <summary>
		/// Occurs when the value of the Value property changes between posts to the server.
		/// </summary>
		public event EventHandler ValueChanged;

		/// <summary>
		/// Occurs when the value of the Value property changes between posts to the server.
		/// </summary>
		protected virtual void OnValueChanged(EventArgs eventArgs)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, eventArgs);
			}
		}
		#endregion

		#region AutoPostBack
		/// <summary>
		/// Gets or sets a value indicating whether an automatic postback to the server will occur whenever the user changes the selection of the list.
		/// </summary>
		public virtual bool AutoPostBack
		{
			get { return radioButtonList.AutoPostBack; }
			set { radioButtonList.AutoPostBack = value; }
		}
		#endregion

		#region YesText
		/// <summary>
		/// Text for "Yes" choice.
		/// Supports localization pattern.
		/// </summary>
		[DefaultValue("Yes")]
		public string YesText
		{
			get { return yesItem.Text; }
			set { yesItem.Text = value; }
		}
		#endregion

		#region NoText

		/// <summary>
		/// Text for "No" choice.
		/// Supports localization pattern.
		/// </summary>
		[DefaultValue("No")]
		public string NoText
		{
			get { return noItem.Text; }
			set { noItem.Text = value; }
		}

		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public SwitchButton()
		{
			radioButtonList = new RadioButtonList();
			yesItem = new ListItem("Yes", "1") { Selected = false };
			noItem = new ListItem("No", "0") { Selected = true };
			radioButtonList.Items.Add(yesItem);
			radioButtonList.Items.Add(noItem);
			radioButtonList.SelectedIndexChanged += RadioButtonList_SelectedIndexChanged;
		}
		#endregion

		#region RadioButtonList_SelectedIndexChanged
		private void RadioButtonList_SelectedIndexChanged(object sender, EventArgs e)
		{
			OnValueChanged(e);
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Raises the Init event. This notifies the control to perform any steps necessary for its creation on a page request.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EnsureChildControls();
		}
		#endregion

		#region CreateChildControls
		/// <summary>
		/// Notifies any controls that use composition-based implementation to create any
		/// child controls they contain in preperation for postback or rendering.
		/// </summary>
		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			Controls.Add(radioButtonList);
		}
		#endregion

		#region Render
		/// <summary>
		/// Outputs control content to a provided HTMLTextWriter output stream.
		/// </summary>
		protected override void Render(HtmlTextWriter writer)
		{
			yesItem.Text = HttpUtilityExt.GetResourceString(YesText);
			noItem.Text = HttpUtilityExt.GetResourceString(NoText);

			base.Render(writer);
		}
		#endregion
	}
}