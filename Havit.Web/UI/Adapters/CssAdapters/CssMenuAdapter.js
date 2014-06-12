var hoverClass = "AspNet-Menu-Hover";
var topmostClass = "AspNet-Menu";
var userAgent = navigator.userAgent;
var versionOffset = userAgent.indexOf("MSIE");
var isIE = (versionOffset >= 0);
var isPreIE7 = false;
var fullVersionIE = "";
var majorVersionIE = "";

if (isIE)
{
    fullVersionIE = parseFloat(userAgent.substring(versionOffset+5, userAgent.length));
    majorVersionIE = parseInt('' + fullVersionIE);
    isPreIE7 = majorVersionIE < 7;
}

function Hover__AspNetMenu(elementId) {
/// <summary>
/// Hover menu item.
/// <summary>

	var element = document.getElementById(elementId);
    // if there is a timer for unhover this item then clear it - it is no more intended to unhover item
    if (element.resizingTimer != null)
    {
		clearTimeout(element.resizingTimer);
		element.resizingTimer = null;
    }
    
    AddClass__CssFriendlyAdapters(element, hoverClass);
	element.isHover = true; // for speedup

	// unchanged
    if (isPreIE7)
    {
        var child = element.firstChild;
        while (child)
        {
            if (child.tagName == "UL")
            {
                var grandchild = child.firstChild;
                while (grandchild)
                {
                    if (grandchild.tagName == "LI")
                    {
                        if ((typeof(grandchild.iFrameFormElementMask) != "undefined") && (grandchild.iFrameFormElementMask != null))
                        {
                            grandchild.iFrameFormElementMask.style.display = "block";
                            
                            var w = grandchild.offsetWidth;
                            if ((grandchild.offsetWidth == 0) && (typeof(element.iFrameFormElementMask) != "undefined") && (element.iFrameFormElementMask != null) && (element.iFrameFormElementMask.style.width.length > 0))
                            {
                                w = element.iFrameFormElementMask.style.width;
                            }
                            grandchild.iFrameFormElementMask.style.width = w;
                            
                            var h = grandchild.offsetHeight + 5 /* fudge to cover margins between menu items */;
                            if ((grandchild.offsetHeight == 0) && (typeof(element.iFrameFormElementMask) != "undefined") && (element.iFrameFormElementMask != null) && (element.iFrameFormElementMask.style.height.length > 0))
                            {
                                h = element.iFrameFormElementMask.style.height;
                            }
                            grandchild.iFrameFormElementMask.style.height = h;
                        }
                    }
                    
                    grandchild = grandchild.nextSibling;
                }
            }

            child = child.nextSibling;
        }
    }
}

function Unhover__AspNetMenu(elementId) {
/// <summary>
/// Unhover menu item.
/// <summary>

	var element = document.getElementById(elementId);
	
    // if there is a timer for unhover this item then clear it - we will unhover ourselves
    if (element.resizingTimer != null)
    {
		clearTimeout(element.resizingTimer);
		element.resizingTimer = null;
    }
    
    RemoveClass__CssFriendlyAdapters(element, hoverClass);   
	element.isHover = false; // for speedup

	// unchanged	
    if (isPreIE7)
    {
        var child = element.firstChild;
        while (child)
        {
            if (child.tagName == "UL")
            {
                var grandchild = child.firstChild;
                while (grandchild)	
                {
                    if (grandchild.tagName == "LI")
                    {
                        if ((typeof(grandchild.iFrameFormElementMask) != "undefined") && (grandchild.iFrameFormElementMask != null))
                        {
                            grandchild.iFrameFormElementMask.style.display = "none";
                        }
                    }

                    grandchild = grandchild.nextSibling;
                }
            }

            child = child.nextSibling;
        }
    }
}

function UnhoverUnwelcome__AspNetMenu(elementId) {
/// <summary>
/// Removes hover css class from all siblings of a menu item (elementId) and menu item parents siblings.
/// Leaves hover css class just on the menu item, parents and children.
/// </summary>

	var element = document.getElementById(elementId);
	// remove hover css class from my siblings
    var sibling = element.parentNode.firstChild;
    while (sibling)
    {
		if (sibling.tagName == "LI")
		{
			if (sibling.id != element.id) // do not remove from me (and recursively)
			{				
				UnhoverRecursive__AspNetMenu(sibling.id)
			}
		}
		sibling = sibling.nextSibling;
    }
	
	// remove hover css class from parents' siblings.
	if (element.parentNode.className != topmostClass)
	{
		UnhoverUnwelcome__AspNetMenu(element.parentNode.parentNode.id);
	}
}

function UnhoverRecursive__AspNetMenu(elementId) {	
/// <summary>
/// Removes hover css class from the menu item (elementId) and all children.
/// </summary>

	var element = document.getElementById(elementId);		
	if (element.isHover) // speedup
	{
		var child = element.firstChild;
		while (child)
		{
			if (child.tagName == "UL") // element has other children too, let's get just menu items
			{
				var grandchild = child.firstChild;
				while (grandchild)
				{
					if (grandchild.tagName == "LI")
					{
						UnhoverRecursive__AspNetMenu(grandchild.id); // recursion
					}
					grandchild = grandchild.nextSibling;
				}			
			}
			child = child.nextSibling;
		}

		Unhover__AspNetMenu(elementId);			
	}
}

function SetHover__AspNetMenu() {
/// <summary>
/// Sets up events for menu behavior.
/// </summary>

	var autogeneratedIdCounter = 0; // counter for element id generator
    var menus = document.getElementsByTagName("ul");
    for (var i=0; i<menus.length; i++)
    {
        if (menus[i].className == topmostClass)
        {			
			// the delay for element is value from his menu root
			// this enables usage of multiple menu controls in page, each can have own disapperAfter.
			var disappearAfter = parseInt(menus[i].getAttribute('disappearAfter'), 10);

			var items = menus[i].getElementsByTagName("li");
            
            for (var k=0; k<items.length; k++)
            {
				items[k].resizingTimer = null; // there is no timer on element
				items[k].isHover = false; // the element is not hovered (for speedup)
				items[k].disappearAfter = disappearAfter; // speedup

				// each element has to have id, if no, create one
				if (items[k].id == '')
				{
					items[k].id = 'autogenerated' + autogeneratedIdCounter;
					autogeneratedIdCounter += 1;
				}
			
                items[k].onmouseover = function() {
					var element = this;
					// on mouse over let's unhover all unwelcome hovered menu items
					UnhoverUnwelcome__AspNetMenu(element.id);
					// and hover this menu item
					Hover__AspNetMenu(element.id);					
				};
				
                items[k].onmouseout = function() {
					var element = this;
					// on mouse out set timer to unhover item
					element.resizingTimer = setTimeout(new Function('UnhoverRecursive__AspNetMenu(\'' + element.id + '\');'), element.disappearAfter);
				};
				
				// unchanged
                if (isPreIE7 && ((typeof(items[k].iFrameFormElementMask) == "undefined") || (items[k].iFrameFormElementMask == null)))
                {
                    var iFrameFormElementMask = document.createElement("IFRAME");
                    iFrameFormElementMask.scrolling= "no";
                    iFrameFormElementMask.src = "javascript:false;";
                    iFrameFormElementMask.frameBorder = 0;
                    iFrameFormElementMask.style.display = "none";
                    iFrameFormElementMask.style.position = "absolute";
                    iFrameFormElementMask.style.filter = "progid:DXImageTransform.Microsoft.Alpha(style=0,opacity=0)";

                    iFrameFormElementMask.style.zIndex = -1;
                    items[k].insertBefore(iFrameFormElementMask, items[k].firstChild);
                    items[k].iFrameFormElementMask = iFrameFormElementMask;
                }                
            }
        }
    }
}

window.onload = SetHover__AspNetMenu;
