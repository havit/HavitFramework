using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls.Legacy
{
	/// <summary>
	/// Extended CheckBoxValidator.
	/// </summary>
	public class CheckBoxValidator : Web.UI.WebControls.CheckBoxValidator, IValidatorExtension
	{
		#region Common validator extensions

		#region ShowToolTip
		/// <summary>
		/// Shows ToolTip (or Text if ToolTip not set) as a Bootstrap ToolTip at ControlToValidate when validation fails.
		/// </summary>
		public bool ShowToolTip
		{
			get
			{
				return (bool)(ViewState["ShowToolTip"] ?? true);
			}
			set
			{
				ViewState["ShowToolTip"] = value;
			}
		}
		#endregion

		#region ToolTipPosition
		/// <summary>
		/// ToolTip position.
		/// </summary>
		[DefaultValue(ToolTipPosition.Top)]
		public ToolTipPosition ToolTipPosition
		{
			get
			{
				return (ToolTipPosition)(ViewState["ToolTipPosition"] ?? ToolTipPosition.Top);
			}
			set
			{
				ViewState["ToolTipPosition"] = value;
			}
		}
		#endregion

		#region ControlToValidateInvalidCssClass
		/// <summary>
		/// CssClass name which is added to ControlToValidate when validation fails. 
		/// </summary>
		public string ControlToValidateInvalidCssClass
		{
			get
			{
				return (string)(ViewState["ControlToValidateInvalidCssClass"] ?? ValidatorRenderExtender.DefaultControlToValidateInvalidCssClass);
			}
			set
			{
				ViewState["ControlToValidateInvalidCssClass"] = value;
			}
		}
		#endregion

		#region ControlToValidateInvalidToolTipCssClass
		/// <summary>
		/// CssClass name which is added to a validation tooltip. 
		/// </summary>
		public string ControlToValidateInvalidToolTipCssClass
		{
			get
			{
				return (string)(ViewState["ControlToValidateInvalidToolTipCssClass"] ?? ValidatorRenderExtender.DefaultControlToValidateInvalidToolTipCssClass);
			}
			set
			{
				ViewState["ControlToValidateInvalidToolTipCssClass"] = value;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public CheckBoxValidator()
		{
			ValidatorRenderExtender.Setup(this);
		}
		#endregion

		#region AddAttributesToRender
		/// <summary>
		/// Adds the attributes of this control to the output stream for rendering on the client.		
		/// </summary>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			ValidatorRenderExtender.AddAttributesToRender(this, writer);
		}
		#endregion

		#region OnPreRender
		/// <summary>
		/// Checks the client brower and configures the validator for compatibility prior to rendering.
		/// </summary>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			ValidatorRenderExtender.OnPreRender(this);
		}
		#endregion

		#endregion
	}
}
