namespace Havit.Tools.AzureDevopForcePushChecker;

public class UserPermissions
{
	public required string DisplayName { get; set; }
	public required int Allow { get; set; }
	public required int EffectiveAllow { get; set; }

	public bool HasForcePushPermission()
	{
		return (Allow & 8) == 8;
	}
}
