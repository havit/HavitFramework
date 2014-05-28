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
	/// Extended CustomValidator.
	/// </summary>
	public class CustomValidator : System.Web.UI.WebControls.CustomValidator, IValidatorExtension
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
		[DefaultValue(ValidatorRenderExtender.DefaultControlToValidateInvalidCssClass)]
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

		#region Constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public CustomValidator()
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
