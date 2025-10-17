using System.Runtime.CompilerServices;

namespace Havit.Data.EntityFrameworkCore;

/// <summary>
/// Sestavuje query tagy.
/// </summary>
public static class QueryTagBuilder
{
	/// <summary>
	/// Sestaví Query Tag pro daného membera z daného typu.
	/// </summary>
	public static string CreateTag(Type type, [CallerMemberName] string memberName = null)
	{
		return !String.IsNullOrEmpty(memberName)
			? type.Name + "." + memberName
			: type.Name;
	}
}
