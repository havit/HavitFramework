using System.Reflection;
using Hangfire.Dashboard;

namespace Havit.Hangfire.Extensions.Tags;

internal sealed class EmbeddedResourceDispatcher : IDashboardDispatcher
{
	private readonly string _contentType;
	private readonly Assembly _assembly;
	private readonly string _resourceName;

	public EmbeddedResourceDispatcher(string contentType, Assembly assembly, string resourceName)
	{
		ArgumentNullException.ThrowIfNullOrEmpty(contentType);
		ArgumentNullException.ThrowIfNull(assembly);
		ArgumentNullException.ThrowIfNullOrEmpty(resourceName);

		_contentType = contentType;
		_assembly = assembly;
		_resourceName = resourceName;
	}

	public async Task Dispatch(DashboardContext context)
	{
		context.Response.ContentType = _contentType;
		// TODO Hangfire: Vyøešit expiraci lépe (co když se resource zmìní? budeme mìnit route?)		
		//context.Response.SetExpire(DateTimeOffset.Now.AddYears(1));
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