using System;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Metadata
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