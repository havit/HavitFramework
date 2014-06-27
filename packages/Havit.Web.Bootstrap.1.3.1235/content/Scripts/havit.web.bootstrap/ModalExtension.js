var Havit;
(function (Havit) {
    (function (Web) {
        (function (Bootstrap) {
            (function (UI) {
                (function (WebControls) {
                    (function (ClientSide) {
                        var ModalExtension = (function () {
                            function ModalExtension(modalElementSelector) {
                                this.modalElementSelector = modalElementSelector;
                                this.$modalElement = $(modalElementSelector);

                                var storageKey = 'Havit.Web.Bootstrap.UI.WebControls.ClientSide.ModalExtension.State[' + this.modalElementSelector + ']';
                                this.modalDialogStatePersister = new ModalDialogStatePersister(storageKey);
                            }
                            ModalExtension.getInstance = function (modalElementSelector, createIfNotExists) {
                                if (typeof createIfNotExists === "undefined") { createIfNotExists = true; }
                                var modalExtension = $(modalElementSelector).data('ModalExtension');
                                if ((createIfNotExists) && (modalExtension == null)) {
                                    modalExtension = new ModalExtension(modalElementSelector);
                                    $(modalElementSelector).data('ModalExtension', modalExtension);
                                }
                                return modalExtension;
                            };

                            ModalExtension.prototype.show = function (closeOnEscapeKey, escapePostbackScript, dragMode) {
                                this.closeOnEscapeKey = closeOnEscapeKey;
                                this.escapePostbackScript = escapePostbackScript;
                                this.dragMode = dragMode;

                                this.$modalElement.children('.modal-dialog').css('top', '').css('left', '');

                                this.modalDialogState = new ModalDialogState();
                                this.modalDialogStatePersister.saveState(this.modalDialogState);

                                this.showInternal(dragMode, false);
                                this.attachKeyUpEvent();
                                this.storePreviousModalElement();
                            };

                            ModalExtension.prototype.remainShown = function (closeOnEscapeKey, escapePostbackScript, dragMode) {
                                this.closeOnEscapeKey = closeOnEscapeKey;
                                this.escapePostbackScript = escapePostbackScript;
                                this.dragMode = dragMode;

                                this.modalDialogState = this.modalDialogStatePersister.loadState();
                                if (this.modalDialogState != null) {
                                    this.initializeState();
                                } else {
                                    this.modalDialogState = new ModalDialogState();
                                }

                                if (this.wasPostBack()) {
                                    this.showInternal(dragMode, true);
                                } else {
                                    if (this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        var $previousModalElement = this.getPreviousModalElement();
                                        this.showHideModalInternal($previousModalElement, 'hide', true);
                                        this.showInternal(dragMode, true);
                                    } else {
                                    }
                                }
                                this.attachKeyUpEvent();
                                this.storePreviousModalElement();
                            };

                            ModalExtension.prototype.hide = function () {
                                this.modalDialogStatePersister.deleteState();
                                this.modalDialogState = null;

                                this.stopResizingProcess();

                                var $previousModalElement = this.getPreviousModalElement();
                                if (this.wasPostBack()) {
                                    if (this.$modalElement.hasClass('fade')) {
                                        this.showHideModalInternal(this.$modalElement, 'show', true);
                                        this.showHideModalInternal(this.$modalElement, 'hide', false);
                                    } else {
                                    }
                                } else {
                                    if (this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        this.showHideModalInternal($previousModalElement, 'hide', true);
                                        if (this.$modalElement.hasClass('fade')) {
                                            this.showHideModalInternal(this.$modalElement, 'show', true);
                                            this.showHideModalInternal(this.$modalElement, 'hide', false);
                                        }
                                    } else {
                                        this.showHideModalInternal(this.$modalElement, 'hide', false);
                                    }
                                }

                                if (ModalExtension.keyupListenerElementSelector == this.modalElementSelector) {
                                    $('body').off('keyup.havit.web.bootstrap');
                                    ModalExtension.keyupListenerElementSelector = null;
                                }
                                var parentModalExtension = this.getParentModalExtension();
                                if (parentModalExtension != null) {
                                    parentModalExtension.attachKeyUpEvent();
                                }

                                if (!!jQuery.ui && !!jQuery.ui.version && ($previousModalElement != null)) {
                                    $previousModalElement.children('.modal-dialog.ui-draggable').draggable('destroy');
                                }

                                this.clearPreviousModalElement();
                            };

                            ModalExtension.prototype.showInternal = function (dragMode, suppressAnimation) {
                                var _this = this;
                                this.showHideModalInternal(this.$modalElement, 'show', suppressAnimation);

                                var draggableParams = { handle: ' .modal-header', drag: function (e, ui) {
                                        return _this.drag.call(_this, e, ui);
                                    } };
                                if (dragMode == 'Required') {
                                    this.$modalElement.children('.modal-dialog').draggable(draggableParams);
                                }

                                if (dragMode == 'IfAvailable') {
                                    if (!!jQuery.ui && !!jQuery.ui.version) {
                                        this.$modalElement.children('.modal-dialog').draggable(draggableParams);
                                    }
                                }

                                if (this.modalDialogState.modalPosition == null) {
                                    this.startResizingProcess();
                                }

                                this.$modalElement.children('modal-dialog').children('modal-content').children().children('.modal-body').on('scroll', function () {
                                    return _this.bodyScroll.call(_this);
                                });
                                this.$modalElement.on('scroll', function () {
                                    return _this.modalScroll.call(_this);
                                });
                            };

                            ModalExtension.prototype.showHideModalInternal = function ($modalElement, operation, suppressAnimation) {
                                var hasFade = $modalElement.hasClass('fade');
                                if (suppressAnimation && hasFade) {
                                    $modalElement.removeClass('fade');
                                }

                                if ((operation == 'hide') && suppressAnimation) {
                                    $('.modal-backdrop').removeClass('fade');
                                }

                                if ((operation == 'show') && ($('.modal-backdrop').length > 0)) {
                                    $modalElement.modal({ backdrop: false });
                                }

                                $modalElement.modal(operation);

                                if ((operation == 'show') && suppressAnimation && hasFade) {
                                    $('.modal-backdrop').addClass('fade');
                                }

                                if (suppressAnimation && hasFade) {
                                    $modalElement.addClass('fade');
                                }
                            };

                            ModalExtension.prototype.attachKeyUpEvent = function () {
                                var _this = this;
                                if ((ModalExtension.keyupListenerElementSelector == null) || (this.$modalElement.parents(ModalExtension.keyupListenerElementSelector).length > 0)) {
                                    $('body').off('keyup.havit.web.bootstrap');

                                    ModalExtension.keyupListenerElementSelector = this.modalElementSelector;
                                    if (this.closeOnEscapeKey) {
                                        $('body').on('keyup.havit.web.bootstrap', function (e) {
                                            if (e.which == 27) {
                                                eval(_this.escapePostbackScript);
                                            }
                                        });

                                        try  {
                                            this.$modalElement.focus();
                                        } catch (e) {
                                        }
                                    }
                                }
                            };

                            ModalExtension.prototype.initializeState = function () {
                                var _this = this;
                                if (this.modalDialogState.bodyMaxHeightPx) {
                                    this.$modalElement.children('modal-dialog').children('modal-content').children().children('.modal-body').css('max-height', this.modalDialogState.bodyMaxHeightPx);
                                }

                                if (this.modalDialogState.bodyScrollPosition) {
                                    if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        this.$modalElement.one('shown.bs.modal', function () {
                                            return _this.initializeStateBodyScroll.call(_this, _this.$modalElement);
                                        });
                                    } else {
                                        this.initializeStateBodyScroll.call(this, this.$modalElement);
                                    }
                                }

                                if (this.modalDialogState.modalScrollPosition) {
                                    if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        this.$modalElement.one('shown.bs.modal', function () {
                                            return _this.initializeStateModalScroll.call(_this, _this.$modalElement);
                                        });
                                    } else {
                                    }
                                }

                                if (this.modalDialogState.modalPosition) {
                                    if (this.wasPostBack() || this.wasModalElementUpdatedByParentUpdatePanelInAsynchronousPostback()) {
                                        this.$modalElement.one('shown.bs.modal', function () {
                                            return _this.initializeStateModalPosition.call(_this, _this.$modalElement);
                                        });
                                    } else {
                                    }
                                }
                            };

                            ModalExtension.prototype.initializeStateBodyScroll = function () {
                                this.$modalElement.children('modal-dialog').children('modal-content').children().children('.modal-body').scrollLeft(this.modalDialogState.bodyScrollPosition.left).scrollTop(this.modalDialogState.bodyScrollPosition.top);
                            };

                            ModalExtension.prototype.initializeStateModalScroll = function () {
                                this.$modalElement.scrollLeft(this.modalDialogState.modalScrollPosition.left).scrollTop(this.modalDialogState.modalScrollPosition.top);
                            };

                            ModalExtension.prototype.initializeStateModalPosition = function () {
                                this.$modalElement.children('.modal-dialog').css('left', this.modalDialogState.modalPosition.left + 'px').css('top', this.modalDialogState.modalPosition.top + 'px');
                            };

                            ModalExtension.prototype.drag = function (event, ui) {
                                this.stopResizingProcess();

                                this.modalDialogState.modalPosition = new Position(ui.position.left, ui.position.top);
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                            };

                            ModalExtension.prototype.bodyScroll = function () {
                                var $bodyModal = this.$modalElement.children('modal-dialog').children('modal-content').children().children('.modal-body');
                                this.modalDialogState.bodyScrollPosition = new Position($bodyModal.scrollLeft(), $bodyModal.scrollTop());
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
                            };

                            ModalExtension.prototype.modalScroll = function () {
                                this.modalDialogState.modalScrollPosition = new Position(this.$modalElement.scrollLeft(), this.$modalElement.scrollTop());
                                this.modalDialogStatePersister.saveState(this.modalDialogState);
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

                                    var containerHeight = $modal.innerHeight();
                                    var headerHeight = $modalHeader.outerHeight(true) || 0;
                                    var footerHeight = $modalFooter.outerHeight(true) || 0;
                                    var headerTop = ($modalHeader.length > 0) ? $modalHeader.offset().top : $modalBody.offset().top;

                                    var bodyHeight = containerHeight - headerHeight - footerHeight - (2 * headerTop);

                                    var bodyHeightPx = '';
                                    if (($modal.scrollTop() == 0) && (bodyHeight >= 200)) {
                                        bodyHeightPx = bodyHeight + 'px';
                                    }

                                    $modalBody.css('max-height', bodyHeightPx);
                                    this.modalDialogState.bodyMaxHeightPx = bodyHeightPx;
                                    this.modalDialogStatePersister.saveState(this.modalDialogState);
                                }
                                this.resizingTimer = window.setTimeout(function () {
                                    return _this.processResizingProcess.call(_this);
                                }, 200);
                            };

                            ModalExtension.prototype.getParentModalExtension = function () {
                                var $parentModal = this.$modalElement.parent().closest(".modal");
                                if ($parentModal.length == 0) {
                                    return null;
                                }

                                return ModalExtension.getInstance("#" + $parentModal[0].id, false);
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
                            return ModalExtension;
                        })();
                        ClientSide.ModalExtension = ModalExtension;

                        var ModalDialogState = (function () {
                            function ModalDialogState() {
                                this.modalScrollPosition = null;
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
                                    try  {
                                        var value = JSON.stringify(state);
                                        sessionStorage.setItem(this.storageKey, value);
                                        return true;
                                    } catch (e) {
                                    }
                                }
                                return false;
                            };

                            ModalDialogStatePersister.prototype.loadState = function () {
                                if (typeof (Storage) !== 'undefined') {
                                    try  {
                                        var value = sessionStorage.getItem(this.storageKey);
                                        return JSON.parse(value);
                                    } catch (e) {
                                    }
                                }
                                return null;
                            };

                            ModalDialogStatePersister.prototype.deleteState = function () {
                                if (typeof (Storage) !== 'undefined') {
                                    try  {
                                        sessionStorage.removeItem(this.storageKey);
                                    } catch (e) {
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
                    })(WebControls.ClientSide || (WebControls.ClientSide = {}));
                    var ClientSide = WebControls.ClientSide;
                })(UI.WebControls || (UI.WebControls = {}));
                var WebControls = UI.WebControls;
            })(Bootstrap.UI || (Bootstrap.UI = {}));
            var UI = Bootstrap.UI;
        })(Web.Bootstrap || (Web.Bootstrap = {}));
        var Bootstrap = Web.Bootstrap;
    })(Havit.Web || (Havit.Web = {}));
    var Web = Havit.Web;
})(Havit || (Havit = {}));
