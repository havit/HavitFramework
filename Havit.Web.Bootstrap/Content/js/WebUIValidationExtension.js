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
		var WebFormsOriginals_ValidationSummaryOnSubmit = null; // remember original ValidationSummaryOnSubmit function
		var WebFormsOriginals_ValidatorOnLoad_OnceOnlyEnabled = false;
		// ensures javascript method replacements
		// asynchronous request forces WebUIValidation.js to load for the second time
		// so we have to hook-up again (see ValidatorRenderedExtender class)
		Havit_Validation_StartUp = function () {
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

			if (ValidationSummaryOnSubmit != Havit_ValidationSummaryOnSubmit) {
				WebFormsOriginals_ValidationSummaryOnSubmit = ValidationSummaryOnSubmit;
				ValidationSummaryOnSubmit = Havit_ValidationSummaryOnSubmit;
			}

			// set UI after validators evaluation (null means all validation groups!)
			// to enable validations display after postback
			Havit_UpdateValidatorsExtensionsUI(null, true); 
			WebFormsOriginals_ValidatorOnLoad_OnceOnlyEnabled = true;
		};

		// override (extend) Page_ClientValidate function
		// called when submitting form
		Havit_Page_ClientValidate = function (validationGroup) {			
			var result = WebFormsOriginals_Page_ClientValidate(validationGroup);			
			Havit_UpdateValidatorsExtensionsUI(validationGroup, false); // set UI after validators evaluation

			return result;
		};

		// override (extend) ValidatorOnChange function
		// called when changing control content
		Havit_ValidatorOnChange = function (event) {
			WebFormsOriginals_ValidatorOnChange(event);
			Havit_UpdateValidatorsExtensionsUI(null, false); // set UI after validators evaluation (null means all validation groups!)
		};

		// override (extend) ValidatorOnLoad function
		// called when form loading
		Havit_ValidatorOnLoad = function () {
			WebFormsOriginals_ValidatorOnLoad();
			if (WebFormsOriginals_ValidatorOnLoad_OnceOnlyEnabled) {
				WebFormsOriginals_ValidatorOnLoad_OnceOnlyEnabled = false;
				// set UI after validators evaluation (null means all validation groups!)
				// to enable validations display after asynchronous postback
				Havit_UpdateValidatorsExtensionsUI(null, true); 
			}
		};

		// override (extend) ValidationSummaryOnSubmit function
		// called from Page_ClientValidate
		Havit_ValidationSummaryOnSubmit = function (validationGroup) {
			WebFormsOriginals_ValidationSummaryOnSubmit(validationGroup);
			Havit_ValidationSummary_ProcessToastr(validationGroup);
		}

		// extends UI by the validation result
		Havit_UpdateValidatorsExtensionsUI = function(validationGroup, useAttributeDataInsteadOfIsValid) {
			if (typeof (Page_Validators) == "undefined") {
				return true;
			}

			var passedValidators = new Array();
			var failedValidators = new Array();

			// lets find passed and failed validators
			for (var i = 0; i < Page_Validators.length; i++) {
				var val = Page_Validators[i];
				if (IsValidationGroupMatch(val, validationGroup)) {
					var isValid = useAttributeDataInsteadOfIsValid ? (val.getAttribute("data-val-isvalid") != "False") : val.isvalid;
					(isValid ? passedValidators : failedValidators).push(val); // set validator to array of passed or failed validations
				}
			}

			// remove class and tooltip of passed validators
			passedValidators.forEach(function(item) {
				if (item != null) {
					var validationDisplayTargetControl = item.getAttribute("data-val-validationdisplaytarget");
					if (validationDisplayTargetControl == null) {
						validationDisplayTargetControl = item.controltovalidate;
					};

					var controltovalidateclass = item.getAttribute("data-val-ctvclass"); // control to validate class
					if ((validationDisplayTargetControl != null) && (validationDisplayTargetControl.length > 0)) {
						if ($controlToValidate.attr("tooltipReady")) {
							$("#" + validationDisplayTargetControl).attr("tooltipReady", false).tooltip('destroy'); // destroy existing tooltip
						}
						if ((controltovalidateclass != null) && (controltovalidateclass.length > 0)) {
							$controlToValidate = $("#" + validationDisplayTargetControl);
							$controlToValidate.removeClass(controltovalidateclass); // remove "validation failed" class to a control to validate
						}
					}
				}
			});

			// add class and tooltip of passed validators
			// ensures text concatenation whene there is more then one validator of a control

			var failedValidatorsTooltips = new Array(); // tooltips to create

			// set styles to invalid validations
			failedValidators.forEach(function(item) {
				var validationDisplayTargetControl = item.getAttribute("data-val-validationdisplaytarget");
				if (validationDisplayTargetControl == null) {
					validationDisplayTargetControl = item.controltovalidate;
				};

				var controltovalidateclass = item.getAttribute("data-val-ctvclass");
				var tooltipposition = item.getAttribute("data-val-tt-position");
				var tooltiptext = item.getAttribute("data-val-tt-text");
				if ((validationDisplayTargetControl != null) && (validationDisplayTargetControl.length > 0)) {

					if ((controltovalidateclass != null) && (controltovalidateclass.length > 0)) {
						$("#" + validationDisplayTargetControl).addClass(controltovalidateclass); // add "validation failed" class to a control to validate
					}

					if ((tooltiptext != null) && (tooltiptext.length > 0)) {
						if ((tooltipposition == null) || (tooltipposition.length == 0)) {
							tooltipposition = 'right'; // default position
						}

						// search if there is a tooltip for a control (from another failed validator)
						var validationTooltip = null;
						for (var i = 0; i < failedValidatorsTooltips.length; i++) {
							if ((failedValidatorsTooltips[i].validationDisplayTargetControl == validationDisplayTargetControl) && (failedValidatorsTooltips[i].position == tooltipposition)) {
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
								validationDisplayTargetControl: validationDisplayTargetControl,
								position: tooltipposition,
								text: tooltiptext
							});
						}
					}
				}
			});

			// create tooltips from prepared array
			failedValidatorsTooltips.forEach(function(tooltip) {
				$("#" + tooltip.validationDisplayTargetControl)
					.attr("tooltipReady", true)
					.tooltip({
						'placement': tooltip.position,
						'title': tooltip.text,
						'trigger': 'hover focus',
						'html': true // ensures <br /> to work
					});
			});
		};

		Havit_ValidationSummary_ProcessToastr = function (validationGroup) {

			if (typeof (Page_ValidationSummaries) == "undefined") {
				return;
			}

			// remove previous error message
			if (Havit_ValidationSummary_ShowToastr_Toastrs) {
				Havit_ValidationSummary_ShowToastr_Toastrs.forEach(function(toastrItem) {
					toastr.clear(toastrItem);
				});
			}
			Havit_ValidationSummary_ShowToastr_Toastrs = new Array();

			var validationSummary, summaryIndex, errorMessage;
			for (summaryIndex = 0; summaryIndex < Page_ValidationSummaries.length; summaryIndex++) { // for all validation summaries
				validationSummary = Page_ValidationSummaries[summaryIndex];

				// which are for the validation group and should show toastr
				if (validationSummary && validationSummary.parentNode && IsValidationGroupMatch(validationSummary, validationGroup) && (validationSummary.getAttribute("data-showtoastr") == "True")) {
					// concatenate text for toastr message
					errorMessage = "";
					if (typeof (validationSummary.headertext) == "string") {
						errorMessage += validationSummary.headertext + "<br />";
					}
					for (i = 0; i < Page_Validators.length; i++) {
						if (!Page_Validators[i].isvalid && IsValidationGroupMatch(Page_Validators[i], validationSummary.validationGroup) && (typeof (Page_Validators[i].errormessage) == "string") && (Page_Validators[i].errormessage.length > 0)) {
							errorMessage += Page_Validators[i].errormessage + "<br />";
						}
					}
					Havit_ValidationSummary_ShowToastrError(errorMessage);
				}
			}
		};

		Havit_ValidationSummary_ShowToastrError = function(errorMessage) {
			if (errorMessage.length > 0) {
				var optionsOverride = {
					closeButton: true, // display close button
					tapToDismiss: false, // click do not close message
					timeOut: 0, // message is visible until close button click
				};

				// create and remember toastr message
				Havit_ValidationSummary_ShowToastr_Toastrs[Havit_ValidationSummary_ShowToastr_Toastrs.length] = toastr.error(errorMessage, null, optionsOverride);
			}
		}

		// displayed messages
		Havit_ValidationSummary_ShowToastr_Toastrs = new Array();
	})(jQuery);
}
