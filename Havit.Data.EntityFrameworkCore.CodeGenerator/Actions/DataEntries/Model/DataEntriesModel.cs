namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Actions.DataEntries.Model;

public class DataEntriesModel
{
	public bool UseDataEntrySymbolStorage { get; set; }
	public string NamespaceName { get; set; }
	public string InterfaceName { get; set; }
	public string DbClassName { get; set; }
	public string ModelClassFullName { get; set; }
	public string ModelEntriesEnumerationFullName { get; set; }
	public string RepositoryDependencyFullName { get; set; }
	public List<Entry> Entries { get; set; }

	public class Entry
	{
		public string PropertyName { get; set; }
		public string FieldName { get; set; }
		public bool IsObsolete { get; set; }
	}
}
