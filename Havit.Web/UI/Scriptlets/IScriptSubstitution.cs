namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// Interface pro substituce v klientském skriptu scriptletu.
/// </summary>
public interface IScriptSubstitution
{
	/// <summary>
	/// Substituje ve skriptu.
	/// </summary>
	/// <param name="script">Skript, ve kterém má dojít k substituci.</param>
	/// <returns>Substituovaný skript.</returns>
	string Substitute(string script);
}