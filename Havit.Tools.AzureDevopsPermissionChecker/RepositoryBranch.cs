namespace Havit.Tools.AzureDevopForcePushChecker;

public class RepositoryBranch
{
	public required string RepositoryId { get; set; }
	public required string RepositoryName { get; set; }
	public required string BranchName { get; set; }

	public override string ToString() => RepositoryName + " / " + BranchName;
}
