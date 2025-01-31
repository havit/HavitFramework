using System.Diagnostics;
using System.Reflection;

namespace Havit.Data.Entity.Mapping;

[DebuggerDisplay("{PropertyName,nq}: {IsNullable ? \"Nullable\" : \"Required\",nq}")]
public class MappedProperty
{
	public string PropertyName { get; set; }
	public PropertyInfo Property { get; set; }
	public Type Type { get; set; }
	public bool IsInPrimaryKey { get; set; }
	public bool IsNullable { get; set; }
	public bool IsStoreGenerated { get; set; }
}