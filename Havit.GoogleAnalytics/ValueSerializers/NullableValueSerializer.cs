using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Havit.GoogleAnalytics.ValueSerializers
{
    internal class NullableValueSerializer : IValueSerializer
    {
        public bool CanSerialize(object value)
        {
            Type type = value.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public string Serialize(object value)
        {
            var serializers = ValueSerializerCollection.GetDefaultValueSerializers();

            if (value == null)
            {
                return null;
            }

            PropertyInfo valueInfo = value.GetType().GetProperty("Value");
            object unboxedValue = valueInfo.GetValue(value);
            foreach (var serializer in serializers)
            {
                if (serializer.CanSerialize(unboxedValue))
                {
                    return serializer.Serialize(unboxedValue);
                }
            }

            return unboxedValue.ToString();
        }
    }
}
