using Havit.Tools.AzureDevopForcePushChecker.Api;
using System.Text.Json;

namespace Havit.Tools.AzureDevopForcePushChecker;

public class AzureDevOpsRepositoryReader : AzureDevOpsReaderBase
{
	public AzureDevOpsRepositoryReader(string azureDevOpsOrganization, string azureDevOpsProject, string personalAccessToken) : base(azureDevOpsOrganization, azureDevOpsProject, personalAccessToken)
	{
		// NOOP
	}

	public async Task<List<RepositoryBranch>> GetRepositoriesWithBranchesAsync()
	{
		List<RepositoryBranch> allBranches = new List<RepositoryBranch>();

		Console.WriteLine("Reading repositories...");
		List<RepositoryBranch> repositories = await GetRepositoriesAsync(); // seznam repositories pro foreach cyklus

		//#if DEBUG
		//        repositories = repositories
		//            .Where(repository => repository.RepositoryName.StartsWith("125"))
		//            .Take(5)
		//            .ToList();
		//#endif

		List<RepositoryBranch> result = new List<RepositoryBranch>(repositories); // klon kolekce pro uložení výsledků

		Console.WriteLine("Reading repository branches...");
		foreach (var repository in repositories)
		{
			result.AddRange(await GetBranchesAsync(repository));
		}
		return result
			.OrderBy(item => item.RepositoryName)
			.ThenBy(item => item.BranchName)
			.ToList();
	}

	private async Task<List<RepositoryBranch>> GetRepositoriesAsync()
	{
		string url = $"{AzureDevOpsBaseUrl}/{AzureDevOpsProject}/_apis/git/repositories?api-version=7.1";
		string response = await AzureDevOpsHttpClient.GetStringAsync(url);
		RepositoriesResponse result = JsonSerializer.Deserialize<RepositoriesResponse>(response);

		return result.Value
			.Where(repository => !repository.IsDisabled)
			.Select(repository => new RepositoryBranch
			{
				RepositoryId = repository.Id,
				RepositoryName = repository.Name,
				BranchName = null
			})
			.ToList();
	}

	private async Task<List<RepositoryBranch>> GetBranchesAsync(RepositoryBranch repository)
	{
		string url = $"{AzureDevOpsBaseUrl}/{AzureDevOpsProject}/_apis/git/repositories/{repository.RepositoryId}/refs?filter=heads&api-version=7.1";
		string response = await AzureDevOpsHttpClient.GetStringAsync(url);
		BranchesResponse result = JsonSerializer.Deserialize<BranchesResponse>(response);

		return result.Value
			.Select(refItem => new RepositoryBranch
			{
				RepositoryId = repository.RepositoryId,
				RepositoryName = repository.RepositoryName,
				BranchName = refItem.Name.Replace("refs/heads/", ""),
			})
			.ToList();
	}
}
