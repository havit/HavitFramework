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
			const cellContent = $(this).text().trim();
			const href = $(this).attr('href');
			const count = $(this).attr('rel');
			$(this).replaceWith(`<a href="${href}" class="hx-button btn position-relative">${cellContent} <span class="badge text-bg-danger position-absolute top-0 start-100 translate-middle">${count}</span></a>`);
		});
	});
}