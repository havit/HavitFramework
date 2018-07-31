using System;
using System.Collections.Generic;
using System.Reflection;
using Havit.Diagnostics.Contracts;

namespace Havit.Business.CodeMigrations.DbInjections.ExtendedProperties.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class AttachAttribute : Attribute
	{
		public string EntityName { get; }

		public AttachAttribute(string entityName)
		{
			Contract.Requires<ArgumentNullException>(string.IsNullOrEmpty(entityName));

			EntityName = entityName;
		}
	}
}