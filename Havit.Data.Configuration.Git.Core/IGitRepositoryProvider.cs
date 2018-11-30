namespace Havit.Data.Configuration.Git.Core
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