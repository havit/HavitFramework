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
					|| (repositoryBranchPermission.RepositoryBranch.BranchName.ToLower() is "master" or "main" or "test" or "production" or "preview" or "staging" or "prodbranch" or "testbranch")
					|| repositoryBranchPermission.RepositoryBranch.BranchName.Contains("release"))
				{
					errorLevelFrom = 1;
				}
				else
				{
					errorLevelFrom = 2;
				}

				bool errorLevelReached = repositoryBranchPermission.BranchForcePushPermissions.Count >= errorLevelFrom;

				TextWriter writer = errorLevelReached
					? Console.Error
					: Console.Out;

				if (errorLevelReached)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Environment.ExitCode = 2;
					Console.Write("[ERROR] ");
				}

				writer.WriteLine($"Repository: {repositoryBranchPermission.RepositoryBranch.RepositoryName}, Branch {repositoryBranchPermission.RepositoryBranch.BranchName}");
				foreach (var permission in repositoryBranchPermission.BranchForcePushPermissions)
				{
					writer.WriteLine("  " + permission.DisplayName);
				}
				writer.WriteLine();

				Console.ForegroundColor = ConsoleColor.Gray;
			}
		}
	}
}