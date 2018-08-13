using System;

namespace Havit.Business.CodeMigrations.Metadata
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
	public class GenerateIndexesAttribute : Attribute
	{
		public bool GenerateIndexes { get; set; }

		public GenerateIndexesAttribute(bool generateIndexes = true)
		{
			GenerateIndexes = generateIndexes;
		}
	}
}