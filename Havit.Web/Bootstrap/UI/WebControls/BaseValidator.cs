using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Havit.Web.Bootstrap.UI.WebControls
{
    /// <summary>
    /// Bázová třída pro Havit validátory, která rozšiřuje <see cref="System.Web.UI.WebControls.BaseValidator"/> o implementaci rozhraní <see cref="Havit.Web.Bootstrap.UI.WebControls.IValidatorExtension"/>.
    /// </summary>
    public abstract class BaseValidator : System.Web.UI.WebControls.BaseValidator, IValidatorExtension
	{
		#region Common validator extensions

		#region ShowToolTip
		/// <summary>
		/// Shows ToolTip (or Text if ToolTip not set) as a Bootstrap ToolTip at ControlToValidate when validation fails.
		/// </summary>
		public virtual bool ShowToolTip
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
		[DefaultValue(ToolTipPosition.Right)]
		public virtual ToolTipPosition ToolTipPosition
		{
			get
			{
				return (ToolTipPosition)(ViewState["ToolTipPosition"] ?? ToolTipPosition.Right);
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
		[DefaultValue(ValidatorRenderExtender.DefaultControlToValidateInvalidCssClass)]
		public virtual string ControlToValidateInvalidCssClass
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
		[DefaultValue(ValidatorRenderExtender.DefaultControlToValidateInvalidToolTipCssClass)]
		public virtual string ControlToValidateInvalidToolTipCssClass
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
		protected BaseValidator()
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