using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties
{
	/// <summary>
	/// Bázová trieda pre definovanie extended properties pre DB injections.
	/// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DbInjectionExtendedPropertiesAttribute : Attribute
    {
		/// <summary>
		/// Type objektu, na ktorom sa bude definovať extended property.
		/// </summary>
        public abstract string ObjectType { get; }

		/// <summary>
		/// Z <see cref="MemberInfo"/> vyextrahuje potrebné extended properties.
		/// </summary>
		/// <returns><see cref="Dictionary{TKey,TValue}"/> s kľúčami a hodnotami extended properties.</returns>
        public abstract IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo);
    }
}