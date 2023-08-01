namespace Havit.Services.Ares;

/// <summary>
/// Registry ARESu.
/// </summary>
[Flags]
public enum AresRegistr
{
	/// <summary>
	/// Základní údaje z více registrů.
	/// </summary>
	Basic = 1,

	/// <summary>
	/// Obchodní rejstřík.
	/// </summary>
	ObchodniRejstrik = 2
}
