using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.GoogleAnalytics.ValueSerializers
{
    internal class EnumValueSerializer : IValueSerializer
    {
        public bool CanSerialize(object value)
        {
            if (value == null)
            {
                return false;
            }

            return value.GetType().IsEnum;
        }

        public string Serialize(object value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            ParameterValueAttribute attribute = type.GetField(name)
                .GetCustomAttribute<ParameterValueAttribute>();

            return attribute?.Value;
        }
    }
}
