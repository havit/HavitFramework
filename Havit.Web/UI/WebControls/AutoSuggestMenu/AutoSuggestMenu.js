//==============================
//AutoSuggestMenu version 1.1.0
//==============================


///////////////////////////////////////
//Class for rendering suggestions menu
///////////////////////////////////////

function AutoSuggestMenu()
{	
    //Constants
    var REFRESH_TYPE_COMPLETE       ="Complete";
    var REFRESH_TYPE_NEXT_PAGE      ="Next Page";
    var REFRESH_TYPE_PREVIOUS_PAGE  ="Previous Page";
    
    
    //Use self to handle events with specific object
    var self=this;
  
    //Properties
    self.id=null;
    self.textBoxID=null;
    self.hiddenSelectedValueID=null;
    self.clearTextOnNoSelection = false;
    self.autoPostBackScript = null;

    self.minSuggestChars=1;
	self.maxSuggestChars=5;
	self.keyPressDelay=300;
    
    self.usePaging=true;
    self.pageSize = 10;
    self.context = null;
    
    self.maxHeight=null;
   
    self.cssClass="asmMenu";
    self.menuItemCssClass="asmMenuItem";
    self.selMenuItemCssClass="asmSelMenuItem";
    self.navigationLinkCssClass="asmNavigationLink";
    
    self.useIFrame=true;
    self.updateTextBoxOnUpDown=true;
    self.resourceDir=null;
        
    self.menuItems=new Array();    //Array of AutoSuggestMenuItems
    self.onGetMenuItems=null;      //Overridable method to return suggestions
    self.onTextBoxUpdate=null;     //Overridable event handler that is called after textbox is updated with suggestion
    
    // JK: Doplneno
    self.onMenuHiddenEventHandlers = new Array();
        
    //Internal attributes
    var _dom=null;  	
    
	var _oldTextBoxValue="";
	var _selMenuItemIndex = null;	
	var _cancelSubmit=false;
    var _iFrame=null;
	var _keyPressTimer=null;
	var _onBlurTimer=null
    var _cancelOnBlur=false;
    var _hasVerticalScrollbar=false;
    
    var _pageIndex=0;
    var _totalResults=0;
    var _refreshType;
		
    //================================================
	//Private methods
	//================================================
	
			
	function getTextBoxCtrl()
	{
		return document.getElementById(self.textBoxID);
	}
		

    function getMenuItemsCount()
    {
        return self.menuItems.length;
    }
	
	
	function ensureMenuItemVisible(menuItemIndex)
	{  
	    TRACE("AutoSuggestMenu.ensureMenuItemVisible menuItemIndex=" + menuItemIndex + ", _hasVerticalScrollbar=" + _hasVerticalScrollbar);        
	   
	    if (!_hasVerticalScrollbar)
	        return;
	        	    
	    var menuStartY = _dom.scrollTop;
	    var menuEndY = _dom.scrollTop + _dom.offsetHeight;
	    	    
	    var menuItem=self.menuItems[menuItemIndex];
	    var menuItemDiv=menuItem.getDOM();
	    
	    var menuItemStartY = menuItemDiv.offsetTop;
	    var menuItemEndY = menuItemDiv.offsetTop + menuItemDiv.offsetHeight;
	
	    TRACE("AutoSuggestMenu.ensureMenuItemVisible menuStartY=" + menuStartY + ", menuEndY=" + menuEndY);        
	    TRACE("AutoSuggestMenu.ensureMenuItemVisible menuItemStartY=" + menuItemStartY + ", menuItemEndY=" + menuItemEndY);        
	
	    if (menuItemStartY < menuStartY)
	        _dom.scrollTop=menuItemStartY;
	    
	    if (menuItemEndY > menuEndY)
	        _dom.scrollTop=_dom.scrollTop + (menuItemEndY - menuEndY);
	}
	
	
	
	function moveUp()
	{
	    TRACE("AutoSuggestMenu.moveUp _selMenuItemIndex=" + _selMenuItemIndex);        
	    
	    if (_selMenuItemIndex==null)
		    itemIndex=getMenuItemsCount()-1;    //Select last item
		else
		    itemIndex=_selMenuItemIndex - 1;
		  		
		//Check if menu item exists
		if (itemIndex >= 0)
		{
            selectMenuItem(itemIndex, self.updateTextBoxOnUpDown);
            ensureMenuItemVisible(itemIndex);
		}
	}


	function moveDown()
	{
	    TRACE("AutoSuggestMenu.moveDown _selMenuItemIndex=" + _selMenuItemIndex);
		var itemIndex;
		
		if (_selMenuItemIndex==null)
		    itemIndex=0;
		else
		    itemIndex=_selMenuItemIndex + 1;
		
		if(itemIndex < getMenuItemsCount())
		{
			selectMenuItem(itemIndex, self.updateTextBoxOnUpDown);
			ensureMenuItemVisible(itemIndex);
		}
	}


	//Highlights menu item
	function highlightMenuItem(itemIndex)
	{
	    if (_selMenuItemIndex!=null)
	    {
		    if (_selMenuItemIndex==itemIndex)
			    return;    
	
	        //Unhighlight previously higlighted item
	        var menuItem=self.menuItems[_selMenuItemIndex];
		    menuItem.unhighlight();
		}
		
		var menuItem=self.menuItems[itemIndex];
        menuItem.highlight();
	}
	
	
	function selectMenuItem(itemIndex, updateTextBox)
	{
	    TRACE("AutoSuggestMenu.selectMenuItem itemIndex=" + itemIndex);
	      
	    highlightMenuItem(itemIndex);
        _selMenuItemIndex=itemIndex;
        
	    //Check if already selected
	    if ((updateTextBox==null) || (updateTextBox==true))
	    {
	        updateTextBoxValue();
	    }
  
	}

	
	function updateTextBoxValue()
	{
	    var menuItem=self.getSelectedMenuItem();   

        //Set selected value of control to the value of selected menu item
		self.setSelectedValue(menuItem.value);
     
        var preventUpdate=false;
        
        //Only call handler if it was specified
        if (self.onTextBoxUpdate)
        {  
            var evt=new TextBoxUpdateEvent();
            evt.source=self;
            evt.selMenuItem=menuItem;
            
            eval(self.onTextBoxUpdate + "(evt);");
            
            //Default text box update can be prevented if user calls evt.preventDefault
            preventUpdate=evt.getPreventDefault();
        }
        
        
        if (!preventUpdate)
        {
       	    //Update text box text	
		    var textBox=getTextBoxCtrl();
		    
		    // JIØÍ KANDA:
		    // døíve:
		    // textBox.value = menuItem.label;
		    // nyní:
			// zajistujeme vyvolani udalosti onchange, pokud je zmenen text.
			var oldValue = textBox.value;
			var newValue = menuItem.label;
			if (oldValue != newValue)
			{
				textBox.value = newValue;
				if (textBox.fireEvent)
				{
					textBox.fireEvent("onchange");
				}
				else
				{
					if (textBox.dispatchEvent)
					{
						var changeEvent = document.createEvent("HTMLEvents");
						changeEvent.initEvent("change", true, true);
						textBox.dispatchEvent(changeEvent);
					}
				}
			}

		}
    }
		

	function getTextBoxValue()
	{
		var textBox=getTextBoxCtrl();
		return(textBox.value);
	}
	
	
	function focusOnTextBox()
	{
	    //Clear out the timer that hides the menu
	    window.clearTimeout(_onBlurTimer);
		_onBlurTimer=null;
		
	    var textBox=getTextBoxCtrl();

	    if (XUtils.isIE())
	    {
	    	//Vybereme vše
			textBox.createTextRange().select();
	    }	
	    
	    textBox.focus();
	    
	}
	
	
	function isPreviousPageLinkEnabled()
	{
        var enabled=(_pageIndex!=0);
	    return enabled;
	}
	
	
	function isNextPageLinkEnabled()
	{
	    //Get number of menu items up to current page
	    var numMenuItems=(_pageIndex * self.pageSize) + self.menuItems.length;
	    
	    TRACE("AutoSuggestMenu.isNextPageLinkEnabled numMenuItems=" + numMenuItems + ", _totalResults=" + _totalResults);
	    if (numMenuItems < _totalResults)
	        return true;
	    else
	        return false;
	}
	
	
	function renderNavigationControlsMenuItem()
	{
	    var showPrev=isPreviousPageLinkEnabled();
	    var showNext=isNextPageLinkEnabled();
	    
	    TRACE("AutoSuggestMenu.renderNavigationControlsMenuItem showPrev=" + showPrev + ", showNext=" + showNext);
	    
	    if (!showPrev && !showNext)
	        return;
	   
        var div=XUtils.createElement("div");
        
        var table=XUtils.createElement("table");
        table.width="50px"
        
        var tbody=XUtils.createElement("tbody");
        var tr=XUtils.createElement("tr");

        //Left cell
        var td=XUtils.createElement("td");
        td.width="20px";
        td.align="left";
           
        if (showPrev)
        {
            var link=XUtils.createElement("a"); 
            link.className=self.navigationLinkCssClass;
            link.href="";
            
            link.innerHTML = "&lt;&lt;"
            link.onclick=self.onPreviousPage; 
            
            td.appendChild(link);
        }
        tr.appendChild(td);
                
        //Add a separator cell in the middle
        td=XUtils.createElement("td");
        td.width="10px";
        tr.appendChild(td);
    
    
        //Right cell
        td=XUtils.createElement("td");
        td.width="20px";
        td.align="right";
        
        if (showNext)
        {
           var link=XUtils.createElement("a"); 
           link.className=self.navigationLinkCssClass;
           link.href="";
           link.innerHTML="&gt;&gt;"
           link.onclick=self.onNextPage; 
          
           td.appendChild(link);
        }
       
        tr.appendChild(td);
       
        //Append table to div 
        tbody.appendChild(tr);
        table.appendChild(tbody);
        div.appendChild(table);
        
        //TRACE("AutoSuggestMenu.renderNavigationControlsMenuItem  div.innerHTML=" + div.innerHTML);
        _dom.appendChild(div); 
	}
	
	
	function createIFrame()
    {
        TRACE("AutoSuggestMenu.createIFrame");

        //Create a frame to make sure there is no    
        var iFrame=XUtils.createElement("IFRAME");
        
//    	var blankPage=self.resourcesDir + "/Blank.html";
//    	TRACE("AutoSuggestMenu.createIFrame blankPage=" + blankPage);

        iFrame.setAttribute("src",  self.blankPage);
        
        iFrame.style.position="absolute";
        iFrame.style.visibility="hidden";
        
        iFrame.style.left   = 0;
        iFrame.style.top    = 0;
        
        iFrame.style.width  = "0px";
        iFrame.style.height = "0px";
        
        return iFrame;   
    }
  
  
   	
   
	function renderMenuItems()
	{
	    TRACE("AutoSuggestMenu.renderMenuItems");

	    //Remove child divs
        while (_dom.childNodes[0])
        {
            _dom.removeChild(_dom.childNodes[0]);
        }   
        
        var menuItem;
        var menuItemDiv;
        var func;
        
        //Render menu items
	    for (count=0; count < self.menuItems.length; count++)
	    {
	        menuItem=self.menuItems[count];
	        
	        if (!menuItem.cssClass)
	            menuItem.cssClass=self.menuItemCssClass
	            
	        if (!menuItem.selCssClass)
	            menuItem.selCssClass=self.selMenuItemCssClass;
	    
	        //Assign parent menu and index to each item, 
	        //so they can call menu.onMenuItemMouseOver(); and menu.onMenuItemMouseClick();
	        menuItem.index  =count;
	        menuItem.menu   =self;
	        
	        menuItemDiv = menuItem.render();
            
            _dom.appendChild(menuItemDiv);    
	    }	   
	    
	    if (self.usePaging)
	    {
            renderNavigationControlsMenuItem();
	    }
	    
	    
	    
	    _hasVerticalScrollbar=false;
	    
	    //Update menu height
	    if (self.maxHeight)
	    {
	        var maxHeight=parseInt(self.maxHeight);
	        
	        _dom.style.height=null;
	         
	        TRACE("AutoSuggestMenu.renderMenuItems _dom.offsetHeight=" + _dom.offsetHeight + ", maxHeight=" + maxHeight);

	        if (_dom.offsetHeight > maxHeight)
	        {
	            _dom.style.height=maxHeight + "px";
	            _dom.scrollTop=0;
	            _hasVerticalScrollbar=true;
            }
	        
	        TRACE("AutoSuggestMenu.renderMenuItems _dom.style.height=" + _dom.style.height);
	    }	        
	}
	
    
    function refreshMenuItems(refreshType)
	{
	    if (!refreshType)
	        _refreshType=REFRESH_TYPE_COMPLETE;
	    else
	        _refreshType=refreshType;
	    
	    TRACE("AutoSuggestMenu.refreshMenuItems _refreshType=" + _refreshType);
	   
	    if (self.isVisible())
	        self.hide();
	    
	    //Get menu items
	    if (self.onGetMenuItems==null)
	        throw "Handler of AutoSuggestMenu.onGetMenuItems was not specified."
	    
	    var value=getTextBoxValue(); 
	    value=value.replace(/\"/g, "\\\"");
		
		switch (_refreshType)
	    {
	        case REFRESH_TYPE_COMPLETE:
	            _pageIndex=0;
	            break;
	            
	        case REFRESH_TYPE_NEXT_PAGE:
	            _pageIndex++;
	            break;

	        case REFRESH_TYPE_PREVIOUS_PAGE:
	            _pageIndex--;
	            break;
	    }
	    
	    var func=self.onGetMenuItems + "(\"" + value + "\", " + 
	                                                    self.usePaging + ", " +
	                                                    _pageIndex + ", " +
	                                                    self.pageSize + ", " +
														(self.context == null ? "null" : "'" + self.context.replace(/\'/g, "\\\'") + "'") + ", " +
														"self.refreshMenuItemsCallback)";
	    TRACE("AutoSuggestMenu.refreshMenuItems func=" + func);
	    eval(func);
	}
	
		
	//This function is a continuation of refreshmenuItems
	self.refreshMenuItemsCallback = function(jsonData)
    {
        TRACE("AutoSuggestMenu.refreshMenuItemsCallback");
             
        var json=eval("(" + jsonData + ")");      
        var jsonMenuItem;
        var menuItem;
        
        //Clear out old menu items
        self.menuItems=new Array();
        
        for (count = 0; count < json.menuItems.length; count++)
        {
            jsonMenuItem=json.menuItems[count];
           
            menuItem=new AutoSuggestMenuItem();
            menuItem.label=jsonMenuItem.label;
            menuItem.value=jsonMenuItem.value;    
            
            if (jsonMenuItem.isSelectable!=null)
                menuItem.isSelectable=jsonMenuItem.isSelectable; 
                
            if (jsonMenuItem.cssClass!=null)
                menuItem.cssClass=jsonMenuItem.cssClass;   
            
            addMenuItem(menuItem);
        }
             
         TRACE("AutoSuggestMenu.refreshMenuItemsCallback getMenuItemsCount()=" + getMenuItemsCount());
       
        if (getMenuItemsCount() > 0)
        {
           if ((_refreshType==REFRESH_TYPE_COMPLETE) && self.usePaging)
            {
                //Save total number of available suggestions
                _totalResults=json.totalResults;
            }
            
            renderMenuItems();
	    	    
	        self.show();
	    }
    }
	
	
	function addMenuItem(menuItem)
	{   
	    self.menuItems[self.menuItems.length]=menuItem;
	}
	
		
    
    //================================================
	//Public methods
	//================================================
		
	self.setSelectedValue = function(value)
	{
		TRACE("AutoSuggestMenu.setSelectedValue value=" + value);
	
		var hdnSelectedValue=document.getElementById(self.hiddenSelectedValueID);
		hdnSelectedValue.value=value;
	}
    
    
    self.getSelectedValue = function()
	{
		TRACE("AutoSuggestMenu.getSelectedValue");
	
		var hdnSelectedValue=document.getElementById(self.hiddenSelectedValueID);
		return hdnSelectedValue.value;
	}
	
    self.getSelectedValueHiddenField = function()
	{
		TRACE("AutoSuggestMenu.getSelectedValueHiddenField");
	
		return document.getElementById(self.hiddenSelectedValueID);
	}

	self.getSelectedMenuItem = function()
    {
        TRACE("AutoSuggestMenu.getHighlightedMenuItem _selMenuItemIndex=" + _selMenuItemIndex);
		
        if (_selMenuItemIndex!=null)
            return self.menuItems[_selMenuItemIndex];
        else
            return null;
    }
	
			
	self.isVisible = function()
	{
	    if (!_dom)
	        return false;
	        
		if (_dom.style.visibility == 'hidden')
			return false;
		else
			return true;
	}
	    
    
    
    function updateIFrame()
    {
        _iFrame.style.left   = _dom.style.left;
        _iFrame.style.top    = _dom.style.top;
     
        _iFrame.style.width  = _dom.offsetWidth + "px";
        _iFrame.style.height = _dom.offsetHeight + "px";
	}
	
	
	
	self.show = function ()
	{
	    TRACE("AutoSuggestMenu.show _dom=" + _dom);
	
	    if (_dom == null)
            self.render();
            
        var textBox=getTextBoxCtrl();
        _dom.style.left	=XUtils.getAbsoluteLeft(textBox)+ "px";
        _dom.style.top	=XUtils.getAbsoluteTop(textBox) + textBox.offsetHeight + "px";
        
                
		if (_iFrame)
		{
		    updateIFrame();
		    
	        _iFrame.style.visibility="visible";
	    }
	   
	   // JK: Pøesunuto sem z funkce render.
		_dom.style.minWidth = textBox.clientWidth + "px";
		
	    _dom.style.visibility = "visible";
	}
	
				
	self.hide = function()
	{
	    TRACE("AutoSuggestMenu.hide");
	
	    if (!self.isVisible())
	    {
	        TRACE("AutoSuggestMenu.Hide already hidden");
	        return;
	    }
  	    
	    _selMenuItemIndex=null;
	  
	    _dom.style.visibility = "hidden";
	    
	    if (_iFrame)
	        _iFrame.style.visibility="hidden";
	    
	    // JK: Doplnìna obsluha "událostí"    
		for(var i = 0; i < self.onMenuHiddenEventHandlers.length; i++)
		{
			self.onMenuHiddenEventHandlers[i]();
		}	        
	}
	
    
    self.render = function()
	{
	    TRACE("AutoSuggestMenu.render");
	    
	    if (self.id==null)
	        throw "id is required.";
	    
	    if (self.textBoxID==null)
	        throw "textBoxID is required.";
	    
	     if (self.hiddenSelectedValueID==null)
	        throw "hiddenSelectedValueID is required.";
	   
	    var textBox=getTextBoxCtrl();
	           
	    //Only render menu once. 
	    //After that just replace the menu Items.
	    var menuDiv;
        menuDiv = XUtils.createElement('div');
        
        menuDiv.id=self.id;
        menuDiv.className=self.cssClass;
        menuDiv.sourceObject = self;
		
        /**************************/        
        // JK: Pøesunuto do show. Zde je obèas textBox.clientWidth, zatímco v èase volání 
        // funkce show je v této vlastnosti již správná hodnota.
        // Tím napravujeme napø. chybnou velikost v dialogu.
		//menuDiv.style.minWidth = textBox.clientWidth + "px";
        /**************************/
        
        XUtils.addEventListener(menuDiv, "scroll",    self.onMenuScroll);
                    
     	TRACE("AutoSuggestMenu.render absoluteLeft=" + XUtils.getAbsoluteLeft(textBox) + ", absoluteTop=" + XUtils.getAbsoluteTop(textBox));
		
        //Move menu right under text box
        menuDiv.style.left	=XUtils.getAbsoluteLeft(textBox)+ "px";
        menuDiv.style.top	=XUtils.getAbsoluteTop(textBox) + textBox.offsetHeight + "px";
       
        menuDiv.style.visibility = 'hidden';    
   
        //Add event listeners to text box
        XUtils.addEventListener(textBox, "keydown",  self.onTextBoxKeyDown);
        XUtils.addEventListener(textBox, "keypress", self.onTextBoxKeyPress);
        XUtils.addEventListener(textBox, "keyup",    self.onTextBoxKeyUp);
        XUtils.addEventListener(textBox, "blur",     self.onTextBoxBlur);

        // JK: Doplnìno
        textBox.autoSuggestMenuDiv = menuDiv.id;
        XUtils.addEventListener(textBox, "focus", self.onTextBoxFocus);
		        		
        //Disable autocomplete on textbox
        textBox.setAttribute("autocomplete", "off");
        
   		TRACE("AutoSuggestMenu.render Moving to menuDiv.style.left=" + menuDiv.style.left + ", " + menuDiv.style.top);
		
		_dom=menuDiv;
		
        
        if (XUtils.isIE() && self.useIFrame)
        {
            _iFrame=createIFrame(); //Use IFrame to overlap Select controls in IE
           
            if (_dom.style.zIndex==null)
                _dom.style.zIndex=0;
                
            _iFrame.style.zIndex=_dom.style.zIndex;
           
            document.body.appendChild(_iFrame);
           
            //Display menu in front of iframe
            _dom.style.zIndex=_dom.style.zIndex+1001;
        }
         
        TRACE("AutoSuggestMenu.render  _dom.style.zIndex=" + _dom.style.zIndex);
		
        //Add menu to the page
        document.body.appendChild(_dom);
    }
    
        

    //=================================================
    //Event handlers
    //=================================================
    
     
    //Called from AutoSuggestMenuItem when clicked
    self.onMenuItemClick = function(itemIndex)
	{
    	TRACE("AutoSuggestMenu.onMenuItemClick  itemIndex=" + itemIndex);
	
		selectMenuItem(itemIndex);
		self.hide();
		
		//onBlur was called when user clicked on item. So switch the focus back to TextBox
		focusOnTextBox();
	}


    //Called from AutoSuggestMenuItem when higlighted
	self.onMenuItemMouseOver = function(itemIndex)
	{
	    //TRACE("AutoSuggestMenu.onMenuItemMouseOver itemIndex=" + itemIndex);
		selectMenuItem(itemIndex, false);
	}
	
	
	
    //The rest of the events are called from textbox
	self.onTextBoxKeyDown = function(evt)
	{
		TRACE("AutoSuggestMenu.OnTextBoxKeyDown  " + XUtils.getEventKey(evt) + ", " + self.textBoxID);
		
		//Save current text box value before key press takes affect
		_oldTextBoxValue=getTextBoxValue();
		TRACE("AutoSuggestMenu.OnTextBoxKeyDown  old text box value='" + _oldTextBoxValue + "'");
	
		var key=XUtils.getEventKey(evt);
				
		TRACE("AutoSuggestMenu.OnTextBoxKeyDown  Key is " + key);
				
		//Detect if the user is using the down button
		if(key==38) //Up arrow
		{
			moveUp();
		}
		else if(key==40) //Down arrow
		{
			moveDown();
		}
		else if(key==13) //Enter
		{
			TRACE("AutoSuggestMenu.OnTextBoxKeyDown : isVisible - " + self.isVisible());
			if (self.isVisible())
			{
			    if (!self.updateTextBoxOnUpDown)
			        updateTextBoxValue();
			    
				self.hide();
				
				_cancelSubmit=true;
     		}
     		else
     		{
     			_cancelSubmit=false;
     		}
		}
						
		return true;
	}
	
		
	self.onTextBoxKeyPress = function(evt)
	{
		TRACE("AutoSuggestMenu.onTextBoxKeyPress : " + XUtils.getEventKey(evt));
		
		if ((XUtils.getEventKey(evt)==13) && (_cancelSubmit)) 
		{
		    if (!evt) 
                evt = window.event;
   
			evt.cancelBubble = true;
			evt.returnValue = false;
			
			if (evt.stopPropagation)   //For FireFox
		    {
		        evt.preventDefault();
		        evt.stopPropagation();
		    }
		}
			
		return true;
	}
		
	
	self.onTextBoxKeyUp = function(evt)
	{
		var key=XUtils.getEventKey(evt);
		
		TRACE("AutoSuggestMenu.onTextBoxKeyUp " + key);
		
		var newValue=getTextBoxValue();
			
		//Skip up/down/enter
		// JK: Doplìno ignorování tabelkátoru
		if ((key!=38) && (key!=40) && (key!=13) && (key!=9))
		{		
			//Limit num of characters to display suggestions	
			if ((newValue.length > 0) &&
			    (newValue.length >= self.minSuggestChars) &&
			    (newValue.length <= self.maxSuggestChars))
			{
			    //Set timer to update div.  If user types quickly return suggestions when he stops.  
              	var divMenu = _dom;
				if (_keyPressTimer!=null) 
				    window.clearTimeout(_keyPressTimer);
								
			    //Setup a callback function with timer
			    TRACE("AutoSuggestMenu.OnTextBoxKeyUp newValue=" + newValue + ", self.keyPressDelay=" + self.keyPressDelay);	
				_keyPressTimer = window.setTimeout(self.onTextBoxKeyUpTimer, self.keyPressDelay);
			}
			else
			{
			    //Hide the menu if it is visible
			    if (self.isVisible())
			        self.hide();
			}
		
		    TRACE("AutoSuggestMenu.onTextBoxKeyUp self.oldTextBoxValue=" + _oldTextBoxValue + ", newValue=" + newValue);
		
    		if (_oldTextBoxValue!=newValue)
    			self.setSelectedValue("");
		}
	}
				
	
	self.onTextBoxKeyUpTimer = function()
	{
	    TRACE("AutoSuggestMenu.onTextBoxKeyUpTimer");
	    refreshMenuItems();
	}


	self.onTextBoxBlur = function ()
	{
		TRACE("AutoSuggestMenu.onTextBoxBlur");

		//Hide menu with a slight delay - in case there was a click
		if (_cancelOnBlur)
			focusOnTextBox();
		else
			_onBlurTimer = window.setTimeout(self.hide, 500);

		_cancelOnBlur = false;

		// JK: Doplnìno
		var textBox = getTextBoxCtrl();
		var hiddenFieldElement = self.getSelectedValueHiddenField();
		var textBoxElement = getTextBoxCtrl();

		if ((hiddenFieldElement.focusedExtra == true) && (!self.isVisible()))
		{
			hiddenFieldElement.focusedExtra = false;

			if ((self.clearTextOnNoSelection == true) && (hiddenFieldElement.value == ''))
			{
				textBox.value = '';
			}

			if (hiddenFieldElement.lastValueExtra != hiddenFieldElement.value)
			{
				var autoPostBackScript = self.autoPostBackScript;
				if (autoPostBackScript != null)
				{
					eval(autoPostBackScript);
				}
			}
		}

	}	
	
	
	self.onNextPage = function()
	{
	    TRACE("AutoSuggestMenu.onNextPage");
        focusOnTextBox();
	    
	    refreshMenuItems(REFRESH_TYPE_NEXT_PAGE);
	    
	    return false;
	}
	
	
	self.onPreviousPage = function()
	{
	    TRACE("AutoSuggestMenu.onNextPage");
	    focusOnTextBox();
	     
	    refreshMenuItems(REFRESH_TYPE_PREVIOUS_PAGE);
	    
	    return false;
	}
	
	
	self.onMenuScroll = function()
	{
	    TRACE("AutoSuggestMenu.onMenuScrol");
	    focusOnTextBox();
	   
	    _cancelOnBlur=true;
	}
	
	self.addMenuHiddenHandler = function(eventHandler)
	{
		self.onMenuHiddenEventHandlers.push(eventHandler);
	}

	// JK: Doplnìno
	self.onTextBoxFocus = function ()
	{
		var hiddenFieldElement = self.getSelectedValueHiddenField();
		// hodnoty pojmenovám se suffixem "Extra", abych zabránil konfliktu s hodnotami používanými v pickerech aplikací
		// (v dobì tìchto úprav aplikace používají stejnì pojmenované hodnoty na stejných elementech).
		hiddenFieldElement.lastValueExtra = hiddenFieldElement.value;
		hiddenFieldElement.focusedExtra = true;

		// vybereme v textboxu vše
		// pøedpokládáme, že v textboxu je buï vybraná hodnota, nebo nic (øeší nové ASM + vlastní øešení pomocí JS ve starších aplikacích)
		var textBoxElement = getTextBoxCtrl();
		if ((textBoxElement != null) && textBoxElement.createTextRange)
		{
			var textRange = textBoxElement.createTextRange().select();
		}

	}
	
}


//Static methods

AutoSuggestMenu.getMenu = function(menuID)
{
    var div=$(menuID);
    
    if (div==null)
        throw "AutoSuggestMenu (ID: '" + menuID + "') doesn't exist";
          
    var menu=div.sourceObject;
    return menu;
}

// JK: Doplnìno
AutoSuggestMenu.getMenuForElement = function(elementID)
{
	var element=$(elementID);
    
    if (element==null)
        throw "Element (ID: '" + element + "') doesn't exist";
        
    return AutoSuggestMenu.getMenu(element.autoSuggestMenuDiv);
}
