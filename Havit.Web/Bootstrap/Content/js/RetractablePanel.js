function Havit_RetractablePanel_SaveCollapsibleStatus() {
	var status = new Object();
	$('.collapse').each(function () {
		var shown = $(this).hasClass('in');
		status[this.id] = shown;
	});

	if (typeof (Storage) !== 'undefined') {
		// Code for localStorage/sessionStorage.
		var json = JSON.stringify(status);
		sessionStorage.setItem(this.id, json);
	}
	else {
		// Sorry! No Web Storage support..
	}
}

function Havit_RetractablePanel_LoadCollapsibleStatus(itemId) {
	if (typeof (Storage) !== 'undefined') {
		if (sessionStorage.getItem(itemId) != null) {
			var status = JSON.parse(sessionStorage.getItem(itemId));
			for (id in status) {
				if (status[id]) {
					$('#' + id).addClass('in');
				}
			}
		}
	}
}

function Havit_RetractablePanel_Init() {
	$('.collapse').on('hidden.bs.collapse', Havit_RetractablePanel_SaveCollapsibleStatus);
	$('.collapse').on('shown.bs.collapse', Havit_RetractablePanel_SaveCollapsibleStatus);
}