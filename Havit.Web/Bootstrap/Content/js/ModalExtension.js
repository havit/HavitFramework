/// <reference path="../../../scripts/typings/jquery/jquery.d.ts" />
/// <reference path="../../../scripts/typings/jqueryui/jqueryui.d.ts" />
/// <reference path="../../../scripts/typings/bootstrap/bootstrap.d.ts" />
var Havit;
(function (Havit) {
    (function (Web) {
        (function (Bootstrap) {
            (function (UI) {
                (function (WebControls) {
                    (function (ClientSide) {
                        var ModalExtension = (function () {
                            function ModalExtension() {
                            }
                            // #region Instance (singleton)
                            ModalExtension.instance = function () {
                                if (this._instance == null) {
                                    this._instance = new ModalExtension();
                                }
                                return this._instance;
                            };

                            ModalExtension.prototype.show = function (modalElementSelector, closeOnEscapeKey, escapePostbackScript, dragMode) {
                                // shows dialog
                                // no matters how, when..., just show modal dialog
                                this.currentModalElement = this.showInternal(modalElementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, false);
                            };

                            ModalExtension.prototype.remainShown = function (modalElementSelector, closeOnEscapeKey, escapePostbackScript, dragMode) {
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
                            };

                            ModalExtension.prototype.hide = function (modalElementSelector) {
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
                            };

                            ModalExtension.prototype.showInternal = function (modalElementSelector, closeOnEscapeKey, escapePostbackScript, dragMode, suppressAnimation) {
                                var _this = this;
                                var $modalElement = $(modalElementSelector);

                                this.showHideModalInternal($modalElement, 'show', suppressAnimation);

                                if (closeOnEscapeKey) {
                                    $('body').on('keyup.havit.web.bootstrap', function (e) {
                                        if (e.which == 27) {
                                            eval(escapePostbackScript);
                                        }
                                    });
                                }

                                var draggableParams = { handle: '.modal-header', stop: function () {
                                        return _this.dragStop.call(_this);
                                    } };
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

                            ModalExtension.prototype.dragStop = function (event, ui) {
                                this.stopResizingProcess();
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
                                    if (bodyHeight >= 200) {
                                        bodyHeightPx = bodyHeight + "px";
                                    }

                                    $modalBody.css("max-height", bodyHeightPx);
                                }

                                this.resizingTimer = window.setTimeout(function () {
                                    return _this.processResizingProcess.call(_this);
                                }, 200); // this is a workaround for setting height in transitions/animations where setting value once at shown.bs.modal fails
                            };
                            return ModalExtension;
                        })();
                        ClientSide.ModalExtension = ModalExtension;
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
