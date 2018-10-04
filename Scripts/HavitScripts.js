function havitParseInt(value) {
/// <summary>Prevadi retezec na cele cislo. Argumentem muze byt cislo obsahujici whitespaces (napr. "10 000").</summary>

	value = value.replace(/\s/g, "");
	value = value.replace(/&nbsp;/gi, "");
	value = value.replace(/\xA0/gi, "");
	return parseInt(value, 10);
}

function havitParseIntSafe(value, valueOnException) {
/// <summary>Prevadi retezec na cele cislo, pokud se prevod nepodari, vrati se valueOnException.</summary>

	var result = havitParseInt(value);
	return isNaN(result) ? valueOnException : result;	
}

function havitParseFloat(value) {
/// <summary>Prevadi retezec na desetinne cislo. Argumentem muze byt cislo obsahujici whitespaces (napr. "10 000").</summary>

	value = value.replace(/\s/g, "");
	value = value.replace(/,/gi, ".");
	value = value.replace(/&nbsp;/gi, "");
	value = value.replace(/\xA0/gi, "");
	return parseFloat(value);
}

function havitParseFloatSafe(value, valueOnException) {
/// <summary>Prevadi retezec na desetinne cislo, pokud se prevod nepodari, vrati se valueOnException.</summary>

	var result = havitParseFloat(value);
	return isNaN(result) ? valueOnException : result;	
}

function havitFormatInt(value) {
/// <summary>Formatuje cele cislo s oddelovaci tisicu.</summary>
/// <param name="value">cislo ke zformatovani</param>

	var result = "";
	var originalValue = value;
	var digit = 0;
	
	if (value == null) 
	{
		return "";
	}
		
	if (typeof(value) != "number" || isNaN(value))
	{
		return Number.NaN;
	}
		
	if (value == 0)
	{
		return "0";
	}
	
	value = Math.abs(value);
	
	while (value != 0)
	{
		if (digit % 3 == 0 && digit > 0)
		{
			result = " " + result;
		}
		result = (value % 10) + result;
		value = Math.floor(value / 10);
		digit += 1;
	}

	if (originalValue < 0)
    {
		result = "-" + result;
	}
		
	return result;
}

function havitFormatFloat(value, decimals) {
/// <summary>Formatuje desetinne cislo, cela cast je s oddelovaci tisicu. Pocet desetinnych mist je dan parametrem decimals.</summary>
/// <param name="value">cislo ke zformatovani</param>
/// <param name="decimals">pocet desetinnych mist</param>
	
	if (value == null) 
	{
		return "";
	}
		
	if (typeof(value) != "number" || isNaN(value))
	{
		return Number.NaN;
	}
		
	var originalValue = value;
	var result = havitFormatInt(Math.floor(Math.abs(value)));
	
	value = Math.abs(value);	
	
	var exp = 1;
	for (var i = 0; i < decimals; i++)
	{
		exp *= 10;
	}
	
	value = Math.round(value * exp) % exp;

	if (decimals > 0)
	{
		result = result + ",";
		for (var i = 0; i < decimals; i++)
		{			
			exp /= 10;
			result = result + Math.floor(value / exp);
			value %= exp;
		}
	}
	
	if (originalValue < 0)
	{
		result = "-" + result;
	}
	
	return result;
}

function havitReformatNumber(value) {
/// <summary>
/// Preformatuje cislo.
/// Parametrem je retezec, napr. "1234567,1234567", vysledkem je formatovane cislo se shodnym poctem desetinnych mist,
/// v uvedenem pripade "1 234 567,1234567"
/// </summary>

	var workValue = value.replace(/\s/g, "");
	var parsedValue = havitParseFloat(workValue);

	if (!isNaN(parsedValue))
	{
		var index = workValue.indexOf(",");
		if (index >= 0)
		{
			return havitFormatFloat(parsedValue, workValue.length - index - 1);
		}
		else
		{
			return havitFormatInt(parsedValue);		
		}
	}
	return value;
}

function havitParseDate(item) {
/// <summary>Prevadi retezec na desetinne cislo, pokud se prevod nepodari, vrati se valueOnException.</summary>
        
   re = /^(\d+)\.(\d+)\.(\d+)$/
   if (re.test(item))
   {
      var myArray = re.exec(item);         
      var d = new Date();
      d.setFullYear(myArray[3]);      
      d.setMonth(myArray[2] - 1);      
      d.setDate(myArray[1]);                              
      d.setHours(0, 0, 0, 0);      
      return d;
   }
   else
   {   
      return null;
   }          
}

function havitIsChecked(elements, value) {
/// <summary>
/// Vraci true, pokud nejaky element v predanych elementech obsahuje hodnotu rovnou druhemu parametru a zaroven je tento element vybran (zaskrtnut).
/// Slouzi ke snadnemu overeni vybrane hodnoty na RadioButtonListu.
/// <param name="elements">mnozina (pole) html elementu</param>
/// <param name="value">hodnota, ktera se hleda</param>
/// </summary>

	var element = havitFindElementInArray(elements, value);
	if (element != null && element.checked)
	{
		return true;
	}
	return false;
}

function havitFindElementInArray(elements, value) {
/// <summary>
/// Z kolekce elementu vrati ten, ktery ma hodnotu value rovnou predanemu parametru.
/// Pokud neni element nalezen, vraci null.
/// </summary>
/// <param name="elements">mnozina (pole) html elementu</param>
/// <param name="value">hodnota, ktera se hleda</param>

	for (var i = 0; i < elements.length; i++)
	{
		if (elements[i].value == value)
		{
			return elements[i];
		}
	}
	return null;
}

function havitShowElement(element) {
/// <summary>
/// Zobrazi element, ktery byl drive schovan. Nastavuje visibility i display na "".
/// <summary>

	element.style.visibility = "";
	element.style.display = "";
}

function havitHideElement(element, keepSpace) {
/// <summary>
/// Schova element. Pokud je keepSpace true, nastavuje visibility na hidden, pokud je keepSpace false, nastavuje display na none.
/// <summary>
/// <param name="element">element, ktery se ma skryt</param>
/// <param name="keepSpace">Pokud je keepSpace true, nastavuje visibility na hidden, pokud je keepSpace false, nastavuje display na none.</param>

	if (keepSpace)
	{
		element.style.visibility = "hidden";
	}
	else
	{
		element.style.display = "none";
	}
}

function havitBlockElement(element) {
/// <summary>
/// Zablokuje element - nastavi disabled, readonly a odstraní obsluhu onClick.
/// <summary>

	if (element.getAttribute("disabled") != null)
	{
		element.setAttribute("oldDisabled", element.disabled);
		element.disabled = true;
	}
	
	if (element.getAttribute("readonly") != null)
	{
		element.setAttribute("oldReadOnly", element.readonly);
		element.readonly = true;
	}

	if (element.getAttribute("onclick") != null)
	{
		element.setAttribute("oldOnClick", element.onclick);
		element.onclick = "";
	}
	
	if (element.children)
	{
		for (var i = 0; i < element.children.length; i++)
		{
			havitBlockElement(element.children[i]);
		}
	}
}

function havitUnblockElement(element) {
/// <summary>
/// Odblokuje element - vrati hodnoty disabled, readonly a odstrani obsluhu onClick na stav pred volanim havitBlockElement().
/// <summary>

	if (element.getAttribute("oldDisabled") != null)
	{
		element.disabled = element.getAttribute("oldDisabled");
		element.removeAttribute("oldDisabled");
	}
	
	if (element.getAttribute("oldReadOnly") != null)
	{
		element.readonly = element.getAttribute("oldReadOnly");
		element.removeAttribute("oldReadOnly");
	}

	if (element.getAttribute("oldOnClick") != null)
	{
		element.onclick = element.getAttribute("oldOnClick");
		element.removeAttribute("oldOnClick");
	}

	if (element.children)
	{
		for (var i = 0; i < element.children.length; i++)
		{
			havitUnblockElement(element.children[i]);
		}
	}
}

function havitCopyToClipboard(text) {
/// <summary>
/// Zkopirujte text do clipboardu.
/// <summary>
/// <remarks>
///	http://www.stefanjelner.de/item/213/
/// Mozilla Firefox: V 'about:config' nastavit 'signed.applets.codebase_principal_support' na 'true'.
/// </remarks>
	if (window.clipboardData && clipboardData.setData)
	{
		clipboardData.setData("Text", text);
		return true;
	}
	if (window.netscape)
	{
		netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
		var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
		if (!clip) return false;
		var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
		if (!trans) return false;
		trans.addDataFlavor('text/unicode');
		var str = new Object();
		var len = new Object();
		var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
		str.data = text;
		trans.setTransferData("text/unicode",str,text.length*2);
		var clipid = Components.interfaces.nsIClipboard;
		if (!clipid) return false;
		clip.setData(trans,null,clipid.kGlobalClipboard);
		return true;
	}
	return false;
}

var _havitLastShownDialogElementId = null;

function havitShowDialog(elementId)
{
/// <summary>
/// Zobrazi dialog.
/// <summary>
	var element = document.getElementById(elementId);
	_havitLastShownDialogElementId = elementId;
	_havitGetDialogOverlay().style.display = 'block';

    _havitStoreScrollPosition();

	if (window._havitDialog_ScrollTo00Attached != true)
	{
		if (window.attachEvent)
	    {
			window.attachEvent("onscroll", _havitDialog_ScrollTo00);
		}
		else
	    {
			window.addEventListener("scroll", _havitDialog_ScrollTo00, false);
		}
	    window._havitDialog_ScrollTo00Attached = true;
    }  
    window.scroll(0, 0);
        
	if (_havitIsPreIE7())
	{
		_havitHideSelects(document);
		_havitRestoreSelects(element);
	}
	
	element.style.display = 'block';
	try 
	{
		element.focus();
	}
	catch (e)
	{
	}
}

function _havitDialog_ScrollTo00()
{
    window.scroll(0,0);
}

function havitHideDialog(elementId)
{
/// <summary>
/// Skryje dialog.
/// <summary>
	if (document.getElementById(elementId) != null)
	{
		document.getElementById(elementId).style.display = 'none';
	}

	if (elementId == _havitLastShownDialogElementId) // pokud zavíráme poslední otevřený dialog, jinak již byl otevřen jiný
	{
		_havitGetDialogOverlay().style.display = 'none';
	}
	if (_havitIsPreIE7())
	{
		_havitRestoreSelects(document);
	}
	
	_havitRestoreScrollPosition();
	try 
	{
		if (window._havitDialog_ScrollTo00Attached == true)
		{
			if (window.attachEvent)
			{        
	            window.detachEvent("onscroll", _havitDialog_ScrollTo00);
		    }
			else
	        {
		        window.removeEventListener("scroll", _havitDialog_ScrollTo00, false);
			}
			window._havitDialog_ScrollTo00Attached = false;
		}
    }
    catch (e)
    {
    }   

}

function _havitGetDialogOverlay()
{
/// <summary>
/// (Vytváøí a) vrací element, který pøekrývá okno.
/// <summary>
	var overlay = document.getElementById('webdialogoverlay');
	if (overlay == null)
	{	
		overlay = document.createElement('div');
		overlay.id = 'webdialogoverlay';
		document.body.appendChild(overlay);
	}
	return overlay;
}

function havitSetDialogSize(dialogElementId, widthUnit, heightUnit, marginLeftUnit, marginTopUnit)
{
/// <summary>
/// Nastaví dialogu velikost a umístí jej doprostøed.
/// Centrování je øešeno "fintou", viz http://interval.cz/clanky/zarovnani-prezentace-na-stred-pomoci-css/
/// <summary>
	var dialogElement = document.getElementById(dialogElementId);	
	dialogElement.style.width = widthUnit;
	dialogElement.style.height = heightUnit;
	dialogElement.style.left = '50%';
	dialogElement.style.top = '50%';
	dialogElement.style.marginLeft = marginLeftUnit;
	dialogElement.style.marginTop = marginTopUnit;
}


var _originalScrollLeft = null;
var _originalScrollTop = null;

function _havitStoreScrollPosition()
{
    _originalScrollLeft = 0;
    _originalScrollTop = 0;
	if (document.documentElement && document.documentElement.scrollTop)
	{
		_originalScrollLeft = document.documentElement.scrollLeft;
		_originalScrollTop = document.documentElement.scrollTop;
	}
	else
	{
	    if (document.body)
	    {
		    _originalScrollLeft = document.body.scrollLeft;
		    _originalScrollTop = document.body.scrollTop;
	    }
	}
}

function _havitRestoreScrollPosition()
{
    if ((_originalScrollLeft != null) && (_originalScrollTop != null))
    {
        window.scroll(_originalScrollLeft, _originalScrollTop);
    }
}

function _havitHideSelects(element)
{
	var selects = element.getElementsByTagName('select');
	for(i = 0; i < selects.length; i++)
	{
		selects[i].setAttribute('originalVisibility', selects[i].style.visibility);
		selects[i].style.visibility = 'hidden';
	}
}

function _havitRestoreSelects(element)
{
	var selects = element.getElementsByTagName('select');
	for(i = 0; i < selects.length; i++)
	{	
		var originalVisibility = selects[i].getAttribute('originalVisibility');
		if ((originalVisibility != null) && (typeof originalVisibility != undefined))
		{
			selects[i].style.visibility = selects[i].getAttribute('originalVisibility');
			selects[i].getAttribute('originalVisibility', null);
		}
	}
}

function _havitIsPreIE7()
{
	var userAgent = navigator.userAgent;
	var versionOffset = userAgent.indexOf("MSIE");
	var isIE = (versionOffset >= 0);
	if (isIE)
	{
	    fullVersionIE = parseFloat(userAgent.substring(versionOffset+5, userAgent.length));
	    majorVersionIE = parseInt('' + fullVersionIE);
	    if (majorVersionIE < 7)
	    {
			return true;
	    }
	}
	return false;
}