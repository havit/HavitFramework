function Havit_CollapsiblePanel_SaveCollapsibleStatus(collapsiblePanelClientId, collapseHiddenFieldClientID, autoPostBackScript) {
	var collapsed = !$('#' + collapsiblePanelClientId).hasClass('in');

	$('#' + collapseHiddenFieldClientID).val(collapsed);

	if (autoPostBackScript != '')
	{
		eval(autoPostBackScript);
	}
}

function Havit_CollapsiblePanel_Init(collapsiblePanelClientId, collapseHiddenFieldClientID, autoPostBackScript) {
	$('#' + collapsiblePanelClientId).on('hidden.bs.collapse', function () {
		Havit_CollapsiblePanel_SaveCollapsibleStatus(collapsiblePanelClientId, collapseHiddenFieldClientID, autoPostBackScript);
	});

	$('#' + collapsiblePanelClientId).on('shown.bs.collapse', function () {
		Havit_CollapsiblePanel_SaveCollapsibleStatus(collapsiblePanelClientId, collapseHiddenFieldClientID, autoPostBackScript);
	});
}