using System;

namespace Havit.WebApplicationTest.HavitWebBootstrapTests.Controls
{
	public class TestUriValidator : Havit.Web.Bootstrap.UI.WebControls.BaseValidator
	{
		protected override bool EvaluateIsValid()
		{
			string controlValidationValue = this.GetControlValidationValue(this.ControlToValidate);
			if (!string.IsNullOrWhiteSpace(controlValidationValue))
			{
				return Uri.IsWellFormedUriString(controlValidationValue, UriKind.Absolute);
			}
			else
			{
				return true;
			}
		}
	}
}