namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services;

public static class FieldNamingStrategy
{
	public static string GetFieldName(string name)
	{
		return "_" + name[0].ToString().ToLower() + name.Substring(1);
	}
}
