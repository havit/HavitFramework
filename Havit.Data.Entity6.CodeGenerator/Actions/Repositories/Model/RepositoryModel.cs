namespace Havit.Data.Entity.CodeGenerator.Actions.Repositories.Model
{
	public class RepositoryModel
	{
		public string NamespaceName { get; set; }
		public string DbRepositoryName { get; set; }
		public string DbRepositoryBaseName { get; set; }
		public string InterfaceRepositoryName { get; set; }
		public string ModelClassNamespace { get; set; }
		public string ModelClassFullName { get; set; }
		public string DataSourceDependencyFullName { get; set; }
		public bool GenerateGetObjectByEntryEnumMethod { get; set; }
	}
}
