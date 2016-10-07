﻿using System.Collections.Generic;

namespace Havit.Data.Entity.CodeGenerator.Actions.DataEntries.Model
{
	public class DataEntriesModel
	{
		public bool UseDataEntrySymbolStorage { get; set; }
		public string NamespaceName { get; set; }
		public string InterfaceName { get; set; }
		public string DbClassName { get; set; }
		public string ModelClassFullName { get; set; }
		public string ModelEntriesEnumerationFullName { get; set; }
		public List<string> Entries { get; set; }
	}
}
