using System;
using System.Collections.Generic;
using System.Reflection;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.DbInjections.ExtendedProperties
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DbInjectionExtendedPropertiesAttribute : Attribute
    {
        public abstract string ObjectType { get; }

        public abstract IDictionary<string, string> GetExtendedProperties(MemberInfo memberInfo);
    }
}