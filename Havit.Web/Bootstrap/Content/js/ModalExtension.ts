/// <reference path="../../../scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../../scripts/typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../../scripts/typings/bootstrap/bootstrap.d.ts" />

module Havit.Web.Bootstrap.UI.WebControls.ClientSide {
    export class ModalExtension {
        private currentStateStorageKey = "Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.CurrentState";

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
        private currentState: ModalDialogState;

        constructor() {
        }

        public show(modalElementSelector: string, closeOnEscapeKey: boolean, escapePostbackScript: string, dragMode: string) {
            // shows dialog
            // no matters how, when..., just show modal dialog
            this.currentState = new ModalDialogState(); // new dialog -> new style
            ModalDialogStatePersister.trySaveState(this.currentStateStorageKey, this.currentState); // ensures that remainShow sees the same state

            this.currentModalElement = this.showInternal(modalElementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, false);
        }

        public remainShown(modalElementSelector: string, closeOnEscapeKey: boolean, escapePostbackScript: string, dragMode: string) {
            // ensures dialog is open after postback or asynchronous postback
            this.currentState = ModalDialogStatePersister.loadState(this.currentStateStorageKey);
            if (this.currentState != null) {                
                this.initializeState($(modalElementSelector));
            } else {
                this.currentState = new ModalDialogState();
            }

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
            ModalDialogStatePersister.deleteState(this.currentStateStorageKey);
            this.currentState = null;

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

            // do not listen to keyup anymore
            $('body').off('keyup.havit.web.bootstrap');

            // clear grad mode if it is on
            if (!!jQuery.ui && !!jQuery.ui.version && (this.currentModalElement != null)) {
                this.currentModalElement.find(".modal-dialog.ui-draggable").draggable('destroy');
            }

            this.currentModalElement = null;
        }

        private showInternal(modalElementSelector: string, closeOnEscapeKey: boolean, escapePostbackScript: string, dragMode: string, suppressAnimation: boolean) {
            var $modalElement = $(modalElementSelector);

            // show modal
            this.showHideModalInternal($modalElement, 'show', suppressAnimation);

            // listen to keyup to catch escape key
            if (closeOnEscapeKey) {
                $('body').on('keyup.havit.web.bootstrap', (e) => {
                    if (e.which == 27) {
                        eval(escapePostbackScript);
                    }
                });
            }

            // switch on drag mode if enabled
            var draggableParams = { handle: ' .modal-header', drag: (e, ui) => this.drag.call(this, e, ui) };
            if (dragMode == "Required") {
                $modalElement.find('.modal-dialog').draggable(draggableParams);
            }

            if (dragMode == "IfAvailable") {
                if (!!jQuery.ui && !!jQuery.ui.version) {
                    $modalElement.find('.modal-dialog').draggable(draggableParams);
                }
            }

            if (this.currentState.modalPosition == null) {
                // start resizing process if modal dialog not moved
                this.startResizingProcess();
            }

            // listen do scroll events to persist state between postbacks
            $(modalElementSelector).find('.modal-body').on("scroll", () => this.bodyScroll.call(this));
            $(modalElementSelector).on("scroll", () => this.modalScroll.call(this));
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

        private initializeState($modalElement: JQuery) {

            if (this.currentState.bodyMaxHeightPx) {
                // restores body max-height value (height of modal body)
                $modalElement.find(".modal-body").css("max-height", this.currentState.bodyMaxHeightPx);
            }

            if (this.currentState.bodyScrollPosition) {
                // restores body scroll position (scroll position of modal body)
                if (this.currentModalElement != null) {
                    // if updated only nested UpdatePanel, dialog currently opened and no shown event occures
                    // updates body scroll immadiatelly
                    this.initializeStateBodyScroll.call(this, $modalElement);
                } else {
                    // if updated only parent UpdatePanel or postback, dialog will be opened
                    // updates body scroll after modal is shown
                    $modalElement.one("shown.bs.modal", () => this.initializeStateBodyScroll.call(this, $modalElement));
                }
            }

            if (this.currentState.modalScrollPosition) {
                // restores modal scroll position (scroll position of the whole modal)
                if (this.currentModalElement != null) {
                    // if updated only nested UpdatePanel, dialog currently opened and no scrolling required
                    // NOOP
                } else {
                    // if updated only parent UpdatePanel or postback, dialog will be opened
                    // updates modal scroll after modal is shown
                    $modalElement.one("shown.bs.modal", () => this.initializeStateModalScroll.call(this, $modalElement));
                }
            }

            if (this.currentState.modalPosition) {
                // restores modal position (drag position of the whole modal)
                if (this.currentModalElement != null) {
                    // if updated only nested UpdatePanel, dialog currently opened and no scrolling required
                    // NOOP
                } else {
                    // if updated only parent UpdatePanel or postback, dialog will be opened
                    // updates modal position after modal is shown
                    $modalElement.one("shown.bs.modal", () => this.initializeStateModalPosition.call(this, $modalElement));
                }
            }

        }

        private initializeStateBodyScroll($modalElement: JQuery) {
            $modalElement.find(".modal-body")
                .scrollLeft(this.currentState.bodyScrollPosition.left)
                .scrollTop(this.currentState.bodyScrollPosition.top);
        }

        private initializeStateModalScroll($modalElement: JQuery) {
            $modalElement
                .scrollLeft(this.currentState.modalScrollPosition.left)
                .scrollTop(this.currentState.modalScrollPosition.top);
        }

        private initializeStateModalPosition($modalElement: JQuery) {
            $modalElement.find('.modal-dialog')
                .css("left", this.currentState.modalPosition.left + "px")
                .css("top", this.currentState.modalPosition.top + "px");
        }

        private drag(event: Event, ui: JQueryUI.DraggableEventUIParams) {
            // stop resizing when dialog is dragged
            this.stopResizingProcess();

            // persist drag position
            this.currentState.modalPosition = new Position(ui.position.left, ui.position.top);
            ModalDialogStatePersister.trySaveState(this.currentStateStorageKey, this.currentState);
        }

        private bodyScroll() {
            // persist modal body scroll position
            var $bodyModal = this.currentModalElement.find(".modal-body");
            this.currentState.bodyScrollPosition = new Position($bodyModal.scrollLeft(), $bodyModal.scrollTop());
            ModalDialogStatePersister.trySaveState(this.currentStateStorageKey, this.currentState);
        }

        private modalScroll() {
            // persist modal scroll position
            this.currentState.modalScrollPosition = new Position(this.currentModalElement.scrollLeft(), this.currentModalElement.scrollTop());
            ModalDialogStatePersister.trySaveState(this.currentStateStorageKey, this.currentState);
        }

        private startResizingProcess() {
            // starts resizing process if not already started
            if (this.resizingTimer == null) {
                this.processResizingProcess();
            }
        }

        private stopResizingProcess() {
            // stops resizing process if it is running
            if (this.resizingTimer != null) {
                window.clearTimeout(this.resizingTimer);
                this.resizingTimer = null;
            }
        }

        private processResizingProcess() {
            // sets modal body max-height to fit on screen
            if (this.currentModalElement != null) {
                var $modal = this.currentModalElement;
                var $modalDialog = $modal.find(".modal-dialog");
                var $modalContent = $modalDialog.find(".modal-content");
                var $modalHeader = $modalContent.find(".modal-header");
                var $modalBody = $modalContent.find(".modal-body");
                var $modalFooter = $modalContent.find(".modal-footer");

                if (($modalHeader.length == 0) && ($modalBody.length == 0)) {
                    // ModalDialog is not visible (rendered)
                    return;
                }

                var containerHeight = $modal.innerHeight();
                var headerHeight = $modalHeader.outerHeight(true) || 0;
                var footerHeight = $modalFooter.outerHeight(true) || 0;
                var headerTop = ($modalHeader.length > 0) ? $modalHeader.offset().top : $modalBody.offset().top;

                var bodyHeight = containerHeight - headerHeight - footerHeight - (2 * headerTop);

                var bodyHeightPx: string = "";
                if (($modal.scrollTop() == 0) && (bodyHeight >= 200)) { /* if less then 200 px then switch to standard behavior - scroll dialog content with page scroller */
                    bodyHeightPx = bodyHeight + "px";
                }

                $modalBody.css("max-height", bodyHeightPx);
                this.currentState.bodyMaxHeightPx = bodyHeightPx;
                ModalDialogStatePersister.trySaveState(this.currentStateStorageKey, this.currentState);
            }
            this.resizingTimer = window.setTimeout(() => this.processResizingProcess.call(this), 200); // this is a workaround for setting height in transitions/animations where setting value once at shown.bs.modal fails
        }
    }

    class ModalDialogState {
        public modalScrollPosition: Position = null;
        public bodyScrollPosition: Position = null;
        public modalPosition: Position = null;        
        public bodyMaxHeightPx: string = null;

        constructor() {            
        }
    }

    class ModalDialogStatePersister {
        public static trySaveState(storageKey: string, state: ModalDialogState): boolean {
            if (typeof (Storage) !== "undefined") {
                try {

                    var value = JSON.stringify(state);
                    sessionStorage.setItem(storageKey, value);
                    return true;
                } catch (e) {
                    // NOOP
                }
            }
            return false;
        }

        public static loadState(storageKey: string): ModalDialogState {
            if (typeof (Storage) !== "undefined") {
                try {
                    var value = sessionStorage.getItem(storageKey);
                    return JSON.parse(value);
                } catch (e) {
                    // NOOP
                }
            }
            return null;
        }

        public static deleteState(storageKey: string) {
            if (typeof (Storage) !== "undefined") {
                try {
                    sessionStorage.removeItem(storageKey);
                } catch (e) {
                    // NOOP
                }
            }
        }       
    }

    class Position {
        constructor(public left: number, public top: number) {
            
        }
    }

}