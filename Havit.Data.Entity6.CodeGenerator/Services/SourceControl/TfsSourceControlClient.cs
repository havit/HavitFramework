using Microsoft.TeamFoundation.VersionControl.Client;

namespace Havit.Data.Entity.CodeGenerator.Services.SourceControl
{
	/// <summary>
	/// Source Control helper class working on TFS current local workspace.	
	/// </summary>
	public class TfsSourceControlClient : ISourceControlClient
	{
		private readonly Workspace workspace;
		
		public TfsSourceControlClient(Workspace workspace)
		{
			this.workspace = workspace;
		}

		public void Add(string path)
		{
			lock (workspace)
			{
				workspace.PendAdd(path);
			}
		}

		public void Delete(string path)
		{
			Delete(new string[] { path });
		}

		public void Delete(string[] paths)
		{
			if (paths.Length > 0)
			{
				lock (workspace)
				{
					workspace.PendDelete(paths, RecursionType.None, LockLevel.None, false);
				}
			}
		}

	}
}