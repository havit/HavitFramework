/// <reference path="../../../scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../../scripts/typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../../scripts/typings/bootstrap/index.d.ts" />
var Havit;
(function (Havit) {
    var Web;
    (function (Web) {
        var Bootstrap;
        (function (Bootstrap) {
            var UI;
            (function (UI) {
                var WebControls;
                (function (WebControls) {
                    var ClientSide;
                    (function (ClientSide) {
                        var ModalExtension = /** @class */ (function () {
                            // #endregion
                            function ModalExtension(modalElementSelector) {
                                this.mouseWheelListenerAttached = false;
                                this.modalElementSelector = modalElementSelector;
                                this.$modalElement = $(modalElementSelector);
                                var storageKey = 'Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.State[' + this.modalElementSelector + ']';
                                this.modalDialogStatePersister = new ModalDialogStatePersister(storageKey);
                            }
                            // #region getInstance (static)
                            ModalExtension.getInstance = function (modalElementSelector, createIfNotExists) {
                                if (createIfNotExists === void 0) { createIfNotExists = true; }
                                // gets instance of modal extension for modal element
                                // selector must be in format #elementid
                                var modelExtensionData = $(modalElementSelector).data('ModalExtension');
                                var modalExtension = modelExtensionData;
                                if ((createIfNotExists) && (modalExtension == null)) {
                                    modalExtension = new ModalExtension(modalElementSelector);
                                    $(modalElementSelector).data('ModalExtension', modalExtension);
                                }
                                return modalExtension;
                            };
                            ModalExtension.prototype.show = function (closeOnEscapeKey, escapePostbackScript, dragMode, width) {
                                this.closeOnEscapeKey = closeOnEscapeKey;
                                this.escapePostbackScript = escapePostbackScript;
                                this.dragMode = dragMode;
                                this.width = width;
                                var $modalDialog = this.$modalElement.children('.modal-dialog');
                                $modalDialog.css('width', width); // modal width is changed when child modal is opened
                                // clean state for reused element (dialog open, close, open)
                                $modalDialog.css('top', '').css('left', '');
                                // shows dialog
                                // no matters how, when..., just show modal dialog
                                this.modalDialogState = new ModalDialogState(); // new dialog -> new style
                                this.modalDialogStatePersister.saveState(this.modalDialogState); // ensures that remainShow sees the same state
                                var parentModalExtension = this.getParentModalExtension();
                                if (parentModalExtension != null) {
                                    // if there is parent modal, deactivate it
                                    parentModalExtension.deactivateByChild(this);
                                    $modalDialog.css('visibility', 'visible');
                                }
                                this.showInternal(dragMode, false);
                                this.attachKeyPressEvent();
                                this.attachMouseWheelEvent();
                                this.storePreviousModalElement();
                            };
                            ModalExtension.prototype.remainShown = function (closeOnEscapeKey, escapePostbackScript, dragMode, width) {
                                this.closeOnEscapeKey = closeOnEscapeKey;
                                this.escapePostbackScript = escapePostbackScript;
                                this.dragMode = dragMode;
                                this.width = width;
                                // ensures dialog is open after postback or asynchronous postback
                                this.modalDialogState = this.modalDialogStatePersister.loadState();
                                if (this.modalDialogState != null) {
                                    this.initializeState();
                                }
                                else {
                                    this.modalDialogState = new ModalDialogState();
                                }
                                var $modalDialog = this.$modalElement.children('.modal-dialog');
                                if (!$modalDialog.hasClass('nested')) {
                                    $modalDialog.css('width', this.width); // modal width is changed when child modal is opened
                                    var parentModalExtension = this.getParentModalExtension();
                                    if (parentModalExtension != null) {
                                        // if there is parent modal, deactivate it
                                        parentModalExtension.deactivateByChild(this);
                                        $modalDialog.css('visibility', 'visible');
                                    }
                                }
                                if (this.wasPostBack()) {
                                    // when result of postback, we have to show dialog again, but without animation
                                    this.showInternal(dragMode, true);
                                }
                                else {
                                    // in asynchronous postback, whole dialog can be updated (when there is a parent UpdatePanel) or just content of dialog is refreshed (no parent UpdatePanel updated)
                                    if (this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        // parent UpdatePanel updated, we have to hide previous dialog to remove backdrop and to show current dialog
                                        // both actions must suppress animation (no animation required)
                                        var $previousModalElement = this.getPreviousModalElement();
                                        this.showHideModalInternal($previousModalElement, 'hide', true);
                                        this.showInternal(dragMode, true);
                                    }
                                    else {
                                        // no parent update panel, modal dialog element has not been replaced so no action required
                                        // NOOP
                                    }
                                }
                                this.attachKeyPressEvent();
                                this.attachMouseWheelEvent();
                                this.storePreviousModalElement();
                            };
                            ModalExtension.prototype.hide = function () {
                                this.modalDialogStatePersister.deleteState();
                                // listen no more to scroll events
                                this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body').off('scroll.havit.web.bootstrap');
                                this.$modalElement.off('scroll.havit.web.bootstrap');
                                this.stopResizingProcess();
                                this.modalDialogState = null;
                                var $previousModalElement = this.getPreviousModalElement();
                                if (this.wasPostBack()) {
                                    // when result of postback
                                    if (this.$modalElement.hasClass('fade')) {
                                        // if animation enabled, we have to show modal without animation and hide it with animation					
                                        this.showHideModalInternal(this.$modalElement, 'show', true);
                                        this.showHideModalInternal(this.$modalElement, 'hide', false);
                                    }
                                    else {
                                        // if there is no animation
                                        // NOOP
                                    }
                                }
                                else {
                                    // in asynchronous postback, whole dialog can be updated (when there is a parent UpdatePanel) or just content of dialog is refreshed (no parent UpdatePanel updated)                
                                    if (this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        // parent UpdatePanel updated, we have to hide previous dialog to remove backdrop and to show current dialog without animation and hide it
                                        this.showHideModalInternal($previousModalElement, 'hide', true);
                                        if (this.$modalElement.hasClass('fade')) {
                                            this.showHideModalInternal(this.$modalElement, 'show', true);
                                            this.showHideModalInternal(this.$modalElement, 'hide', false);
                                        }
                                    }
                                    else {
                                        // no parent update panel, modal dialog element has not been replaced so just hide it
                                        this.showHideModalInternal(this.$modalElement, 'hide', false);
                                    }
                                }
                                // do not listen to mousewheel anymore
                                if (this.mouseWheelListenerAttached) {
                                    this.$modalElement.off('mousewheel.havit.web.bootstrap');
                                    this.mouseWheelListenerAttached = false;
                                }
                                // do not listen to keypress anymore
                                // if there is already someone other listening, do not detach event
                                if (ModalExtension.keypressListenerElementSelector == this.modalElementSelector) {
                                    $('body').off('keypress.havit.web.bootstrap');
                                    ModalExtension.keypressListenerElementSelector = null;
                                }
                                var parentModalExtension = this.getParentModalExtension();
                                if (parentModalExtension != null) {
                                    // if there is parent modal, activate it
                                    parentModalExtension.activateByChild(this);
                                }
                                // clear grad mode if it is on
                                if (!!jQuery.ui && !!jQuery.ui.version && ($previousModalElement != null)) {
                                    $previousModalElement.children('.modal-dialog.ui-draggable').draggable('destroy');
                                }
                                this.clearPreviousModalElement();
                            };
                            ModalExtension.prototype.showInternal = function (dragMode, suppressAnimation) {
                                var _this = this;
                                // show modal
                                this.showHideModalInternal(this.$modalElement, 'show', suppressAnimation);
                                // switch on drag mode if enabled
                                var draggableParams = { handle: ' .modal-header', drag: function (e, ui) { return _this.drag.call(_this, e, ui); } };
                                //if (dragMode == 'Required') {
                                //    this.$modalElement.children('.modal-dialog').draggable(draggableParams);
                                //}
                                if (dragMode == 'IfAvailable') {
                                    if (!!jQuery.ui && !!jQuery.ui.version) {
                                        this.$modalElement.children('.modal-dialog').draggable(draggableParams);
                                    }
                                }
                                if (!this.modalDialogState.modalDragged) {
                                    // start resizing process if modal dialog not moved
                                    this.startResizingProcess();
                                }
                                // listen do scroll events to persist state between postbacks
                                this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body').on('scroll.havit.web.bootstrap', function (event) { return _this.bodyScroll.call(_this, event); });
                                this.$modalElement.on('scroll.havit.web.bootstrap', function (event) { return _this.modalScroll.call(_this, event); });
                            };
                            ModalExtension.prototype.showHideModalInternal = function ($modalElement, operation, suppressAnimation) {
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
                                var parentModalExtension = this.getParentModalExtension();
                                if ((operation == 'show') && (parentModalExtension != null) && parentModalExtension.$modalElement.hasClass('nested')) {
                                    $modalElement.modal({ backdrop: false });
                                }
                                // when dialog is closed, it is removed modal-open class from body element by hidden.bs.modal event
                                // but when there is parent modal open (checked by existence of .modal-backdrop), we need to disable removing modal-open class by suppressing hidden.bs.modal event
                                if (operation == 'hide') {
                                    $modalElement.one('hidden.bs.modal', function (e) {
                                        if ($('.modal-backdrop').length > 0) {
                                            e.stopPropagation();
                                        }
                                    });
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
                            };
                            // #region deactivateByChild, activateByChild
                            ModalExtension.prototype.deactivateByChild = function (deactivatedByModalExtension) {
                                // deactivates parent modal when child modal is opened
                                var $modalDialog = this.$modalElement.children('.modal-dialog');
                                this.$modalElement.addClass('nested');
                                $modalDialog.css('left', '0').css('top', '0'); // child modal is positioned from parent modal, so move it to default position if it was dragged
                                $modalDialog.css('width', deactivatedByModalExtension.width); // child modal is positioned into parent modal, so set it's width to child width
                            };
                            ModalExtension.prototype.activateByChild = function (deactivatedByModalExtenstion) {
                                // activates parent modal when child modal is closed
                                this.attachKeyPressEvent();
                                this.attachMouseWheelEvent();
                                this.$modalElement.removeClass('nested');
                                var $modalDialog = this.$modalElement.children('.modal-dialog');
                                $modalDialog.css('width', this.width); // restore own width
                                this.modalDialogState = this.modalDialogStatePersister.loadState();
                                if (this.modalDialogState != null) {
                                    this.initializeState(true); // reinicialize (moved, atc.)
                                }
                            };
                            // #endregion
                            ModalExtension.prototype.attachKeyPressEvent = function () {
                                // listen to keypress to catch escape key
                                var _this = this;
                                // register to keypress event 
                                // - when nobody is already listening or when parent node is listening
                                // - when parent is listening, this is a child, so this event is more important -> clear parents keypress event and attach out event
                                // - if we are parent of child, nothing is done
                                if ((ModalExtension.keypressListenerElementSelector == null) || (this.$modalElement.parents(ModalExtension.keypressListenerElementSelector).length > 0)) {
                                    // if parent registered to event, clear registration
                                    $('body').off('keypress.havit.web.bootstrap');
                                    // we are listener event if we are not listening (by closeOnEscapeKey) - it helps to detect there is child modal
                                    ModalExtension.keypressListenerElementSelector = this.modalElementSelector;
                                    $('body').on('keypress.havit.web.bootstrap', function (keypressEvent) {
                                        if (keypressEvent.which == 13) {
                                            keypressEvent.preventDefault(); // preventing form submit (in IE the first button on page is clicked)
                                        }
                                        if (_this.closeOnEscapeKey) {
                                            if (keypressEvent.which == 27) {
                                                eval(_this.escapePostbackScript);
                                            }
                                        }
                                    });
                                    // we have to focus modal, otherwise Escape key does not work correctly (I do not know why).
                                    try {
                                        this.$modalElement.focus();
                                    }
                                    catch (e) {
                                        // NOOP
                                    }
                                }
                            };
                            ModalExtension.prototype.attachMouseWheelEvent = function () {
                                var _this = this;
                                // prevent scrolling body in Chrome by mouse wheel (causes crazy flickering)
                                if (!this.mouseWheelListenerAttached) {
                                    this.$modalElement.on('mousewheel.havit.web.bootstrap', function (event) { return _this.mouseWheel.call(_this, event); });
                                    this.mouseWheelListenerAttached = true;
                                }
                            };
                            ModalExtension.prototype.initializeState = function (force) {
                                // force meaning: do it now (used to restore state after child modal is closed)
                                var _this = this;
                                if (force === void 0) { force = false; }
                                if (this.modalDialogState.bodyMaxHeightPx) {
                                    // restores body max-height value (height of modal body)
                                    this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body').css('max-height', this.modalDialogState.bodyMaxHeightPx);
                                }
                                if (this.modalDialogState.bodyScrollPosition) {
                                    if (force) {
                                        this.initializeStateBodyScroll();
                                    }
                                    else {
                                        // restores body scroll position (scroll position of modal body)
                                        if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                            // if postback or updated only parent UpdatePanel, dialog will be opened
                                            // updates body scroll after modal is shown
                                            this.$modalElement.one('shown.bs.modal', function () { return _this.initializeStateBodyScroll.call(_this); });
                                        }
                                        else {
                                            // if updated only nested UpdatePanel, dialog currently opened and no shown event occures
                                            // updates body scroll immadiatelly
                                            this.initializeStateBodyScroll();
                                        }
                                    }
                                }
                                if (this.modalDialogState.modalScrollPosition) {
                                    if (force) {
                                        this.initializeStateModalScroll();
                                    }
                                    else {
                                        // restores modal scroll position (scroll position of the whole modal)
                                        if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                            // if postback or updated only parent UpdatePanel, dialog will be opened
                                            // updates modal scroll after modal is shown
                                            this.$modalElement.one('shown.bs.modal', function () { return _this.initializeStateModalScroll.call(_this); });
                                        }
                                        else {
                                            // if updated only nested UpdatePanel, dialog currently opened and no scrolling required
                                            // NOOP
                                        }
                                    }
                                }
                                if (this.modalDialogState.modalPosition) {
                                    if (force) {
                                        this.initializeStateModalPosition();
                                    }
                                    else {
                                        // restores modal position (drag position of the whole modal)
                                        if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                            // if postback or updated only parent UpdatePanel, dialog will be opened
                                            // updates modal position after modal is shown
                                            this.$modalElement.one('shown.bs.modal', function () { return _this.initializeStateModalPosition.call(_this); });
                                        }
                                        else {
                                            // if updated only nested UpdatePanel, dialog currently opened and no scrolling required
                                            // NOOP
                                        }
                                    }
                                }
                            };
                            ModalExtension.prototype.initializeStateBodyScroll = function () {
                                this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body')
                                    .scrollLeft(this.modalDialogState.bodyScrollPosition.left)
                                    .scrollTop(this.modalDialogState.bodyScrollPosition.top);
                            };
                            ModalExtension.prototype.initializeStateModalScroll = function () {
                                this.$modalElement
                                    .scrollLeft(this.modalDialogState.modalScrollPosition.left)
                                    .scrollTop(this.modalDialogState.modalScrollPosition.top);
                            };
                            ModalExtension.prototype.initializeStateModalPosition = function () {
                                this.$modalElement.children('.modal-dialog')
                                    .css('left', this.modalDialogState.modalPosition.left + 'px')
                                    .css('top', this.modalDialogState.modalPosition.top + 'px');
                            };
                            ModalExtension.prototype.drag = function (event, ui) {
                                // stop resizing when dialog is dragged
                                this.stopResizingProcess();
                                // persist drag position
                                this.modalDialogState.modalPosition = new Position(ui.position.left, ui.position.top);
                                this.modalDialogState.modalDragged = true;
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                            };
                            ModalExtension.prototype.mouseWheel = function (event) {
                                // stop propagation (returns false) when event fired for an element in modal dialog
                                return ($(event.target).parents('.modal').length > 0);
                            };
                            ModalExtension.prototype.bodyScroll = function (event) {
                                // persist modal body scroll position
                                var $bodyModal = this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body');
                                this.modalDialogState.bodyScrollPosition = new Position($bodyModal.scrollLeft(), $bodyModal.scrollTop());
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                                event.stopPropagation();
                            };
                            ModalExtension.prototype.modalScroll = function (event) {
                                // persist modal scroll position
                                this.modalDialogState.modalScrollPosition = new Position(this.$modalElement.scrollLeft(), this.$modalElement.scrollTop());
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                                event.stopPropagation();
                            };
                            // #region startResizingProcess, stopResizingProcess, processResizingProcess
                            ModalExtension.prototype.startResizingProcess = function () {
                                // starts resizing process if not already started
                                if (this.resizingTimer == null) {
                                    this.processResizingProcess();
                                }
                            };
                            ModalExtension.prototype.stopResizingProcess = function () {
                                // stops resizing process if it is running
                                if (this.resizingTimer != null) {
                                    window.clearTimeout(this.resizingTimer);
                                    this.resizingTimer = null;
                                }
                            };
                            ModalExtension.prototype.processResizingProcess = function () {
                                var _this = this;
                                // sets modal body max-height to fit on screen
                                if (this.$modalElement != null) {
                                    var $modal = this.$modalElement;
                                    var $modalDialog = $modal.children('.modal-dialog');
                                    var $modalContent = $modalDialog.children('.modal-content');
                                    var $modalHeader = $modalContent.children().children('.modal-header');
                                    var $modalBody = $modalContent.children().children('.modal-body');
                                    var $modalFooter = $modalContent.children().children('.modal-footer');
                                    if (($modalHeader.length == 0) && ($modalBody.length == 0)) {
                                        // ModalDialog is not visible (rendered)
                                        return;
                                    }
                                    var containerHeight = $('.modal.in').innerHeight(); // support for nested modals - first (parent) displayed modal is used 
                                    var headerHeight = $modalHeader.outerHeight(true) || 0;
                                    var footerHeight = $modalFooter.outerHeight(true) || 0;
                                    var headerTop = ($modalHeader.length > 0) ? $modalHeader.offset().top : $modalBody.offset().top;
                                    var documentScrollTop = $(document).scrollTop();
                                    var bodyHeight = containerHeight - headerHeight - footerHeight - (2 * (headerTop - documentScrollTop));
                                    var bodyHeightPx = '';
                                    if (($modal.scrollTop() == 0) && (bodyHeight >= 200)) { /* if less then 200 px then switch to standard behavior - scroll dialog content with page scroller */
                                        bodyHeightPx = bodyHeight + 'px';
                                    }
                                    $modalBody.css('max-height', bodyHeightPx);
                                    this.modalDialogState.bodyMaxHeightPx = bodyHeightPx;
                                    this.modalDialogStatePersister.saveState(this.modalDialogState);
                                }
                                this.resizingTimer = window.setTimeout(function () { return _this.processResizingProcess.call(_this); }, 200); // this is a workaround for setting height in transitions/animations where setting value once at shown.bs.modal fails
                            };
                            // #endregion
                            // #region getParentModalExtension
                            ModalExtension.prototype.getParentModalExtension = function () {
                                // returns modal extenstion of parent modal (null if no parent modal extension found or ModalExtension does not exist)
                                var $parentModal = this.$modalElement.parent().closest('.modal');
                                if ($parentModal.length == 0) {
                                    return null;
                                }
                                return ModalExtension.getInstance('#' + $parentModal[0].id, false);
                            };
                            // #endregion
                            // #region wasPostBack, wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback
                            ModalExtension.prototype.wasPostBack = function () {
                                // when postback occurred whole content of page was refreshed - there is no previous modal element
                                return this.getPreviousModalElement() == null;
                            };
                            ModalExtension.prototype.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback = function () {
                                // returns true when asychronous postback in which modal element was updated
                                if (this.wasPostBack()) {
                                    return false;
                                }
                                var $previousPostback = this.getPreviousModalElement();
                                if ($previousPostback == null) {
                                    return false;
                                }
                                return !$previousPostback.is(this.$modalElement);
                            };
                            // #endregion
                            // #region storePreviousModalElement, getPreviousModalElement, clearPreviousModalElement, getPreviousModalElementKey
                            ModalExtension.prototype.storePreviousModalElement = function () {
                                $(document).data(this.getPreviousModalElementKey(), this.$modalElement);
                            };
                            ModalExtension.prototype.getPreviousModalElement = function () {
                                return $(document).data(this.getPreviousModalElementKey());
                            };
                            ModalExtension.prototype.clearPreviousModalElement = function () {
                                $(document).data(this.getPreviousModalElementKey(), null);
                            };
                            ModalExtension.prototype.getPreviousModalElementKey = function () {
                                return 'Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.PreviousModalElement[' + this.modalElementSelector + ']';
                            };
                            // #endregion
                            ModalExtension.suppressFireDefaultButton = function (event) {
                                // In IE default button is clicked even it is outside of the dialog.
                                // To suppress firing default button click we need to suppress event propagation.
                                if (event.keyCode == 13) {
                                    event.cancelBubble = true;
                                    if (event.stopPropagation) {
                                        event.stopPropagation();
                                    }
                                    return false;
                                }
                                return true;
                            };
                            return ModalExtension;
                        }());
                        ClientSide.ModalExtension = ModalExtension;
                        var ModalDialogState = /** @class */ (function () {
                            function ModalDialogState() {
                                this.modalScrollPosition = null;
                                this.modalDragged = false;
                                this.bodyScrollPosition = null;
                                this.modalPosition = null;
                                this.bodyMaxHeightPx = null;
                            }
                            return ModalDialogState;
                        }());
                        var ModalDialogStatePersister = /** @class */ (function () {
                            function ModalDialogStatePersister(storageKey) {
                                this.storageKey = storageKey;
                            }
                            ModalDialogStatePersister.prototype.saveState = function (state) {
                                if (typeof (Storage) !== 'undefined') {
                                    try {
                                        var value = JSON.stringify(state);
                                        sessionStorage.setItem(this.storageKey, value);
                                        return true;
                                    }
                                    catch (e) {
                                        // NOOP
                                    }
                                }
                                return false;
                            };
                            ModalDialogStatePersister.prototype.loadState = function () {
                                if (typeof (Storage) !== 'undefined') {
                                    try {
                                        var value = sessionStorage.getItem(this.storageKey);
                                        return JSON.parse(value);
                                    }
                                    catch (e) {
                                        // NOOP
                                    }
                                }
                                return null;
                            };
                            ModalDialogStatePersister.prototype.deleteState = function () {
                                if (typeof (Storage) !== 'undefined') {
                                    try {
                                        sessionStorage.removeItem(this.storageKey);
                                    }
                                    catch (e) {
                                        // NOOP
                                    }
                                }
                            };
                            return ModalDialogStatePersister;
                        }());
                        var Position = /** @class */ (function () {
                            function Position(left, top) {
                                this.left = left;
                                this.top = top;
                            }
                            return Position;
                        }());
                    })(ClientSide = WebControls.ClientSide || (WebControls.ClientSide = {}));
                })(WebControls = UI.WebControls || (UI.WebControls = {}));
            })(UI = Bootstrap.UI || (Bootstrap.UI = {}));
        })(Bootstrap = Web.Bootstrap || (Web.Bootstrap = {}));
    })(Web = Havit.Web || (Havit.Web = {}));
})(Havit || (Havit = {}));
