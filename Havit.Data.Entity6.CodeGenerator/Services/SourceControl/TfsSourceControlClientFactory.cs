using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Services.Common;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace Havit.Data.Entity.CodeGenerator.Services.SourceControl
{
	public class TfsSourceControlClientFactory : ISourceControlClientFactory
	{
		public ISourceControlClient Create(string path)
		{
			WorkspaceInfo workspaceInfo = GetWorkspaceInfo(path);
			if (workspaceInfo == null)
			{
				return new NullSourceControlClient();
			}

			Workspace folderWorkspace = GetWorkspace(workspaceInfo);
			return new TfsSourceControlClient(folderWorkspace);
		}

		public void Release(ISourceControlClient sourceControl)
		{
			// NOOP
		}

		private WorkspaceInfo GetWorkspaceInfo(string folder)
		{
			//WorkspaceInfo[] allLocalWorkspaceInfo = Workstation.Current.GetAllLocalWorkspaceInfo();
			//if (allLocalWorkspaceInfo.Length == 1)
			//{
			//	return allLocalWorkspaceInfo[0];
			//}

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

		private Workspace GetWorkspace(WorkspaceInfo workspaceInfo)
		{
			string tfsName = workspaceInfo.ServerUri.AbsoluteUri;
			var credentials = new VssCredentials(new WindowsCredential(System.Net.CredentialCache.DefaultCredentials));
			var projects = new TfsTeamProjectCollection(TfsTeamProjectCollection.GetFullyQualifiedUriForName(tfsName), credentials/*, new UICredentialsProvider()*/);

			return workspaceInfo.GetWorkspace(projects);
		}
	}
}
