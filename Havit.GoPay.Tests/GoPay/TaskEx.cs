using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Tests.GoPay;

internal static class TaskEx
{
	public static Task<T> FromResult<T>(T result)
	{
		TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
		tcs.SetResult(result);
		return tcs.Task;
	}
}
