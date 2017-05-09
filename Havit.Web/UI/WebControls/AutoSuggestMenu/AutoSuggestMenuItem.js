///////////////////////////////////////////////////
//Menu Item
///////////////////////////////////////////////////
function AutoSuggestMenuItem(label, value) {
    //Use self to handle events with specific object
    var self = this;
    //Properties
    self.label = label;
    self.value = value;
    self.isSelectable = true;
    //The following properties are set by AutoSuggestMenu.renderMenuItems
    self.cssClass = null;
    self.selCssClass = null;
    //These should not be modified by user directly
    self.index = null;
    self.menu = null; //Menu to which the item belongs to
    //Internals
    var _dom;
    self.render = function () {
        TRACE("AutoSuggestMenuItem.render self.label=" + self.label + ", self.value=" + self.value + ", self.cssClass=" + self.cssClass);
        //Only render menu once. 
        //After that just replace the menu Items.
        var div = XUtils.createElement('div');
        div.className = self.cssClass;
        div.innerHTML = self.label;
        if (self.isSelectable) {
            //Attach event handlers
            div.onmouseover = self.onMouseOver;
            div.onclick = self.onClick;
        }
        _dom = div;
        return _dom;
    };
    self.highlight = function () {
        _dom.className = self.selCssClass;
    };
    self.unhighlight = function () {
        _dom.className = self.cssClass;
    };
    self.getDOM = function () {
        return _dom;
    };
    //==========================
    //Event handlers
    //==========================
    self.onMouseOver = function () {
        TRACE("AutoSuggestMenuItem.onMouseOver");
        self.menu.onMenuItemMouseOver(self.index);
    };
    self.onClick = function () {
        TRACE("AutoSuggestMenuItem.onClick");
        self.menu.onMenuItemClick(self.index);
    };
}
