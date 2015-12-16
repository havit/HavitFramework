using System;

namespace Havit.Data.Entity.Patterns.DataLoaders
{
	internal class PropertyToLoad
	{
		public string PropertyName { get; set; }

		public Type SourceType { get; set; }

		public Type TargetType { get; set; }

		public Type CollectionItemType { get; set; }

		public bool IsCollection
		{
			get { return CollectionItemType != null; }
		}

		public bool CollectionUnwrapped { get; set; }
	}
}