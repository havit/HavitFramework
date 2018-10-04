using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Web.Bootstrap.UI.WebControls
{
	internal interface IValidatorExtension
	{
		string ControlToValidate { get; }
		string ControlToValidateInvalidCssClass { get; }
		string ControlToValidateInvalidToolTipCssClass { get; }
		string ToolTip { get; }
		string Text { get; }
		string ErrorMessage { get; }
		bool ShowToolTip { get; }
		ToolTipPosition ToolTipPosition { get; }
	}
}
