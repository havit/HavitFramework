var Havit_BootstrapExtensions_ResizeModal = function() { };

if (!window.jQuery) {
	alert('Modals.js: jQuery must be loaded prior to WebUIValidationExtension.js.');
} else {
	(function($) {
		Havit_BootstrapExtensions_ResizeModal = function() {

			$('.modal:visible').each(function(modalIndex, modal) {
				var $modal = $(modal);
				var $modalDialog = $modal.children(".modal-dialog");
				var $modalContent = $modalDialog.children(".modal-content");
				var $modalHeader = $modalContent.children().children(".modal-header");
				var $modalBody = $modalContent.children().children(".modal-body");
				var $modalFooter = $modalContent.children().children(".modal-footer");

				var containerHeight = $modal.innerHeight();
				var headerHeight = $modalHeader.outerHeight(true) || 0;
				var footerHeight = $modalFooter.outerHeight(true) || 0;
				var headerTop = ($modalHeader.length > 0) ? $modalHeader.offset().top : $modalBody.offset().top;

				var bodyHeight = containerHeight - headerHeight - footerHeight - (2 * headerTop);

				if (bodyHeight < 200) { /* if less then 200 px then switch to standard behavior - scroll dialog content with page scroller */
					$modalBody.css("max-height", "");
				} else {
					$modalBody.css("max-height", bodyHeight + "px");
				}
			});
		};

		$(window).resize(Havit_BootstrapExtensions_ResizeModal);
		$(document).on("shown.bs.modal", "div.modal", Havit_BootstrapExtensions_ResizeModal);
	})(jQuery);
}
