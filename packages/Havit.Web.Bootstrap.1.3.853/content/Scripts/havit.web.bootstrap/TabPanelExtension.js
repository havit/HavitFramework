if (!window.jQuery) {
	alert('WebUIValidationExtension.js: jQuery must be loaded prior to WebUIValidationExtension.js.');
} else {
	(function ($) {
		$(document).on('click', '[data-toggle="tab.havit"]', function (e) {
			e.preventDefault(); // do not navigate

			// remember active tab - find closest element with data-tab-persister, take the attribute value and set value to the element with this selector
			$($(this).closest('[data-tab-persister]').attr('data-tab-persister')).val($(this).attr('href'));

			// set active tab
			$(this).tab('show');
			$(this).blur();

		});
	})(jQuery);
}
