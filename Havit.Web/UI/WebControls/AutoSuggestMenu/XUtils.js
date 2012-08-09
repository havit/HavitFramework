//=====================
//XUtils version 1.0.0
//====================

function XUtils()
{}


//========================================
//DOM functions
//========================================

var g_elementTypes=new Array();

XUtils.createElement = function(elementType)
{

    if(!g_elementTypes[elementType]) 
        g_elementTypes[elementType] = document.createElement(elementType);
        
    return g_elementTypes[elementType].cloneNode(true);
}


XUtils.deleteNode = function (node)
{
    node.parentNode.removeChild(node);
}


XUtils.isAncestorNode = function(ancestorNode, childNode)
{
    var node=childNode.parentNode;
    
    while (node!=null)
    {
        if (node==ancestorNode)
            return true;
            
        node=node.parentNode;
    }
    
    return false;
}


XUtils.addEventListener = function (object, eventType, notifyFunction)
{
    TRACE("XUtils.addEventListener eventType=" + eventType);
    
    if (XUtils.isIE())
       object.attachEvent("on" + eventType, notifyFunction);
    else
        object.addEventListener(eventType, notifyFunction, false);
}



XUtils.removeEventListener = function (object, eventType, notifyFunction)
{
    if (XUtils.isIE())
        object.detachEvent("on" + eventType, notifyFunction);
    else
        object.removeEventListener(eventType, notifyFunction, false);
}




//=========================================
//Position and control manipulation
//=========================================

XUtils.getEventPosition = function(evt) 
{
    var posx = 0;
    var posy = 0;

    if (!evt) 
        evt = window.event;
        
    if (evt.pageX || evt.pageY) 	
    {
        posx = evt.pageX;
        posy = evt.pageY;
    }
    else if (evt.clientX || evt.clientY) 	
    {
        posx = evt.clientX + document.body.scrollLeft
                + document.documentElement.scrollLeft;
        posy = evt.clientY + document.body.scrollTop
                 + document.documentElement.scrollTop;
    }
	
    TRACE("XUtils.getEventPosition " + posx + ", " + posy);


    var pos=new Array();
    pos[0]=posx;
    pos[1]=posy;
    return pos;
}


XUtils.getEventKey = function(evt) 
{
    if (!evt) 
        evt = window.event;
  	
	
	var code = (evt.charCode) ? evt.charCode :
			((evt.keyCode) ? evt.keyCode :
			((evt.which) ? evt.which : 0));
			
	return code; 
}


XUtils.isPointInDiv = function(x, y, div) 
{
    TRACE("XUtils.isPointInDiv x=" + x + ", y=" + y);
    TRACE("XUtils.isPointInDiv div.offsetLeft=" + div.offsetLeft + ", div.offsetWidth=" + div.offsetWidth);
    TRACE("XUtils.isPointInDiv div.offsetTop=" + div.offsetTop + ", div.offsetHeight=" + div.offsetHeight);

    if (((x >= div.offsetLeft) && (x <= div.offsetLeft + div.offsetWidth))
        && ((y >= div.offsetTop) && (y <= div.offsetTop + div.offsetHeight)))
        return true;
    else
        return false;   
}


XUtils.getAbsoluteLeft = function(obj)
{
    var left = obj.offsetLeft;

    while(obj.offsetParent!=null)
    {
        obj=obj.offsetParent;
        left=left + obj.offsetLeft;
    }
   
    return left;
}



XUtils.getAbsoluteTop = function(obj)
{
    var top=obj.offsetTop;

    while (obj.offsetParent!=null)
    {
        obj=obj.offsetParent;
        top=top + obj.offsetTop;
    }
   
    return top;
}


//Add iframe under div to fix the drop-down issues in IE
//Returns IFrame
XUtils.overlayIFrame = function(div) 
{
    TRACE("XUtils.overlayIFrame");
    
    div.style.zIndex=div.style.zIndex+1;
    
    //Create a frame to make sure there is no    
    var iFrame=XUtils.createElement("IFRAME");
	
    iFrame.setAttribute("src", "");
    iFrame.style.position="absolute";

    iFrame.style.left   = div.style.left;
    iFrame.style.top    = div.style.top;
    iFrame.style.width  = div.offsetWidth + 'px';
    iFrame.style.height = div.offsetHeight + 'px';
		
    div.parentNode.appendChild(iFrame);
    
    return iFrame;
}


//Returns div containing passed div and new IFrame
//Resizing new div will resize IFrame and content div inside it
XUtils.combineWithIFrame = function(div) 
{
    var divContainer=XUtils.createElement("div");
	
	divContainer.style.position="absolute";

    divContainer.style.left   = div.style.left;
    divContainer.style.top    = div.style.top;
    divContainer.style.width  = div.clientWidth + 'px';
    divContainer.style.height = div.clientHeight + 'px';
	
    //Create a frame to make sure there is no    
    var iFrame=XUtils.createElement("IFRAME");
	
    iFrame.setAttribute("src", "");
    iFrame.style.position="absolute";

    iFrame.style.left   = "0px";
    iFrame.style.top    = "0px";
    iFrame.style.width  = '100%';
    iFrame.style.height = '100%';
		
    divContainer.appendChild(iFrame);
    
    var parentNode=div.parentNode;
    parentNode.removeChild(div);
   
    divContainer.appendChild(div);
    div.style.left   = "0px";
    div.style.top    = "0px";
    div.style.width  = "100%";
    div.style.height = "100%";
	
    parentNode.appendChild(divContainer);
    return divContainer;
}


//Return a directory for an included file
XUtils.getIncludeScriptDir = function(fileName)
{
    var scripts=document.getElementsByTagName("script");

    var path;
    var regExp=new RegExp(fileName + "\\.js(\\?.*)?$");
    
    for (var i = 0; i < scripts.length; i++) 
    { 
        path = scripts[i].getAttribute("src");
        
        if (path && path.match(regExp))
        {
            var dir = path.replace(regExp, '');
            return dir;
        }
    }
    
    return null;
}


//===========================
//Browser information
//===========================

XUtils.isIE = function()
{
    return ( navigator.appName=="Microsoft Internet Explorer" ); 
}


XUtils.isFireFox = function()
{
    return ( navigator.appName=="Netscape" );
}


function TRACE(sText)
{
    var txtTrace=document.getElementById("txtTrace");
    
    if (txtTrace!=null)
        txtTrace.value = txtTrace.value + sText + "\n";
}


function $(elementID)
{ 
    return document.getElementById(elementID); 
}


//===========================
//Text Formatting 
//===========================

var g_monthNames = new Array(
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
);


var g_dayNames = new Array(
    'Sunday',
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday'
);


XUtils.padLeft=function(text, length, padChar) 
{
    var numPadChars=length - text.toString().length
    
    var result="";
    for (count=0; count<numPadChars; count++)
    { 
        result=result + padChar;
    }
    
    result=result+text;
    
    return result;
}
           
           
XUtils.formatDateTime=function(date, format)
{
    var result="";
                
    result=format.replace(/(ampm|yyyy|mmmm|mmm|mm|dddd|ddd|dd|hh|nn|ss\/p)/gi,
    function($1)
    {
        var text;
        switch ($1.toLowerCase())
        {
            case 'ampm':  
                text=date.getHours() < 12 ? 'am' : 'pm';
                break;
                
            case 'yyyy': 
                text=date.getFullYear();
                break;
            
            case 'mmmm': 
                text=g_monthNames[date.getMonth()];
                break;
                                    
            case 'mmm':
                text=g_monthNames[date.getMonth()].substr(0, 3);
                break;
            
                
            case 'mm':   
                text=(date.getMonth() + 1);
                text=Utils.padLeft(text, 2, '0');
                break;
            
            case 'dddd': 
                text=g_dayNames[date.getDay()];
                break;
            
            case 'ddd':  
                text=g_dayNames[date.getDay()]
                text=Utils.padLeft(text, 3, '0');
                break;
            
            case 'dd':   
                text=date.getDate();
                text=XUtils.padLeft(text, 2, '0');
                break;
                
            case 'hh':  
                text=(h = date.getHours() % 12) ? h : 12;
                text=XUtils.padLeft(text, 2, '0');
                break;
            
            case 'nn':   
                text=date.getMinutes();
                text=XUtils.padLeft(text, 2, '0');
                break;
                
            case 'ss':   
                text=date.getSeconds();
                text=XUtils.padLeft(text, 2, '0');
                break;
        }
        
        return text;
    }); //End format.replace function call here
    
    return result;
}    

//Sets date to 1/1/1900.  Same as .NET Date()
XUtils.cleanTime=function(dt)
{
    var newTime=new Date(Date.parse("1/1/1900"));

    newTime.setHours(dt.getHours());    
    newTime.setMinutes(dt.getMinutes());
     
    return newTime;
}


//===========================
//Form control methods
//===========================


//Public static methods 
XUtils.setSelectedOption=function(ddl, selValue)
{
    var optionValue;
    for (var count=0; count < ddl.length; count++) 
    {
        optionValue=ddl.options[count].value;
        if (optionValue==selValue)
        {
            ddl.options[count].selected=true
            break;
        }
    }
}


XUtils.getSelectedRadioValue = function(radioButtonList) 
{
    var options = radioButtonList.getElementsByTagName('input');
    var opt;
    
    for(i=0; i<options.length; i++)
    {
        var opt = options[i];
        if(opt.checked)
        {
            return opt.value;
        }
    }
    
    return null;
}


XUtils.getSelectedOptionValue=function(ddl)
{     
    return ddl.options[ddl.selectedIndex].value;
}
        

XUtils.copyDDLValue=function(fromID, toID)
{
    //alert("CopyDDL: " + fromID + ", " + toID);
    
    ddlFrom=$(fromID);
    ddlTo=$(toID);
     
    selValue=XUtils.getSelectedOptionValue(ddlFrom);

    XUtils.setSelectedOption(ddlTo, selValue);
}


XUtils.copyTextBoxValue=function (fromTextBoxID, toTextBoxID)
{
    $(toTextBoxID).value=$(fromTextBoxID).value;
}


XUtils.checkAll=function(ctrlID, checked)
{
    var checkBox;
    var checkBoxID;
    var count=0;
    
    while (true)
    {
        checkBoxID=ctrlID + "_" + count;
        checkBox=$(checkBoxID);
        
        if (checkBox==null)
            break;
       
        checkBox.checked=checked;
        count=count+1;
    }
}



//===========================
//Array utils
//===========================

XUtils.removeElementAt=function( array, index )
{
    array = array.splice(index,1);
}


 
XUtils.getScreenResolution=function()
{
    return ( screen.width + " x " +  screen.height)
}

