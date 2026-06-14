namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public static class FieldNamingStrategy
{
	public static string GetFieldName(string name)
	{
		return "_" + Char.ToLowerInvariant(name[0]) + name.Substring(1);
	}
}
