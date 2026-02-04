if (window.location.pathname.toLowerCase().endsWith('/recurring'.toLowerCase())) {
	$(function () {
		$(".js-jobs-list table.table td:nth-child(2)").each(function () {
			const cellContent = $(this).text().trim();
			$(this).html(`<a href="/hangfire/tags/search/${cellContent}">${cellContent}</a>`);
		});
	});
}

if (window.location.pathname.toLowerCase().endsWith('/tags/search'.toLowerCase())) {
	$(function () {
		$(".tags.row a").each(function () {
			$(this).addClass("btn btn-sm btn-primary");
			$(this).removeAttr("style");
		});
	});
}