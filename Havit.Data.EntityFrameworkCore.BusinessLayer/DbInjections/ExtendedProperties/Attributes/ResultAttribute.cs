using System;
using System.Collections.Generic;
using System.Reflection;
using Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.StoredProcedures;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ResultAttribute : DbInjectionExtendedPropertiesAttribute
	{
		public override string ObjectType { get; } = "PROCEDURE";

		public StoredProcedureResultType ResultType { get; }

		public ResultAttribute(StoredProcedureResultType resultType)
		{
			ResultType = resultType;
		}

		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo)
		{
			return new Dictionary<string, string>
			{
				{ "Result", ResultType.ToString() }
			};
		}
	}
}