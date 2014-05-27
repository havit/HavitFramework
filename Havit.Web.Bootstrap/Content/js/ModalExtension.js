var Havit_BootstrapExtensions_ResizeModal = function() { };

if (!window.jQuery) {
	alert('Modals.js: jQuery must be loaded prior to WebUIValidationExtension.js.');
} else {
	(function ($) {
		var havit_BootstrapExtensions_TimerID = null;
		var havit_BootstrapExtensions_ResizeModal = function() {

			$('.modal').each(function(modalIndex, modal) {
				var $modal = $(modal);
				var $modalDialog = $modal.children(".modal-dialog");
				var $modalContent = $modalDialog.children(".modal-content");
				var $modalHeader = $modalContent.children().children(".modal-header");
				var $modalBody = $modalContent.children().children(".modal-body");
				var $modalFooter = $modalContent.children().children(".modal-footer");

				if (($modalHeader.length == 0) && ($modalBody.length == 0)) {
					// ModalDialog is not visible (rendered)
					return;
				}

				var containerHeight = $modal.innerHeight();
				var headerHeight = $modalHeader.outerHeight(true) || 0;
				var footerHeight = $modalFooter.outerHeight(true) || 0;
				var headerTop = ($modalHeader.length > 0) ? $modalHeader.offset().top : $modalBody.offset().top;

				var bodyHeight = containerHeight - headerHeight - footerHeight - (2 * headerTop);

				var bodyHeightPx = "";
				if (bodyHeight >= 200) { /* if less then 200 px then switch to standard behavior - scroll dialog content with page scroller */
					var bodyHeightPx = bodyHeight + "px";
				}
				$modalBody.css("max-height", bodyHeightPx);
			});
			havit_BootstrapExtensions_TimerID = window.setTimeout(havit_BootstrapExtensions_ResizeModal, 200); // this is a workaround for setting height in transitions/animations where setting value once at shown.bs.modal fails
		};

		havit_BootstrapExtensions_ClearTimer = function () {
			if (havit_BootstrapExtensions_TimerID != null) {
				window.clearTimeout(havit_BootstrapExtensions_TimerID);
				havit_BootstrapExtensions_TimerID = null;
			}
		}

		//$(window).resize(Havit_BootstrapExtensions_ResizeModal);
		$(document).on("shown.bs.modal", "div.modal", havit_BootstrapExtensions_ResizeModal);
		$(document).on("hide.bs.modal", "div.modal", havit_BootstrapExtensions_ClearTimer);
	})(jQuery);
}
