namespace Havit.Business.Configuration.NetCore
{
	/// <summary>
	/// Abstraction for retrieving current branch in a specified Git repository.
	/// </summary>
	public interface IGitRepositoryProvider
	{
		/// <summary>
		/// Returns current branch in Git repository of referencing project.
		/// </summary>
		string GetBranch(string path);
	}
}