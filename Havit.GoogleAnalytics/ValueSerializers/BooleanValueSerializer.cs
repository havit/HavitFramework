using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.ValueSerializers
{
    internal class BooleanValueSerializer : IValueSerializer
    {
        public bool CanSerialize(object value)
        {
            return value is bool;
        }

        public string Serialize(object value)
        {
            return (bool)value switch
            {
                true => "1",
                false => "0"
            };
        }
    }
}
