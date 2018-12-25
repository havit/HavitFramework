using System;
using System.Reflection;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments
{
	internal static class MemberInfoExtensions
	{
		public static string FullName(this MemberInfo memberInfo)
		{
			Contract.Requires<ArgumentNullException>(memberInfo != null);

			return $"{memberInfo.DeclaringType.FullName}.{memberInfo.Name}";
		}
	}
}