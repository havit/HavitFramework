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
                        var ModalExtension = (function () {
                            function ModalExtension(modalElementSelector) {
                                this.mouseWheelListenerAttached = false;
                                this.modalElementSelector = modalElementSelector;
                                this.$modalElement = $(modalElementSelector);
                                var storageKey = 'Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.State[' + this.modalElementSelector + ']';
                                this.modalDialogStatePersister = new ModalDialogStatePersister(storageKey);
                            }
                            ModalExtension.getInstance = function (modalElementSelector, createIfNotExists) {
                                if (createIfNotExists === void 0) { createIfNotExists = true; }
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
                                $modalDialog.css('width', width);
                                $modalDialog.css('top', '').css('left', '');
                                this.modalDialogState = new ModalDialogState();
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                                var parentModalExtension = this.getParentModalExtension();
                                if (parentModalExtension != null) {
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
                                this.modalDialogState = this.modalDialogStatePersister.loadState();
                                if (this.modalDialogState != null) {
                                    this.initializeState();
                                }
                                else {
                                    this.modalDialogState = new ModalDialogState();
                                }
                                var $modalDialog = this.$modalElement.children('.modal-dialog');
                                if (!$modalDialog.hasClass('nested')) {
                                    $modalDialog.css('width', this.width);
                                    var parentModalExtension = this.getParentModalExtension();
                                    if (parentModalExtension != null) {
                                        parentModalExtension.deactivateByChild(this);
                                        $modalDialog.css('visibility', 'visible');
                                    }
                                }
                                if (this.wasPostBack()) {
                                    this.showInternal(dragMode, true);
                                }
                                else {
                                    if (this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        var $previousModalElement = this.getPreviousModalElement();
                                        this.showHideModalInternal($previousModalElement, 'hide', true);
                                        this.showInternal(dragMode, true);
                                    }
                                    else {
                                    }
                                }
                                this.attachKeyPressEvent();
                                this.attachMouseWheelEvent();
                                this.storePreviousModalElement();
                            };
                            ModalExtension.prototype.hide = function () {
                                this.modalDialogStatePersister.deleteState();
                                this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body').off('scroll.havit.web.bootstrap');
                                this.$modalElement.off('scroll.havit.web.bootstrap');
                                this.stopResizingProcess();
                                this.modalDialogState = null;
                                var $previousModalElement = this.getPreviousModalElement();
                                if (this.wasPostBack()) {
                                    if (this.$modalElement.hasClass('fade')) {
                                        this.showHideModalInternal(this.$modalElement, 'show', true);
                                        this.showHideModalInternal(this.$modalElement, 'hide', false);
                                    }
                                    else {
                                    }
                                }
                                else {
                                    if (this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        this.showHideModalInternal($previousModalElement, 'hide', true);
                                        if (this.$modalElement.hasClass('fade')) {
                                            this.showHideModalInternal(this.$modalElement, 'show', true);
                                            this.showHideModalInternal(this.$modalElement, 'hide', false);
                                        }
                                    }
                                    else {
                                        this.showHideModalInternal(this.$modalElement, 'hide', false);
                                    }
                                }
                                if (this.mouseWheelListenerAttached) {
                                    this.$modalElement.off('mousewheel.havit.web.bootstrap');
                                    this.mouseWheelListenerAttached = false;
                                }
                                if (ModalExtension.keypressListenerElementSelector == this.modalElementSelector) {
                                    $('body').off('keypress.havit.web.bootstrap');
                                    ModalExtension.keypressListenerElementSelector = null;
                                }
                                var parentModalExtension = this.getParentModalExtension();
                                if (parentModalExtension != null) {
                                    parentModalExtension.activateByChild(this);
                                }
                                if (!!jQuery.ui && !!jQuery.ui.version && ($previousModalElement != null)) {
                                    $previousModalElement.children('.modal-dialog.ui-draggable').draggable('destroy');
                                }
                                this.clearPreviousModalElement();
                            };
                            ModalExtension.prototype.showInternal = function (dragMode, suppressAnimation) {
                                var _this = this;
                                this.showHideModalInternal(this.$modalElement, 'show', suppressAnimation);
                                var draggableParams = { handle: ' .modal-header', drag: function (e, ui) { return _this.drag.call(_this, e, ui); } };
                                if (dragMode == 'IfAvailable') {
                                    if (!!jQuery.ui && !!jQuery.ui.version) {
                                        this.$modalElement.children('.modal-dialog').draggable(draggableParams);
                                    }
                                }
                                if (!this.modalDialogState.modalDragged) {
                                    this.startResizingProcess();
                                }
                                this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body').on('scroll.havit.web.bootstrap', function (event) { return _this.bodyScroll.call(_this, event); });
                                this.$modalElement.on('scroll.havit.web.bootstrap', function (event) { return _this.modalScroll.call(_this, event); });
                            };
                            ModalExtension.prototype.showHideModalInternal = function ($modalElement, operation, suppressAnimation) {
                                var hasFade = $modalElement.hasClass('fade');
                                if (suppressAnimation && hasFade) {
                                    $modalElement.removeClass('fade');
                                }
                                if ((operation == 'hide') && suppressAnimation) {
                                    $('.modal-backdrop').removeClass('fade');
                                }
                                var parentModalExtension = this.getParentModalExtension();
                                if ((operation == 'show') && (parentModalExtension != null) && parentModalExtension.$modalElement.hasClass('nested')) {
                                    $modalElement.modal({ backdrop: false });
                                }
                                if (operation == 'hide') {
                                    $modalElement.one('hidden.bs.modal', function (e) {
                                        if ($('.modal-backdrop').length > 0) {
                                            e.stopPropagation();
                                        }
                                    });
                                }
                                $modalElement.modal(operation);
                                if ((operation == 'show') && suppressAnimation && hasFade) {
                                    $('.modal-backdrop').addClass('fade');
                                }
                                if (suppressAnimation && hasFade) {
                                    $modalElement.addClass('fade');
                                }
                            };
                            ModalExtension.prototype.deactivateByChild = function (deactivatedByModalExtension) {
                                var $modalDialog = this.$modalElement.children('.modal-dialog');
                                this.$modalElement.addClass('nested');
                                $modalDialog.css('left', '0').css('top', '0');
                                $modalDialog.css('width', deactivatedByModalExtension.width);
                            };
                            ModalExtension.prototype.activateByChild = function (deactivatedByModalExtenstion) {
                                this.attachKeyPressEvent();
                                this.attachMouseWheelEvent();
                                this.$modalElement.removeClass('nested');
                                var $modalDialog = this.$modalElement.children('.modal-dialog');
                                $modalDialog.css('width', this.width);
                                this.modalDialogState = this.modalDialogStatePersister.loadState();
                                if (this.modalDialogState != null) {
                                    this.initializeState(true);
                                }
                            };
                            ModalExtension.prototype.attachKeyPressEvent = function () {
                                var _this = this;
                                if ((ModalExtension.keypressListenerElementSelector == null) || (this.$modalElement.parents(ModalExtension.keypressListenerElementSelector).length > 0)) {
                                    $('body').off('keypress.havit.web.bootstrap');
                                    ModalExtension.keypressListenerElementSelector = this.modalElementSelector;
                                    $('body').on('keypress.havit.web.bootstrap', function (keypressEvent) {
                                        if (keypressEvent.which == 13) {
                                            keypressEvent.preventDefault();
                                        }
                                        if (_this.closeOnEscapeKey) {
                                            if (keypressEvent.which == 27) {
                                                eval(_this.escapePostbackScript);
                                            }
                                        }
                                    });
                                    try {
                                        this.$modalElement.focus();
                                    }
                                    catch (e) {
                                    }
                                }
                            };
                            ModalExtension.prototype.attachMouseWheelEvent = function () {
                                var _this = this;
                                if (!this.mouseWheelListenerAttached) {
                                    this.$modalElement.on('mousewheel.havit.web.bootstrap', function (event) { return _this.mouseWheel.call(_this, event); });
                                    this.mouseWheelListenerAttached = true;
                                }
                            };
                            ModalExtension.prototype.initializeState = function (force) {
                                var _this = this;
                                if (force === void 0) { force = false; }
                                if (this.modalDialogState.bodyMaxHeightPx) {
                                    this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body').css('max-height', this.modalDialogState.bodyMaxHeightPx);
                                }
                                if (this.modalDialogState.bodyScrollPosition) {
                                    if (force) {
                                        this.initializeStateBodyScroll();
                                    }
                                    else {
                                        if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                            this.$modalElement.one('shown.bs.modal', function () { return _this.initializeStateBodyScroll.call(_this); });
                                        }
                                        else {
                                            this.initializeStateBodyScroll();
                                        }
                                    }
                                }
                                if (this.modalDialogState.modalScrollPosition) {
                                    if (force) {
                                        this.initializeStateModalScroll();
                                    }
                                    else {
                                        if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                            this.$modalElement.one('shown.bs.modal', function () { return _this.initializeStateModalScroll.call(_this); });
                                        }
                                        else {
                                        }
                                    }
                                }
                                if (this.modalDialogState.modalPosition) {
                                    if (force) {
                                        this.initializeStateModalPosition();
                                    }
                                    else {
                                        if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                            this.$modalElement.one('shown.bs.modal', function () { return _this.initializeStateModalPosition.call(_this); });
                                        }
                                        else {
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
                                this.stopResizingProcess();
                                this.modalDialogState.modalPosition = new Position(ui.position.left, ui.position.top);
                                this.modalDialogState.modalDragged = true;
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                            };
                            ModalExtension.prototype.mouseWheel = function (event) {
                                return ($(event.target).parents('.modal').length > 0);
                            };
                            ModalExtension.prototype.bodyScroll = function (event) {
                                var $bodyModal = this.$modalElement.children('.modal-dialog').children('.modal-content').children().children('.modal-body');
                                this.modalDialogState.bodyScrollPosition = new Position($bodyModal.scrollLeft(), $bodyModal.scrollTop());
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                                event.stopPropagation();
                            };
                            ModalExtension.prototype.modalScroll = function (event) {
                                this.modalDialogState.modalScrollPosition = new Position(this.$modalElement.scrollLeft(), this.$modalElement.scrollTop());
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                                event.stopPropagation();
                            };
                            ModalExtension.prototype.startResizingProcess = function () {
                                if (this.resizingTimer == null) {
                                    this.processResizingProcess();
                                }
                            };
                            ModalExtension.prototype.stopResizingProcess = function () {
                                if (this.resizingTimer != null) {
                                    window.clearTimeout(this.resizingTimer);
                                    this.resizingTimer = null;
                                }
                            };
                            ModalExtension.prototype.processResizingProcess = function () {
                                var _this = this;
                                if (this.$modalElement != null) {
                                    var $modal = this.$modalElement;
                                    var $modalDialog = $modal.children('.modal-dialog');
                                    var $modalContent = $modalDialog.children('.modal-content');
                                    var $modalHeader = $modalContent.children().children('.modal-header');
                                    var $modalBody = $modalContent.children().children('.modal-body');
                                    var $modalFooter = $modalContent.children().children('.modal-footer');
                                    if (($modalHeader.length == 0) && ($modalBody.length == 0)) {
                                        return;
                                    }
                                    var containerHeight = $('.modal.in').innerHeight();
                                    var headerHeight = $modalHeader.outerHeight(true) || 0;
                                    var footerHeight = $modalFooter.outerHeight(true) || 0;
                                    var headerTop = ($modalHeader.length > 0) ? $modalHeader.offset().top : $modalBody.offset().top;
                                    var documentScrollTop = $(document).scrollTop();
                                    var bodyHeight = containerHeight - headerHeight - footerHeight - (2 * (headerTop - documentScrollTop));
                                    var bodyHeightPx = '';
                                    if (($modal.scrollTop() == 0) && (bodyHeight >= 200)) {
                                        bodyHeightPx = bodyHeight + 'px';
                                    }
                                    $modalBody.css('max-height', bodyHeightPx);
                                    this.modalDialogState.bodyMaxHeightPx = bodyHeightPx;
                                    this.modalDialogStatePersister.saveState(this.modalDialogState);
                                }
                                this.resizingTimer = window.setTimeout(function () { return _this.processResizingProcess.call(_this); }, 200);
                            };
                            ModalExtension.prototype.getParentModalExtension = function () {
                                var $parentModal = this.$modalElement.parent().closest('.modal');
                                if ($parentModal.length == 0) {
                                    return null;
                                }
                                return ModalExtension.getInstance('#' + $parentModal[0].id, false);
                            };
                            ModalExtension.prototype.wasPostBack = function () {
                                return this.getPreviousModalElement() == null;
                            };
                            ModalExtension.prototype.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback = function () {
                                if (this.wasPostBack()) {
                                    return false;
                                }
                                var $previousPostback = this.getPreviousModalElement();
                                if ($previousPostback == null) {
                                    return false;
                                }
                                return !$previousPostback.is(this.$modalElement);
                            };
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
                            ModalExtension.suppressFireDefaultButton = function (event) {
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
                        })();
                        ClientSide.ModalExtension = ModalExtension;
                        var ModalDialogState = (function () {
                            function ModalDialogState() {
                                this.modalScrollPosition = null;
                                this.modalDragged = false;
                                this.bodyScrollPosition = null;
                                this.modalPosition = null;
                                this.bodyMaxHeightPx = null;
                            }
                            return ModalDialogState;
                        })();
                        var ModalDialogStatePersister = (function () {
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
                                    }
                                }
                            };
                            return ModalDialogStatePersister;
                        })();
                        var Position = (function () {
                            function Position(left, top) {
                                this.left = left;
                                this.top = top;
                            }
                            return Position;
                        })();
                    })(ClientSide = WebControls.ClientSide || (WebControls.ClientSide = {}));
                })(WebControls = UI.WebControls || (UI.WebControls = {}));
            })(UI = Bootstrap.UI || (Bootstrap.UI = {}));
        })(Bootstrap = Web.Bootstrap || (Web.Bootstrap = {}));
    })(Web = Havit.Web || (Havit.Web = {}));
})(Havit || (Havit = {}));
