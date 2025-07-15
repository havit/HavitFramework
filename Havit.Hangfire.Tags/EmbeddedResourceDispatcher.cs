using System.Reflection;
using Hangfire.Dashboard;

namespace Havit.Hangfire.Tags;

internal sealed class EmbeddedResourceDispatcher(string contentType, Assembly assembly, string resourceName) : IDashboardDispatcher
{
	private readonly string _contentType = contentType ?? throw new ArgumentNullException(nameof(contentType));

	public async Task Dispatch(DashboardContext context)
	{
		context.Response.ContentType = _contentType;
		context.Response.SetExpire(DateTimeOffset.Now.AddYears(1));
		await WriteResponseAsync(context.Response).ConfigureAwait(continueOnCapturedContext: false);
	}

	private Task WriteResponseAsync(DashboardResponse response)
	{
		return WriteResourceAsync(response, assembly, resourceName);
	}

	private static async Task WriteResourceAsync(DashboardResponse response, Assembly resourceAssembly, string resourceName)
	{
		using Stream inputStream = resourceAssembly.GetManifestResourceStream(resourceName)
			?? throw new ArgumentException($"Resource with name {resourceName} not found in assembly {resourceAssembly}.");
		await inputStream.CopyToAsync(response.Body).ConfigureAwait(continueOnCapturedContext: false);
	}
}