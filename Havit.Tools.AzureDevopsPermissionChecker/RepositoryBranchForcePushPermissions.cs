namespace Havit.Tools.AzureDevopForcePushChecker;

public class RepositoryBranchForcePushPermissions
{
	public required RepositoryBranch RepositoryBranch { get; set; }
	public required List<UserPermissions> BranchForcePushPermissions { get; set; }

}
