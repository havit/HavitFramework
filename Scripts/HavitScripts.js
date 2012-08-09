// Prevadi retezec na cele cislo. Argumentem muze byt cislo obsahujici whitespaces (napr. "10 000").
function havitParseInt(value)
{
	value = value.replace(/\s/g, "");
	value = value.replace(/&nbsp;/gi, "");
	value = value.replace(/\xA0/gi, "");
	return parseInt(value, 10);
}

// Prevadi retezec na cele cislo, pokud se prevod nepodari, vrati se valueOnException.
function havitParseIntSafe(value, valueOnException)
{
	var result = havitParseInt(value);
	return isNaN(result) ? valueOnException : result;	
}

// Prevadi retezec na desetinne cislo. Argumentem muze byt cislo obsahujici whitespaces (napr. "10 000").
function havitParseFloat(value)
{
	value = value.replace(/\s/g, "");
	value = value.replace(/,/gi, ".");
	value = value.replace(/&nbsp;/gi, "");
	value = value.replace(/\xA0/gi, "");
	return parseFloat(value);
}

// Prevadi retezec na desetinne cislo, pokud se prevod nepodari, vrati se valueOnException.
function havitParseFloatSafe(value, valueOnException)
{
	var result = havitParseFloat(value);
	return isNaN(result) ? valueOnException : result;	
}

// Formatuje cele cislo s oddelovaci tisicu.
function havitFormatInt(value)
{
	var result = "";
	var originalValue = value;
	var digit = 0;
	
	if (value == null) 
		return "";
		
	if (typeof(value) != "number" || isNaN(value))
		return Number.NaN;
		
	if (value == 0)
		return "0";
	
	value = Math.abs(value);
	
	while (value != 0)
	{
		if (digit % 3 == 0 && digit > 0)
			result = " " + result;
		result = (value % 10) + result;
		value = Math.floor(value / 10);
		digit += 1;
	}

	if (originalValue < 0)
		result = "-" + result;
		
	return result;
}

// Formatuje desetinne cislo, cela cast je s oddelovaci tisicu. Pocet desetinnych mist je dan parametrem radix.
function havitFormatFloat(value, radix)
{	
	if (value == null) 
		return "";
		
	if (typeof(value) != "number" || isNaN(value))
		return Number.NaN;
		
	var originalValue = value;
	var result = havitFormatInt(Math.floor(Math.abs(value)));
	
	value = Math.abs(value);	
	
	var exp = 1;
	for (var i = 0; i < radix; i++)
		exp *= 10;
	
	value = Math.round(value * exp) % exp;

	if (radix > 0)
	{
		result = result + ",";
		for (var i = 0; i < radix; i++)
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

// Preformatuje cislo.
// Parametrem je retezec, napr. "1234567,1234567", vysledkem je formatovane cislo se shodnym poctem desetinnych mist,
// v uvedenem pripade "1 234 567,1234567"
function havitReformatNumber(value)
{
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
			return havitFormatInt(parsedValue);		
	}
	return value;
}

// Vraci true, pokud nejaky element v predanych elementech obsahuje hodnotu rovnou druhemu parametru a zaroven je tento element vybran (zaskrtnut).
// Slouzi ke snadnemu overeni vybrane hodnoty na RadioButtonListu.
// elements - mnozina (pole) html elementu.
// value - hodnota, ktera se hleda
function havitIsChecked(elements, value)
{
	var element = havitFindElementInArray(elements, value);
	if (element != null && element.checked)
		return true;
	return false;
}

// Z kolekce elementu vrati ten, ktery ma hodnotu value rovnou predanemu parametru.
// Pokud neni element nalezen, vraci null.
function havitFindElementInArray(elements, value)
{
	for (var i = 0; i < elements.length; i++)
	{
		if (elements[i].value == value)
			return elements[i];
	}
	return null;
}

// Zobrazi element, ktery byl drive schovan. Nastavuje visibility i display na "".
function havitShowElement(element)
{
	element.style.visibility = "";
	element.style.display = "";
}

// Schova element. Pokud je keepSpace true, nastavuje visibility na hidden, pokud je keepSpace false, nastavuje display na none.
function havitHideElement(element, keepSpace)
{
	if (keepSpace)
		element.style.visibility = "hidden";
	else
		element.style.display = "none";
}

function havitBlockElement(element)
{
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
		for (var i = 0; i < element.children.length; i++)
			havitBlockElement(element.children[i]);
}

function havitUnblockElement(element)
{
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
		for (var i = 0; i < element.children.length; i++)
			havitUnblockElement(element.children[i]);
}


function havitParseDate(item) 
{        
   re = /^(\d+)\.(\d+)\.(\d+)$/
   if (re.test(item)) {         
      var myArray = re.exec(item);         
      var d = new Date();
      d.setFullYear(myArray[3]);      
      d.setMonth(myArray[2] - 1);      
      d.setDate(myArray[1]);                              
      d.setHours(0, 0, 0, 0);      
      return d;
   } else {   
      return null;
   }             
}


function days_between(date1, date2) {
    
    var ONE_DAY = 1000 * 60 * 60 * 24

    var date1_ms = date1.getTime()
    var date2_ms = date2.getTime()
    
    var difference_ms = date2_ms - date1_ms
    
    return Math.round(difference_ms/ONE_DAY)

}
