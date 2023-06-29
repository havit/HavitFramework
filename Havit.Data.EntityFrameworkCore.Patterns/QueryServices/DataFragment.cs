using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.Patterns.QueryServices;

public class DataFragment<TItem>
{
	public List<TItem> Data { get; init; }

	public int TotalCount { get; init; }

	public void Deconstruct(out List<TItem> data, out int totalCount)
	{
		data = this.Data;
		totalCount = this.TotalCount;
	}

}