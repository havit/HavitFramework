if (!window.jQuery) {
	alert('WebUIValidationExtension.js: jQuery must be loaded prior to WebUIValidationExtension.js.');
} else {
	(function($) {

		if (typeof Page_ClientValidate == 'undefined') {
			alert('WebUIValidationExtension.js: WebUIValidation.js must be loaded prior to WebUIValidationExtension.js.');
			return;
		}

		var WebFormsOriginals_Page_ClientValidate = null; // remember original Page_ClientValidate function
		var WebFormsOriginals_ValidatorOnChange = null; // remember original ValidatorOnChange function
		var WebFormsOriginals_ValidatorOnLoad = null; // remember original ValidatorOnLoad function

		// ensures javascript method replacements
		// asynchronous request forces WebUIValidation.js to load for the second time
		// so we have to hook-up again (see ValidatorRenderedExtender class)
		Havit_EnsureValidatorsExtensionsHookup = function() {
			if (Page_ClientValidate != Havit_Page_ClientValidate) {
				WebFormsOriginals_Page_ClientValidate = Page_ClientValidate;
				Page_ClientValidate = Havit_Page_ClientValidate;
			}

			if (ValidatorOnChange != Havit_ValidatorOnChange) {
				WebFormsOriginals_ValidatorOnChange = ValidatorOnChange;
				ValidatorOnChange = Havit_ValidatorOnChange;
			}

			if (ValidatorOnLoad != Havit_ValidatorOnLoad) {
				WebFormsOriginals_ValidatorOnLoad = ValidatorOnLoad;
				ValidatorOnLoad = Havit_ValidatorOnLoad;
			}

		};

		// override (extend) Page_ClientValidate function
		// called when submitting form
		Havit_Page_ClientValidate = function (validationGroup) {			
			var result = WebFormsOriginals_Page_ClientValidate(validationGroup);			
			Havit_UpdateValidatorsExtensionsUI(validationGroup); // set UI after validators evaluation
			return result;
		};

		// override (extend) ValidatorOnChange function
		// called when changing control content
		Havit_ValidatorOnChange = function (event) {
			WebFormsOriginals_ValidatorOnChange(event);
			Havit_UpdateValidatorsExtensionsUI(null); // set UI after validators evaluation (null means all validation groups!)
		};

		// override (extend) ValidatorOnLoad function
		// called when form loading
		Havit_ValidatorOnLoad = function () {
			WebFormsOriginals_ValidatorOnLoad();
			Havit_UpdateValidatorsExtensionsUI(null); // set UI after validators evaluation (null means all validation groups!)
		};

		// extends UI by the validation result
		Havit_UpdateValidatorsExtensionsUI = function(validationGroup) {
			if (typeof (Page_Validators) == "undefined") {
				return true;
			}

			var passedValidators = new Array();
			var failedValidators = new Array();

			// lets find passed and failed validators
			for (var i = 0; i < Page_Validators.length; i++) {
				var val = Page_Validators[i];
				if (IsValidationGroupMatch(val, validationGroup)) {
					(val.isvalid ? passedValidators : failedValidators).push(val); // set validator to array of passed or failed validations
				}
			}

			// remove class and tooltip of passed validators
			passedValidators.forEach(function(item) {
				if (item != null) {
					var controltovalidate = item.controltovalidate;
					var controltovalidateclass = item.getAttribute("data-val-ctvclass"); // control to validate class
					var tooltiptext = item.getAttribute("data-val-tt-text");
					if ((controltovalidate != null) && (controltovalidate.length > 0)) {
						if ((controltovalidateclass != null) && (controltovalidateclass.length > 0)) {
							$("#" + controltovalidate).removeClass(controltovalidateclass); // remove "validation failed" class to a control to validate
						}
						if ((tooltiptext != null) && (tooltiptext.length > 0)) {
							$("#" + controltovalidate).tooltip('destroy'); // destroy existing tooltip
						}
					}
				}
			});

			// add class and tooltip of passed validators
			// ensures text concatenation whene there is more then one validator of a control

			var failedValidatorsTooltips = new Array(); // tooltips to create

			// set styles to invalid validations
			failedValidators.forEach(function(item) {
				var controltovalidate = item.controltovalidate;
				var controltovalidateclass = item.getAttribute("data-val-ctvclass");
				var tooltipposition = item.getAttribute("data-val-tt-position");
				var tooltiptext = item.getAttribute("data-val-tt-text");
				if ((controltovalidate != null) && (controltovalidate.length > 0)) {

					if ((controltovalidateclass != null) && (controltovalidateclass.length > 0)) {
						$("#" + controltovalidate).addClass(controltovalidateclass); // add "validation failed" class to a control to validate
					}

					if ((tooltiptext != null) && (tooltiptext.length > 0)) {
						if ((tooltipposition == null) || (tooltipposition.length == 0)) {
							tooltipposition = 'right'; // default position
						}

						// search if there is a tooltip for a control (from another failed validator)
						var validationTooltip = null;
						for (var i = 0; i < failedValidatorsTooltips.length; i++) {
							if ((failedValidatorsTooltips[i].controltovalidate == controltovalidate) && (failedValidatorsTooltips[i].position == tooltipposition)) {
								validationTooltip = failedValidatorsTooltips[i];
								break;
							}
						}

						if (validationTooltip != null) {
							// if there already is a tooltip, add text
							validationTooltip.text = validationTooltip.text + "<br />" + tooltiptext;
						} else {
							// if there is not a tooltip, create it
							failedValidatorsTooltips.push({
								controltovalidate: controltovalidate,
								position: tooltipposition,
								text: tooltiptext
							});
						}
					}
				}
			});

			// create tooltips from prepared array
			failedValidatorsTooltips.forEach(function(tooltip) {
				$("#" + tooltip.controltovalidate).tooltip({
					'placement': tooltip.position,
					'title': tooltip.text,
					'trigger': 'hover focus',
					'html': true // ensures <br /> to work
				});
			});
		}
	})(jQuery);
}
