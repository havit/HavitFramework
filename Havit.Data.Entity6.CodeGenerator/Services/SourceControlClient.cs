using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace Havit.Data.Entity.CodeGenerator.Services
{
	/// <summary>
	/// Source Control helper class working on TFS current local workspace.	
	/// </summary>
	public class SourceControlClient
	{
		#region Private members
		private readonly Workspace workspace;
		#endregion
		
		#region private member functions
		public SourceControlClient(Workspace workspace)
		{
			this.workspace = workspace;
		}
		#endregion

		#region WorkspaceName
		public string WorkspaceName
		{
			get
			{
				lock (workspace)
				{
					return workspace.Name;
				}
			}
		}
		#endregion

		#region PendAdd
		public void PendAdd(string path)
		{
			lock (workspace)
			{
				//try
				//{
					workspace.PendAdd(path);
				//}
				//catch (Exception)
				//{
				//}
			}
		}
		#endregion

		#region PendDelete
		public void PendDelete(string path)
		{
			lock (workspace)
			{
				//try
				//{
					workspace.PendDelete(path);
				//}
				//catch
				//{
					
				//}
			}
		}
		#endregion

		#region PendRename
		public void PendRename(string source, string target)
		{
			lock (workspace)
			{
				workspace.PendRename(source, target);
			}
		}
		#endregion

		#region GetWorkspaceInfo (static)
		private static WorkspaceInfo GetWorkspaceInfo(string folder)
		{
			WorkspaceInfo[] allLocalWorkspaceInfo = Workstation.Current.GetAllLocalWorkspaceInfo();
			if (allLocalWorkspaceInfo.Length == 1)
			{
				return allLocalWorkspaceInfo[0];
			}

			string currentDirectory = folder;
			WorkspaceInfo localWorkspaceInfo = Workstation.Current.GetLocalWorkspaceInfo(currentDirectory);
			if (localWorkspaceInfo != null)
			{
				return localWorkspaceInfo;
			}

			WorkspaceInfo[] localWorkspaceInfoRecursively = Workstation.Current.GetLocalWorkspaceInfoRecursively(currentDirectory);
			if (localWorkspaceInfoRecursively.Length != 1)
			{
				return null;
			}

			return localWorkspaceInfoRecursively[0];
		}
		#endregion

		#region GetWorkspace (static)
		private static Workspace GetWorkspace(WorkspaceInfo workspaceInfo)
		{
			string tfsName = workspaceInfo.ServerUri.AbsoluteUri;
			var credentials = System.Net.CredentialCache.DefaultCredentials; //new System.Net.NetworkCredential(userName, password, domain);
			var projects = new TfsTeamProjectCollection(TfsTeamProjectCollection.GetFullyQualifiedUriForName(tfsName), credentials/*, new UICredentialsProvider()*/);
			
			return workspaceInfo.GetWorkspace(projects);
		}
		#endregion

		#region GetByFolder
		public static SourceControlClient GetByFolder(string folder)
		{
			WorkspaceInfo workspaceInfo = GetWorkspaceInfo(folder);
			if (workspaceInfo == null)
			{
				return null;
			}

			Workspace folderWorkspace = GetWorkspace(workspaceInfo);
			return new SourceControlClient(folderWorkspace);
		}
		#endregion
	}
}