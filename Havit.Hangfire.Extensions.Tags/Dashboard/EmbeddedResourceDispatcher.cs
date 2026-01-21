using System.Reflection;
using Hangfire.Dashboard;

namespace Havit.Hangfire.Extensions.Tags.Dashboard;

internal sealed class EmbeddedResourceDispatcher : IDashboardDispatcher
{
	private readonly string _contentType;
	private readonly Assembly _assembly;
	private readonly string _resourceName;

	public EmbeddedResourceDispatcher(string contentType, Assembly assembly, string resourceName)
	{
		ArgumentException.ThrowIfNullOrEmpty(contentType);
		ArgumentNullException.ThrowIfNull(assembly);
		ArgumentException.ThrowIfNullOrEmpty(resourceName);

		_contentType = contentType;
		_assembly = assembly;
		_resourceName = resourceName;
	}

	public async Task Dispatch(DashboardContext context)
	{
		context.Response.ContentType = _contentType;
		await WriteResponseAsync(context.Response).ConfigureAwait(continueOnCapturedContext: false);
	}

	private Task WriteResponseAsync(DashboardResponse response)
	{
		return WriteResourceAsync(response, _assembly, _resourceName);
	}

	private static async Task WriteResourceAsync(DashboardResponse response, Assembly resourceAssembly, string resourceName)
	{
		using Stream inputStream = resourceAssembly.GetManifestResourceStream(resourceName)
			?? throw new ArgumentException($"Resource with name {resourceName} not found in assembly {resourceAssembly}.");
		await inputStream.CopyToAsync(response.Body).ConfigureAwait(continueOnCapturedContext: false);
	}
}