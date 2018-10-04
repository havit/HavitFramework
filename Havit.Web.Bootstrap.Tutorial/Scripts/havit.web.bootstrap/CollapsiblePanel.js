function Havit_CollapsiblePanel_SaveCollapsibleStatus(collapsiblePanelClientId, collapseHiddenFieldClientID, autoPostBackScript) {
	// Hack, aby se vyvolal partial postback v update panelu.
	// Podminka pro vyvolani partial postbacku ne nasledujici:
	// if (activeElement && (
	//	(activeElement.name === eventTarget) ||
	//	testCausesPostBack(activeElement.href) ||
	//	testCausesPostBack(activeElement.onclick) ||
	//	testCausesPostBack(activeElement.onchange)
	//	))
	// ActiveElement je prvek, na ktery se klika.
	// Name u velkeho mnozstvi HTML elementu nelze pouzit, proto se muselo jit jinou cestou a konkretne tak, ze do onclicku se dalo
	// return false a nasledne GetPostBackEventReference(...). "testCausesPostBack(activeElement.onclick)" je pak true,
	// podminka vyse se vyhodnoti take jako true a je generovan partial postback.
	var prm = Sys.WebForms.PageRequestManager.getInstance();
	var name = document.createAttribute("onclick");
	name.value = "return false; " + autoPostBackScript + ";";
	prm._activeElement.attributes.setNamedItem(name);
	
	var collapsed = !$('#' + collapsiblePanelClientId).hasClass('in');
	$('#' + collapseHiddenFieldClientID).val(collapsed);

	if (autoPostBackScript != '')
	{
		eval(autoPostBackScript);
	}
}

function Havit_CollapsiblePanel_Init(collapsiblePanelClientId, collapseHiddenFieldClientID, autoPostBackScript) {
	$('#' + collapsiblePanelClientId).on('hidden.bs.collapse', function() {
		Havit_CollapsiblePanel_SaveCollapsibleStatus(collapsiblePanelClientId, collapseHiddenFieldClientID, autoPostBackScript);
	});

	$('#' + collapsiblePanelClientId).on('shown.bs.collapse', function() {
		Havit_CollapsiblePanel_SaveCollapsibleStatus(collapsiblePanelClientId, collapseHiddenFieldClientID, autoPostBackScript);
	});
}