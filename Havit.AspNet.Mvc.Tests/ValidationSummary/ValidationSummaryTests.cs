using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Havit.AspNet.Mvc.ValidationSummary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.AspNet.Mvc.Tests.ValidationSummary;

[TestClass]
public class ValidationSummaryTests
{
	[TestMethod]
	public void ToastrValidationSummary_Render_ShouldEncodeMessage()
	{
		// arrange
		List<ModelError> errors = new List<ModelError>
		{
			new ModelError("This should be \"HTML\" encoded."),
			new ModelError("This should be \"HTML\" encoded too.")
		};
		IValidationSummary validationSummary = new ToastrValidationSummary(errors);

		// act
		MvcHtmlString renderedString = validationSummary.Render();

		// assert
		Assert.AreEqual("<script type=\"text/javascript\">toastr.error(\"This should be &quot;HTML&quot; encoded.<br />This should be &quot;HTML&quot; encoded too.\");</script>", renderedString.ToString());
	}
}
