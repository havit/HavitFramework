using System;

namespace Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;

internal static class QueryTagBuilder
{
	public static string CreateTag(Type type, string memberName) => type.GetType().Name + "." + memberName;
}
