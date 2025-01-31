using System.Web;
using System.Web.UI;

namespace Havit.Web.UI.Scriptlets;

/// <summary>
/// Control builder pro <see cref="Scriptlet">Scriptlet</see>.
/// Omezuje chybné použití controlu <see cref="Scriptlet">Scriptlet</see>.
/// </summary>
internal class NoLiteralContolBuilder : ControlBuilder
{
	public override bool AllowWhitespaceLiterals()
	{
		return false;
	}

	public override void AppendLiteralString(string s)
	{
		if (s.Trim().Length > 0)
		{
			throw new HttpException("Textový literál je na nepovoleném místě.");
		}
	}
}