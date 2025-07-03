namespace Havit.Tools.AzureDevopForcePushChecker;

public class Program
{
	public static async Task Main(string[] args)
	{
		if (args.Length < 3)
		{
			Console.WriteLine("Usage: Havit.Tools.AzureDevopForcePushChecker AzureDevOpsOrganization AzureDevOpsProject PersonalAccessToken");
			Environment.ExitCode = 1;
			return;
		}

		string azureDevOpsOrganization = args[0];
		string azureDevOpsProject = args[1];
		string personalAccessToken = args[2];

		using var azureDevOpsRepositoryReader = new AzureDevOpsRepositoryReader(azureDevOpsOrganization, azureDevOpsProject, personalAccessToken);
		using var azureDevOpsPermissionsReader = new AzureDevOpsPermissionsReader(azureDevOpsOrganization, azureDevOpsProject, personalAccessToken);

		AzureDevOpsForcePushReader azureDevOpsForcePushReader = new AzureDevOpsForcePushReader(azureDevOpsRepositoryReader, azureDevOpsPermissionsReader);

		var repositoryBranchPermissions = await azureDevOpsForcePushReader.GetRepositoryBranchesPermissionsAsync();
		foreach (var repositoryBranchPermission in repositoryBranchPermissions)
		{
			if (repositoryBranchPermission.BranchForcePushPermissions.Count > 0)
			{
				int errorLevelFrom;
				if ((repositoryBranchPermission.RepositoryBranch.BranchName == null)
					|| (repositoryBranchPermission.RepositoryBranch.BranchName.ToLower() is "master" or "main" or "test" or "production" or "preview" or "staging" or "uat")
					|| repositoryBranchPermission.RepositoryBranch.BranchName.Contains("release"))
				{
					errorLevelFrom = 1;
				}
				else
				{
					errorLevelFrom = 2;
				}

				bool errorLevelReached = repositoryBranchPermission.BranchForcePushPermissions.Count >= errorLevelFrom;

				if (errorLevelReached)
				{
					Environment.ExitCode = 2;

					Console.Write($"Repository: {repositoryBranchPermission.RepositoryBranch.RepositoryName}");
					if (!String.IsNullOrEmpty(repositoryBranchPermission.RepositoryBranch.BranchName))
					{
						Console.Write("Branch { repositoryBranchPermission.RepositoryBranch.BranchName}");
					}
					Console.WriteLine(); // end of line
					foreach (var permission in repositoryBranchPermission.BranchForcePushPermissions)
					{
						Console.WriteLine("  " + permission.DisplayName);
					}
					Console.WriteLine();

					Console.ForegroundColor = ConsoleColor.Gray;
				}
			}
		}
	}
}