using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Attributes
{
	/// <summary>
	/// Specifies the default value for a property as a SQL statement (expression).
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class DefaultValueSqlAttribute : Attribute
	{
		/// <summary>
		/// SQL Statement (expression).
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Konstuktor.
		/// </summary>
		public DefaultValueSqlAttribute(string value)
		{
			Value = value;
		}
	}
}
