/////////////////////////////////////////////////////
//Classes used in AutoSuggestMenu Event handlers
/////////////////////////////////////////////////////
function TextBoxUpdateEvent() {
    var self = this;
    //Properties
    self.source = null;
    self.selMenuItem = null;
    //Internal
    var _preventDefault = false;
    //Public methods
    self.preventDefault = function () {
        _preventDefault = true;
    };
    self.getPreventDefault = function () {
        return _preventDefault;
    };
}
