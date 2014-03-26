using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	/// <summary>
	/// Extended CompareValidator.
	/// </summary>
	public class CompareValidator : System.Web.UI.WebControls.CompareValidator, IValidatorExtension
	{
		#region Common validator extensions

		#region ShowTooltip
		/// <summary>
		/// Shows ToolTip (or Text if ToolTip not set) as a Bootstrap ToolTip at ControlToValidate when validation fails.
		/// </summary>
		[DefaultValue(true)]
		public bool ShowTooltip
		{
			get
			{
				return (bool)(ViewState["ShowTooltip"] ?? true);
			}
			set
			{
				ViewState["ShowTooltip"] = value;
			}
		}
		#endregion

		#region TooltipPosition
		/// <summary>
		/// Tooltip position.
		/// </summary>
		[DefaultValue(TooltipPosition.Right)]
		public TooltipPosition TooltipPosition
		{
			get
			{
				return (TooltipPosition)(ViewState["TooltipPosition"] ?? TooltipPosition.Right);
			}
			set
			{
				ViewState["TooltipPosition"] = value;
			}
		}
		#endregion

		#region ControlToValidateInvalidCssClass
		/// <summary>
		/// CssClass name which is added to ControlToValidate when validation fails. 
		/// </summary>
		[DefaultValue("invalid")]
		public string ControlToValidateInvalidCssClass
		{
			get
			{
				return (string)(ViewState["ControlToValidateInvalidCssClass"] ?? "invalid");
			}
			set
			{
				ViewState["ControlToValidateInvalidCssClass"] = value;
			}
		}
		#endregion

		#region OnInit
		/// <summary>
		/// Registers the validator on the page.
		/// </summary>
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			ValidatorRenderExtender.OnInit(this);
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

		#region Explicit IValidatorExtension implementation
		/// <summary>
		/// The effective enabled property value taking into account that a parent control maybe disabled.
		/// </summary>
		bool IValidatorExtension.IsEnabled
		{
			get { return this.IsEnabled; }
		}

		/// <summary>
		/// Gets a value that indicates whether the client's browser supports uplevel rendering. This property is read-only.
		/// </summary>
		bool IValidatorExtension.RenderUpLevel
		{
			get { return this.RenderUplevel; }
		}
		#endregion

		#endregion
	}
}
