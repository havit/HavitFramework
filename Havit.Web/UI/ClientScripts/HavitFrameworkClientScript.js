﻿var havitNumericBoxExtensions = {
	init: function (settings) {
		// Initializes HAVIT numeric box extensions
		$("body").on("click", "input[data-havitnumericbox]", havitNumericBoxExtensions.onclick); // hook click event on date time boxes
		$("body").on("change", "input[data-havitnumericbox]", havitNumericBoxExtensions.onchange); // hook click event on date time boxes
		$("body").on("keypress", "input[data-havitnumericbox]:not([data-havitnumericbox-suppressfilterkey])", havitNumericBoxExtensions.onkeypress); // hook keypress event on date time boxes
	},
	
	onclick: function() {
		$(this).select(); // select content of textbox
	},

	onchange: function () {
		havitNumericBoxExtensions.fixDecimals($(this));
	},
	
	onkeypress: function (e) {
		havitNumericBoxExtensions.checkValidKey($(this), e);
	},
	
	fixDecimals: function ($this) {
		var decimals = $this.attr("data-havitnumericbox-decimals");
		var decimalSeparator = $this.attr("data-havitnumericbox-decimalseparator");

		if (decimals && decimalSeparator) {
			var value = $this.val();
			var position = value.indexOf(decimalSeparator);

			if (position >= 0) {
				value = value.substr(0, position + parseInt(decimals, 10) + 1);
				$this.val(value);
			}
		}
	},
	
	checkValidKey: function ($this, e) {
		// is not called for special keys - // page up, page down, home, end, insert, delete, arrows keys (except FF)

		if (e.ctrlKey || e.altKey || e.metaKey) {
			return;
		}

		if ((e.charCode == 0) && (e.keyCode != 0)) {
			// special keys in FF
			return;
		}

		var decimals = $this.attr("data-havitnumericbox-decimals");
		var decimalSeparator = $this.attr("data-havitnumericbox-decimalseparator");
		var thousandsSeparator = $this.attr("data-havitnumericbox-thousandsseparator");
		var allowNegative = $this.attr("data-havitnumericbox-allownegative");

		var charCode = e.charCode ? e.charCode : e.keyCode;
		if (charCode) {
			if ((charCode < 32)
				|| ((charCode >= 48) && (charCode <= 57)) // 0-9
				|| (decimalSeparator && decimals && (parseInt(decimals, 10) > 0) && (String.fromCharCode(charCode) == decimalSeparator) && ($this.val().indexOf(decimalSeparator) == -1) /* decimal separator not entered */)
				|| (thousandsSeparator && (String.fromCharCode(charCode) == thousandsSeparator))
				|| (allowNegative && (allowNegative == "true") && (String.fromCharCode(charCode) == "-") && ($this.val().indexOf("-") == -1) /* minus not entered */)
			) {
				// valid character
			} else {
				// invalid character
				e.preventDefault();
			}
		}
	}

};

var havitDateTimeBoxExtensions = {
	
	init: function (settings) {
		// Initializes HAVIT date time box extensions
		$("body").on("click", "input[data-havitdatetimebox]", havitDateTimeBoxExtensions.onclick); // hook click event on date time boxes
		$("body").on("change", "input[data-havitdatetimebox]", havitDateTimeBoxExtensions.onchange); // hook change event on date time boxes
		$("body").on("keypress", "input[data-havitdatetimebox]:not([data-havitdatetimebox-suppressfilterkey])", havitDateTimeBoxExtensions.onkeypress); // hook keypress event on date time boxes
	},
	
	onclick: function() {
		$(this).select(); // select content of textbox
	},
	
	onchange: function (e) {
		havitDateTimeBoxExtensions.autoCompleteDate($(this), e); // autocompletes date
	},
	
	onkeypress: function (e) {
		havitDateTimeBoxExtensions.replaceComma($(this), e); // transforms "," to "." while typing
		havitDateTimeBoxExtensions.checkValidKey($(this), e); // checks if pressed valid key
	},
	
	autoCompleteDate: function ($this, e)
	{
		var datumSeparator = $this.attr("data-havitdatetimebox-dateseparator");
		if (!datumSeparator) {
			return;
		}
		
		var datum = new Date();
		var datumMesic = datum.getMonth() + 1;
		var datumRok = datum.getFullYear();
		var regDen = '([1-9]|0[1-9]|[1-2]\\d|3[01])';
		var regDenCely = '(0[1-9]|[1-2]\\d|3[01])';
		var regMesic = '([1-9]|0[1-9]|1[0-2])';
		var regMesicCely = '(0[1-9]|1[0-2])';
		var regRok = '((19|20)\\d\\d)';
		var patterns = new Array (
			new Array
				(regDen + '(\\' + datumSeparator + ')?',
				'\$1' + datumSeparator + datumMesic + datumSeparator + datumRok
			),
			new Array(
				regDen + '(\\' + datumSeparator + ')' + regMesic + '(\\' + datumSeparator + ')?',
				'\$1' + datumSeparator + '\$3' + datumSeparator + datumRok
			),
			new Array(
				regDenCely + regMesicCely + '(\\' + datumSeparator + ')?',
				'\$1' + datumSeparator + '\$2' + datumSeparator + datumRok
			),
			new Array(
				regDenCely + '(\\' + datumSeparator + ')?' + regMesicCely + '(\\' + datumSeparator + ')?' + regRok,
				'\$1' + datumSeparator + '\$3' + datumSeparator + '\$5'
			)
		);
			
		var i = 0;
		var found = false;
		while (i < patterns.length && !found) {
			var regPattern = new RegExp('^' + patterns[i][0] + '$');
			var replacement = patterns[i][1];
			var value = $this.val();
			if (regPattern.test(value)) {
				var newValue = value.replace(regPattern, replacement);
				if (value != newValue) {
					$this.val(newValue);
					found = true;
				}
			}
			i++;
		}

		// před výměnou hodnoty se aplikují validátory
		// potřebujeme zavolat druhé volání onchange, po výměně hodnoty, které validátory zase vypne
		// musíme ale zastavit propagaci události, protože jinak by se mohl volat dvakrát postback
		// v případě autopostbacku, proto return false		
		if (found) {
			$this.trigger("change");
			e.preventDefault();
		}
	},
	
	replaceComma: function ($this, e)
	{
		// zdroj: http://jsfiddle.net/EXH2k/6/
			
		var dateSeparator = $this.attr("data-havitdatetimebox-dateseparator");
		if (e.which && dateSeparator) {				
			var charStr = String.fromCharCode(e.which);
			var transformedChar = (((dateSeparator == ".") && (charStr == ",")) || ((dateSeparator != ".") && (charStr == "."))) ? dateSeparator : charStr;
			if (transformedChar != charStr) {

				var getInputSelection = function (el) {
					var start = 0, end = 0, normalizedValue, range,
						textInputRange, len, endRange;

					if (typeof el.selectionStart == "number" && typeof el.selectionEnd == "number") {
						start = el.selectionStart;
						end = el.selectionEnd;
					} else {
						range = document.selection.createRange();

						if (range && range.parentElement() == el) {
							len = el.value.length;
							normalizedValue = el.value.replace(/\r\n/g, "\n");

							// Create a working TextRange that lives only in the input
							textInputRange = el.createTextRange();
							textInputRange.moveToBookmark(range.getBookmark());

							// Check if the start and end of the selection are at the very end
							// of the input, since moveStart/moveEnd doesn't return what we want
							// in those cases
							endRange = el.createTextRange();
							endRange.collapse(false);

							if (textInputRange.compareEndPoints("StartToEnd", endRange) > -1) {
								start = end = len;
							} else {
								start = -textInputRange.moveStart("character", -len);
								start += normalizedValue.slice(0, start).split("\n").length - 1;

								if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
									end = len;
								} else {
									end = -textInputRange.moveEnd("character", -len);
									end += normalizedValue.slice(0, end).split("\n").length - 1;
								}
							}
						}
					}

					return {
						start: start,
						end: end
					};						
				};

				var offsetToRangeCharacterMove = function (el, offset) {
					return offset - (el.value.slice(0, offset).split("\r\n").length - 1);
				};
					
				var setInputSelection = function (el, startOffset, endOffset) {
					el.focus();
					if (typeof el.selectionStart == "number" && typeof el.selectionEnd == "number") {
						el.selectionStart = startOffset;
						el.selectionEnd = endOffset;
					} else {
						var range = el.createTextRange();
						var startCharMove = offsetToRangeCharacterMove(el, startOffset);
						range.collapse(true);
						if (startOffset == endOffset) {
							range.move("character", startCharMove);
						} else {
							range.moveEnd("character", offsetToRangeCharacterMove(el, endOffset));
							range.moveStart("character", startCharMove);
						}
						range.select();
					}
				}

				var sel = getInputSelection($this[0]), val = $this.val();
				$this.val(val.slice(0, sel.start) + transformedChar + val.slice(sel.end));

				// Move the caret
				setInputSelection($this[0], sel.start + 1, sel.start + 1);
				return false;
			}
		}			
	},

	checkValidKey: function ($this, e) {
		// is not called for special keys - // page up, page down, home, end, insert, delete, arrows keys (except FF)
		
		if (e.ctrlKey || e.altKey || e.metaKey) {
			return;
		}

		if ((e.charCode == 0) && (e.keyCode != 0)) {
			// special keys in FF
			return;
		}

		var dateSeparator = $this.attr("data-havitdatetimebox-dateseparator");
		var timeSeparator = $this.attr("data-havitdatetimebox-timeseparator");
		
		var charCode = e.charCode ? e.charCode : e.keyCode;
		if (charCode) {
			if ((charCode < 32)
				|| ((charCode >= 48) && (charCode <= 57)) // 0-9
				|| ((charCode == 32) || (charCode == 160)) // space					
				|| (dateSeparator && (String.fromCharCode(charCode) == dateSeparator))
				|| (timeSeparator && (String.fromCharCode(charCode) == timeSeparator))
			) {
				// valid character
			} else {
				// invalid character
				e.preventDefault();
			}
		}
	}
	
};

var havitGridViewExtensions = {
	setExternalEditorEditedRow: function(gridviewID, rowIndex, cssClass) {
		$('#' + gridviewID + ' > tbody > tr.' + cssClass).removeClass(cssClass);
		if (rowIndex >= 0) {
			$('#' + gridviewID + ' > tbody > tr:nth(' + rowIndex + ')').addClass(cssClass);
		}
	}
};

$(document).ready(havitDateTimeBoxExtensions.init);
$(document).ready(havitNumericBoxExtensions.init);

var havitBrowserNavigationControllerExtension = {
	startup: function(backUrl) {
		if (window.history && window.history.replaceState) {
			var href = window.location.href;
			window.history.replaceState(null, document.title, href + "#!/back");
			window.history.pushState(null, document.title, href);

			$(window).off("popstate.hfw");
			$(window).on("popstate.hfw", function() {
				if (window.location.hash === "#!/back") {
					window.history.replaceState(null, document.title, href);
					window.setTimeout(function() {
						window.location.replace((backUrl.length > 0) ? backUrl : href);
					}, 0);
				}
			});
		}
	}
};