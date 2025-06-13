using System.Net.Http.Headers;
using System.Text;

namespace Havit.Tools.AzureDevopForcePushChecker;

public abstract class AzureDevOpsReaderBase : IDisposable
{
	protected HttpClient AzureDevOpsHttpClient { get; private set; }
	protected string AzureDevOpsOrganization { get; private set; }
	protected string AzureDevOpsProject { get; private set; }
	protected string AzureDevOpsBaseUrl { get; private set; }

	protected AzureDevOpsReaderBase(string azureDevOpsOrganization, string azureDevOpsProject, string personalAccessToken)
	{
		AzureDevOpsOrganization = azureDevOpsOrganization;
		AzureDevOpsProject = azureDevOpsProject;
		AzureDevOpsBaseUrl = $"https://dev.azure.com/{azureDevOpsOrganization}";

		AzureDevOpsHttpClient = new HttpClient();
		AzureDevOpsHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalAccessToken}")));
		AzureDevOpsHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
	}

	public virtual void Dispose()
	{
		AzureDevOpsHttpClient?.Dispose();
		AzureDevOpsHttpClient = null;
	}
}