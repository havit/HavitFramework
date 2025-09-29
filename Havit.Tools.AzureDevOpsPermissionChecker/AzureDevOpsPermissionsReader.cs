using Havit.Tools.AzureDevopForcePushChecker.Api;
using System.Text.Json;

namespace Havit.Tools.AzureDevopForcePushChecker;

public class AzureDevOpsPermissionsReader : AzureDevOpsReaderBase
{
	public AzureDevOpsPermissionsReader(
		string azureDevOpsOrganization,
		string azureDevOpsProject,
		string personalAccessToken) : base(azureDevOpsOrganization, azureDevOpsProject, personalAccessToken)
	{
		// NOOP
	}

	public async Task<List<UserPermissions>> GetBranchPermissionsAsync(RepositoryBranch repositoryBranch)
	{
		List<UserPermissions> permissions = new List<UserPermissions>();

		// Security namespace pro Git repositories
		string securityNamespaceId = "2e9eb7ed-3c0a-47d4-87c1-0ffdd275fd87"; // Git Repositories

		// TODO Načíst z API
		// https://dev.azure.com/havit/_apis/projects/DEV?api-version=7.1

		string _projectId = "fd51f9bd-46eb-4a1b-98c8-b26cba5651c4";

		// Konstrukce security token pro konkrétní repository/branch
		var token = GitUtils.CalculateSecurableFromBranchName(Guid.Parse(_projectId), Guid.Parse(repositoryBranch.RepositoryId), repositoryBranch.BranchName);

		string url = $"{AzureDevOpsBaseUrl}/_apis/accesscontrollists/{securityNamespaceId}?token={Uri.EscapeDataString(token)}&includeExtendedInfo=true&recurse=false&api-version=7.1";
		string response = await AzureDevOpsHttpClient.GetStringAsync(url);
		var result = JsonSerializer.Deserialize<AccessControlListResponse>(response);

		foreach (var acl in result.Value)
		{
			foreach (var ace in acl.AcesDictionary)
			{
				var displayName = await GetDisplayNameAsync(ace.Key);
				permissions.Add(new UserPermissions
				{
					DisplayName = displayName,
					Allow = ace.Value.Allow,
					EffectiveAllow = ace.Value.ExtendedInfo.EffectiveAllow
				});
			}
		}

		return permissions;
	}

	private /*async*/ Task<string> GetDisplayNameAsync(string userId)
	{
		int index = userId.LastIndexOf("\\");
		return Task.FromResult(userId.Substring(index + 1));

		//if (!_userDictionary.TryGetValue(userId, out string displayName))
		//{

		//    try
		//    {
		//        string url = $"https://vssps.dev.azure.com/{AzureDevOpsOrganization}/_apis/identities/{Uri.EscapeDataString("vaclavek@havit.cz")}?api-version=7.1";
		//        string response = await AzureDevOpsHttpClient.GetStringAsync(url);
		//        var userInfo = JsonSerializer.Deserialize<GetUserInfoResponse>(response);
		//        displayName = userInfo.DisplayName;
		//    }
		//    catch (Exception e)
		//    {
		//        displayName = userId;
		//    }
		//    _userDictionary.Add(userId, displayName);
		//}
		//return displayName;
	}
	//Dictionary<string, string> _userDictionary = new Dictionary<string, string>();

}
