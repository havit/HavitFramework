var havitNumericBoxExtensions = {
    init: function (settings) {
        // Initializes HAVIT numeric box extensions
        $("body").on("click", "input[data-havitnumericbox]", havitNumericBoxExtensions.onclick); // hook click event on date time boxes
        $("body").on("blur", "input[data-havitnumericbox]", havitNumericBoxExtensions.onblur); // hook blur event on date time boxes
        $("body").on("change", "input[data-havitnumericbox]", havitNumericBoxExtensions.onchange); // hook click event on date time boxes
        $("body").on("keypress", "input[data-havitnumericbox]:not([data-havitnumericbox-suppressfilterkey])", havitNumericBoxExtensions.onkeypress); // hook keypress event on date time boxes
    },
    onclick: function () {
        var $this = $(this);
        if ($this.data("havitnumericbox-selectonclick")) {
            if ($this.data("havit-select-suppressed") == undefined) {
                $this.select(); // select content of textbox
                $this.data("havit-select-suppressed", true); // enable just one selection per one focus (otherwise not possible to click to certain position)
            }
        }
    },
    onblur: function () {
        var $this = $(this);
        $this.removeData("havit-select-suppressed");
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
                || ((allowNegative == "true") && (String.fromCharCode(charCode) == "-"))) {
            }
            else {
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
        $("body").on("blur", "input[data-havitdatetimebox]", havitDateTimeBoxExtensions.onblur); // hook blur event on date time boxes
        $("body").on("change", "input[data-havitdatetimebox]", havitDateTimeBoxExtensions.onchange); // hook change event on date time boxes
        $("body").on("keypress", "input[data-havitdatetimebox]:not([data-havitdatetimebox-suppressfilterkey])", havitDateTimeBoxExtensions.onkeypress); // hook keypress event on date time boxes
    },
    onclick: function () {
        var $this = $(this);
        if ($this.data("havitdatetimebox-selectonclick")) {
            if ($this.data("havit-select-suppressed") == undefined) {
                $this.select(); // select content of textbox
                $this.data("havit-select-suppressed", true); // enable just one selection per one focus (otherwise not possible to click to certain position)
            }
        }
    },
    onblur: function () {
        var $this = $(this);
        $this.removeData("havit-select-suppressed");
    },
    onchange: function (e) {
        havitDateTimeBoxExtensions.autoCompleteDate($(this), e); // autocompletes date
    },
    onkeypress: function (e) {
        havitDateTimeBoxExtensions.replaceComma($(this), e); // transforms "," to "." while typing
        havitDateTimeBoxExtensions.checkValidKey($(this), e); // checks if pressed valid key
    },
    autoCompleteDate: function ($this, e) {
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
        var patterns = new Array(new Array(regDen + '(\\' + datumSeparator + ')?', '\$1' + datumSeparator + datumMesic + datumSeparator + datumRok), new Array(regDen + '(\\' + datumSeparator + ')' + regMesic + '(\\' + datumSeparator + ')?', '\$1' + datumSeparator + '\$3' + datumSeparator + datumRok), new Array(regDenCely + regMesicCely + '(\\' + datumSeparator + ')?', '\$1' + datumSeparator + '\$2' + datumSeparator + datumRok), new Array(regDenCely + '(\\' + datumSeparator + ')?' + regMesicCely + '(\\' + datumSeparator + ')?' + regRok, '\$1' + datumSeparator + '\$3' + datumSeparator + '\$5'));
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
    replaceComma: function ($this, e) {
        // zdroj: http://jsfiddle.net/EXH2k/6/
        var dateSeparator = $this.attr("data-havitdatetimebox-dateseparator");
        if (e.which && dateSeparator) {
            var charStr = String.fromCharCode(e.which);
            var transformedChar = (((dateSeparator == ".") && (charStr == ",")) || ((dateSeparator != ".") && (charStr == "."))) ? dateSeparator : charStr;
            if (transformedChar != charStr) {
                var getInputSelection = function (el) {
                    var start = 0, end = 0, normalizedValue, range, textInputRange, len, endRange;
                    if (typeof el.selectionStart == "number" && typeof el.selectionEnd == "number") {
                        start = el.selectionStart;
                        end = el.selectionEnd;
                    }
                    else {
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
                            }
                            else {
                                start = -textInputRange.moveStart("character", -len);
                                start += normalizedValue.slice(0, start).split("\n").length - 1;
                                if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
                                    end = len;
                                }
                                else {
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
                    }
                    else {
                        var range = el.createTextRange();
                        var startCharMove = offsetToRangeCharacterMove(el, startOffset);
                        range.collapse(true);
                        if (startOffset == endOffset) {
                            range.move("character", startCharMove);
                        }
                        else {
                            range.moveEnd("character", offsetToRangeCharacterMove(el, endOffset));
                            range.moveStart("character", startCharMove);
                        }
                        range.select();
                    }
                };
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
                || (timeSeparator && (String.fromCharCode(charCode) == timeSeparator))) {
            }
            else {
                // invalid character
                e.preventDefault();
            }
        }
    }
};
var havitGridViewExtensions = {
    initializeRowClick: function (gridviewID) {
        $('#' + gridviewID).off('click.havit.web');
        $('#' + gridviewID).on('click.havit.web', "[data-rowclick]", havitGridViewExtensions.handleRowClick);
    },
    handleRowClick: function (e) {
        var $element = $(e.target);
        while ($element != null) {
            $element = $element.not(":input").not("[href]").not("[data-suppressrowclick]");
            if ($element.length == 0) {
                break;
            }
            var rowClick = $element.data("rowclick");
            if (rowClick) {
                e.stopPropagation();
                window.setTimeout(function () {
                    eval(rowClick);
                }, 1);
                break;
            }
            $element = $element.parent();
        }
        ;
    },
    setExternalEditorEditedRow: function (gridviewID, rowIndex, cssClass) {
        $('#' + gridviewID + ' > tbody > tr.' + cssClass).removeClass(cssClass);
        if (rowIndex >= 0) {
            $('#' + gridviewID + ' > tbody > tr:nth(' + rowIndex + ')').addClass(cssClass);
        }
    }
};
var havitDropDownCheckBoxListExtensions = {
    init: function () {
        $("select[data-dropdowncheckboxlist]").each(function (index, item) {
            var $item = $(item);
            if ($item.data("multipleSelect")) {
                return;
            }
            var isOpen = $item.data("dropdowncheckboxlist-isopen") || false;
            var selectAll = $item.data("dropdowncheckboxlist-showselectall") || false;
            var selectAllText = $item.data("dropdowncheckboxlist-selectalltext") || false;
            var allSelected = $item.data("dropdowncheckboxlist-allselectedtext") || false;
            var placeholder = $item.data("dropdowncheckboxlist-placeholder") || '';
            var width = $item.data("dropdowncheckboxlist-width") || ($(item).width() + 24 /* šířka checkboxu */);
            var multiple = $item.data("dropdowncheckboxlist-itemwidth") ? true : false;
            var multipleWidth = $item.data("dropdowncheckboxlist-itemwidth") || 0;
            var single = $item.data("dropdowncheckboxlist-single") ? true : false;
            var filter = $item.data("dropdowncheckboxlist-filter") ? true : false;
	        var noMatchesFound = $item.data("dropdowncheckboxlist-nomatchesfound");

            var multipleSelectParams = {
                isOpen: isOpen,
                selectAll: selectAll,
                allSelected: allSelected,
                placeholder: placeholder,
                width: width,
                single: single,
                multiple: multiple,
                multipleWidth: multipleWidth,
                countSelected: false,
                filter: filter
            };

            if (selectAllText) {
                multipleSelectParams.selectAllText = selectAllText;
                multipleSelectParams.selectAllDelimiter = ['', ''];
            }

            if (noMatchesFound) {
                multipleSelectParams.noMatchesFound = noMatchesFound;
            }

            var onclickscript = $item.data("dropdowncheckboxlist-onclickscript");
            var onblurscript = $item.data("dropdowncheckboxlist-onblurscript");
            var onopenscript = $item.data("dropdowncheckboxlist-onopenscript");
            var onclosescript = $item.data("dropdowncheckboxlist-onclosescript");
            if (onclickscript) {
                multipleSelectParams.onClick = function (view) { eval(onclickscript); };
            }
            if (onblurscript) {
                multipleSelectParams.onBlur = function (view) { eval(onblurscript); };
            }
            if (onopenscript) {
                multipleSelectParams.onOpen = function (view) { eval(onopenscript); };
            }
            if (onclosescript) {
                multipleSelectParams.onClose = function (view) { eval(onclosescript); };
            }
            $(item).multipleSelect(multipleSelectParams);
        });
    }
};
var havitBrowserNavigationControllerExtension = {
    startup: function (backUrl) {
        if (window.history && window.history.replaceState) {
            var href = window.location.href;
            window.history.replaceState(null, document.title, href + "#!/back");
            window.history.pushState(null, document.title, href);
            $(window).off("popstate.hfw");
            $(window).on("popstate.hfw", function () {
                if (window.location.hash === "#!/back") {
                    window.history.replaceState(null, document.title, href);
                    window.setTimeout(function () {
                        window.location.replace((backUrl.length > 0) ? backUrl : href);
                    }, 0);
                }
            });
        }
    }
};
var havitAutoCompleteTextBoxExtensions = {
    init: function () {
        $("span[data-autocompletetextbox]").each(function (index, item) {
            var $item = $(item);
            var serviceurl = $item.data("serviceurl");
            var minchars = ($item.data("minchars") != undefined) ? $item.data("minchars") : 1;
            var deferrequest = $item.data("deferrequest") || 0;
            var nocache = $item.data("nocache") || false;
            var maxheight = $item.data("maxheight") || 300;
            var queryParams = $item.data("params") || {};
            var orientation = $item.data("orientation") || "bottom";
            var showNoSuggestionNotice = ($item.data("shownosuggestionnotice") == "True");
            var noSuggestionNotice = $item.data("nosuggestionnotice");
            var options = {
                type: "GET",
                serviceUrl: serviceurl,
                minChars: minchars,
                deferRequestBy: deferrequest,
                noCache: nocache,
                params: queryParams,
                maxHeight: maxheight,
                orientation: orientation,
                showNoSuggestionNotice: showNoSuggestionNotice,
                noSuggestionNotice: noSuggestionNotice,
                preserveInput: true,
                triggerSelectOnValidInput: false,
                onSelect: function (suggestion) {
                    havitAutoCompleteTextBoxExtensions.onSelect(suggestion, $item);
                }
            };
            var $textbox = $item.children("input[type='text']");
            var $hiddenfield = $item.children("input[type='hidden']");
            var $clearTextLink = $item.children("a[data-clearText]");
            $item.data("selectedvalue", $textbox.val());
            $textbox.blur(function () { havitAutoCompleteTextBoxExtensions.onBlur($textbox, $hiddenfield, $item); });
            $textbox.autocomplete(options);
            $clearTextLink.click(function (event) {
                event.preventDefault();
                havitAutoCompleteTextBoxExtensions.onClickClearTextLink($textbox, $hiddenfield, $item);
                $clearTextLink.hide();
            });
            $clearTextLink.css({
                "color": $textbox.css("color")
            });

            if ($textbox.val() == "") {
                $clearTextLink.hide();
            }
            else {
                $clearTextLink.show();
            }
        });
    },
    onSelect: function (suggestion, item) {
        var $item = $(item);
        var $hiddenfield = $item.children("input[type='hidden']");
        var $textbox = $item.children("input[type='text']");
        $hiddenfield.val(suggestion.data);
        $textbox.val(suggestion.value);
        $item.data("selectedvalue", suggestion.value);
        // zrušíme nastavený timer, který vznikl při onBlur textboxu
        var timerId = $textbox.data("timerId");
        if (timerId != undefined) {
            clearTimeout(timerId);
            $textbox.data("timerId", undefined);
        }
        var $clearTextLink = $item.children("a[data-clearText]");
        var onselectscript = $item.data("onselectscript");
        havitAutoCompleteTextBoxExtensions.fireOnSelectScriptEvent(suggestion, onselectscript);
        var postbackScript = $item.data("postbackscript");
        if (postbackScript != undefined) {
            havitAutoCompleteTextBoxExtensions.doDefferedPostback.call(window, postbackScript);
        }

        if (suggestion.value) {
            $clearTextLink.show();
        }
        else {
            $clearTextLink.hide();
        }
    },
    onBlur: function (textbox, hiddenfield, item) {
        var $item = $(item);
        var selectedvalue = $item.data("selectedvalue");
        var $textbox = $(textbox);
        var $hiddenfield = $(hiddenfield);
        var allowInvalidSelection = $(item).data("allowinvalidselection") == 'True';
        var nullable = $(item).data("nullable") == 'True';
        var $clearTextLink = $item.children("a[data-clearText]");
        var postbackScript = $item.data("postbackscript");
        if (!allowInvalidSelection) {
            // pokud není povolený nevalidní výběr, provedeme validaci
            if ($textbox.val() != selectedvalue) {
                $textbox.val("");
                $hiddenfield.val("");
                $item.data("selectedvalue", "");
                var onselectscript = $item.data("onselectscript");
                if (onselectscript != undefined) {
                    var suggestion = {
                        data: "",
                        value: ""
                    };
                    // metoda se volá odloženě, protože může nastat volání metody onSelect při výběru myší s nabídnutých položek a v takovém případě se volání ruší
                    var timerId = setTimeout(havitAutoCompleteTextBoxExtensions.fireOnSelectScriptEvent, 120, suggestion, onselectscript);
                    $textbox.data("timerId", timerId);
                }
                //pokud došlo ke smazání hodnoty (validní) a je povolena null hodnota, provedeme postback
                if ($textbox.val() == "" && postbackScript != undefined && nullable) {
                    havitAutoCompleteTextBoxExtensions.doDefferedPostback.call(window, postbackScript);
                }

                if ($textbox.val() == "") {
                    $clearTextLink.hide();
                }
            }
        }
        else {
            if (selectedvalue != $textbox.val()) {
                $item.data("selectedvalue", $textbox.val());
                $hiddenfield.val("");
                // pokud je povolený nevalidní výběr a je nastavený autopostback, provedeme ho
                if (postbackScript != undefined) {
                    havitAutoCompleteTextBoxExtensions.doDefferedPostback.call(window, postbackScript);
                }

                if ($textbox.val() != "") {
                    $clearTextLink.show();
                }
                else {
                    $clearTextLink.hide();
                }
            }
        }
    },
    fireOnSelectScriptEvent: function (suggestion, onselectscript) {
        if (onselectscript) {
            if (!havitAutoCompleteTextBoxExtensions.doOnSelectScript(suggestion, onselectscript)) {
                return;
            }
        }
    },
    doOnSelectScript: function (suggestion, onselectscript) {
        return eval(onselectscript) || true;
    },
    doDefferedPostback: function (script) {
        setTimeout(havitAutoCompleteTextBoxExtensions.doPostback, 120, script);
    },
    doPostback: function (script) {
        eval(script);
    },
    onClickClearTextLink: function ($textbox, $hiddenfield, $item) {
        $textbox.val("");
        havitAutoCompleteTextBoxExtensions.onBlur($textbox, $hiddenfield, $item);
    }
};
var havitControlFocusPersisterExtensions = {
    init: function () {
        var lastFocusPersister = $("#_lastFocusPersister");
        var lastFocusedControl = $("#" + lastFocusPersister.val());
        if (lastFocusedControl != null) {
            try {
                lastFocusedControl.focus();
            }
            catch (ex) { }
            ;
        }
        $("body")
            .off("focus.hfw", ":focusable")
            .on("focus.hfw", ":focusable", function () {
            var lastFocusPersister = $("#_lastFocusPersister");
            if (lastFocusPersister != null) {
                lastFocusPersister.val(this.id);
            }
        });
    }
};
$(document).ready(havitDateTimeBoxExtensions.init);
$(document).ready(havitNumericBoxExtensions.init);
