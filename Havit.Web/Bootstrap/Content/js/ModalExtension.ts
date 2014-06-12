/// <reference path="../../../scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../../scripts/typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../../scripts/typings/bootstrap/bootstrap.d.ts" />

module Havit.Web.Bootstrap.UI.WebControls.ClientSide {
    export class ModalExtension {

        // #region Instance (singleton)
        public static instance(): ModalExtension {
            if (this._instance == null) {
                this._instance = new ModalExtension();
            }
            return this._instance;
        }
        private static _instance: ModalExtension;
        // #endregion

        private resizingTimer: number;
        private currentModalElement: JQuery;

        constructor() {
        }

        public show(modalElementSelector: string, closeOnEscapeKey: boolean, escapePostbackScript: string, dragMode: string) {
            // shows dialog
            // no matters how, when..., just show modal dialog
            this.currentModalElement = this.showInternal(modalElementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, false);
        }

        public remainShown(modalElementSelector: string, closeOnEscapeKey: boolean, escapePostbackScript: string, dragMode: string) {            
            var $modalElement = $(modalElementSelector);

            if (this.currentModalElement == null) {
                // when result of postback, havit_BootstrapExtensions_CurrentDialog is null (whole page refreshed)
                // We have to show dialog again, but without animation.
                this.currentModalElement = this.showInternal(modalElementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, true);
            } else {
                // in asynchronous postback, whole dialog can be updated (when there is a parent UpdatePanel) or just content of dialog is refreshed (no parent UpdatePanel updated)
                // when no parent UpdatePanel, currentModalElement and $element are still the same instances
                if ($modalElement.is(this.currentModalElement)) {
                    // no parent update panel, modal dialog element has not been replaced so no action required
                    // NOOP
                } else {
                    // parent UpdatePanel updated, we have to hide previous dialog to remove backdrop and to show current dialog
                    // both actions must suppress animation (no animation required)			
                    this.showHideModalInternal(this.currentModalElement, 'hide', true);
                    this.currentModalElement = this.showInternal(modalElementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, true);
                }
            }
        }

        public hide(modalElementSelector: string) {
            this.stopResizingProcess();

            var $modalElement = $(modalElementSelector);
            if (this.currentModalElement == null) {
                // when result of postback, currentModalElement is null (whole page refreshed)
                if ($modalElement.hasClass('fade')) {
                    // if animation enabled, we have to show modal without animation and hide it with animation					
                    this.showHideModalInternal($modalElement, 'show', true);
                    this.showHideModalInternal($modalElement, 'show', true);
                    this.showHideModalInternal($modalElement, 'hide', false);
                } else {
                    // it there is no animation
                    // NOOP
                }
            } else {
                // in asynchronous postback, whole dialog can be updated (when there is a parent UpdatePanel) or just content of dialog is refreshed (no parent UpdatePanel updated)
                // when no parent UpdatePanel, currentModalElement and $element are still the same instances
                if ($modalElement.is(this.currentModalElement)) {
                    // no parent update panel, modal dialog element has not been replaced so just hide it
                    this.showHideModalInternal($modalElement, 'hide', false);
                } else {
                    // parent UpdatePanel updated, we have to hide previous dialog to remove backdrop and to show current dialog without animation and hide it
                    this.showHideModalInternal(this.currentModalElement, 'hide', true);
                    if ($modalElement.hasClass('fade')) {
                        this.showHideModalInternal($modalElement, 'show', true);
                        this.showHideModalInternal($modalElement, 'hide', false);
                    }
                }
            }

            $('body').off('keyup.havit.web.bootstrap');

            if (!!jQuery.ui && !!jQuery.ui.version && (this.currentModalElement != null)) {
                this.currentModalElement.find(".modal-dialog.ui-draggable").draggable('destroy');
            }

            this.currentModalElement = null;
        }

        private showInternal(modalElementSelector: string, closeOnEscapeKey: boolean, escapePostbackScript: string, dragMode: string, suppressAnimation: boolean) {
            var $modalElement = $(modalElementSelector);

            this.showHideModalInternal($modalElement, 'show', suppressAnimation);

            if (closeOnEscapeKey) {
                $('body').on('keyup.havit.web.bootstrap', function (e) {
                    if (e.which == 27) {
                        eval(escapePostbackScript);
                    }
                });
            }
            
            var draggableParams = { handle: '.modal-header', stop: () => this.dragStop.call(this) };
            if (dragMode == "Required") {
                $(modalElementSelector + ' .modal-dialog').draggable(draggableParams);
            }

            if (dragMode == "IfAvailable") {
                if (!!jQuery.ui && !!jQuery.ui.version) {
                    $(modalElementSelector + ' .modal-dialog').draggable(draggableParams);
                }
            }

            this.startResizingProcess();

            return $modalElement;
        }

        private showHideModalInternal($modalElement: JQuery, operation: string, suppressAnimation: boolean) {
            var hasFade = $modalElement.hasClass('fade');
            if (suppressAnimation && hasFade) {
                $modalElement.removeClass('fade');
            }

            // if we want to hide modal without animation, we have to remove fade class from backdrop
            if ((operation == 'hide') && suppressAnimation) {
                $('.modal-backdrop').removeClass('fade');
            }

            $modalElement.modal(operation);

            // if we shown modal without animation, but animation support is enabled for modal, we must add fade class to backdrop
            // show operation set fade class to backdrop if dialog supports animation
            if ((operation == 'show') && suppressAnimation && hasFade) {
                $('.modal-backdrop').addClass('fade');
            }

            if (suppressAnimation && hasFade) {
                $modalElement.addClass('fade');
            }
        }

        private dragStop(event: Event, ui: JQueryUI.DraggableEventUIParams) {
            
            this.stopResizingProcess();
        }

        private startResizingProcess() {
            if (this.resizingTimer == null) {
                this.processResizingProcess();
            }
        }

        private stopResizingProcess() {
            if (this.resizingTimer != null) {
                window.clearTimeout(this.resizingTimer);
                this.resizingTimer = null;
            }
        }

        private processResizingProcess() {
            if (this.currentModalElement != null) {
                var $modal = this.currentModalElement;
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
                    bodyHeightPx = bodyHeight + "px";
                }

                $modalBody.css("max-height", bodyHeightPx);
            }

            this.resizingTimer = window.setTimeout(() => this.processResizingProcess.call(this), 200); // this is a workaround for setting height in transitions/animations where setting value once at shown.bs.modal fails
        } 
    }
}