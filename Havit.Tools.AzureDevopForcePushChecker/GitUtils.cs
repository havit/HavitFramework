using System.Diagnostics;
using System.Text;

namespace Havit.Tools.AzureDevopForcePushChecker;

// zdroj: https://devblogs.microsoft.com/devops/git-repo-tokens-for-the-security-service/

public class GitUtils
{
	public const string SecurableRoot = "repoV2/";
	public static readonly int MaxGitRefNameLength = 400;

	/// <summary>
	/// Generate a security namespace token for a given project, repo, or branch.
	/// </summary>
	/// <param name="projectId">Project ID</param>
	/// <param name="repositoryId">Repository ID</param>
	/// <param name="branchName">Branch name - may be a partial name or be omitted</param>
	/// <returns>A security namespace token suitable for use with <see cref="Microsoft.VisualStudio.Services.Security.Client.SecurityHttpClient"/></returns>
	public static string CalculateSecurableFromBranchName(Guid projectId, Guid repositoryId, string branchName)
	{
		return CalculateSecurable(projectId, repositoryId, branchName, s_refsHeadsPrefix);
	}

	private static string CalculateSecurable(Guid projectId, Guid repositoryId, string refName, string refFirstTwoParts)
	{
		ValidateSecurableInputs(projectId, repositoryId, refName, refFirstTwoParts);

		StringBuilder securable = new StringBuilder(SecurableRoot);

		// Append the team project GUID
		if (projectId != Guid.Empty)
		{
			securable.Append(projectId);
			securable.Append("/");

			// Append the repository GUID if applicable.
			if (repositoryId != Guid.Empty)
			{
				securable.Append(repositoryId.ToString());
				securable.Append("/");

				// Append the ref name if one is provided.
				// The security namespace is case insensitive; Git ref names are case sensitive.
				// Encode the ref name into a case-insensitive format.
				// To save space, the first two components of the ref (refs/heads/, refs/tags/, etc.) are not hashed.
				if (!string.IsNullOrEmpty(refName))
				{
					refName = refName.TrimEnd(c_slash);

					// Append the first two parts as-is.
					securable.Append(refFirstTwoParts);

					// Translate the ref name.
					string[] nameParts = refName.Split(s_slashSeparator);

					// Append each encoded section and cap it off with a slash.
					foreach (string namePart in nameParts)
					{
						securable.Append(StringFromByteArray(Encoding.Unicode.GetBytes(namePart)));
						securable.Append(c_slash);
					}
				}
			}
		}

		return securable.ToString();
	}

	private static void ValidateSecurableInputs(Guid projectId, Guid repositoryId, string refName, string refFirstTwoParts)
	{
		// If you pass in a repositoryId, you must pass in a team project
		Debug.Assert(projectId != Guid.Empty || repositoryId == Guid.Empty);

		// If you pass in a ref name, then you must pass in a repository id
		Debug.Assert(string.IsNullOrEmpty(refName) || repositoryId != Guid.Empty);

		// First two parts of ref name must be lower case, contain two slashes, and end with a slash
		Debug.Assert(refFirstTwoParts.Equals(refFirstTwoParts.ToLower()));
		Debug.Assert(refFirstTwoParts.Split(s_slashSeparator).Count() == 3);
		Debug.Assert(refFirstTwoParts.EndsWith("/"));

		// Total ref name length must be under a certain size
		Debug.Assert((refFirstTwoParts + refName).Length <= MaxGitRefNameLength);
	}

	private static string StringFromByteArray(byte[] byteArray)
	{
		if (null == byteArray)
		{
			throw new ArgumentNullException("byteArray");
		}

		StringBuilder sb = new StringBuilder(byteArray.Length * 2);

		for (int i = 0; i < byteArray.Length; i++)
		{
			byte b = byteArray[i];

			char first = (char)(((b >> 4) & 0x0F) + 0x30);
			char second = (char)((b & 0x0F) + 0x30);

			sb.Append(first >= 0x3A ? (char)(first + 0x27) : first);
			sb.Append(second >= 0x3A ? (char)(second + 0x27) : second);
		}

		return sb.ToString();
	}

	private const char c_slash = '/';
	private static readonly char[] s_slashSeparator = new char[] { c_slash };
	private const string s_refsHeadsPrefix = "refs/heads/";
}
