namespace Havit.Tools.AzureDevopForcePushChecker;

public class AzureDevOpsForcePushReader(
	AzureDevOpsRepositoryReader _azureDevOpsRepositoryReader,
	AzureDevOpsPermissionsReader _azureDevOpsPermissionsReader)
{
	public async Task<List<RepositoryBranchForcePushPermissions>> GetRepositoryBranchesPermissionsAsync()
	{
		List<RepositoryBranch> repositoryBranches = await _azureDevOpsRepositoryReader.GetRepositoriesWithBranchesAsync();
		List<RepositoryBranchForcePushPermissions> result = new List<RepositoryBranchForcePushPermissions>(repositoryBranches.Count);

		Console.WriteLine("Reading repository branches permissions...");
		foreach (var repositoryBranch in repositoryBranches)
		{
			var branchForcePushPermissions = (await _azureDevOpsPermissionsReader.GetBranchPermissionsAsync(repositoryBranch))
				.Where(permission => permission.HasForcePushPermission())
				.ToList();

			result.Add(new RepositoryBranchForcePushPermissions
			{
				RepositoryBranch = repositoryBranch,
				BranchForcePushPermissions = branchForcePushPermissions
			});
		}

		return result;
	}
}
