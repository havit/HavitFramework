namespace Havit.Business.Configuration
{
	/// <summary>
	/// Abstraction for retrieving current branch in currently used Git repository.
	/// </summary>
	public interface ICurrentGitRepositoryProvider
	{
		/// <summary>
		/// Returns current branch in Git repository of referencing project.
		/// </summary>
		string GetCurrentBranch();
	}
}