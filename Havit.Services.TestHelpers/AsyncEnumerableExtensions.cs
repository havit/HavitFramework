using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.TestHelpers;

public static class AsyncEnumerableExtensions
{
	public static async Task<List<TSource>> ToListAsync<TSource>(this IAsyncEnumerable<TSource> source)
	{
		List<TSource> list = new List<TSource>();

		await foreach (var item in source)
		{
			list.Add(item);
		}

		return list;
	}
}
