/// <reference path="../../../scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../../scripts/typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../../scripts/typings/bootstrap/bootstrap.d.ts" />

module Havit.Web.Bootstrap.UI.WebControls.ClientSide {
    export class ModalExtension {
        
        // #region getInstance (static)
        public static getInstance(modalElementSelector: string, createIfNotExists: boolean = true): ModalExtension {
            // gets instance of modal extension for modal element
            // selector must be in format #elementid
            var modalExtension = <ModalExtension>$(modalElementSelector).data('ModalExtension');
            if ((createIfNotExists) && (modalExtension == null)) {
                modalExtension = new ModalExtension(modalElementSelector);
                $(modalElementSelector).data('ModalExtension', modalExtension);
            }
            return modalExtension;
        }
        // #endregion

        // #region keyupListenerElementSelector (static)
        // static field for registering which modal has currently registerek keyup event
        private static keyupListenerElementSelector: string;
        // #endregion

        // #region ModalExtension state fields               
        private modalElementSelector: string; // selector of modal element       
        private $modalElement: JQuery; // JQuery object of modal element
        private modalDialogStatePersister: ModalDialogStatePersister; // modal state persister

        private closeOnEscapeKey: boolean; // true if should close on escape key (set in show and remainshown methods)
        private escapePostbackScript: string; // postback code for close on escape key (set in show and remainshown methods)
        private dragMode: string; // dragmode - None, IfAvailable, Required (set in show and remainshown methods)
                
        private resizingTimer: number; // timer for resizing dialog when window resized
        private modalDialogState: ModalDialogState; // state of modal dialog (drag position, scroll position, etc.)
        // #endregion

        constructor(modalElementSelector: string) {
            this.modalElementSelector = modalElementSelector;
            this.$modalElement = $(modalElementSelector);

            var storageKey = 'Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.State[' + this.modalElementSelector + ']';
            this.modalDialogStatePersister = new ModalDialogStatePersister(storageKey);
        }

        public show(closeOnEscapeKey: boolean, escapePostbackScript: string, dragMode: string) {
            this.closeOnEscapeKey = closeOnEscapeKey;
            this.escapePostbackScript = escapePostbackScript;
            this.dragMode = dragMode;

            // clean state for reused element (dialog open, close, open)
            this.$modalElement.find('.modal-dialog')
                .css('top', '')
                .css('left', '');

            // shows dialog
            // no matters how, when..., just show modal dialog
            this.modalDialogState = new ModalDialogState(); // new dialog -> new style
            this.modalDialogStatePersister.saveState(this.modalDialogState); // ensures that remainShow sees the same state

            this.showInternal(dragMode, false);
            this.attachKeyUpEvent();
            this.storePreviousModalElement();
        }

        public remainShown(closeOnEscapeKey: boolean, escapePostbackScript: string, dragMode: string) {
            this.closeOnEscapeKey = closeOnEscapeKey;
            this.escapePostbackScript = escapePostbackScript;
            this.dragMode = dragMode;

            // ensures dialog is open after postback or asynchronous postback
            this.modalDialogState = this.modalDialogStatePersister.loadState();
            if (this.modalDialogState != null) {                
                this.initializeState();
            } else {
                this.modalDialogState = new ModalDialogState();
            }

            if (this.wasPostBack()) {
                // when result of postback, we have to show dialog again, but without animation
                this.showInternal(dragMode, true);
            } else {
                // in asynchronous postback, whole dialog can be updated (when there is a parent UpdatePanel) or just content of dialog is refreshed (no parent UpdatePanel updated)
                if (this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                    // parent UpdatePanel updated, we have to hide previous dialog to remove backdrop and to show current dialog
                    // both actions must suppress animation (no animation required)
                    var $previousModalElement = this.getPreviousModalElement();
                    this.showHideModalInternal($previousModalElement, 'hide', true);
                    this.showInternal(dragMode, true);
                } else {
                    // no parent update panel, modal dialog element has not been replaced so no action required
                    // NOOP
                }
            }
            this.attachKeyUpEvent();
            this.storePreviousModalElement();
        }

        public hide() {
            this.modalDialogStatePersister.deleteState();
            this.modalDialogState = null;

            this.stopResizingProcess();

            var $previousModalElement = this.getPreviousModalElement();
            if (this.wasPostBack()) {
                // when result of postback
                if (this.$modalElement.hasClass('fade')) {
                    // if animation enabled, we have to show modal without animation and hide it with animation					
                    this.showHideModalInternal(this.$modalElement, 'show', true);
                    this.showHideModalInternal(this.$modalElement, 'hide', false);
                } else {
                    // if there is no animation
                    // NOOP
                }
            } else {
                // in asynchronous postback, whole dialog can be updated (when there is a parent UpdatePanel) or just content of dialog is refreshed (no parent UpdatePanel updated)                
                if (this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                    // parent UpdatePanel updated, we have to hide previous dialog to remove backdrop and to show current dialog without animation and hide it
                    this.showHideModalInternal($previousModalElement, 'hide', true);
                    if (this.$modalElement.hasClass('fade')) {
                        this.showHideModalInternal(this.$modalElement, 'show', true);
                        this.showHideModalInternal(this.$modalElement, 'hide', false);
                    }
                } else {
                    // no parent update panel, modal dialog element has not been replaced so just hide it
                    this.showHideModalInternal(this.$modalElement, 'hide', false);
                }
            }

            // do not listen to keyup anymore
            // if there is already someone other listening, do not detach event
            if (ModalExtension.keyupListenerElementSelector == this.modalElementSelector) {
                $('body').off('keyup.havit.web.bootstrap');
                ModalExtension.keyupListenerElementSelector = null;
            }
            var parentModalExtension: ModalExtension = this.getParentModalExtension();
            if (parentModalExtension != null) {
                parentModalExtension.attachKeyUpEvent();
            }

            // clear grad mode if it is on
            if (!!jQuery.ui && !!jQuery.ui.version && ($previousModalElement != null)) {
                $previousModalElement.find('.modal-dialog.ui-draggable').draggable('destroy');
            }

            this.clearPreviousModalElement();
        }

        private showInternal(dragMode: string, suppressAnimation: boolean) {
            // show modal
            this.showHideModalInternal(this.$modalElement, 'show', suppressAnimation);

            // switch on drag mode if enabled
            var draggableParams = { handle: ' .modal-header', drag: (e, ui) => this.drag.call(this, e, ui) };
            if (dragMode == 'Required') {
                this.$modalElement.find('.modal-dialog').draggable(draggableParams);
            }

            if (dragMode == 'IfAvailable') {
                if (!!jQuery.ui && !!jQuery.ui.version) {
                    this.$modalElement.find('.modal-dialog').draggable(draggableParams);
                }
            }

            if (this.modalDialogState.modalPosition == null) {
                // start resizing process if modal dialog not moved
                this.startResizingProcess();
            }

            // listen do scroll events to persist state between postbacks
            this.$modalElement.find('.modal-body').on('scroll', () => this.bodyScroll.call(this));
            this.$modalElement.on('scroll', () => this.modalScroll.call(this));
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

            // when nested dialog is shown we do not create next backdrop
            // otherwise there are multiple backdrops under all modals
            if ((operation == 'show') && ($('.modal-backdrop').length > 0)) {
                $modalElement.modal({ backdrop: false });
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

        private attachKeyUpEvent() {
            // listen to keyup to catch escape key

            // register to keyup event 
            // - when nobody is already listening or when parent node is listening
            // - when parent is listening, this is a child, so this event is more important -> clear parents keyup event and attach out event
            // - if we are parent of child, nothing is done
            if ((ModalExtension.keyupListenerElementSelector == null) || (this.$modalElement.parents(ModalExtension.keyupListenerElementSelector).length > 0)) {
                // if parent registered to event, clear registration
                $('body').off('keyup.havit.web.bootstrap');
                // we are listener event if we are not listening (by closeOnEscapeKey) - it helps to detect there is child modal
                ModalExtension.keyupListenerElementSelector = this.modalElementSelector;
                if (this.closeOnEscapeKey) {
                    $('body').on('keyup.havit.web.bootstrap', (e) => {
                        if (e.which == 27) {
                            eval(this.escapePostbackScript);
                        }
                    });

                    // we have to focus modal, otherwise Escape key does not work correctly (I do not know why).
                    try {
                        this.$modalElement.focus();
                    } catch (e) {
                        // NOOP
                    }
                }
            }
        }

        private initializeState() {

            if (this.modalDialogState.bodyMaxHeightPx) {
                // restores body max-height value (height of modal body)
                this.$modalElement.find('.modal-body').css('max-height', this.modalDialogState.bodyMaxHeightPx);
            }

            if (this.modalDialogState.bodyScrollPosition) {
                // restores body scroll position (scroll position of modal body)
                if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                    // if postback or updated only parent UpdatePanel, dialog will be opened
                    // updates body scroll after modal is shown
                    this.$modalElement.one('shown.bs.modal', () => this.initializeStateBodyScroll.call(this, this.$modalElement));
                } else {
                    // if updated only nested UpdatePanel, dialog currently opened and no shown event occures
                    // updates body scroll immadiatelly
                    this.initializeStateBodyScroll.call(this, this.$modalElement);
                }
            }

            if (this.modalDialogState.modalScrollPosition) {
                // restores modal scroll position (scroll position of the whole modal)
                if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                    // if postback or updated only parent UpdatePanel, dialog will be opened
                    // updates modal scroll after modal is shown
                    this.$modalElement.one('shown.bs.modal', () => this.initializeStateModalScroll.call(this, this.$modalElement));
                } else {
                    // if updated only nested UpdatePanel, dialog currently opened and no scrolling required
                    // NOOP
                }
            }

            if (this.modalDialogState.modalPosition) {
                // restores modal position (drag position of the whole modal)
                if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                    // if postback or updated only parent UpdatePanel, dialog will be opened
                    // updates modal position after modal is shown
                    this.$modalElement.one('shown.bs.modal', () => this.initializeStateModalPosition.call(this, this.$modalElement));
                } else {
                    // if updated only nested UpdatePanel, dialog currently opened and no scrolling required
                    // NOOP
                }
            }
        }

        private initializeStateBodyScroll() {
            this.$modalElement.find('.modal-body')
                .scrollLeft(this.modalDialogState.bodyScrollPosition.left)
                .scrollTop(this.modalDialogState.bodyScrollPosition.top);
        }

        private initializeStateModalScroll() {
            this.$modalElement
                .scrollLeft(this.modalDialogState.modalScrollPosition.left)
                .scrollTop(this.modalDialogState.modalScrollPosition.top);
        }

        private initializeStateModalPosition() {
            this.$modalElement.find('.modal-dialog')
                .css('left', this.modalDialogState.modalPosition.left + 'px')
                .css('top', this.modalDialogState.modalPosition.top + 'px');
        }

        private drag(event: Event, ui: JQueryUI.DraggableEventUIParams) {
            // stop resizing when dialog is dragged
            this.stopResizingProcess();

            // persist drag position
            this.modalDialogState.modalPosition = new Position(ui.position.left, ui.position.top);
            this.modalDialogStatePersister.saveState(this.modalDialogState);
        }

        private bodyScroll() {
            // persist modal body scroll position
            var $bodyModal = this.$modalElement.find('.modal-body');
            this.modalDialogState.bodyScrollPosition = new Position($bodyModal.scrollLeft(), $bodyModal.scrollTop());
            this.modalDialogStatePersister.saveState(this.modalDialogState);
        }

        private modalScroll() {
            // persist modal scroll position
            this.modalDialogState.modalScrollPosition = new Position(this.$modalElement.scrollLeft(), this.$modalElement.scrollTop());
            this.modalDialogStatePersister.saveState(this.modalDialogState);
        }

        // #region startResizingProcess, stopResizingProcess, processResizingProcess
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
            if (this.$modalElement != null) {
                var $modal = this.$modalElement;
                var $modalDialog = $modal.find('.modal-dialog');
                var $modalContent = $modalDialog.find('.modal-content');
                var $modalHeader = $modalContent.find('.modal-header');
                var $modalBody = $modalContent.find('.modal-body');
                var $modalFooter = $modalContent.find('.modal-footer');

                if (($modalHeader.length == 0) && ($modalBody.length == 0)) {
                    // ModalDialog is not visible (rendered)
                    return;
                }

                var containerHeight = $modal.innerHeight();
                var headerHeight = $modalHeader.outerHeight(true) || 0;
                var footerHeight = $modalFooter.outerHeight(true) || 0;
                var headerTop = ($modalHeader.length > 0) ? $modalHeader.offset().top : $modalBody.offset().top;

                var bodyHeight = containerHeight - headerHeight - footerHeight - (2 * headerTop);

                var bodyHeightPx: string = '';
                if (($modal.scrollTop() == 0) && (bodyHeight >= 200)) { /* if less then 200 px then switch to standard behavior - scroll dialog content with page scroller */
                    bodyHeightPx = bodyHeight + 'px';
                }

                $modalBody.css('max-height', bodyHeightPx);
                this.modalDialogState.bodyMaxHeightPx = bodyHeightPx;
                this.modalDialogStatePersister.saveState(this.modalDialogState);
            }
            this.resizingTimer = window.setTimeout(() => this.processResizingProcess.call(this), 200); // this is a workaround for setting height in transitions/animations where setting value once at shown.bs.modal fails
        }
        // #endregion

        // #region getParentModalExtension
        private getParentModalExtension(): ModalExtension {
            // returns modal extenstion of parent modal (null if no parent modal extension found or ModalExtension does not exist)
            var $parentModal = this.$modalElement.parent().closest(".modal");
            if ($parentModal.length == 0) {
                return null;
            }

            return ModalExtension.getInstance("#" + $parentModal[0].id, false);
        }
        // #endregion

        // #region wasPostBack, wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback
        private wasPostBack(): boolean {
            // when postback occurred whole content of page was refreshed - there is no previous modal element
            return this.getPreviousModalElement() == null;
        }

        private wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback(): boolean {
            // returns true when asychronous postback in which modal element was updated
            if (this.wasPostBack()) {
                return false;
            }

            var $previousPostback = this.getPreviousModalElement();
            if ($previousPostback == null) {
                return false;
            }

            return !$previousPostback.is(this.$modalElement);
        }
        // #endregion

        // #region storePreviousModalElement, getPreviousModalElement, clearPreviousModalElement, getPreviousModalElementKey
        private storePreviousModalElement() {
            $(document).data(this.getPreviousModalElementKey(), this.$modalElement);
        }

        private getPreviousModalElement(): JQuery {
            return $(document).data(this.getPreviousModalElementKey());
        }

        private clearPreviousModalElement() {
            $(document).data(this.getPreviousModalElementKey(), null);
        }

        private getPreviousModalElementKey() {
            return 'Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.PreviousModalElement[' + this.modalElementSelector + ']';
        }
        // #endregion
                
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

        private storageKey: string;

        constructor(storageKey: string) {
            this.storageKey = storageKey;
        }

        public saveState(state: ModalDialogState): boolean {
            if (typeof (Storage) !== 'undefined') {
                try {

                    var value = JSON.stringify(state);
                    sessionStorage.setItem(this.storageKey, value);
                    return true;
                } catch (e) {
                    // NOOP
                }
            }
            return false;
        }

        public loadState(): ModalDialogState {
            if (typeof (Storage) !== 'undefined') {
                try {
                    var value = sessionStorage.getItem(this.storageKey);
                    return JSON.parse(value);
                } catch (e) {
                    // NOOP
                }
            }
            return null;
        }

        public deleteState() {
            if (typeof (Storage) !== 'undefined') {
                try {
                    sessionStorage.removeItem(this.storageKey);
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