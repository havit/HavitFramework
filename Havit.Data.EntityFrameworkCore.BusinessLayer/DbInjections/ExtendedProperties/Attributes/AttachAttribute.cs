using System;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AttachAttribute : Attribute
	{
		public string EntityName { get; }

		public AttachAttribute(string entityName)
		{
			Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(entityName));

			EntityName = entityName;
		}
	}
}