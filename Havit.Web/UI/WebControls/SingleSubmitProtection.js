var _SingleSubmit_IsRecursive = false;
var _SingleSubmit_IsProcessing = false;
var _SingleSubmit_IsProcessing_Disabled = false;
var _SingleSubmit_ProgressLayer_Created = false;

///	<summary>
/// Voláno v WebForm_OnSubmit. Zajistí nastavení IsProcessing, pokud samotný WebForm_OnSubmit vrací true (projdou validátory)
/// a zároveň není nastavení IsProcessing zakázáno.
/// </summary>
function SingleSubmit_OnSubmit()
{
	var result = true;
	if (!_SingleSubmit_IsRecursive)	
	{
		// Rekurzivní volání funkce pro zjištění, zda "projdou validátory".
		// Při prvním vstupu se WebForm_OnSubmit znovu zavolá (rekurze).
		// Při druhém průchodu se metoda SingleSubmit_OnSubmit ignoruje a není volána - tak se zpravují validátory
		// a dozvíme se výsledek.
		if (typeof(WebForm_OnSubmit) != "undefined")
		{
			_SingleSubmit_IsRecursive = true;
			if (typeof(ValidatorOnSubmit) == "function" && ValidatorOnSubmit() == false)
			{
				result = false;
			}
			result = result && WebForm_OnSubmit();
			_SingleSubmit_IsRecursive = false;
		}
		
		// Pokud validátory projdou nebo na stránce nejsou validátory
		// schováme stránku.
		if (result)
		{
			SingleSubmit_SetProcessing();	
		}
		// Povolíme zakázaný SetProcessing, pokud byl zakázán.
		// Pokud stránka obsahuje tlačítko, které nezpůsobuje SetProcessing a po jeho stisknutí neprojdou validátory,
		// musí se napříště znovu zavolat SingleSubmit_SetProcessing_Disable.		
	}
	return result;
}

/// <summary>
/// Povolí zobrazení IsProcessing.
/// </summary>
function SingleSubmit_SetProcessing_Enable()
{
	_SingleSubmit_IsProcessing_Disabled = false;
}

/// <summary>
/// Zakáže zobrazení IsProcessing.
/// </summary>
function SingleSubmit_SetProcessing_Disable()
{
	_SingleSubmit_IsProcessing_Disabled = true;
}

/// <summary>
/// Zobrazí IsProcessing, pokud toto není zakázáno.
/// </summary>
function SingleSubmit_SetProcessing()
{
	// pokud již nejsme ve stavu IsProcessing nebo to není zakázáno
	if (!_SingleSubmit_IsProcessing_Disabled)
	{
		if (!_SingleSubmit_IsProcessing)
		{
			_SingleSubmit_IsProcessing = true;
			
			// pokusíme se najít progress layer, který budeme zobrazovat
			var progressLayer = document.getElementById("progress-layer");		
			_SingleSubmit_ProgressLayer_Created = false;
			// pokud jsme progress layer nenašli, tak jej vytvoříme
			if (progressLayer == null)
			{
				_SingleSubmit_ProgressLayer_Created = true;
				progressLayer = document.createElement("DIV");
				progressLayer.id = "progress-layer";
				document.body.appendChild(progressLayer);
			}
			
			// nastavíme progress layeru velikost a pozici
			progressLayer.style.left = "0px";
			progressLayer.style.top = "0px";												
			progressLayer.style.width = document.documentElement.scrollWidth + "px"; 
			progressLayer.style.height = document.documentElement.scrollHeight + "px"; 
			progressLayer.style.display = "block";
			progressLayer.style.cursor = "progress";
		}
	}
	else	
	{
		SingleSubmit_SetProcessing_Enable();
	}
}

/// <summary>
/// Schová IsProcessing.
/// </summary>
function SingleSubmit_ClearProcessing()
{
	if (_SingleSubmit_IsProcessing)
	{
		// najdeme progress layer
		var progressLayer = document.getElementById("progress-layer");					
		if (progressLayer != null)
		{
			if (_SingleSubmit_ProgressLayer_Created)
			{
				// progressLayer.style.display = "none";
				// pokud jsme je progress layer sami přidali, sami jej zrušíme
				progressLayer.style.cursor = "auto";
				document.body.removeChild(progressLayer);		
			}
			else
			{
				// jinak jej prostě jen schováme
				progressLayer.style.display = "none";
			}
		}
		
		_SingleSubmit_IsProcessing = false;
	}
}

/// <summary>
/// Zajistí, aby po dokončení async pallbacku byl zase zrušen stav IsProcessing.
/// </summary>
function SingleSubmit_Startup()
{	
	Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(SingleSubmit_ClearProcessing);
}
