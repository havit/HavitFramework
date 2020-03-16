using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Havit.GoogleAnalytics
{
    internal class ValueRequiredValidator
    {
        public static bool Validate(object value, MemberInfo memberInfo)
        {
            if (!HasRequiredAttribute(memberInfo))
            {
                return true;
            }

            if (value == null)
            {
                return false;
            }
            else if (value.GetType() == typeof(String))
            {
                return !String.IsNullOrEmpty(value as String);
            }
            else if (value == default)
            {
                return false;
            }

            return true;
        }

        private static bool HasRequiredAttribute(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttribute<RequiredAttribute>() != null;
        }
    }
}
