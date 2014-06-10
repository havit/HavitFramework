var Havit_BootstrapExtensions_ResizeModal = function() { };

if (!window.jQuery) {
	alert('Modals.js: jQuery must be loaded prior to WebUIValidationExtension.js.');
} else {
	(function ($) {
		var havit_BootstrapExtensions_TimerID = null;
		var havit_BootstrapExtensions_HeaderTop = null;
		var havit_BootstrapExtensions_CurrentDialog = null;
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

				if (havit_BootstrapExtensions_HeaderTop == null) {
					var containerHeight = $modal.innerHeight();
					var headerHeight = $modalHeader.outerHeight(true) || 0;
					var footerHeight = $modalFooter.outerHeight(true) || 0;
					var headerTop = ($modalHeader.length > 0) ? $modalHeader.offset().top : $modalBody.offset().top;
					havit_BootstrapExtensions_HeaderTop = headerTop;
				}

				var bodyHeight = containerHeight - headerHeight - footerHeight - (2 * havit_BootstrapExtensions_HeaderTop);

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

		havit_BootstrapExtensions_ShowHideModal = function($element, operation, suppressAnimation) {
			var hasFade = $element.hasClass('fade');
			if (suppressAnimation && hasFade) {
				$element.removeClass('fade');
			}

			// if we want to hide modal without animation, we have to remove fade class from backdrop
			if ((operation == 'hide') && suppressAnimation) {
				$('.modal-backdrop').removeClass('fade');
			}

			$element.modal(operation);

			// if we shown modal without animation, but animation support is enabled for modal, we must add fade class to backdrop
			// show operation set fade class to backdrop if dialog supports animation
			if ((operation == 'show') && suppressAnimation && hasFade) {
				$('.modal-backdrop').addClass('fade');
			}

			if (suppressAnimation && hasFade) {
				$element.addClass('fade');
			}
		}

		havit_BootstrapExtensions_Show = function (elementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, suppressAnimation) {
			var $element = $(elementSelector);

			havit_BootstrapExtensions_HeaderTop = null;
			havit_BootstrapExtensions_ShowHideModal($element, 'show', suppressAnimation);

			if (closeOnEscapeKey) {
				$('body').on('keyup.havit.web.bootstrap', function(e) {
					if (e.which == 27) {
						eval(escapePostbackScript);
					}
				});
			}

			if(dragMode == "Required") {
				$(elementSelector + ' .modal-dialog').draggable({ handle : '.modal-header' });
			}

			if (dragMode == "IfAvailable") {
				if (!!window.jQuery.ui && !!window.jQuery.ui.version) {
					$(elementSelector + ' .modal-dialog').draggable({ handle: '.modal-header' });
				} 
			}

			havit_BootstrapExtensions_CurrentDialog = $element;
		}

		Havit_BootstrapExtensions_Show = function (elementSelector, closeOnEscapeKey, escapePostbackScript, dragMode) {
			// shows dialog
			// no matters how, when..., just show modal dialog
			havit_BootstrapExtensions_Show(elementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, false);
		}

		Havit_BootstrapExtensions_RemainShown = function (elementSelector, closeOnEscapeKey, escapePostbackScript, dragMode) {
			var $element = $(elementSelector);
			
			if (havit_BootstrapExtensions_CurrentDialog == null) {
				// when result of postback, havit_BootstrapExtensions_CurrentDialog is null (whole page refreshed)
				// We have to show dialog again, but without animation.
				havit_BootstrapExtensions_Show(elementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, true);
			} else {
				// in asynchronous postback, whole dialog can be updated (when there is a parent UpdatePanel) or just content of dialog is refreshed (no parent UpdatePanel updated)
				// when no parent UpdatePanel, havit_BootstrapExtensions_CurrentDialog and $element are still the same instances
				if ($element.is(havit_BootstrapExtensions_CurrentDialog)) {
					// no parent update panel, modal dialog element has not been replaced so no action required
					// NOOP
				} else {
					// parent UpdatePanel updated, we have to hide previous dialog to remove backdrop and to show current dialog
					// both actions must suppress animation (no animation required)			
					havit_BootstrapExtensions_ShowHideModal(havit_BootstrapExtensions_CurrentDialog, 'hide', true);
					havit_BootstrapExtensions_Show(elementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, true);
				}
			}
		}

		Havit_BootstrapExtensions_Hide = function (elementSelector) {
			var $element = $(elementSelector);

			if (havit_BootstrapExtensions_CurrentDialog == null) {
				// when result of postback, havit_BootstrapExtensions_CurrentDialog is null (whole page refreshed)
				if ($element.hasClass('fade')) {
					// if animation enabled, we have to show modal without animation and hide it with animation					
					havit_BootstrapExtensions_ShowHideModal($element, 'show', true);
					havit_BootstrapExtensions_ShowHideModal($element, 'hide', false);
				} else {
					// it there is no animation
					// NOOP
				}
			} else {
				// in asynchronous postback, whole dialog can be updated (when there is a parent UpdatePanel) or just content of dialog is refreshed (no parent UpdatePanel updated)
				// when no parent UpdatePanel, havit_BootstrapExtensions_CurrentDialog and $element are still the same instances
				if ($element.is(havit_BootstrapExtensions_CurrentDialog)) {
					// no parent update panel, modal dialog element has not been replaced so just hide it
					havit_BootstrapExtensions_ShowHideModal($element, 'hide', false);
				} else {
					// parent UpdatePanel updated, we have to hide previous dialog to remove backdrop and to show current dialog without animation and hide it
					havit_BootstrapExtensions_ShowHideModal(havit_BootstrapExtensions_CurrentDialog, 'hide', true);
					if ($element.hasClass('fade')) {
						havit_BootstrapExtensions_ShowHideModal($element, 'show', true);
						havit_BootstrapExtensions_ShowHideModal($element, 'hide', false);
					}
				}
			}
			
			$('body').off('keyup.havit.web.bootstrap');

			if (!!window.jQuery.ui && !!window.jQuery.ui.version && (havit_BootstrapExtensions_CurrentDialog != null)) {
				havit_BootstrapExtensions_CurrentDialog.find(".modal-dialog.ui-draggable").draggable('destroy');
			}

			havit_BootstrapExtensions_CurrentDialog = null;
		}

		$(document).on("shown.bs.modal", "div.modal", havit_BootstrapExtensions_ResizeModal);
		$(document).on("hide.bs.modal", "div.modal", havit_BootstrapExtensions_ClearTimer);
	})(jQuery);
}
