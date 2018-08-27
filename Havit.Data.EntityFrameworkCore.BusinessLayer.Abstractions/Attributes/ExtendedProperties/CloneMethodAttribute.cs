﻿using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes.ExtendedProperties
{
	/// <summary>
	/// Extended properties pro CloneMethod.
	/// 
	/// Určuje, zda se má generovat metoda Clone pro klonování objektu. Metoda se generuje jen pro ne-readonly objekty.
	/// </summary>
	public class CloneMethodAttribute : ExtendedPropertiesAttribute
	{
		/// <summary>
		/// Nastavuje přístupový modifikátor ke generované funkci Clone.
		/// </summary>
		public string AccessModifier { get; set; } = null;

		/// <inheritdoc />
		public override IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo) => new Dictionary<string, string>
		{
			{ "CloneMethod", "true" }
		}.AddIfNotDefault("CloneMethodAccessModifier", AccessModifier);
	}
}